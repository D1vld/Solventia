using System.Collections;
using UnityEngine;

public class MoveAndReveal : MonoBehaviour
{
    public GameObject uiPrefab; // Prefab de la UI que vamos a instanciar
    public Transform targetPosition; // La posición destino

    private GameObject instantiatedObject;

    // Este método puede ser llamado desde un evento de botón o cualquier otro trigger
    public void StartMoveAndReveal()
    {
        if (uiPrefab != null && targetPosition != null)
        {
            // Instanciar el objeto UI en la posición objetivo
            instantiatedObject = Instantiate(uiPrefab, targetPosition.position, Quaternion.identity);

            // Aplicar la rotación manualmente
            ApplyRotationCorrection(instantiatedObject);

            // Solo rota la UI si el nombre del prefab es "FinalMenuLateralV2"
            if (uiPrefab.name.Contains("FinalMenuLateralV2"))
            {
                // Inicia la corutina para rotar hacia la cámara
                StartCoroutine(RotateTowardsCamera(instantiatedObject));
            }
        }
        else
        {
            Debug.LogError("El prefab de la UI o la posición destino no están asignados.");
        }
    }

    // Función para corregir la rotación manualmente si la UI aparece al revés
    private void ApplyRotationCorrection(GameObject obj)
    {
        obj.transform.Rotate(0, 180, 0); // Aplica un giro de 180 grados en el eje Y
    }

    // Corutina para rotar el objeto hacia la cámara
    private IEnumerator RotateTowardsCamera(GameObject obj)
    {
        // Espera un frame para asegurar que el objeto ha sido correctamente instanciado
        yield return null;

        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - obj.transform.position;
            directionToCamera.y = 0; // Mantener solo la rotación en el eje Y

            if (directionToCamera != Vector3.zero)
            {
                Quaternion lookRotation = Quaternion.LookRotation(directionToCamera);
                obj.transform.rotation = lookRotation; // Rotar hacia la cámara sin ningún ajuste adicional
            }
        }
        else
        {
            Debug.LogError("No se encontró una cámara principal (MainCamera) en la escena.");
        }
    }
}

