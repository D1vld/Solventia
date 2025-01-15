using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonActivator : MonoBehaviour
{
    // Referencia al botón que quieres ejecutar después de 5 segundos
    public Button myButton;

    // Este método se llama cada vez que el objeto se activa
    void OnEnable()
    {
        // Log de inicio del proceso
        Debug.LogWarning("El objeto ha sido activado. El temporizador ha comenzado. El botón se ejecutará en 5 segundos.");

        // Inicia la corrutina para simular el clic del botón después de 5 segundos
        StartCoroutine(ClickButtonAfterTime(5f));
    }

    // Corrutina que espera 5 segundos y luego ejecuta el botón
    IEnumerator ClickButtonAfterTime(float time)
    {
        // Espera el tiempo especificado
        yield return new WaitForSeconds(time);

        // Log de aviso que el tiempo ha pasado
        Debug.LogWarning($"Han pasado {time} segundos. Ejecutando el botón ahora.");

        // Simula el clic del botón
        myButton.onClick.Invoke();

        // Log de que el botón ha sido ejecutado
        Debug.LogWarning("El botón ha sido clicado.");
    }
}
