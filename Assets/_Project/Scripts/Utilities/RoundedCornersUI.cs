using Nobi.UiRoundedCorners;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(ImageWithRoundedCorners))]
public class RoundedCornersUI : MonoBehaviour
{
    ImageWithRoundedCorners _roundedCorners;
    
    void Reset()
    {
        _roundedCorners = GetComponent<ImageWithRoundedCorners>();
        _roundedCorners.radius = ((RectTransform)transform).rect.width * .5f;
        _roundedCorners.Validate();
        _roundedCorners.Refresh();
    }
}