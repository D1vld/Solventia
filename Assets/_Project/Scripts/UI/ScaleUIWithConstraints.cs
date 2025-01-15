using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaintainScaleAndPositionOnScale : MonoBehaviour
{
    public RectTransform target; // El RectTransform de la interfaz principal
    private Vector3 initialScale;
    private Vector3[] initialPositions;

    void Start()
    {
        initialScale = target.localScale;
        initialPositions = new Vector3[target.childCount];

        // Guardar las posiciones iniciales de todos los elementos hijos
        for (int i = 0; i < target.childCount; i++)
        {
            initialPositions[i] = target.GetChild(i).localPosition;
        }
    }

    void Update()
    {
        // Calcular la relación de escala inversa
        float inverseScaleRatioX = initialScale.x / target.localScale.x;
        float inverseScaleRatioY = initialScale.y / target.localScale.y;

        // Ajustar la posición y escala de todos los elementos hijos
        for (int i = 0; i < target.childCount; i++)
        {
            RectTransform child = target.GetChild(i) as RectTransform;
            if (child != null)
            {
                // Ajustar posición
                child.localPosition = new Vector3(
                    initialPositions[i].x * inverseScaleRatioX,
                    initialPositions[i].y * inverseScaleRatioY,
                    initialPositions[i].z
                );

                // Ajustar escala
                child.localScale = new Vector3(inverseScaleRatioX, inverseScaleRatioY, 1);
            }
        }
    }
}
