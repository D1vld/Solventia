#if UNITY_EDITOR
using UnityEngine.Assertions;
#endif
using Cysharp.Threading.Tasks;
using LitMotion;
using LitMotion.Extensions;
using Oculus.Interaction;
using Oculus.Interaction.HandGrab;
using Oculus.Interaction.Input;
using UnityEngine;
using UnityEngine.UI;

public class HandMenuVerticalLayoutController : Debuggable
{
    [Header("References")]
    [Header("Hand")] [Space]
    [SerializeField] HandRef _HandRef;
    [SerializeField] PokeInteractor _HandPokeInteractor;
    [SerializeField] HandGrabInteractor _HandGrabInteractor;
    [SerializeField] DistanceHandGrabInteractor _HandDistanceGrabInteractor;
    [SerializeField] RayInteractor _HandRayInteractor;
    [Header("Layout")] [Space]
    [SerializeField] VerticalLayoutGroup _LayoutGroup;
    [SerializeField] RectTransform _LayoutRT, _SelectionBackgroundRT;
    [SerializeField] CanvasGroup _SelectionBackground;
    [Space]
    [SerializeField] Transform _WorldCanvasT;
    [SerializeField] MenuLayoutItemController _ItemPrefab;
    [SerializeField] MenuLayoutContent _Content;
    [SerializeField] MenuLayoutItemController[] _Items;
    [Space] [Header("Configuration")] [Space]
    [SerializeField] bool _HandAngleAxisConstrainsEnabled = true;
    [SerializeField] Vector2 _HandAngleAxisXAllowedRange = new(-60, 45),
        _HandAngleAxisYNotAllowedRange = new(75, 150);
    [SerializeField] float _SelectionVerticalThreshold = 1.5f,
        _SelectionHoverTimeInSec = .16f;
    [SerializeField] float _LayoutHideDelayTimeInSec = 2, _LayoutHiddenCoyoteTimeInSec = 3;
    [SerializeField] float _HandFollowSmoothTime = .16f, _HandPinchMaxDistance = 5;
    [Space] [Header("Offsets")] [Space]
    [SerializeField] Vector3 _HandFollowLayoutOffset = new(.3f, .15f, .1f);
    [SerializeField] Vector3 _HandPinchLayoutOffset = new(.1f, .2f, .4f),
        _HandPinchSelectionOffset = new(-.02f, 0, 0);
    [Space] [Header("Tweening")] [Space]
    [SerializeField] float _LayoutScaleTweenDuration = .15f;
    [SerializeField] float _LayoutPositionTweenDuration = .25f;
    [SerializeField] float _SelectionBackgroundAlphaTweenDuration = .3f;
    [SerializeField] float _SelectionBackgroundPositionTweenDuration = .75f;
    [SerializeField][Range(1, 5)] int _SelectionItemHoverPunchFrequency = 2;
    [SerializeField] float _SelectionItemHoverPunchStrength = .1f, _SelectionItemHoverPunchDuration = .3f;
    [Space] [Header("Audio")] [Space]
    [SerializeField] AudioWrapper _SelectionAudio;
    [SerializeField] AudioWrapper _ExpandAudio, _ShrinkAudio;


    int _selectedItemID = -1, _selectionHoverItemID = -1;
    System.DateTime? _selectionHoverTime, _layoutShownTime, _layoutHiddenCoyoteTime;

    bool _isHandPinch, _isHandFist, _isLayoutOpen, _isLayoutVisible, _isLayoutModification;
    Vector3 _handLastPosition, _handVelocity, _layoutVelocity;
    MotionHandle _layoutScaleMotion, _layoutPositionMotion,
        _selectionBackgroundAlphaMotion, _selectionBackgroundPositionMotion;
    
    System.DateTime? layoutHideTargetTime => !_layoutShownTime.HasValue ? null :
        _layoutShownTime.Value + System.TimeSpan.FromSeconds(_LayoutHideDelayTimeInSec);
    bool isLayoutHideTime => _LayoutHideDelayTimeInSec > 0 && _layoutShownTime.HasValue && System.DateTime.UtcNow
        >= layoutHideTargetTime!.Value;
    
    System.DateTime? layoutHiddenCoyoteTime => !_layoutHiddenCoyoteTime.HasValue ? null :
        _layoutHiddenCoyoteTime.Value + System.TimeSpan.FromSeconds(_LayoutHiddenCoyoteTimeInSec);
    bool isLayoutHiddenCoyoteTime => _layoutHiddenCoyoteTime.HasValue && System.DateTime.UtcNow
        <= layoutHiddenCoyoteTime!.Value;
    
