using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Rigidbody))]
public class PolarityManager : MonoBehaviour, IMagnetic
{
    [SerializeField] private Polarity currentPolarity = Polarity.Positive;
    [SerializeField] private float magneticForce = 10f;
    [SerializeField] private Renderer ballRenderer;
    [SerializeField] private LayerMask affectedLayers = ~0;
    [SerializeField] private float fieldRange = 5f;
    public float GetFieldRange() => fieldRange;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        UpdateColor();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePolarity();
        }
    }

    private void TogglePolarity()
    {
        currentPolarity = currentPolarity == Polarity.Positive ? Polarity.Negative : Polarity.Positive;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (ballRenderer)
            ballRenderer.material.color = currentPolarity == Polarity.Positive ? Color.red : Color.blue;
    }

    private void FixedUpdate()
    {
        // Le joueur émet lui aussi un champ magnétique (comme l'ancre)
        Collider[] cols = Physics.OverlapSphere(transform.position, fieldRange, affectedLayers, QueryTriggerInteraction.Ignore);

        foreach (Collider col in cols)
        {
            IMagnetic targetMag = col.GetComponentInParent<IMagnetic>();
            if (targetMag == null || targetMag == (IMagnetic)this) continue;

            Rigidbody targetRb = (targetMag as MonoBehaviour)?.GetComponent<Rigidbody>();
            if (targetRb == null) continue;

            Vector3 dir = targetRb.position - transform.position;
            float dist = dir.magnitude;
            if (dist < 0.1f) continue;

            float sign = (targetMag.GetPolarity() == currentPolarity) ? 1f : -1f;
            Vector3 force = dir.normalized * (magneticForce / (dist * dist)) * sign;

            targetRb.AddForce(force, ForceMode.Force);
        }
    }

    // Interface IMagnetic
    public Polarity GetPolarity() => currentPolarity;
    public bool EmitsField() => true;
    public void ApplyMagneticForce(IMagnetic other) { /* non utilisé ici */ }
}
