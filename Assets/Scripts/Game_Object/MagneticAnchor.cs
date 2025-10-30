using UnityEngine;
using System.Linq;

public class MagneticAnchor : MonoBehaviour, IMagnetic
{
    [SerializeField] private Polarity polarity = Polarity.Positive;
    [SerializeField] private float fieldStrength = 30f;
    [SerializeField] private float fieldRange = 10f;
    [SerializeField] private float minDistance = 0.5f;
    [SerializeField] private LayerMask affectedLayers = ~0;

    public Polarity GetPolarity() => polarity;
    public bool EmitsField() => true;

    private void FixedUpdate()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, fieldRange, affectedLayers, QueryTriggerInteraction.Ignore);

        foreach (var col in cols)
        {
            IMagnetic target = col.GetComponentInParent<IMagnetic>();
            if (target == null || target == (IMagnetic)this) continue;

            Rigidbody targetRb = (target as MonoBehaviour)?.GetComponent<Rigidbody>();
            if (targetRb == null) continue;

            Vector3 direction = targetRb.position - transform.position;
            float distance = direction.magnitude;

            if (distance < minDistance)
            {
                targetRb.linearVelocity = Vector3.zero;
                continue;
            }

            float sign = (target.GetPolarity() == polarity) ? -1f : 1f;

            // force plus “arcade” → 1/distance
            Vector3 force = direction.normalized * (fieldStrength / distance) * sign;

            targetRb.AddForce(force, ForceMode.Acceleration); // ignore la masse
        }
    }

    public void ApplyMagneticForce(IMagnetic other) { }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = (polarity == Polarity.Positive) ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(transform.position, fieldRange);
    }
}