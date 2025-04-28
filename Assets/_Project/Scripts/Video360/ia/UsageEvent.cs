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
    public string eventId;       // Identificador único para el evento.
    public string sessionId;     // Identificador de la sesión en la que se produjo el evento.
    public string eventName;     // Descripción o nombre del evento.
    public string timestamp;     // Marca de tiempo en formato ISO 8601.
    public string objectId;      // Identificación del objeto o UI involucrada.
    public float duration;       // Duración del evento (si es aplicable).
    public AdditionalData additionalData;  // Datos adicionales en formato serializable.

    // Constructor que inicializa el evento y asigna automáticamente el GUID y la marca de tiempo.
    public UsageEvent(string sessionId, string eventName, string objectId, float duration = 0f)
    {
        this.eventId = Guid.NewGuid().ToString();
        this.sessionId = sessionId;
        this.eventName = eventName;
        this.timestamp = DateTime.UtcNow.ToString("o"); // Formato ISO 8601.
        this.objectId = objectId;
        this.duration = duration;
        this.additionalData = new AdditionalData();
    }
}
