using System;
using UnityEngine;

/// <summary>
/// Funciones auxiliares para IDs y nombres de archivo de métricas.
/// </summary>
public static class MetricsHelper
{
    public const string appName = "Cedaga";

    // Contador de eventos para la sesión actual
    private static int eventCounter = 0;

    /// <summary>
    /// Genera un sessionId tipo "Solventia-YYYYMMDD-NNN".
    /// Reinicia el contador de eventos para la nueva sesión.
    /// </summary>
    public static string GenerateSessionID()
    {
        string today = DateTime.UtcNow.ToString("yyyyMMdd");
        string key   = $"SessionCount_{today}";

        int count = PlayerPrefs.GetInt(key, 0) + 1;
        //string randomNumber = UnityEngine.Random.Range(1000, 9999).ToString();
        PlayerPrefs.SetInt(key, count);
        PlayerPrefs.Save();

        eventCounter = 0;                         // ← reset
        return $"{appName}-{today}-{count:D3}";
    }

    /// <summary>
    /// Genera un eventId "evt-YYYYMMDD-<sesión>-<####>".
    /// Garantiza que siempre devuelve un ID válido aunque reciba parámetro nulo.
    /// </summary>
    public static string GenerateEventID(string sessionId)
    {
        // Salvaguarda: sessionId nulo / vacío → generamos uno nuevo
        if (string.IsNullOrEmpty(sessionId))
        {
            Debug.LogWarning("[Metrics] sessionId nulo: se crea uno nuevo on-the-fly.");
            sessionId = GenerateSessionID();
        }

        eventCounter++;

        // Formato esperado: "<app>-YYYYMMDD-NNN"
        string[] parts = sessionId.Split('-');
        if (parts.Length < 3)
        {
            Debug.LogWarning($"[Metrics] sessionId con formato inesperado: {sessionId}");
            string dateFallback   = DateTime.UtcNow.ToString("yyyyMMdd");
            string eventNumFbk    = eventCounter.ToString("D4");
            return $"evt-{dateFallback}-000-{eventNumFbk}";
        }

        string datePart   = parts[1];
        string sessionNum = parts[2];
        string eventNum   = eventCounter.ToString("D4");

        return $"evt-{datePart}-{sessionNum}-{eventNum}";
    }

    /// <summary>
    /// Devuelve "metrics_SolventiaYYYY-MM-DD.json" (zona UTC).
    /// </summary>
    public static string GetCurrentDateFilename()
    {
        return "metrics_" + appName + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".json";
    }
}