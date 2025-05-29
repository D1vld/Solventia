using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// SyncManager: Clase para sincronizar m�tricas diarias con el backend.
/// </summary>
public class SyncManager : MonoBehaviour
{
    #region Inspector Fields
    [Header("Sync Configuration")]
    [Tooltip("URL base del servicio de m�tricas.")]
    [SerializeField] private string hostUrl = "https://data.test.tripilots.com";
    [Tooltip("Clave API para autenticaci�n con el endpoint.")]
    [SerializeField] private string apiKey;
    [Tooltip("Plataforma.")]
    [SerializeField] private string platform;
    [Tooltip("Versi�n de compilaci�n de la aplicaci�n.")]
    [SerializeField] private string buildVersion;
    [Tooltip("Entorno (dev, stage, prod).")]
    [SerializeField] private string environment;
    [Tooltip("Intervalo en segundos para reintentar la sincronizaci�n si falla.")]
    [SerializeField] private float retryInterval = 60f;
    [Tooltip("Check true sincroniza ficheros de metricas, check false no los sincroniza para debug.")]
    [SerializeField] private bool syncFiles;
    [SerializeField] private MetricsManager _metricsManager;
    #endregion

    private DateTime lastSuccessfulSyncDate;
    private const string LastSyncDateKey = "LastSyncDate";
    private Coroutine syncCoroutine;

    private void Awake()
    {
        if (PlayerPrefs.HasKey(LastSyncDateKey))
        {
            DateTime.TryParse(PlayerPrefs.GetString(LastSyncDateKey), out lastSuccessfulSyncDate);
        }
        else
        {
            lastSuccessfulSyncDate = DateTime.MinValue;
        }

    }


    /// <summary>
    /// Inicia la rutina de sincronizaci�n diaria.
    /// </summary>
    public void StartSync()

    {

        if (syncCoroutine == null && syncFiles)

        {

            syncCoroutine = StartCoroutine(SyncRoutine());

        }

    }
 

    /// <summary>
    /// Coroutine que comprueba cada retryInterval y lanza la sincronizaci�n
    /// si a�n no se ha hecho en el d�a actual.
    /// </summary>
    private IEnumerator SyncRoutine()
    {
        while (true)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                //if (DateTime.UtcNow.Date > lastSuccessfulSyncDate.Date)
                //{
                    yield return StartCoroutine(SyncFiles());
                //}
            }
            yield return new WaitForSeconds(retryInterval);
        }
    }

    /// <summary>
    /// Escanea el directorio local y sincroniza archivos .json pendientes,
    /// elimin�ndolos tras una subida exitosa. Excluye el fichero del d�a actual.
    /// </summary>
    private IEnumerator SyncFiles()
    {
        string dataPath = Application.persistentDataPath;
        string[] files = Directory.GetFiles(dataPath, "*.json");
        string todayString = DateTime.UtcNow.ToString("yyyy-MM-dd");

        foreach (string filePath in files)
        {
            string fileName = Path.GetFileName(filePath);

            // Excluir el fichero del d�a actual
            if (fileName.Contains(todayString))
                continue;

            // Excluir ficheros que NO correspondan a esta app
            if (!fileName.Contains(MetricsHelper.appName))
            continue;
            // Leer contenido del fichero
            byte[] fileData;
            try
            {
                fileData = File.ReadAllBytes(filePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error leyendo el fichero {fileName}: {e}");
                continue;
            }

            // Preparar formulario multipart/form-data
            WWWForm form = new WWWForm();
            form.AddField("appName", MetricsHelper.appName);
            if (!string.IsNullOrEmpty(platform))
                form.AddField("platform", platform);
            if (!string.IsNullOrEmpty(buildVersion))
                form.AddField("buildVersion", buildVersion);
            if (!string.IsNullOrEmpty(environment))
                form.AddField("env", environment);
            form.AddBinaryData("file", fileData, fileName, "application/json");

            // Enviar petici�n
            using (UnityWebRequest request = UnityWebRequest.Post($"{hostUrl}/metric", form))
            {
                request.SetRequestHeader("x-api-key", apiKey);
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success && request.responseCode >= 200 && request.responseCode < 300)
                {
                    Debug.Log($"Fichero subido correctamente: {fileName}");
                    try
                    {
                        File.Delete(filePath);
                       // _metricsManager?.LogUsageEvent("SyncFileMetrics", fileName, "0");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Error al eliminar el fichero {fileName}: {ex}");
                    }
                    
                }
                else
                {
                    Debug.LogError($"Error al subir el fichero {fileName}: {request.error}");
                    yield break;
                }
            }
        }

        // Actualizar fecha de �ltima sincronizaci�n exitosa
        lastSuccessfulSyncDate = DateTime.UtcNow.Date;
        PlayerPrefs.SetString(LastSyncDateKey, lastSuccessfulSyncDate.ToString("yyyy-MM-dd"));
        PlayerPrefs.Save();

        yield return null;
    }
}
