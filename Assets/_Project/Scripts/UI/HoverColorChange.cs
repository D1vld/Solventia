using UnityEngine;
using UnityEngine.UI;

public class HoverColorChanges : MonoBehaviour
{
    public Color hoverColor = Color.red;
    public Color defaultColor = Color.white;
    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        if (image != null)
        {
            image.color = defaultColor;
        }
    }

    public void OnHoverEnter()
    {
        if (image != null)
        {
            image.color = hoverColor;
        }
    }

    public void OnHoverExit()
    {
        if (image != null)
        {
            image.color = defaultColor;
        }
    }
}
