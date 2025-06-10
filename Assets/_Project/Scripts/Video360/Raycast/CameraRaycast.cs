using UnityEngine;
using UnityEngine.UI;

public class CameraRaycast : MonoBehaviour
{
    public float rayDistance = 100f;
    public RectTransform maskRect1;
    public RectTransform maskRect2;
    public Camera camera;

    public Image imageToChangeColor;

    private float lookTime = 0f;
    private Toggle currentToggle = null;
    private bool isLookingAtToggle = false;

    private const float autoFireInterval = 0.50f;
    private float lookTime2 = 0f;
    private Toggle currentToggle2 = null;
    private bool isLookingAtToggle2 = false;

    private bool canInvoke = true;  // New flag to control invocation

    void Update()
    {
        if (camera == null) return;

        Ray ray = new Ray(camera.transform.position, camera.transform.forward);
        RaycastHit hit;

        //-----►  Si NO tocamos nada o tocamos algo que NO sea Toggle / Toggle2
        bool hitSomething = Physics.Raycast(ray, out hit, rayDistance);
        if (!hitSomething ||                                  // nada
            !(hit.collider.CompareTag("Toggle") ||
              hit.collider.CompareTag("Toggle2")))            // otro tag
        {
            ResetState();
            return;                                           // ¡salimos ya!
        }

        //-----►  A partir de aquí sabemos que SÍ es Toggle o Toggle2
        if (hit.collider.CompareTag("Toggle"))
        {
            Toggle tgl = hit.collider.GetComponent<Toggle>();
            if (tgl != null)
            {
                if (currentToggle != tgl)
                {
                    if (currentToggle != null && currentToggle.isOn)
                        currentToggle.isOn = false;

                    currentToggle = tgl;
                    lookTime = 0f;
                }

                lookTime += Time.deltaTime;
                if (lookTime >= 3f && !tgl.isOn) tgl.isOn = true;

                isLookingAtToggle = true;
                if (imageToChangeColor) imageToChangeColor.color = Color.red;
            }
        }
        else /* Toggle2 */
        {
            Toggle tgl = hit.collider.GetComponent<Toggle>();
            if (tgl != null)
            {
                if (currentToggle2 != tgl)
                {
                    currentToggle2 = tgl;
                    lookTime2 = 0f;
                    tgl.onValueChanged.Invoke(tgl.isOn);      // primera llamada
                }

                lookTime2 += Time.deltaTime;

                if (lookTime2 >= autoFireInterval && canInvoke)
                {
                    tgl.onValueChanged.Invoke(tgl.isOn);
                    lookTime2 = 0f;
                    canInvoke = false;  // Prevent further invocation
                }
            }

            // Reset flag when the look time is reset
            if (lookTime2 < autoFireInterval)
            {
                canInvoke = true;  // Allow invocation again once the timer is reset
            }
        }
    }

    //-----►  Todo el reseteo en un único sitio
    void ResetState()
    {
        if (currentToggle && currentToggle.isOn) currentToggle.isOn = false;
        currentToggle = null;
        currentToggle2 = null;

        isLookingAtToggle = false;
        isLookingAtToggle2 = false;

        lookTime = 0f;
        lookTime2 = 0f;

        if (imageToChangeColor) imageToChangeColor.color = Color.white;
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