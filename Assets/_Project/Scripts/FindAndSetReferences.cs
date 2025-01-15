using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class FindAndSetReferences : MonoBehaviour
{
public string keyboardTag = "UI_Keyboard"; // Tag para el teclado
    public string secondObjectTag = "Virtual_Key"; // Tag para el segundo objeto

    // Usamos Start en lugar de Awake para comenzar la corrutina
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.1f); // Espera 0.1 segundos

        SetReferences(); // Llama a la función para asignar referencias después de la espera
    }

    // Función para buscar y asignar referencias
    private void SetReferences()
    {
        // Busca los objetos por tag en toda la escena, incluso si están desactivados
        GameObject keyboardObject = FindObjectWithTag(keyboardTag);
        GameObject secondObject = FindObjectWithTag(secondObjectTag);

        // Obtiene el componente ShowKeyboardOnSelect del mismo objeto
        ShowKeyboardOnSelect showKeyboardScript = GetComponent<ShowKeyboardOnSelect>();

        if (showKeyboardScript != null)
        {
            if (keyboardObject != null)
            {
                showKeyboardScript.keyboardObject = keyboardObject; // Asigna la referencia del teclado
            }
            else
            {
                Debug.LogWarning($"Keyboard Object with tag '{keyboardTag}' not found");
            }

            if (secondObject != null)
            {
                showKeyboardScript.secondObject = secondObject; // Asigna la referencia del segundo objeto
            }
            else
            {
                Debug.LogWarning($"Second Object with tag '{secondObjectTag}' not found");
            }
        }
        else
        {
            Debug.LogWarning("ShowKeyboardOnSelect script not found on this GameObject");
        }
    }

    // Función para buscar un objeto por tag en toda la escena, incluyendo objetos desactivados
    private GameObject FindObjectWithTag(string tag)
    {
        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            if (obj.CompareTag(tag) && obj.hideFlags == HideFlags.None)
            {
                return obj;
            }
        }
        return null;
    }
}
