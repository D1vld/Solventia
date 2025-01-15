using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ToggleActivator : MonoBehaviour
{
    // Referencia al Toggle que quieres ejecutar después de 5 segundos
    public Toggle myToggle;

    // Este método se llama solo una vez cuando el objeto se inicia
    void Start()
    {
        // Log de inicio del proceso
        Debug.LogWarning("El objeto ha sido iniciado. El temporizador ha comenzado. El Toggle se ejecutará en 5 segundos.");

        // Inicia la corrutina para simular el clic del Toggle después de 5 segundos
        StartCoroutine(ToggleAfterTime(5f));
    }

    // Corrutina que espera 5 segundos y luego ejecuta el Toggle
    IEnumerator ToggleAfterTime(float time)
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(time);

        // Log de aviso que el tiempo ha pasado
        Debug.LogWarning($"Han pasado {time} segundos. Cambiando el estado del Toggle ahora.");

        // Cambia el estado del Toggle
        myToggle.SetEnabled(!myToggle.enabledInHierarchy);

        // Log de que el Toggle ha sido activado/desactivado
        Debug.LogWarning("El Toggle ha cambiado su estado.");
    }
}

