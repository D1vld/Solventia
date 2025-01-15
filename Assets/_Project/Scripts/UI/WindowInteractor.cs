using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowInteractor : MonoBehaviour
{
    public int prefabIndex; // Campo público para configurarlo desde el Inspector
    private WindowManager windowManager;

    [Header("Configuración de Ventanas")]
    [SerializeField] private bool useNotificationID = false; // Bool para determinar si se usa el notificationID

    // Referencia al NotificationManager
    private NotificationManager notificationManager;

    void Start()
    {
        // Buscar el WindowManager en la escena
        windowManager = FindObjectOfType<WindowManager>();
        if (windowManager == null)
        {
            throw new System.Exception("WindowManager no encontrado en la escena.");
        }

        // Buscar el NotificationManager en la escena
        notificationManager = FindObjectOfType<NotificationManager>();
        if (notificationManager != null)
        {
            // Si el bool useNotificationID es true, asignar el prefabIndex al valor del notificationID
            if (useNotificationID)
            {
                prefabIndex = notificationManager.notificationID -1;
                Debug.Log($"El prefabIndex se ha asignado con el valor del notificationID: {prefabIndex}");
            }
        }
        else
        {
            Debug.LogError("NotificationManager no encontrado en la escena.");
        }
    }

    // Método para abrir una ventana desde un botón
    public void OnOpenWindowButtonClick()
    {
        if (windowManager != null)
        {
            // Verificar si la ventana ya está abierta
            if (windowManager.IsWindowOpen(prefabIndex))
            {
                var window = windowManager.GetWindowByPrefabIndex(prefabIndex);
                if (window != null)
                {
                    windowManager.CloseWindow(window);
                }
            }
            else
            {
                // Verificar si es una ventana panorámica y si ya hay una abierta
                if (windowManager.windowPrefabs[prefabIndex].CompareTag("Panoramicas") && windowManager.IsPanoramicaWindowOpen())
                {
                    Debug.Log("Ya hay una ventana panorámica abierta.");
                }
                else
                {
                    windowManager.OpenWindow(prefabIndex);
                }
            }
        }
        else
        {
            throw new System.Exception("WindowManager no está asignado.");
        }
    }

    // Método para abrir una ventana existente desde un botón
    public void OnOpenExistingWindowButtonClick(int existingIndex)
    {
        if (windowManager != null)
        {
            // Obtener la ventana existente basada en el índice
            GameObject existingWindow = windowManager.existingWindows[existingIndex];

            // Verificar si la ventana está activa y visible
            if (existingWindow != null && existingWindow.activeSelf)
            {
                // Si está activa y visible, la cerramos utilizando el índice
                windowManager.CloseExistingWindow(existingIndex);
            }
            else
            {
                // Si no está activa, la volvemos a abrir
                windowManager.OpenExistingWindow(existingIndex);
            }
        }
        else
        {
            throw new System.Exception("WindowManager no está asignado.");
        }
    }

    // Método para cerrar todas las ventanas desde un botón
    public void OnCloseAllWindowsButtonClick()
    {
        if (windowManager != null)
        {
            windowManager.CloseAllWindows();
        }
        else
        {
            throw new System.Exception("WindowManager no está asignado.");
        }
    }
}