    bool isUsingHandInteractors => _HandPokeInteractor.HasCandidate ||
                                   _HandRayInteractor.HasCandidate ||
                                   _HandGrabInteractor.HasCandidate ||
                                   _HandDistanceGrabInteractor.HasCandidate;

    int selectionHoverItemID
    {
        set
        {
            switch (value)
            {
                case < 0 when _selectionHoverTime.HasValue || _selectionHoverItemID >= 0:
                    _selectionHoverItemID = -1;
                    _selectionHoverTime = null;
                    return;

                case >= 0 when _selectionHoverItemID != value && value < _Items.Length:
                    _selectionHoverTime = System.DateTime.UtcNow;
                    // var wasSelectionHoverComplete = _selectionHoverItemID == -1;
                    // if (!wasSelectionHoverComplete || VerifyItemID(_selectedItemID))
                    // {
                    //     var previousSelectionHoverItemID = wasSelectionHoverComplete
                    //         ? _selectedItemID : _selectionHoverItemID;
                    //     _Items[previousSelectionHoverItemID].SelectionHover(false);
                    // }
                    _Items[_selectionHoverItemID = value].PunchOnSelectionHover(_SelectionItemHoverPunchStrength,
                        _SelectionItemHoverPunchDuration, _SelectionItemHoverPunchFrequency);
                    return;
            }
            
            if (!_selectionHoverTime.HasValue || System.DateTime.UtcNow < _selectionHoverTime.Value
                + System.TimeSpan.FromSeconds(_SelectionHoverTimeInSec)) return;
            UpdateSelectedItem(_selectionHoverItemID);
        }
    }


    public void UpdateLayoutPositionOnHandPinchPose()
    {
        if (TryGetHandPinchPosition(_HandPinchLayoutOffset, out var layoutPosition))
            TweenLayoutPosition(layoutPosition, _LayoutPositionTweenDuration);
    }
    public void SetHandPinchPose(bool value) => _isHandPinch = value;
    public void SetHandFistPose(bool value) => _isHandFist = value;
    public void ShowLayoutOnHandFistPose()
    {
        if (_isLayoutVisible || isUsingHandInteractors || !isLayoutHiddenCoyoteTime) return;
        if (TryGetHandPinchPosition(_HandFollowLayoutOffset, out var targetPosition))
            TweenLayoutPosition(targetPosition, _HandFollowSmoothTime);
        ShowLayout();
    }
    public void HideLayoutOnHandFistPose() => HideLayout();

