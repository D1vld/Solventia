using System;
using System.Collections.Generic;

[Serializable]
public class SessionPayload
{
    /// <summary>
    /// Identificador de la sesi�n.
    /// </summary>
    public string sessionId;

    /// <summary>
    /// Lista de todos los eventos registrados en la sesi�n.
    /// </summary>
    public List<UsageEvent> events;
}
public class SessionPayloadList

{

    public List<SessionPayload> sessions = new List<SessionPayload>();

}
 
