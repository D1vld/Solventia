using System;

public static class MetricsHelper
{
    // Genera un identificador único para la sesión utilizando GUID.
    public static string GenerateSessionID()
    {
        return Guid.NewGuid().ToString();
    }

    // Devuelve el nombre del archivo para el día actual en formato "metrics-YYYY-MM-DD.json".
    public static string GetCurrentDateFilename()
    {
        return "metrics-" + DateTime.UtcNow.ToString("yyyy-MM-dd") + ".json";
    }
}
