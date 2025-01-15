using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NotificationSender : MonoBehaviour
{
    public GameObject activePrefab; // El prefab que está activo o con el que estás interactuando.
    
    private Dictionary<string, int> prefabNameToIdMap = new Dictionary<string, int>
    {
        { "UI_Bao_CarruselModular", 1 },
        { "UI_CMelo_CarruselModular", 2 },
        { "UI_VC_CarruselModular", 3 },
        { "UI_Faro_CarruselModular", 4 },
        { "UI_CDuna_CarruselModular", 5 },
        { "UI_ACont_CarruselModular", 6 },
        { "UI_ACata_CarruselModular", 7 },
        { "UI_ABuena_CarruselModular", 8 },
        { "UI_CBeach_CarruselModular", 9 },
        { "UI_Alnter_CarruselModular", 10 }
    };

    public void SendNotificationToAgent()
    {
        StartCoroutine(SendNotificationRequest());
    }

    private IEnumerator SendNotificationRequest()
    {
        if (activePrefab == null)
        {
            Debug.LogError("No se ha asignado ningún prefab activo.");
            yield break;
        }

        // Obtener el nombre del prefab activo
        string prefabName = activePrefab.name.Replace("(Clone)", "").Trim();

        // Obtener el prefabIndex del diccionario
        if (!prefabNameToIdMap.TryGetValue(prefabName, out int prefabIndex))
        {
            Debug.LogError("No se encontró un ID para el prefab: " + prefabName);
            yield break;
        }

        // Enviar la solicitud de notificación con el prefabIndex obtenido
        string url = $"https://triportation-backend.onrender.com/environments/{prefabIndex}/notify";
        UnityWebRequest request = new UnityWebRequest(url, "PATCH");
        request.SetRequestHeader("Content-Type", "application/json");

        // Crear el JSON para la solicitud
        string json = JsonUtility.ToJson(new { NotificationType = "usuario" });
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Enviar la solicitud y esperar la respuesta
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Notificación enviada exitosamente.");
        }
        else
        {
            Debug.LogError("Error al enviar la notificación: " + request.error);
        }
    }
}
