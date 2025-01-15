using UnityEngine;

namespace Template360
{
    public class GeneralManager : MonoBehaviour
    {
        public static GeneralManager Instance { get; private set; }

        [SerializeField] private UIManager _UIManager;
        [SerializeField] private VideoManager _VideoManager;
        public UIManager UI => _UIManager;
        public VideoManager VideoManager => _VideoManager;
        // Add here any other manager


        // Start is called before the first frame update
        void Awake()
        {
            Initialize();
        }

        void Initialize()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            _UIManager.Initialize();
            _VideoManager.Initialize();
        }
    }
}