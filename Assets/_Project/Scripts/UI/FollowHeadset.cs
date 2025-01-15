using System.Collections;
using UnityEngine;

public class FollowHeadset : MonoBehaviour
{
    public Transform headset; // La transform de la cámara o del casco de VR
    public string headsetTag = "MainCamera"; // Tag para buscar el transform del headset
    public float smoothTime = 0.3f; // El tiempo de suavizado
    public Vector3 offset; // Desplazamiento de la interfaz desde la cámara
    public float rotationDelay = 0.5f; // Tiempo de delay para la rotación en segundos

    private Vector3 velocity = Vector3.zero;
    private Quaternion targetRotation;
    private bool isRotationDelayed = false;
    private Quaternion lastHeadsetRotation;

    // Variable para controlar si el seguimiento está activo o no
    public bool isFollowing = true;

    void Start()
    {
        // Buscar el transform del headset utilizando el tag
        if (headset == null && !string.IsNullOrEmpty(headsetTag))
        {
            GameObject headsetObject = GameObject.FindGameObjectWithTag(headsetTag);
            if (headsetObject != null)
            {
                headset = headsetObject.transform;
            }
            else
            {
                Debug.LogError("No se encontró ningún GameObject con el tag: " + headsetTag);
            }
        }

        if (headset != null)
        {
            // Inicializar la rotación objetivo con la rotación actual del headset
            targetRotation = Quaternion.LookRotation(headset.forward);
            lastHeadsetRotation = headset.rotation;
        }
    }

    void Update()
    {
        if (isFollowing && headset != null)
        {
            // Calcula la posición objetivo con el desplazamiento
            Vector3 targetPosition = headset.position + headset.forward * offset.z + headset.right * offset.x + headset.up * offset.y;

            // Suaviza la posición de la interfaz hacia la posición objetivo
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);

            // Detectar el cambio de rotación del casco
            if (Quaternion.Angle(headset.rotation, lastHeadsetRotation) > 1f && !isRotationDelayed)
            {
                StartCoroutine(DelayedRotation());
                lastHeadsetRotation = headset.rotation;
            }
        }
    }

    IEnumerator DelayedRotation()
    {
        isRotationDelayed = true;

        // Esperar el tiempo de delay
        yield return new WaitForSeconds(rotationDelay);

        // Actualizar la rotación objetivo después del delay
        targetRotation = Quaternion.LookRotation(headset.forward);

        // Aplicar la rotación suavizada
        float elapsedTime = 0f;
        Quaternion startingRotation = transform.rotation;
        while (elapsedTime < smoothTime)
        {
            transform.rotation = Quaternion.Slerp(startingRotation, targetRotation, (elapsedTime / smoothTime));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Asegurarse de que la rotación final sea exacta
        transform.rotation = targetRotation;

        isRotationDelayed = false;
    }

    // Método para activar/desactivar el seguimiento desde otro script
    public void ToggleFollowing(bool shouldFollow)
    {
        isFollowing = shouldFollow;
    }
}
