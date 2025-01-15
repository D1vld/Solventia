using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPositionWithFixedDistance : MonoBehaviour
{
    public Transform headset; // La transform de la cámara o del casco de VR
    public float heightOffset = 0.0f; // Desplazamiento vertical desde la altura del casco
    private Vector3 initialPosition; // Guardar la posición inicial del objeto

    void Start()
    {
        // Guardar la posición inicial
        initialPosition = transform.position;

        // Iniciar la corrutina para desactivar el script después de 2 segundos
        StartCoroutine(DeactivateAfterDelay());
    }

    public void AdjustHeight() // Nueva función para ajustar solo la altura
    {
        // Recalcular la altura cuando sea necesario
        AdjustHeightOnly();
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

        // Ajustar solo la altura
        AdjustHeightOnly();

        // Rotar la UI para que siempre mire hacia el headset (MainCamera) con un giro de 180º en el eje Y
        RotateToFaceHeadset();
    }

    void AdjustHeightOnly()
    {
        if (headset != null)
        {
            // Mantener la posición inicial en X y Z, pero ajustar solo la altura relativa al headset
            Vector3 targetPosition = initialPosition; // Mantener la posición original en X y Z
            targetPosition.y = headset.position.y + heightOffset; // Ajustar solo la altura

            // Asignar la nueva altura al objeto sin modificar la X y Z
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

