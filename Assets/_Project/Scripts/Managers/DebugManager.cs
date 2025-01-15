using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    [SerializeField] List<DebuggableSerializable> _Debuggables;

    internal void Initialize()
    {
        foreach (var debuggable in _Debuggables)
        {
            debuggable._Debuggable.SetDebugCalls(debuggable._DebugCalls);
            debuggable.SetOnDebugCallsChangedCallbackSub(true, debuggable._Debuggable.SetDebugCalls);
        }
    }
    internal void Dispose()
    {
        foreach (var debuggable in _Debuggables)
            debuggable.SetOnDebugCallsChangedCallbackSub(false, debuggable._Debuggable.SetDebugCalls);
    }

    void OnValidate()
    {
        foreach (var debuggable in _Debuggables)
            debuggable.Validate();
    }
    
    
    [System.Serializable]
    class DebuggableSerializable
    {
        [SerializeField] internal Debuggable _Debuggable;
        [SerializeField] internal bool _DebugCalls;
        
        bool _debugCallsValidated;
        System.Action<bool> _onDebugCallsChanged;

        internal void Validate()
        {
            if (_DebugCalls == _debugCallsValidated) return;
            _onDebugCallsChanged?.Invoke(_debugCallsValidated = _DebugCalls);
        }
    
        internal void SetOnDebugCallsChangedCallbackSub(bool subscribe, System.Action<bool> callback)
        {
            _onDebugCallsChanged -= callback;
            if (subscribe) _onDebugCallsChanged += callback;
        }
    }
}

public class Debuggable : MonoBehaviour
{
    internal bool _DebugCalls
    {
        get;
        private set;
    }

    internal void SetDebugCalls(bool debugCalls)
    {
        _DebugCalls = debugCalls;
    }
    
    protected void ShowDebugCall(string debugMessage)
    {
        if (_DebugCalls) Debug.Log(debugMessage);
    }
}