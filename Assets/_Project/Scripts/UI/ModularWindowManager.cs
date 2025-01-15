using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModularWindowManager : MonoBehaviour
{
    [SerializeField] private GameObject windowsStacker;  // El contenedor principal
    [SerializeField] private Transform[] windowPositions; // Las posiciones predefinidas para las ventanas

    private List<GameObject> activeWindows = new List<GameObject>();  // Lista de ventanas activas

    // Método para abrir una nueva ventana en el Windows Stacker
    public void OpenWindow(GameObject windowPrefab)
    {
        if (activeWindows.Count < windowPositions.Length)
        {
            // Instancia una nueva ventana
            GameObject newWindow = Instantiate(windowPrefab);
            
            // Asigna la ventana a la primera posición libre dentro de Windows Stacker
            AssignWindowToPosition(newWindow);
        }
        else
        {
            // Opcional: Si se alcanza el máximo de ventanas, podrías reemplazar la más antigua o hacer otra acción
            Debug.Log("No hay más posiciones disponibles para nuevas ventanas.");
        }
    }

    // Asignar la ventana a la primera posición disponible
    private void AssignWindowToPosition(GameObject window)
    {
        // Encuentra la primera posición libre en la que no haya una ventana activa
        for (int i = 0; i < windowPositions.Length; i++)
        {
            if (i >= activeWindows.Count)
            {
                // Posiciona la ventana en la posición libre dentro del Windows Stacker
                window.transform.SetParent(windowsStacker.transform, false);
                window.transform.position = windowPositions[i].position;
                window.transform.rotation = windowPositions[i].rotation;
                window.transform.localScale = Vector3.one;

                // Añadir la ventana a la lista de ventanas activas
                activeWindows.Add(window);
                break;
            }
        }
    }

    // Método para cerrar una ventana
    public void CloseWindow(GameObject window)
    {
        if (activeWindows.Contains(window))
        {
            activeWindows.Remove(window);
            Destroy(window);
            RearrangeWindows();
        }
    }

    // Método para reorganizar las ventanas restantes después de cerrar una
    private void RearrangeWindows()
    {
        for (int i = 0; i < activeWindows.Count; i++)
        {
            activeWindows[i].transform.position = windowPositions[i].position;
            activeWindows[i].transform.rotation = windowPositions[i].rotation;
        }
    }
}
