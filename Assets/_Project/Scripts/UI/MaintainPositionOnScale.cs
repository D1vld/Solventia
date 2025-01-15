using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainPositionOnScale : MonoBehaviour
{
    public RectTransform target; // The RectTransform of the window
    public Vector2 offset; // The offset from the top-right corner

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // Update the position of the "X" button based on the target window's size and the specified offset
        rectTransform.anchoredPosition = new Vector2(target.rect.width / 2 - offset.x, target.rect.height / 2 - offset.y);
    }
}
