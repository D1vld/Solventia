using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ventana : MonoBehaviour
{
    public void SetPosition(Transform newTransform, float duration)
    {
        StartCoroutine(MoveToPosition(newTransform, duration));
    }

    private IEnumerator MoveToPosition(Transform newTransform, float duration)
    {
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        // No modificar la escala inicial
        Vector3 startScale = transform.localScale;

        Vector3 endPosition = newTransform.position;
        Quaternion endRotation = newTransform.rotation;
        // Mantener la escala del prefab
        Vector3 endScale = startScale;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            transform.rotation = Quaternion.Lerp(startRotation, endRotation, elapsed / duration);
            transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration); // Esto no cambiará la escala
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurar la posición, rotación y escala finales
        transform.position = endPosition;
        transform.rotation = endRotation;
        transform.localScale = endScale; // Esto no cambiará la escala
    }
}
