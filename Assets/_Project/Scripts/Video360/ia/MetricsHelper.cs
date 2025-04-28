using System;
using UnityEngine;

public static class MetricsHelper
{
    public const string appName = "Solventia";

    // Contador de eventos para la sesi�n actual
    private static int eventCounter = 0;

    /// <summary>
    /// Genera un sessionId tipo "NombreProyecto-YYYYMMDD-N".
    /// Reinicia el contador de eventos para la nueva sesi�n.
    /// </summary>
    public static string GenerateSessionID()
    {
        // 1. Fecha en formato YYYYMMDD
        string today = DateTime.UtcNow.ToString("yyyyMMdd");

        // 2. Clave en PlayerPrefs para contar sesiones diarias
        string key = $"SessionCount_{today}";

        // 3. Incrementa contador (valor por defecto 0)
        int count = PlayerPrefs.GetInt(key, 0) + 1;
        PlayerPrefs.SetInt(key, count);
        PlayerPrefs.Save();

        // 4. REINICIAMOS contador de eventos al comenzar nueva sesi�n
        eventCounter = 0;  // <-- MODIFICACI�N para eventCounter

        // 5. Construye el sessionId
        return $"{appName}-{today}-{count:D3}";
    }

    /// <summary>
    /// Genera un eventId basado en fecha, n�mero de sesi�n y un incremental:
    /// "evt-YYYYMMDD-<sesi�n>-<evento 4 d�gitos>".
    /// </summary>
    public static string GenerateEventID(string sessionId)
    {
        // 1. Incrementamos el contador de eventos
        eventCounter++;   // <-- MODIFICACI�N para incrementar evento

        // 2. Extraemos fecha y n�mero de sesi�n de sessionId
        //    Formato espera: "NombreProyecto-YYYYMMDD-N"
        string[] parts = sessionId.Split('-');
        string datePart = parts[1]; // "YYYYMMDD"
        string sessionNum = parts[2]; // "N"

        // 3. Formateamos el incremental a 4 d�gitos con ceros a la izquierda
        string eventNum = eventCounter.ToString("D4");

        // 4. Construye el eventId final
        return $"evt-{datePart}-{sessionNum}-{eventNum}";  // <-- MODIFICACI�N en construcci�n de eventId
    }

    // Devuelve el nombre del archivo para el d�a actual en formato "metrics-YYYY-MM-DD.json".
    public static string GetCurrentDateFilename()
    {
        return "metrics_"+appName + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".json";
    }
}
