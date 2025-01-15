using UnityEngine;
using TMPro;

namespace Template360
{
    public class UIManager : MonoBehaviour
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