    void Update()
    {
        if (!isActiveAndEnabled || _isLayoutModification) return;
        if (_LayoutRT.localPosition != default) _LayoutRT.localPosition = default;
        if (isUsingHandInteractors || !_HandRef.Hand.GetRootPose(out var handRootPose))
        {
            if (_isLayoutVisible) HideLayout();
            return;
        }
        switch (_isHandPinch)
        {
            case true when !_isLayoutOpen:
                ExpandLayout();
                return;
            
            case false when _isLayoutOpen:
                ShrinkLayout();
                return;
        }
        if (!_isLayoutVisible && !isLayoutHiddenCoyoteTime) return;
        _handVelocity = (handRootPose.position - _handLastPosition) / Time.deltaTime;
        _handLastPosition = handRootPose.position;
        switch (_isHandPinch)
        {
            case true:
            {
                UpdateLayoutHorizontalPosition();
                if (_isLayoutVisible && _isLayoutOpen)
                    UpdateSelection();
            
                void UpdateLayoutHorizontalPosition()
                {
                    if (_layoutPositionMotion.IsActive() || Mathf.Abs(_handVelocity.x) > .05f ||
                        !TryGetHandPinchPosition(_HandPinchLayoutOffset, out var layoutPosition) ||
                        Mathf.Abs(layoutPosition.x - _WorldCanvasT.position.x) <= _HandPinchMaxDistance * .01f)
                        return;
                    layoutPosition.y = _WorldCanvasT.position.y;
                    TweenLayoutPosition(layoutPosition, _LayoutPositionTweenDuration);
                }
                
                void UpdateSelection()
                {
                    if (!TryGetHandPinchPosition(_HandPinchSelectionOffset, out var pinchPosition)) return;
                    for (var i = 0; i < _Items.Length; i++)
                    {
                        if (i == _selectedItemID) continue;
                        var itemY = _Items[i].Icon.transform.position.y;
                        if (Mathf.Abs(itemY - pinchPosition.y) <= _SelectionVerticalThreshold * .01f)
                            selectionHoverItemID = i;
                    }
                }
            }
                break;

            case false:
            {
                if (_isLayoutVisible && isLayoutHideTime)
                {
                    HideLayout(false);
                    return;
                }
                UpdateLayoutPosition();
                switch (_isHandFist)
                {
                    case true when _isLayoutVisible:
                        HideLayout();
                        break;

                    case false:
                        switch (!_HandAngleAxisConstrainsEnabled || VerifyHandAngleRange())
                        {
                            case false when _isLayoutVisible:
                                HideLayout();
                                break;

                            case true when !_isLayoutVisible && isLayoutHiddenCoyoteTime:
                                ShowLayout();
                                break;
                        }
                        break;
                }

                void UpdateLayoutPosition()
                {
                    if (!TryGetHandPinchPosition(_HandFollowLayoutOffset, out var layoutPosition)) return;
                    _WorldCanvasT.position = Vector3.SmoothDamp(_WorldCanvasT.position, layoutPosition,
                        ref _layoutVelocity, _HandFollowSmoothTime);
                }
                
                bool VerifyHandAngleRange()
                {
                    var handWorldUpAngle = Vector3.SignedAngle(handRootPose.right, Vector3.up, handRootPose.forward);
                    var axisX = handWorldUpAngle >= _HandAngleAxisXAllowedRange.x &&
                                handWorldUpAngle <= _HandAngleAxisXAllowedRange.y;
                    var handLookPalmAngle =
                        Vector3.SignedAngle(handRootPose.up, _WorldCanvasT.forward, -handRootPose.right);
                    if (handLookPalmAngle < 0) handLookPalmAngle += 360;
                    var axisY = !(handLookPalmAngle > _HandAngleAxisYNotAllowedRange.x &&
                                  handLookPalmAngle < _HandAngleAxisYNotAllowedRange.y);
                    return axisX && axisY;
                }
            }
                break;
        }
    }
    
    bool TryGetHandPinchPosition(Vector3 offset, out Vector3 position)
    {
        position = default;
        var hand = _HandRef.Hand;
        if (!hand.GetRootPose(out var handRootPose) ||
            !hand.GetJointPose(HandJointId.HandThumbTip, out var thumbTipPose) ||
            !hand.GetJointPose(HandJointId.HandIndexTip, out var indexTipPose)) return false;
        var thumbTipPosition = thumbTipPose.position;
        var indexTipPosition = indexTipPose.position;
        var thumbTipToIndexTip = indexTipPosition - thumbTipPosition;
        position = thumbTipPosition + thumbTipToIndexTip * .5f;
        position += handRootPose.right.normalized * (offset.x * .1f) +
                    handRootPose.forward.normalized * (offset.y * .1f) +
                    handRootPose.up.normalized * (offset.z * .1f);
        return true;
    }


    #region Item Selection

    bool VerifyItemID(int id)
    {
#if UNITY_EDITOR
        Assert.IsTrue(_Items.Length == _Content.Items.Length);
#endif
        return id >= 0 && id < _Items.Length;
    }

    async void UpdateSelectedItem(int id)
    {
        Deselect();
        await TrySelectAsync();

        void Deselect()
        {
            if (!VerifyItemID(_selectedItemID)) return;
            _Items[_selectedItemID].Deselect();
        }
        async UniTask<bool> TrySelectAsync()
        {
            if (!VerifyItemID(id))
            {
                await ClearSelectionVFXAsync();
                _selectedItemID = -1;
                return false;
            }

            _selectedItemID = id;
            UpdateSelectionVFXAsync();
            await PlayAudioAsync(_SelectionAudio);
            _Items[_selectedItemID].Select();
            selectionHoverItemID = -1;
            return true;
        }
    }
    
