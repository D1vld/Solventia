using System;
using System.Collections.Generic;

[Serializable]
public class SessionPayload
{
    /// <summary>
    /// Identificador de la sesión.
    /// </summary>
    public string sessionId;

    /// <summary>
    /// Lista de todos los eventos registrados en la sesión.
    /// </summary>
    public List<UsageEvent> events;
}
