using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCameraY : MonoBehaviour
{
    private Transform cameraTransform;  // La referencia al objeto con el tag "MainCamera"

    void Start()
    {
        // Buscar el objeto con el tag "MainCamera"
        GameObject cameraObject = GameObject.FindWithTag("MainCamera");
        if (cameraObject != null)
        {
            cameraTransform = cameraObject.transform;
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'MainCamera'.");
        }
    }

    void Update()
    {
        if (cameraTransform != null)
        {
            // Obtener la posición del objeto y de la cámara, ignorando el eje Y
            Vector3 directionToCamera = cameraTransform.position - transform.position;
            directionToCamera.y = 0;  // Ignorar la diferencia en altura (eje Y)

            // Actualizar la rotación del objeto solo en el eje Y
            if (directionToCamera != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
                transform.rotation = targetRotation;
            }
        }
    }
}
