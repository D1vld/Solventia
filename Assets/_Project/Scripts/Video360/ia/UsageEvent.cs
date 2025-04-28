using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AdditionalDataEntry
{
    public string key;
    public string value;

    public AdditionalDataEntry(string key, string value)
    {
        this.key = key;
        this.value = value;
    }
}

[Serializable]
public class AdditionalData
{
    // Utilizamos una lista de pares clave-valor para serializar los datos adicionales.
    public List<AdditionalDataEntry> entries = new List<AdditionalDataEntry>();

    public void AddEntry(string key, string value)
    {
        entries.Add(new AdditionalDataEntry(key, value));
    }
}

[Serializable]
public class UsageEvent
{
    public string eventId;       // Identificador �nico para el evento.
    public string sessionId;     // Identificador de la sesi�n en la que se produjo el evento.
    public string eventName;     // Descripci�n o nombre del evento.
    public string timestamp;     // Marca de tiempo en formato ISO 8601.
    public string objectId;      // Identificaci�n del objeto o UI involucrada.
    public string duration;       // Duraci�n del evento (si es aplicable).
    public AdditionalData additionalData;  // Datos adicionales en formato serializable.

    // Constructor que inicializa el evento y asigna autom�ticamente el GUID y la marca de tiempo.
    public UsageEvent(string sessionId, string eventName, string objectId, string duration = "")
    {
        this.eventId = MetricsHelper.GenerateEventID(sessionId);
        this.sessionId = sessionId;
        this.eventName = eventName;
        this.timestamp = DateTime.UtcNow.ToString("s"); // Formato ISO 8601. "yyyy-MM-ddTHH:mm:ss'Z'"
        this.objectId = objectId;
        this.duration = duration;
        this.additionalData = new AdditionalData();
    }
}
