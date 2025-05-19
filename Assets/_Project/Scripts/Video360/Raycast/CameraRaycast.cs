using UnityEngine;
using UnityEngine.UI;

public class CameraRaycast : MonoBehaviour
{
    public float rayDistance = 100f;
    public RectTransform maskRect1;
    public RectTransform maskRect2;
    public Camera camera;

    // La imagen cuyo color se cambiará
    public Image imageToChangeColor;

    // Variables para objetos con tag "Toggle" (se activan a los 3 seg)
    private float lookTime = 0f;
    private Toggle currentToggle = null;
    private bool isLookingAtToggle = false;

    // Variables para objetos con tag "Toggle2" (ejecutan el botón cada 0.5 seg)    
    private const float autoFireInterval = 0.50f;   // cada medio segundo
    private float lookTime2 = 0f;
    private Toggle currentToggle2 = null;           // ← ¡ahora Toggle!
    private bool isLookingAtToggle2 = false;

    private float noHitTimer = 0f;
    private const float noHitTimeout = 2f;

    void Update()
    {
        if (camera == null)
        {
            Debug.LogError("Cámara no asignada en el Inspector.");
            return;
        }

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, rayDistance))
        {
            noHitTimer = 0f;

            // Ignoramos objetos llamados "panelInteractable"
            if (hit.collider.gameObject.name == "panelInteractable")
                return;

            // Si el objeto tiene el tag "Toggle", se activa al mirar 3 segundos
            if (hit.collider.gameObject.CompareTag("Toggle"))
            {
                Toggle toggleComp = hit.collider.gameObject.GetComponent<Toggle>();
                if (toggleComp != null)
                {
                    if (currentToggle != toggleComp)
                    {
                        if (currentToggle != null && currentToggle.isOn)
                        {
                            currentToggle.isOn = false;
                        }
                        currentToggle = toggleComp;
                        lookTime = 0f;
                    }

                    lookTime += Time.deltaTime;
                    if (lookTime >= 3f && !toggleComp.isOn)
                    {
                        toggleComp.isOn = true;
                    }
                    isLookingAtToggle = true;

                    // Cambiar el color de la imagen a rojo cuando se está mirando el toggle
                    if (imageToChangeColor != null)
                    {
                        imageToChangeColor.color = Color.red;
                    }
                }
            }
            // Si el objeto tiene el tag "Toggle2", se ejecuta su botón
            // y se vuelve a activar cada 0.5 segundos mientras se mantenga la mirada
            else if (hit.collider.gameObject.CompareTag("Toggle2"))
            {
                Toggle tgl = hit.collider.GetComponent<Toggle>();
                if (tgl != null)
                {
                    // Nuevo toggle al que miramos
                    if (currentToggle2 != tgl)
                    {
                        currentToggle2 = tgl;
                        lookTime2 = 0f;
                        // ──► Primera ejecución inmediata
                        tgl.onValueChanged.Invoke(tgl.isOn);
                    }

                    // Mientras se mantenga la mirada, volver a lanzar el evento
                    lookTime2 += Time.deltaTime;
                    if (lookTime2 >= autoFireInterval)
                    {
                        tgl.onValueChanged.Invoke(tgl.isOn);
                        lookTime2 = 0f;
                    }

                    isLookingAtToggle2 = true;

                    if (imageToChangeColor != null)
                        imageToChangeColor.color = Color.red;
                }
            }
            /*else
            {
                // Si se mira a otro objeto, reiniciamos ambos estados
                if (isLookingAtToggle && currentToggle != null && currentToggle.isOn)
                {
                    currentToggle.isOn = false;
                }
                currentToggle = null;
                lookTime = 0f;
                isLookingAtToggle = false;

                if (isLookingAtToggle2 && currentButton2 != null)
                {
                    currentButton2 = null;
                    lookTime2 = 0f;
                    isLookingAtToggle2 = false;
                }

                // Si no se está mirando un "Toggle" o "Toggle2", revertir el color de la imagen a blanco
                if (imageToChangeColor != null)
                {
                    imageToChangeColor.color = Color.white;
                }
            }*/
        }
        else
        {
            // Si el raycast no toca nada, aumentamos el temporizador y reiniciamos los estados tras 2 seg.
            noHitTimer += Time.deltaTime;
            if (noHitTimer >= noHitTimeout)
            {
                if (currentToggle != null && currentToggle.isOn)
                {
                    currentToggle.isOn = false;
                }
                currentToggle = null;
                lookTime = 0f;
                isLookingAtToggle = false;

                lookTime2 = 0f;
                isLookingAtToggle2 = false;

                // Si no se está mirando un "Toggle" o "Toggle2", revertir el color de la imagen a blanco
                if (imageToChangeColor != null)
                {
                    imageToChangeColor.color = Color.white;
                }
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

        return IsInsideMask(maskRect1, corners) || IsInsideMask(maskRect2, corners);
    }

    bool IsInsideMask(RectTransform maskRect, Vector3[] corners)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector3 screenPos = camera.WorldToScreenPoint(corners[i]);
            if (RectTransformUtility.RectangleContainsScreenPoint(maskRect, screenPos))
                return true;
        }
        return false;
    }
}
