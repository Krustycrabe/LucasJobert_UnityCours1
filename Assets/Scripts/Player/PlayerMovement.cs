using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody _rb;
    private float _horizontal;
    private float _vertical;
    private Vector3 _inputDir;

    [Header("Movement Settings")]
    [SerializeField] private float _acceleration = 20f;   // vitesse à laquelle la force s'applique
    [SerializeField] private float _maxSpeed = 8f;        // vitesse max de la balle
    [SerializeField] private float _turnResponsiveness = 10f; // réactivité du changement de direction
    [SerializeField] private float _groundFriction = 3f;  // force qui freine légèrement la balle

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        _inputDir = new Vector3(_horizontal, 0f, _vertical).normalized;
    }

    void FixedUpdate()
    {
        // Si y a une direction d'entrée
        if (_inputDir.sqrMagnitude > 0.01f)
        {
            // Force directionnelle (plus réactive)
            Vector3 targetVelocity = _inputDir * _maxSpeed;

            // On calcule la différence entre la vitesse actuelle et celle voulue
            Vector3 velocityChange = (targetVelocity - _rb.linearVelocity);
            velocityChange.y = 0f; // pas de force verticale

            // On limite la force pour éviter des à-coups trop violents
            velocityChange = Vector3.ClampMagnitude(velocityChange, _acceleration * Time.fixedDeltaTime);

            _rb.AddForce(velocityChange * _turnResponsiveness, ForceMode.VelocityChange);
        }
        else
        {
            // Applique une légère friction naturelle au sol
            Vector3 friction = -_rb.linearVelocity * _groundFriction * Time.fixedDeltaTime;
            friction.y = 0;
            _rb.AddForce(friction, ForceMode.VelocityChange);
        }

        // Clamp vitesse max
        Vector3 flatVel = new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z);
        if (flatVel.magnitude > _maxSpeed)
        {
            flatVel = flatVel.normalized * _maxSpeed;
            _rb.linearVelocity = new Vector3(flatVel.x, _rb.linearVelocity.y, flatVel.z);
        }
    }
}

