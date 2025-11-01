using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnergyZone : MonoBehaviour
{
    [Header("Magnetic Settings")]
    [SerializeField] private Polarity requiredPolarity = Polarity.Positive;
    [SerializeField] private float attractionStrength = 25f;
    [SerializeField] private float snapDistance = 1.2f;
    [SerializeField] private float snapSpeed = 7f;
    [SerializeField] private Transform snapPoint;

    private IMagnetic currentMagnetic;
    private IEnergySource currentEnergySource;
    private Rigidbody rb;
    private bool isSnapping = false;
    private bool zoneIsActive = false;

    public bool IsZoneActive => zoneIsActive;

    private void OnTriggerEnter(Collider other)
    {
        // ✅ Ignore complètement le Player (il a un PolarityManager)
        if (other.GetComponentInParent<PolarityManager>() != null)
        {
            Debug.Log($"{name} : Player détecté → attraction ignorée.");
            return;
        }

        // ✅ Continue uniquement pour des MagneticObjects
        rb = other.attachedRigidbody;
        currentMagnetic = other.GetComponentInParent<IMagnetic>();
        currentEnergySource = other.GetComponentInParent<IEnergySource>();

        if (rb == null || currentMagnetic == null)
            return;

        // Même polarité → répulsion
        if (currentMagnetic.GetPolarity() == requiredPolarity)
        {
            Repulse(rb);
            ResetZone();
        }
    }

    private void FixedUpdate()
    {
        if (rb == null || currentMagnetic == null)
            return;

        if (isSnapping)
            return;

        // Attraction uniquement si polarité opposée
        if (currentMagnetic.GetPolarity() != requiredPolarity)
        {
            Vector3 dir = snapPoint.position - rb.position;
            float dist = dir.magnitude;

            rb.AddForce(dir.normalized * attractionStrength, ForceMode.Acceleration);

            if (dist < snapDistance)
                StartSnapping();
        }
    }

    private void StartSnapping()
    {
        isSnapping = true;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        // ✅ Active la zone uniquement si c’est une vraie source d’énergie
        if (currentEnergySource != null)
        {
            zoneIsActive = true;
            Debug.Log($"{name} activée (source d'énergie détectée)");
        }
        else
        {
            Debug.Log($"{name} : objet magnétique snappé mais pas d'énergie (non-alimentant)");
        }
    }

    private void Update()
    {
        if (isSnapping && rb != null)
        {
            rb.transform.position = Vector3.Lerp(
                rb.transform.position,
                snapPoint.position,
                Time.deltaTime * snapSpeed
            );

            rb.transform.rotation = Quaternion.Lerp(
                rb.transform.rotation,
                snapPoint.rotation,
                Time.deltaTime * snapSpeed
            );
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Ignore les faux exits liés à la physique
        if (rb == null) return;

        float dist = Vector3.Distance(rb.position, snapPoint.position);
        if (dist < snapDistance * 1.2f)
            return;

        ResetZone();
    }

    private void ResetZone()
    {
        if (rb != null)
            rb.constraints = RigidbodyConstraints.None;

        rb = null;
        currentMagnetic = null;
        currentEnergySource = null;
        isSnapping = false;
        zoneIsActive = false;
    }

    private void Repulse(Rigidbody body)
    {
        Vector3 dir = body.position - snapPoint.position;
        body.AddForce(dir.normalized * attractionStrength * 2f, ForceMode.Impulse);
    }
}
