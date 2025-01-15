using UnityEngine;


public class PassthroughOpacityController : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;
    public float transitionDuration = 1.0f; // Duración de la transición en segundos

    void Start()
    {
        // Buscar el objeto con el tag "OFF"
        GameObject passthroughObject = GameObject.FindGameObjectWithTag("Off");

        if (passthroughObject != null)
        {
            // Asignar el componente OVRPassthroughLayer
            passthroughLayer = passthroughObject.GetComponent<OVRPassthroughLayer>();

            if (passthroughLayer == null)
            {
                Debug.LogError("El objeto con tag 'OFF' no tiene el componente OVRPassthroughLayer.");
            }
        }
        else
        {
            Debug.LogError("No se encontró ningún objeto con el tag 'off'.");
        }
    }

    // Método que puedes llamar desde un botón para cambiar la opacidad
    public void ChangeOpacity(float targetOpacity)
    {
        if (passthroughLayer != null)
        {
            StartCoroutine(LerpOpacity(targetOpacity));
        }
    }

    // Coroutine para hacer la transición de opacidad de forma suave
    private System.Collections.IEnumerator LerpOpacity(float targetOpacity)
    {
        float initialOpacity = passthroughLayer.textureOpacity;
        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float newOpacity = Mathf.Lerp(initialOpacity, targetOpacity, elapsedTime / transitionDuration);
            passthroughLayer.textureOpacity = newOpacity;
            yield return null; // Esperar al siguiente frame
        }

        // Asegurarse de que al final llegue al valor exacto
        passthroughLayer.textureOpacity = targetOpacity;
    }
}
