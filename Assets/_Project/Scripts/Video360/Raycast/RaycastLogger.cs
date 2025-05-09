using UnityEngine;
using UnityEngine.UI;

public class RaycastLogger : MonoBehaviour
{
    public Camera playerCamera;
    public float raycastDistance = 10f;
    public string tagToSearch = "Toggle";  // Tag a buscar en los objetos

    void Update()
    {
        // Lanzamos un raycast desde la cámara hacia el frente
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        if (Physics.Raycast(ray, out hit, raycastDistance))
        {
            // Verificamos si el objeto tocado tiene el tag "Toggle"
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(tagToSearch)) // Verificar si el tag coincide
            {
                // Si el objeto tiene el tag "Toggle", verificamos si tiene el componente Toggle
                Toggle toggleComponent = hitObject.GetComponent<Toggle>();

                if (toggleComponent != null)
                {
                    // Si tiene un Toggle, mostramos el nombre del objeto y activamos el estado
                    Debug.LogError("Error: El objeto '" + hitObject.name + "' tiene un componente Toggle.");
                    
                    // Simulamos la selección del Toggle (cambiamos su estado)
                    toggleComponent.isOn = !toggleComponent.isOn; // Alterna el estado (seleccionado/desmarcado)

                    // También se pueden activar los objetos dentro del Toggle si es necesario
                    if (toggleComponent.isOn)
                    {
                        // Activamos elementos dentro del Toggle si está seleccionado
                        Debug.Log("Toggle activado, se activa el contenido interno.");
                    }
                    else
                    {
                        // Desactivamos elementos dentro del Toggle si no está seleccionado
                        Debug.Log("Toggle desactivado, se desactiva el contenido interno.");
                    }
                }
            }
            else
            {
                // Si no tiene el tag "Toggle", mostramos el nombre sin el error
                Debug.LogWarning("Objeto detectado sin el tag 'Toggle': " + hitObject.name);
            }
        }
    }
}