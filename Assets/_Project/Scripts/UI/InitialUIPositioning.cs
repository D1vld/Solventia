using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialUIPositioning : MonoBehaviour
{
    public Transform headset; // La transform de la cámara o del casco de VR
    public float heightOffset = 0.0f; // Desplazamiento vertical desde la altura del casco

    void Start()
    {
        // Iniciar la corrutina para desactivar el script después de 2 segundos
        StartCoroutine(DeactivateAfterDelay());
    }

    void Update()
    {
        // Buscar el transform del headset utilizando el tag "MainCamera" si no está asignado
        if (headset == null)
        {
            GameObject headsetObject = GameObject.FindGameObjectWithTag("MainCamera");
            if (headsetObject != null)
            {
                headset = headsetObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró ningún GameObject con el tag: MainCamera");
                return; // Salir del método si no se encuentra el headset
            }
        }

        // Posicionar la UI ajustando solo la altura
        AdjustHeight();

        // Rotar la UI para que siempre mire hacia el headset (MainCamera) con un giro de 180º en el eje Y
        RotateToFaceHeadset();
    }

    void AdjustHeight()
    {
        if (headset != null)
        {
            // Mantener la posición inicial en X y Z, pero ajustar solo la altura relativa al headset
            Vector3 targetPosition = transform.position; // Mantener la posición X y Z actuales
            targetPosition.y = headset.position.y + heightOffset; // Ajustar solo la altura

            // Asignar la nueva altura al objeto sin modificar X y Z
            transform.position = targetPosition;
        }
    }

    void RotateToFaceHeadset()
    {
        if (headset != null)
        {
            // Calcula la rotación para que la UI mire hacia el headset
            Vector3 directionToFace = headset.position - transform.position;
            directionToFace.y = 0; // Mantener la rotación solo en el eje Y para que no se incline hacia arriba o abajo

            // Calcular la rotación hacia el headset
            Quaternion targetRotation = Quaternion.LookRotation(directionToFace);

            // Sumar 180 grados en el eje Y
            targetRotation *= Quaternion.Euler(0, 180, 0);

            // Aplicar la rotación
            transform.rotation = targetRotation;
        }
    }

    IEnumerator DeactivateAfterDelay()
    {
        // Esperar 2 segundos
        yield return new WaitForSeconds(0.5f);

        // Desactivar el script
        this.enabled = false;
    }
}
