using UnityEngine;
using UnityEngine.UI;

public class CameraRaycast : MonoBehaviour
{
    public float rayDistance = 100f;
    public RectTransform maskRect1;
    public RectTransform maskRect2;
    public Camera camera;

    private float lookTime = 0f;  // Tiempo que el jugador ha estado mirando al toggle
    private Toggle currentToggle = null;  // El toggle actual que el jugador está mirando
    private bool isLookingAtToggle = false;  // Flag para verificar si estamos mirando un toggle
    private float noHitTimer = 0f;  // Temporizador de inactividad para cuando el raycast no toca nada
    private const float noHitTimeout = 2f;  // Tiempo de espera sin tocar nada antes de reiniciar

    void Update()
    {
        if (camera == null)
        {
            Debug.LogError("Cámara no asignada en el Inspector.");
            return;
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        // Restablecemos el temporizador de inactividad si tocamos algo
        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            noHitTimer = 0f;  // Reiniciamos el temporizador cuando se detecta un objeto

            // Ignoramos objetos llamados "panelInteractable"
            if (hit.collider.gameObject.name == "panelInteractable")
            {
                return;
            }

            // Si el objeto tocado es un Toggle, iniciamos el temporizador
            if (hit.collider.gameObject.CompareTag("Toggle"))
            {
                Toggle toggleComp = hit.collider.gameObject.GetComponent<Toggle>();
                if (toggleComp != null)
                {
                    // Si es un nuevo toggle, reiniciamos el temporizador
                    if (currentToggle != toggleComp)
                    {
                        // Si hay un toggle previamente activado, desactivamos y reiniciamos el temporizador
                        if (currentToggle != null && currentToggle.isOn)
                        {
                            currentToggle.isOn = false;
                        }

                        currentToggle = toggleComp;
                        lookTime = 0f;  // Reiniciamos el temporizador al mirar un nuevo toggle
                    }

                    // Aumentamos el tiempo mirando el toggle
                    lookTime += Time.deltaTime;

                    // Si ha pasado 3 segundos mirando el toggle, activamos el toggle
                    if (lookTime >= 3f && !toggleComp.isOn)
                    {
                        toggleComp.isOn = true;
                    }

                    // Marcamos que estamos mirando un toggle
                    isLookingAtToggle = true;
                }
            }
            else
            {
                // Si miramos a otro objeto que no es un Toggle y no estamos mirando a un toggle, desactivamos el toggle y reiniciamos el temporizador
                if (isLookingAtToggle && currentToggle != null && currentToggle.isOn)
                {
                    currentToggle.isOn = false;  // Desactivamos el toggle si estaba activado
                }

                currentToggle = null;  // Reiniciamos el toggle actual
                lookTime = 0f;  // Reiniciamos el temporizador
                isLookingAtToggle = false;  // Indicamos que ya no estamos mirando un toggle
            }

            // Si el objeto tocado es un botón, comprobamos si está dentro de las áreas visibles de las máscaras
            Button button = hit.collider.gameObject.GetComponent<Button>();
            if (button != null && IsObjectInVisibleArea(hit.collider.gameObject))
            {
                button.onClick.Invoke();
            }
        }
        else
        {
            // Si el raycast no toca nada, aumentamos el temporizador de inactividad
            noHitTimer += Time.deltaTime;

            // Si han pasado 2 segundos sin tocar nada, reiniciamos el estado del toggle
            if (noHitTimer >= noHitTimeout)
            {
                currentToggle = null;  // Reiniciamos el toggle actual
                lookTime = 0f;  // Reiniciamos el temporizador
                isLookingAtToggle = false;  // Indicamos que ya no estamos mirando un toggle
            }
        }
    }

    bool IsObjectInVisibleArea(GameObject obj)
    {
        RectTransform rectTransform = obj.GetComponent<RectTransform>();
        if (rectTransform == null)
            return false;

        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);

        if (IsInsideMask(maskRect1, corners) || IsInsideMask(maskRect2, corners))
        {
            return true;
        }

        return false;
    }

    bool IsInsideMask(RectTransform maskRect, Vector3[] corners)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(corners[i]);
            if (RectTransformUtility.RectangleContainsScreenPoint(maskRect, screenPos))
            {
                return true;
            }
        }
        return false;
    }
}
