using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// MetricsManager: Clase singleton para capturar y gestionar el buffer de UsageEvent.
/// </summary>
public class MetricsManager : MonoBehaviour
{
    // Buffer de eventos en memoria.
    private List<UsageEvent> eventBuffer = new List<UsageEvent>();

    // Identificador de la sesión actual y el nombre del fichero.
    private string sessionId;
    private string currentFilename;

    // Configuraciones para el volcado del buffer.
    [SerializeField]
    private float flushInterval = 10f;        // Tiempo (segundos) para volcar el buffer periódicamente.
    [SerializeField]
    private int maxBufferSize = 10;             // Tamaño máximo del buffer para disparar un volcado inmediato.

    private void Awake()
    {
        // Inicializar el sessionId y el nombre del fichero (basados en la fecha actual).
        sessionId = MetricsHelper.GenerateSessionID();
        currentFilename = MetricsHelper.GetCurrentDateFilename();
    }
    private void Start()
    {
        // Iniciar la Coroutine de volcado periódico.
        StartCoroutine(FlushBufferPeriodically());
    }

    /// <summary>
    /// Registra un nuevo evento y lo agrega al buffer.
    /// </summary>
    /// <param name="eventName">Nombre o descripción del evento (ej: SessionStarted, ButtonClicked).</param>
    /// <param name="objectId">Identificador del objeto o UI afectada.</param>
    /// <param name="duration">Duración del evento (0 si no aplica).</param>
    public void LogUsageEvent(string eventName, string objectId, float duration = 0f)
    {
        // Crear una instancia de UsageEvent con la información requerida.
        UsageEvent newEvent = new UsageEvent(sessionId, eventName, objectId, duration);
        eventBuffer.Add(newEvent);

        // Verificar si se alcanzó el tamaño máximo del buffer.
        if (eventBuffer.Count >= maxBufferSize)
        {
            // Iniciar un volcado inmediato si el buffer es demasiado grande.
            StartCoroutine(FlushBuffer());
        }
    }

    /// <summary>
    /// Coroutine que periódicamente revisa y vacía el buffer en disco.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlushBufferPeriodically()
    {
        while (true)
        {
            // Espera el intervalo configurado.
            yield return new WaitForSeconds(flushInterval);

            // Si hay eventos en el buffer, volcarlos a disco.
            if (eventBuffer.Count > 0)
            {
                yield return StartCoroutine(FlushBuffer());
            }
        }
    }

    /// <summary>
    /// Coroutine encargada de vaciar el buffer y escribir los eventos en el fichero diario.
    /// </summary>
    /// <returns></returns>
    private IEnumerator FlushBuffer()
    {
        // Copia del buffer y limpieza inmediata para evitar pérdidas si la escritura se retrasa.
        List<UsageEvent> eventsToFlush = new List<UsageEvent>(eventBuffer);
        eventBuffer.Clear();

        // Construir la ruta completa del fichero usando Application.persistentDataPath.
        string filePath = Path.Combine(Application.persistentDataPath, currentFilename);

        // Realizar la escritura en disco en modo append.
        try
        {
            using (StreamWriter sw = new StreamWriter(filePath, true))
            {
                foreach (UsageEvent usageEvent in eventsToFlush)
                {
                    string jsonStr = JsonUtility.ToJson(usageEvent);
                    sw.WriteLine(jsonStr); // Cada línea representa un evento en formato JSON.
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error al volcar eventos a disco: " + e);
            // Si ocurre un error, se reincorporan los eventos al inicio del buffer.
            eventBuffer.InsertRange(0, eventsToFlush);
        }

        yield return null;
    }

    /// <summary>
    /// Se asegura que se vacíen los eventos pendientes cuando se cierra la aplicación.
    /// </summary>
    private void OnApplicationQuit()
    {
        if (eventBuffer.Count > 0)
        {
            string filePath = Path.Combine(Application.persistentDataPath, currentFilename);
            try
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    foreach (UsageEvent usageEvent in eventBuffer)
                    {
                        string jsonStr = JsonUtility.ToJson(usageEvent);
                        sw.WriteLine(jsonStr);
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error al vaciar eventos en OnApplicationQuit: " + e);
            }
        }
    }
}
