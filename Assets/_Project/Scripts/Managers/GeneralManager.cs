using UnityEngine;

public class GeneralManager : MonoBehaviour
{
    // Managers
    public ConfigManager configManager;
    public UiManager uiManager;
    public ApiManager apiManager;
    public AnalyticsManager analyticsManager;
    public InputManager inputManager;
    public DebugManager debugManager;

    // Singleton
    private static GeneralManager _Instance;
    public static GeneralManager Instance
    {
        get
        {
            return _Instance;
        }
    }

    #region Public Methods

    public void SessionRestart()
    {
        analyticsManager.EndSession();
    }

    #endregion

    #region Private Methods
    private void Start()
    {
        if (_Instance != null)
        {
            Destroy(this);
            return;
        }
        _Instance = this;
        Initialize();
    }

    private void Initialize()
    {
        configManager.Initialize();
        uiManager.Initialize();
        apiManager.Initialize();
        inputManager.Initialize();
        debugManager.Initialize();
        analyticsManager.Initialize();
    }

    private void OnApplicationQuit()
    {
        analyticsManager.EndSession();
        debugManager.Dispose();
    }
    #endregion
}