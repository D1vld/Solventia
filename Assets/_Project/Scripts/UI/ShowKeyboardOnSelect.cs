using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShowKeyboardOnSelect : MonoBehaviour, ISelectHandler
{
public GameObject keyboardObject; // Referencia al GameObject que contiene el teclado
    public GameObject secondObject; // Referencia al segundo GameObject
    private OVRVirtualKeyboardInputFieldTextHandler ovrInputFieldHandler; // Referencia al script OVR

    private void Start()
    {
        // Comienza la corrutina para precargar ambos objetos
        StartCoroutine(PreloadObjects());
    }

    public void OnSelect(BaseEventData eventData)
    {
        ActivateKeyboard();
    }

    private void ActivateKeyboard()
    {
        if (keyboardObject != null && !keyboardObject.activeSelf)
        {
            keyboardObject.SetActive(true);
        }
        if (secondObject != null && !secondObject.activeSelf)
        {
            secondObject.SetActive(true);
        }
    }

    private IEnumerator PreloadObjects()
    {
        if (keyboardObject != null)
        {
            // Activar el teclado para precargar
            keyboardObject.SetActive(true);
            yield return null; // Esperar un frame para que cargue

            // Desactivar el teclado hasta que sea necesario
            keyboardObject.SetActive(false);
        }

        if (secondObject != null)
        {
            // Activar el segundo objeto para precargar
            secondObject.SetActive(true);
            yield return null; // Esperar un frame para que cargue

            // Buscar el componente OVRVirtualKeyboardInputFieldTextHandler
            ovrInputFieldHandler = secondObject.GetComponent<OVRVirtualKeyboardInputFieldTextHandler>();

            // Si se encuentra el componente, asignar el Input Field usando la propiedad InputField
            if (ovrInputFieldHandler != null)
            {
                InputField inputField = GetComponent<InputField>();
                if (inputField != null)
                {
                    Debug.Log("Input Field found and assigned using the InputField property");
                    ovrInputFieldHandler.InputField = inputField; // Usar la propiedad InputField para asignar el campo
                }
                else
                {
                    Debug.LogWarning("Input Field component not found on this GameObject");
                }
            }
            else
            {
                Debug.LogWarning("OVRVirtualKeyboardInputFieldTextHandler component not found on the second object");
            }

            // Desactivar el segundo objeto hasta que sea necesario
            secondObject.SetActive(false);
        }
    }
}