    async void UpdateSelectionVFXAsync(bool isOpening = false)
    {
        switch (VerifyItemID(_selectedItemID))
        {
            case false when isOpening:
                await TweenSelectionBackgroundAlphaAsync(true);
                return;
            
            case false:
                await ClearSelectionVFXAsync();
                return;
            
            case true:
                break;
        }
        var itemPositionY = _Items[_selectedItemID].Icon.rectTransform.anchoredPosition.y;
        _selectionBackgroundPositionMotion.Kill();
        if (!isOpening) await SnapBackgroundPositionAsync(itemPositionY);
        else
        {
            _SelectionBackgroundRT.anchoredPosition = new Vector2(0, itemPositionY);
            await TweenSelectionBackgroundAlphaAsync(true);
        }
        
        async UniTask SnapBackgroundPositionAsync(float positionY)
        {
            await TweenBackgroundPositionAsync();

            async UniTask TweenBackgroundPositionAsync()
                => await (_selectionBackgroundPositionMotion = LMotion.Create(_SelectionBackgroundRT.anchoredPosition.y,
                            positionY, _SelectionBackgroundPositionTweenDuration)
                        .IgnoreTimeScale().WithEase(Ease.OutCubic)
                        .BindToAnchoredPositionY(_SelectionBackgroundRT))
                    .ToUniTask().SuppressCancellationThrow();
        }
    }
    
    async UniTask ClearSelectionVFXAsync()
    {
        foreach (var item in _Items) item.ClearSelectionVFX();
        await TweenSelectionBackgroundAlphaAsync(false);
        _selectionBackgroundPositionMotion.Kill();
        _SelectionBackgroundRT.anchoredPosition = new Vector2(0, -_LayoutRT.rect.height * .5f);
    }

    async UniTask TweenSelectionBackgroundAlphaAsync(bool visible)
    {
        _selectionBackgroundAlphaMotion.Kill();
        await (_selectionBackgroundAlphaMotion = LMotion.Create(_SelectionBackground.alpha, visible ? 1 : 0,
                _SelectionBackgroundAlphaTweenDuration).IgnoreTimeScale()
            .WithEase(visible ? Ease.InCubic : Ease.OutCubic)
            .BindToCanvasGroupAlpha(_SelectionBackground))
            .ToUniTask().SuppressCancellationThrow();
    }

    #endregion


    #region Layout

    void TweenLayoutPosition(Vector3 targetPosition, float duration)
    {
        _layoutPositionMotion.Kill();
        _layoutPositionMotion = LMotion.Create(_WorldCanvasT.position, targetPosition, duration)
            .IgnoreTimeScale().WithEase(Ease.OutCubic)
            .BindToPosition(_WorldCanvasT);
    }

    async UniTask TweenLayoutScaleAsync(Vector3 scale)
    {
        var visible = scale.sqrMagnitude > 0;
        var easing = visible ? Ease.OutBack : Ease.OutCubic;
        _layoutScaleMotion.Kill();
        await (_layoutScaleMotion = LMotion.Create(_LayoutRT.localScale, scale, _LayoutScaleTweenDuration)
            .IgnoreTimeScale().WithEase(easing)
            .BindToLocalScale(_LayoutRT)).ToUniTask().SuppressCancellationThrow();
        _isLayoutVisible = visible;
    }

    async void ShowLayout()
    {
        _isLayoutModification = true;
        await ClearSelectionVFXAsync();
        if (TryGetHandPinchPosition(_HandFollowLayoutOffset, out var layoutPosition))
            _WorldCanvasT.position = layoutPosition;
        await TweenSelectionBackgroundAlphaAsync(true);
        await TweenLayoutScaleAsync(Vector3.one);
        _layoutShownTime = System.DateTime.UtcNow;
        _isLayoutModification = false;
    }

    async void HideLayout(bool coyoteTime = true)
    {
        _isLayoutModification = true;
        _layoutShownTime = null;
        await TweenLayoutScaleAsync(Vector3.zero);
        _LayoutRT.pivot = Vector2.one * .5f;
        _layoutHiddenCoyoteTime = coyoteTime && _selectedItemID >= 0 ? System.DateTime.UtcNow : null;
        _isLayoutOpen = _isLayoutModification = false;
    }

