using System.Collections;
using UnityEngine;

public class ReorderMenu : MonoBehaviour
{
    private Transform targetObject;  // El objeto al que queremos posicionar el objeto actual
    private UIPositionWithFixedDistance uiPositionScript; // Referencia al script UIPositionWithFixedDistance
    public float lerpDuration = 0.5f; // Duración de la transición

    void OnEnable()
    {
        // Buscar el objeto con el tag "Respawn"
        GameObject target = GameObject.FindWithTag("Respawn");
        if (target != null)
        {
            targetObject = target.transform;
            uiPositionScript = target.GetComponent<UIPositionWithFixedDistance>(); // Obtener el componente UIPositionWithFixedDistance
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'Respawn'.");
        }

        // Suscribirse al evento
        WindowManager.OnReorderWindowsEvent += StartReposition;
    }

    void OnDisable()
    {
        // Desuscribirse del evento
        WindowManager.OnReorderWindowsEvent -= StartReposition;
    }

    void StartReposition()
    {
        if (targetObject != null)
        {
            StartCoroutine(MoveToTarget(targetObject.position, lerpDuration));
        }
    }

    // Corutina para mover suavemente el objeto a la posición objetivo usando Lerp
    IEnumerator MoveToTarget(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;  // Espera al siguiente frame
        }

        // Asegurarse de que la posición final sea exacta
        transform.position = targetPosition;

        // Si el script UIPositionWithFixedDistance está presente, ajustar solo la altura
        if (uiPositionScript != null)
        {
            uiPositionScript.AdjustHeight(); // Ajustar solo la altura si es necesario
        }
    }
}
