using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public delegate void OnReorderWindowsDelegate();
    public static event OnReorderWindowsDelegate OnReorderWindowsEvent;

    #region  Variables
    [Header("Cantidad de Ventanas y Formato de Ventanas")]
    public int maxWindows = 4;
    public List<GameObject> windowPrefabs;
    public List<GameObject> existingWindows; // Lista de GameObjects existentes en la escena
    [Tooltip("Ubicacion de las ventanas y el orden por el que van transladandose")]
    public List<Transform> windowPositions; // Usar Transform en lugar de Vector3
    public float transitionDuration = 0.5f; // Duración de la transición


    private List<GameObject> windows = new List<GameObject>();
    #endregion
    void Start()
    {
        // Asegurarse de que la lista windowPositions tenga suficientes elementos y no sean nulos
        if (windowPositions == null || windowPositions.Count < maxWindows)
        {
            throw new Exception("La lista windowPositions no está correctamente inicializada o no tiene suficientes elementos.");
        }
        else
        {
            for (int i = 0; i < windowPositions.Count; i++)
            {
                if (windowPositions[i] == null)
                {
                    throw new Exception("Elemento nulo encontrado en windowPositions en el índice: " + i);
                }
            }
        }

        // Verificar que todos los prefabs tengan el componente WindowIdentifier
        foreach (var prefab in windowPrefabs)
        {
            if (prefab.GetComponent<WindowIdentifier>() == null)
            {
                throw new Exception("El prefab " + prefab.name + " no tiene el componente WindowIdentifier.");
            }
        }
    }
    #region Funciones
    public bool IsWindowOpen(GameObject window)
    {
        return existingWindows.Contains(window);
    }

    public bool IsPanoramicaWindowOpen()
    {
        foreach (var window in windows)
        {
            if (window.CompareTag("Panoramicas"))
            {
                return true;
            }
        }
        return false;
    }



    public void OpenWindow(int prefabIndex)
    {
        if (prefabIndex < 0 || prefabIndex >= windowPrefabs.Count)
        {
            throw new ArgumentException("Índice de prefab inválido.");
        }

        // Verificar si la ventana ya está abierta
        if (IsWindowOpen(prefabIndex))
        {
            Debug.Log("La ventana ya está abierta.");
            return;
        }

        // Verificar si hay una ventana 'Panoramicas' ya abierta
        if (windowPrefabs[prefabIndex].CompareTag("Panoramicas") && IsPanoramicaWindowOpen())
        {
            Debug.Log("Ya hay una ventana panorámica abierta.");
            return;
        }

        // Instanciar y posicionar la nueva ventana
        CreateAndPositionWindow(prefabIndex);
    }



    public void OpenExistingWindow(int existingIndex)
    {
        if (existingIndex < 0 || existingIndex >= existingWindows.Count)
        {
            throw new ArgumentException("Índice de ventana existente inválido.");
        }

        GameObject existingWindow = existingWindows[existingIndex];

        // Verificar si la ventana está oculta y no activa
        if (existingWindow != null && !existingWindow.activeSelf)
        {
            // Reactivar la ventana
            existingWindow.SetActive(true);

            // Insertar la ventana en la lista de ventanas activas si no está ya allí
            if (!windows.Contains(existingWindow))
            {
                windows.Insert(0, existingWindow);
            }

            // Posicionar la ventana en la primera posición de la lista
            existingWindow.GetComponent<Ventana>().SetPosition(windowPositions[0], transitionDuration);

            // Reordenar todas las ventanas
            ReorderWindows();
        }
        else if (existingWindow != null && existingWindow.activeSelf)
        {
            Debug.Log("La ventana ya está visible.");
        }
        else
        {
            throw new Exception("La ventana especificada es nula o no se pudo activar.");
        }
    }


    public void CloseWindow(GameObject window, bool destroy = true)
{
    if (windows.Contains(window))
    {
        windows.Remove(window);  // Remover completamente de la lista de ventanas activas

        if (existingWindows.Contains(window))
        {
            window.SetActive(false);  // Solo desactivar si es parte de ventanas existentes
        }
        else
        {
            if (destroy)
            {
                Destroy(window);  // Destruir si no pertenece a existingWindows
            }
            else
            {
                window.SetActive(false);  // Solo desactivar si no se va a destruir
            }
        }
        ReorderWindows();  // Reordenar ventanas después de cerrar una
    }
}


    // Método para cerrar una ventana existente
    public void CloseExistingWindow(int existingIndex)
    {
        if (existingIndex < 0 || existingIndex >= existingWindows.Count)
        {
            throw new ArgumentException("Índice de ventana existente inválido.");
        }

        GameObject existingWindow = existingWindows[existingIndex];

        if (windows.Contains(existingWindow))
        {
            CloseWindow(existingWindow, false); // Solo desactiva la ventana, no la destruye
        }
    }

    private void CreateAndPositionWindow(int prefabIndex)
    {
        // Si ya hay el máximo número de ventanas, elimina la última
        if (windows.Count >= maxWindows)
        {
            CloseWindow(windows[windows.Count - 1]);
        }

        // Instanciar el prefab y verificar el componente WindowIdentifier
        GameObject newWindow = Instantiate(windowPrefabs[prefabIndex]);
        newWindow.name = windowPrefabs[prefabIndex].name + "(Clone)";
        WindowIdentifier identifier = newWindow.GetComponent<WindowIdentifier>();

        if (identifier != null)
        {
            windows.Insert(0, newWindow);
            Ventana ventanaComponent = newWindow.GetComponent<Ventana>();
            if (ventanaComponent != null)
            {
                ventanaComponent.SetPosition(windowPositions[0], transitionDuration);
            }
            else
            {
                throw new Exception("El objeto instanciado no tiene el componente Ventana.");
            }
            ReorderWindows();
        }
        else
        {
            Destroy(newWindow);  // Destruir el objeto si no tiene el componente requerido
            throw new Exception("El objeto instanciado no tiene el componente WindowIdentifier: " + newWindow.name);
        }
    }

    public void ReorderWindows()
{
    int visibleWindowIndex = 0;

    for (int i = 0; i < windows.Count; i++)
    {
        if (windows[i] != null && windows[i].activeSelf)  // Solo reordenar ventanas activas
        {
            if (visibleWindowIndex < windowPositions.Count && windowPositions[visibleWindowIndex] != null)
            {
                Ventana ventanaComponent = windows[i].GetComponent<Ventana>();
                if (ventanaComponent != null)
                {
                    ventanaComponent.SetPosition(windowPositions[visibleWindowIndex], transitionDuration);
                    visibleWindowIndex++;
                }
            }
        }
    }

    // Disparar el evento cuando se hayan reordenado las ventanas
    OnReorderWindowsEvent?.Invoke();
}


    // Métodos Públicos para Acceder y Manipular Ventanas desde Otros Scripts
    public void AddWindow(GameObject window)
    {
        if (window.GetComponent<WindowIdentifier>() != null)
        {
            windows.Add(window);
        }
        else
        {
            throw new Exception("El objeto añadido no tiene el componente WindowIdentifier.");
        }
    }

    public void RemoveWindow(GameObject window)
    {
        if (windows.Contains(window))
        {
            windows.Remove(window);
        }
        else
        {
            throw new Exception("La ventana a eliminar no se encuentra en la lista.");
        }
    }

    public int GetPrefabIndex(GameObject prefab)
    {
        return windowPrefabs.IndexOf(prefab);
    }

    public List<GameObject> GetWindows()
    {
        return windows;
    }

    public void CloseAllWindows()
    {
        // Desactivar ventanas existentes
        foreach (var existingWindow in existingWindows)
        {
            if (existingWindow != null)
            {
                existingWindow.SetActive(false);
            }
        }

        // Eliminar ventanas de prefabs
        foreach (var window in new List<GameObject>(windows))
        {
            if (window != null)
            {
                bool destroy = !existingWindows.Contains(window);
                CloseWindow(window, destroy);
            }
        }

        // Desactivar objetos con el tag "3D"
        GameObject[] objects3D = GameObject.FindGameObjectsWithTag("3D");
        foreach (var obj in objects3D)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
    }

    // Nuevo método para verificar si una ventana está abierta
    public bool IsWindowOpen(int prefabIndex)
    {
        string windowName = windowPrefabs[prefabIndex].name + "(Clone)";
        foreach (var window in windows)
        {
            if (window.name == windowName)
            {
                return true;
            }
        }
        return false;
    }
    public void ShowExistingWindow(int existingIndex)
    {
        if (existingIndex < 0 || existingIndex >= existingWindows.Count)
        {
            Debug.LogError("Índice de ventana existente inválido.");
            return;
        }

        GameObject existingWindow = existingWindows[existingIndex];

        // Verificar si la ventana está oculta y no activa
        if (existingWindow != null && !existingWindow.activeSelf)
        {
            // Activar la ventana
            existingWindow.SetActive(true);

            // Verificar si ya está en la lista de ventanas activas, si no, agregarla
            if (!windows.Contains(existingWindow))
            {
                windows.Add(existingWindow);  // Asegúrate de que esté en la lista de ventanas activas
            }

            // Posicionar la ventana
            existingWindow.GetComponent<Ventana>().SetPosition(windowPositions[0], transitionDuration);

            // Reordenar todas las ventanas
            ReorderWindows();
        }
        else if (existingWindow != null && existingWindow.activeSelf)
        {
            Debug.Log("La ventana ya está visible.");
        }
        else
        {
            Debug.LogError("La ventana especificada es nula o no se pudo activar.");
        }
    }


    public void SetWindowVisibility(GameObject window, bool isVisible)
    {
        if (windows.Contains(window))
        {
            window.SetActive(isVisible);  // Cambiar visibilidad
            if (!isVisible)
            {
                // Si la ventana es invisibilizada, se puede reordenar las demás
                ReorderWindows();
            }
        }
        else
        {
            Debug.LogWarning("La ventana especificada no está en la lista de ventanas activas.");
        }
    }


    // Nuevo método para verificar si una ventana existente está abierta
    public bool IsExistingWindowOpen(int existingIndex)
    {
        if (existingIndex < 0 || existingIndex >= existingWindows.Count)
        {
            return false;
        }

        GameObject existingWindow = existingWindows[existingIndex];
        return windows.Contains(existingWindow);
    }
    public GameObject GetWindowByPrefabIndex(int prefabIndex)
    {
        string windowName = windowPrefabs[prefabIndex].name + "(Clone)";
        foreach (var window in windows)
        {
            if (window.name == windowName)
            {
                return window;
            }
        }
        return null;
    }
    #endregion
}