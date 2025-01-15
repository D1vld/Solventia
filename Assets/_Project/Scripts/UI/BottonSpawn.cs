using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottonSpawn : MonoBehaviour
{
     // Referencia al prefab que quieres instanciar
    public GameObject prefab;

    // Referencia al objeto padre en la interfaz donde quieres que aparezca el prefab
    public Transform parentTransform;

    // Método para instanciar el prefab
    public void SpawnPrefab()
    {
        if (prefab != null && parentTransform != null)
        {
            // Instanciar el prefab como hijo del objeto padre
            GameObject spawnedObject = Instantiate(prefab, parentTransform);

            // Ajustar la posición local del prefab para que esté en el centro del objeto padre
            RectTransform spawnedRectTransform = spawnedObject.GetComponent<RectTransform>();
            if (spawnedRectTransform != null)
            {
                spawnedRectTransform.anchoredPosition = Vector2.zero;
                spawnedRectTransform.localScale = Vector3.one;
            }
            else
            {
                // Si no es un RectTransform, solo ajustar la posición y la escala local
                spawnedObject.transform.localPosition = Vector3.zero;
                spawnedObject.transform.localScale = Vector3.one;
            }
        }
        else
        {
            Debug.LogError("Prefab o ParentTransform no están asignados en el inspector.");
        }
    }
}
