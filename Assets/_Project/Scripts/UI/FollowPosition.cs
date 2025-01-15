using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform target;  // El objeto padre o cámara que queremos seguir
    public Transform cameraTransform;  // La cámara del VR
    public float heightOffset = 0.0f;  // Offset de altura ajustable

    void Update()
    {
        if (target != null && cameraTransform != null)
        {
            // Calculamos la nueva posición considerando la altura de la cámara y el offset
            Vector3 newPosition = target.position;
            newPosition.y = cameraTransform.position.y + heightOffset;

            // Actualizamos la posición del objeto (puntos invisibles)
            transform.position = newPosition;
            
            // Rotar para que siempre esté frente a la cámara en el eje Y
            Vector3 lookDirection = cameraTransform.forward;
            lookDirection.y = 0;  // Solo queremos afectar la rotación en el eje Y
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }
    }
}
