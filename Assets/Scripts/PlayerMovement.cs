using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private float _horizontalMovement;
    private float _verticalMovement;
    private Vector3 _movement;
    [SerializeField]  private float _speed = 2f;
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        
        
    }

    void Update()
    {
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");
        _movement = new Vector3(_horizontalMovement, 0f, _verticalMovement);
        _movement.Normalize();
        _movement *= _speed;
        _movement.y = _rb.linearVelocity.y;
        if ( _rb != null)
        {
            _rb.linearVelocity = _movement;
        }
        else
        {
            Debug.LogError("No RigidBody Attached !");
        }
    }
}
