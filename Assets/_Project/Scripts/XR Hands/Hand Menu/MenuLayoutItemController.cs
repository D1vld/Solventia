using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable]
public class MenuLayoutItemController : MonoBehaviour
{
    [SerializeField] UnityEvent _OnSelect, _OnDeselect, _OnSelectionComplete;
    [SerializeField] Image _Icon, _IconInverse;
    public Image Icon => _Icon;
    
    MotionHandle _selectionHoverScaleMotion;
    
    
    public void Select() => _OnSelect?.Invoke();
    public void Deselect() => _OnDeselect?.Invoke();
    public void SelectionComplete() => _OnSelectionComplete?.Invoke();
    
    public void SetIcon(Sprite icon) => _Icon.sprite = _IconInverse.sprite = icon;

    public void PunchOnSelectionHover(float strength, float duration, int frequency)
    {
        _selectionHoverScaleMotion.Kill();
        LMotion.Punch.Create(Vector3.one, Vector3.one * strength, duration)
            .IgnoreTimeScale().WithEase(Ease.InOutCubic).WithFrequency(frequency)
            .BindToLocalScale(transform);
    }

    public void ClearSelectionVFX()
    {
        _selectionHoverScaleMotion.Kill();
        transform.localScale = Vector3.one;
    }
}