    async void ExpandLayout()
    {
        _isLayoutModification = true;
        _layoutShownTime = null;
        await TweenLayoutScaleAsync(Vector3.zero);
        await ClearSelectionVFXAsync();
        _LayoutGroup.gameObject.SetActive(false);
        foreach (var item in _Items) item.gameObject.SetActive(true);
        UpdateLayoutPivotToSelectedItem(_selectedItemID);
        _LayoutGroup.gameObject.SetActive(true);
        PlayAudio(_ExpandAudio);
        await TweenLayoutScaleAsync(Vector3.one);
        UpdateSelectionVFXAsync(_isLayoutOpen = true);
        _isLayoutModification = false;

        void UpdateLayoutPivotToSelectedItem(int i)
        {
            if (!VerifyItemID(i)) return;
            var itemHeight = _ItemPrefab.Icon.rectTransform.rect.height;
            var layoutHeight = _Items.Length * itemHeight
                               + (_Items.Length - 1) * _LayoutGroup.spacing
                               + _LayoutGroup.padding.top + _LayoutGroup.padding.bottom;
            var selectedItemLayoutPosFromTop = (_selectedItemID + .5f) * itemHeight
                                               + i * _LayoutGroup.spacing + _LayoutGroup.padding.top;
            _LayoutRT.pivot = new Vector2(.5f, 1f - selectedItemLayoutPosFromTop / layoutHeight);
        }
    }

    async void ShrinkLayout()
    {
        _isLayoutModification = true;
        PlayAudio(_ShrinkAudio);
        if (!VerifyItemID(_selectedItemID))
        {
            HideLayout();
            return;
        }
        await TweenLayoutScaleAsync(Vector3.zero);
        for (var i = 0; i < _Items.Length; i++)
        {
            if (_selectedItemID == i) continue;
            _Items[i].gameObject.SetActive(false);
        }
        _LayoutRT.pivot = Vector2.one * .5f;
        _isLayoutOpen = false;
        _Items[_selectedItemID].SelectionComplete();
        ShowLayout();
    }

    #endregion

    #region Layout Editor
#if UNITY_EDITOR

    [ContextMenu(nameof(SetLayoutContent))]
    void SetLayoutContent()
    {
        for (var i = _LayoutRT.childCount - 1; i > _Content.Items.Length - 1; i--)
            DestroyImmediate(_LayoutRT.GetChild(i).gameObject);
        _Items = new MenuLayoutItemController[_Content.Items.Length];
        for (var i = 0; i < _Items.Length; i++)
        {
            _Items[i] = _LayoutRT.childCount - 1 > i &&
                        _LayoutRT.GetChild(i + 1).TryGetComponent(out MenuLayoutItemController childItem)
                ? childItem : Instantiate(_ItemPrefab, _LayoutRT);
            _Items[i].gameObject.layer = default;
            _Items[i].gameObject.name = $"[{i}] {_Content.Items[i].Name} (Selectable Item)";
            _Items[i].SetIcon(_Content.Items[i].Icon);
        }
        UpdateSelectedItem(-1);
        _isLayoutVisible = true;
        HideLayout();
    }

    [ContextMenu(nameof(HideLayout))]
    void HideLayoutFromContextMenu() => HideLayout();
    
    [ContextMenu(nameof(ShowLayout))]
    void ShowLayoutFromContextMenu() => ShowLayout();

    void OnDrawGizmos()
    {
        if (_HandRef.Hand is not { IsConnected: true } hand) return;
        if (hand.GetRootPose(out var handRootPose))
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(handRootPose.position, handRootPose.position + handRootPose.up);
            Gizmos.color = Color.red;
            Gizmos.DrawLine(handRootPose.position, handRootPose.position + handRootPose.right);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(handRootPose.position, handRootPose.position + handRootPose.forward);
        }
        if (!_isHandPinch) return;
        if (!_isLayoutVisible || !TryGetHandPinchPosition(_HandPinchSelectionOffset,
                out var pinchPosition)) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_LayoutRT.transform.position.Set(y: pinchPosition.y), .007f);
        for (var i = 0; i < _Items.Length; i++)
        {
            Gizmos.color = Color.red;
            var itemY = _Items[i].Icon.transform.position.y;
            if (i != _selectedItemID && Mathf.Abs(itemY - pinchPosition.y) <= _SelectionVerticalThreshold * .01f)
            {
                selectionHoverItemID = i;
                Gizmos.color = Color.green;
            }
            Gizmos.DrawWireSphere(_LayoutRT.transform.position.Set(y: itemY),
                _SelectionVerticalThreshold * .01f);
        }
    }
#endif
    #endregion


    void PlayAudio(AudioWrapper audioWrapper) => _ = PlayAudioAsync(audioWrapper);

    async UniTask PlayAudioAsync(AudioWrapper audioWrapper)
    {
        if (audioWrapper.DelayInMs > 0) await UniTask.Delay(audioWrapper.DelayInMs, true);
        AudioSource.PlayClipAtPoint(audioWrapper.Clip, _WorldCanvasT.position, audioWrapper.Volume);
    }
}