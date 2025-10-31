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
        currentMagnetic = other.GetComponentInParent<IMagnetic>();
        currentEnergySource = other.GetComponentInParent<IEnergySource>();
        rb = other.attachedRigidbody;

        if (rb == null || currentMagnetic == null)
            return;

        // Si même polarité → on repousse
        if (currentMagnetic.GetPolarity() == requiredPolarity)
        {
            Repulse(rb);
            ResetZone();
            return;
        }

        // Polarité opposée → possible attraction
        zoneIsActive = false; // On la remet à false tant que le snap n’est pas complet
    }

    private void FixedUpdate()
    {
        if (rb == null || currentMagnetic == null)
            return;

        // Si déjà snappé, on garde la position stable
        if (isSnapping)
            return;

        // Polarité opposée → attraction
        if (currentMagnetic.GetPolarity() != requiredPolarity)
        {
            Vector3 dir = snapPoint.position - rb.position;
            float dist = dir.magnitude;

            rb.AddForce(dir.normalized * attractionStrength, ForceMode.Acceleration);

            // Déclenche le snap quand proche
            if (dist < snapDistance)
                StartSnapping();
        }
    }

    private void StartSnapping()
    {
        isSnapping = true;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll; // ✅ Gèle la physique, sans kinematic

        zoneIsActive = true;
        Debug.Log($"{name} → Zone ACTIVÉE ✅");
    }


    private void Update()
    {
        if (!isSnapping || rb == null)
            return;

        // Interpolation fluide vers le snap point
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

    private void OnTriggerExit(Collider other)
    {
        if (rb == null) return;

        float dist = Vector3.Distance(rb.position, snapPoint.position);

        if (dist < snapDistance * 1.2f)
        {
            Debug.Log($"{name} → FAUSSE SORTIE ignorée");
            return;
        }

        Debug.Log($"{name} → VRAIE sortie (désactivation)");
        ResetZone();
    }

    private void Repulse(Rigidbody body)
    {
        Vector3 dir = body.position - snapPoint.position;
        body.AddForce(dir.normalized * attractionStrength * 2f, ForceMode.Impulse);
    }

    private void ResetZone()
    {
        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints.None;
            rb.isKinematic = false;
        }

        rb = null;
        currentMagnetic = null;
        currentEnergySource = null;
        isSnapping = false;
        zoneIsActive = false;
    }
}