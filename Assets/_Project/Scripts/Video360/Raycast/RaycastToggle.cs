using UnityEngine;
using UnityEngine.UI;  // Para usar Toggle
using UnityEngine.EventSystems;  // Para trabajar con interfaces de usuario

public class RaycastToggle : MonoBehaviour
{
    public Camera playerCamera;  // La cámara desde la que se emitirá el Raycast
    public float raycastDistance = 10f;  // Distancia máxima del Raycast
    public LayerMask interactableLayer;  // Para filtrar solo los objetos interactivos (como los Toggles)

    private RaycastHit hitInfo;  // Para almacenar la información del objeto golpeado
    private GameObject lastHitObject;  // Para almacenar el último objeto tocado

    void Update()
    {
        // Lanzamos el Raycast desde la cámara
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));  // Ray en el centro de la pantalla (simula el eyetracking)
        if (Physics.Raycast(ray, out hitInfo, raycastDistance, interactableLayer))
        {
            GameObject hitObject = hitInfo.collider.gameObject;

            // Verificar si el objeto es un Toggle y si no ha sido tocado antes
            if (hitObject.CompareTag("Toggle") && hitObject != lastHitObject)
            {
                lastHitObject = hitObject;
                Toggle toggle = hitObject.GetComponent<Toggle>();

                // Si el Toggle es válido, activamos o desactivamos su estado
                if (toggle != null)
                {
                    toggle.isOn = !toggle.isOn;
                    ExecuteEvents.Execute(toggle.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerClickHandler);
                }
            }
        }
        else
        {
            lastHitObject = null;  // Reseteamos si el raycast no ha tocado nada
        }
    }
}
