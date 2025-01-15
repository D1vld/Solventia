using UnityEngine;
using UnityEngine.XR.Management;

public class RecenterViewMetaXR : MonoBehaviour
{
    public void Recenter()
    {
        // Llama a la función que recentra la vista
        OVRManager.display.RecenterPose();
        Debug.Log("Vista recentrada utilizando MetaXR");
    }
}
