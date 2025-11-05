using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObjectReset : MonoBehaviour
{
    private Vector3 _originPos;
    private Quaternion _originRot;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _originPos = transform.position;
        _originRot = transform.rotation;
    }

    public void ResetToOrigin()
    {
        _rb.linearVelocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        transform.SetPositionAndRotation(_originPos, _originRot);
    }
}