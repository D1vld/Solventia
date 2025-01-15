using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    [SerializeField] Transform _Camera;
    [SerializeField] float _Speed = 5;
    [SerializeField] Vector3 _RotationClamp = Vector3.one * 360;
    
    void LateUpdate()
    {
        var forward = (_Camera.transform.position - transform.position).normalized;
        var targetRotation = Quaternion.LookRotation(forward, Vector3.up);
        var x = Mathf.Clamp(targetRotation.eulerAngles.x, -_RotationClamp.x, _RotationClamp.x);
        var y = Mathf.Clamp(targetRotation.eulerAngles.y, -_RotationClamp.y, _RotationClamp.y);
        var z = Mathf.Clamp(targetRotation.eulerAngles.z, -_RotationClamp.z, _RotationClamp.z);
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(x, y, z), _Speed * Time.deltaTime);
    }
}