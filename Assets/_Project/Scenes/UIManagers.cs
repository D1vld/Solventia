using UnityEngine;
using TMPro;

namespace Template360
{
    public class UIManagers : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI downloadProgressText;
        
        internal void Initialize()
        {
        }
        
        internal void UpdateProgress(string progress)
        {
            downloadProgressText.text = $"{progress}";
        }
    }
}