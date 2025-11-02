using UnityEngine;

[RequireComponent(typeof(Collider))]
public class MagneticZone : MonoBehaviour, IPolarityProvider
{
    [Header("Magnetic Zone Settings")]
    [SerializeField] private Polarity zonePolarity = Polarity.Positive;
    [SerializeField] private float baseForce = 20f;
    [SerializeField] private float distanceFalloff = 1.5f; // 1 = doux, 2 = très fort au centre
    [SerializeField] private float fieldRange = 5f;
    public float GetFieldRange() => fieldRange;
    public Polarity GetPolarity() => zonePolarity;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    private void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb == null) return;

        IMagnetic magnetic = other.GetComponent<IMagnetic>();
        if (magnetic == null) return;

        // Direction centre → objet
        Vector3 dir = other.transform.position - transform.position;
        float dist = dir.magnitude;
        if (dist > fieldRange) return;

        // Falloff (plus proche = plus fort)
        float falloff = 1f / Mathf.Pow(dist + 0.1f, distanceFalloff);

        // Force de base
        Vector3 force = dir.normalized * baseForce * falloff;

        // ---- POLARITÉ ----

        // Même polarité → répulsion
        if (magnetic.GetPolarity() == zonePolarity)
        {
            rb.AddForce(force, ForceMode.Acceleration);
        }
        else
        {
            // Polarité opposée → attraction
            rb.AddForce(-force, ForceMode.Acceleration);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = (zonePolarity == Polarity.Positive) ? Color.red : Color.blue;
        Gizmos.DrawWireSphere(transform.position, fieldRange);
    }
}
