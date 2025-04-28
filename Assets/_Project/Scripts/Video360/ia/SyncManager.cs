using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class SyncManager : MonoBehaviour
{
    // URL del endpoint al que se enviarán los datos.
    public string endpointUrl = "https://tu-dominio/data-service/metrics";
    // Intervalo de tiempo (en segundos) entre cada intento de sincronización.
    public float syncInterval = 60f;

    private void Start()
    {
        // Inicia la rutina de sincronización periódica.
        StartCoroutine(SyncRoutine());
    }

    /// <summary>
    /// Coroutine que, en intervalos definidos, intenta sincronizar los ficheros pendientes.
    /// </summary>
    IEnumerator SyncRoutine()
    {
        while (true)
        {
            // Verificar si hay conexión a Internet.
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                yield return StartCoroutine(SyncFiles());
            }
            yield return new WaitForSeconds(syncInterval);
        }
    }

    /// <summary>
    /// Escanea el directorio local, envía los ficheros pendientes y elimina aquellos que ya se subieron correctamente.
    /// Se excluyen el fichero del día actual y, opcionalmente, el del día anterior.
    /// </summary>
    IEnumerator SyncFiles()
    {
        // Obtiene la ruta donde se almacenan los ficheros.
        string dataPath = Application.persistentDataPath;
        // Busca todos los ficheros que sigan el patrón "metrics-*.json".
        string[] files = Directory.GetFiles(dataPath, "metrics-*.json");

        // Define el nombre del fichero del día actual y del día anterior.
        string todayFile = MetricsHelper.GetCurrentDateFilename();
        string yesterdayFile = "metrics-" + DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd") + ".json";

        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);

            // Se excluyen el fichero de hoy y el de ayer, ya que se mantienen localmente según la especificación.
            if (fileName.Equals(todayFile) || fileName.Equals(yesterdayFile))
            {
                continue;
            }

            // Lee el contenido del fichero a enviar.
            string fileContent;
            try
            {
                fileContent = File.ReadAllText(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError("Error leyendo el fichero " + fileName + ": " + e);
                continue;
            }

            // Configura la petición POST.
            UnityWebRequest request = new UnityWebRequest(endpointUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(fileContent);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            // Envía la petición y espera la respuesta.
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error al subir el fichero " + fileName + ": " + request.error);
                // Si hay error, no se elimina el fichero; se intentará enviarlo en el siguiente ciclo.
            }
            else
            {
                Debug.Log("Fichero subido correctamente: " + fileName);
                // Si la subida es exitosa, se elimina el fichero local.
                try
                {
                    File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error al eliminar el fichero " + fileName + ": " + ex);
                }
            }
        }
        yield return null;
    }
}
