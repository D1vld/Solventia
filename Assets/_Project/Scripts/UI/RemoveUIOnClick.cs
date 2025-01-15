using UnityEngine;

public class RemoveUIOnClick : MonoBehaviour
{
    public GameObject uiToRemove;  // Arrastra el GameObject de la interfaz que deseas modificar o eliminar
    private WindowManager windowManager;

    void Start()
    {
        // Buscar el WindowManager en la escena
        windowManager = FindObjectOfType<WindowManager>();

        if (windowManager == null)
        {
            Debug.LogError("WindowManager no encontrado en la escena.");
        }
    }


    // Método para cambiar la visibilidad de la UI (invisibilizar/visibilizar)
    public void ToggleUIVisibility()
    {
        if (uiToRemove != null)
        {
            bool isCurrentlyVisible = uiToRemove.activeSelf;

            // Cambiar la visibilidad de la UI
            if (windowManager != null)
            {
                if (isCurrentlyVisible)
                {
                    // Invisibilizar la UI y removerla de la lista
                    windowManager.CloseWindow(uiToRemove, false);  // No destruir, solo invisibilizar y remover
                }
                else
                {
                    // Reabrir la ventana si no está visible
                    uiToRemove.SetActive(true);
                    windowManager.AddWindow(uiToRemove);  // Asegurar que se agregue a la lista de ventanas activas
                    GeneralManager.Instance.analyticsManager.AddAiDiscardClick();
                    GeneralManager.Instance.analyticsManager.AddAiInteractionClick();
                }

                // Reordenar las ventanas visibles después de cambiar la visibilidad
                windowManager.ReorderWindows();
            }
        }
        else
        {
            Debug.LogWarning("UI a modificar no está asignada.");
        }
    }


    // Método para eliminar completamente la UI
    public void RemoveUI()
    {
        if (uiToRemove != null)
        {
            // Notificar al WindowManager que la UI ha sido eliminada antes de destruirla
            if (windowManager != null)
            {
                windowManager.CloseWindow(uiToRemove);  // Cerrar la ventana y removerla de la lista en el WindowManager
                windowManager.ReorderWindows();        // Reordenar las ventanas restantes después de eliminar una
            }

            Destroy(uiToRemove);  // Eliminar la UI
            uiToRemove = null;    // Asegurarse de que no se acceda después de destruirla
        }
        else
        {
            Debug.LogWarning("UI a eliminar no está asignada.");
        }
    }
}
