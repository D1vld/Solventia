using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInitializer : MonoBehaviour
{
    private WindowManager windowManager;
    private int prefabIndex;
    public bool isWindow = true; // Nueva variable para identificar si este objeto debe ser gestionado como una ventana

    void Start()
    {
        // Buscar el WindowManager en la escena
        windowManager = FindObjectOfType<WindowManager>();

        if (windowManager != null)
        {
            // Verificar si este objeto debe ser gestionado como una ventana
            if (isWindow)
            {
                // Verificar si el objeto tiene el componente WindowIdentifier
                if (GetComponent<WindowIdentifier>() != null)
                {
                    // Identificar el índice del prefab basado en el objeto padre
                    prefabIndex = windowManager.GetPrefabIndex(this.gameObject);

                    // Registrar esta ventana en el WindowManager
                    windowManager.AddWindow(gameObject);
                }
                else
                {
                    Debug.LogError("El objeto no tiene el componente WindowIdentifier.");
                }
            }
        }
        else
        {
            throw new System.Exception("WindowManager no encontrado en la escena.");
        }
    }

    private void OnDestroy()
    {
        // Remover esta ventana del WindowManager al destruirse
        if (windowManager != null && isWindow)
        {
            windowManager.RemoveWindow(gameObject);
        }
    }
}
