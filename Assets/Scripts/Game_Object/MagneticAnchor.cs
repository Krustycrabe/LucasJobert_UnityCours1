using System.Collections.Generic;
using UnityEngine;

public class MagneticAnchor : MonoBehaviour, IMagnetic
{
    public enum FieldShape { Sphere, Box }

    [Header("Magnetic Properties")]
    [SerializeField] private Polarity polarity = Polarity.Positive;
    [SerializeField] private float approachForce = 60f;
    [SerializeField] private float repulseForce = 60f;
    [SerializeField] private float launchImpulse = 25f;
    [SerializeField] private float magneticRange = 6f;
    [SerializeField] private float stickDistance = 0.6f;
    [SerializeField] private float releaseDistance = 1.2f;
    [SerializeField] private LayerMask affectedLayers = ~0;

    [Header("Field Shape")]
    [SerializeField] private FieldShape fieldShape = FieldShape.Sphere;
    [SerializeField] private Vector3 boxSize = new Vector3(4f, 4f, 4f);
    [SerializeField] private Vector3 fieldOffset = Vector3.zero;

    [Header("Anchor Point")]
    [SerializeField] private Transform magneticPoint;

    [Header("Launch Settings")]
    [Tooltip("Direction locale de lancement quand le joueur inverse sa polarité (par défaut : avant de l’objet)")]
    [SerializeField] private Vector3 localLaunchDirection = Vector3.forward;

    [Header("Scripts à désactiver (ex: PlayerMovement)")]
    [SerializeField] private string[] behaviourTypeNamesToDisable = new string[] { "PlayerMovement" };

    private class AttachState
    {
        public IMagnetic magnetic;
        public Rigidbody rb;
        public bool prevKinematic;
        public bool prevUseGravity;
        public RigidbodyConstraints prevConstraints;
        public List<Behaviour> disabledBehaviours = new();
    }

    private readonly Dictionary<Rigidbody, AttachState> _attached = new();
    private static readonly List<Rigidbody> _toDetachBuffer = new();

    // Interface IMagnetic
    public Polarity GetPolarity() => polarity;
    public bool EmitsField() => true;
    public float GetFieldRange() => magneticRange;
    public void ApplyMagneticForce(IMagnetic other) { }

    private void Start()
    {
        if (magneticPoint == null)
            magneticPoint = transform;
    }

    private void FixedUpdate()
    {
        Vector3 fieldCenter = transform.TransformPoint(fieldOffset);
        Collider[] hits = fieldShape == FieldShape.Sphere
            ? Physics.OverlapSphere(fieldCenter, magneticRange, affectedLayers, QueryTriggerInteraction.Ignore)
            : Physics.OverlapBox(fieldCenter, boxSize * 0.5f, transform.rotation, affectedLayers, QueryTriggerInteraction.Ignore);

        HashSet<Rigidbody> seenThisFrame = new();

        foreach (var col in hits)
        {
            IMagnetic target = col.GetComponentInParent<IMagnetic>();
            if (target == null || target == (IMagnetic)this) continue;

            Rigidbody rb = (target as MonoBehaviour)?.GetComponent<Rigidbody>();
            if (rb == null) continue;

            seenThisFrame.Add(rb);

            Vector3 toAnchor = magneticPoint.position - rb.position;
            float dist = toAnchor.magnitude;
            bool opposite = target.GetPolarity() != polarity;

            if (_attached.TryGetValue(rb, out var st))
            {
                // Déjà attaché
                if (!opposite)
                {
                    DetachAndLaunch(st);
                    continue;
                }

                // Maintien stable (zéro tremblement)
                rb.position = magneticPoint.position;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;

                if (dist > releaseDistance)
                    DetachOnly(st);

                continue;
            }

            // Pas attaché
            if (opposite)
            {
                Vector3 force = toAnchor.normalized * (approachForce / Mathf.Max(0.3f, dist));
                rb.AddForce(force, ForceMode.Force);

                if (dist <= stickDistance)
                    Attach(target, rb);
            }
            else
            {
                Vector3 force = -toAnchor.normalized * (repulseForce / Mathf.Max(0.3f, dist));
                rb.AddForce(force, ForceMode.Force);
            }
        }

        // Détache les objets plus dans le champ
        if (_attached.Count > 0)
        {
            _toDetachBuffer.Clear();
            foreach (var kv in _attached)
                if (!seenThisFrame.Contains(kv.Key))
                    _toDetachBuffer.Add(kv.Key);
            foreach (var rb in _toDetachBuffer)
                if (_attached.TryGetValue(rb, out var st))
                    DetachOnly(st);
        }
    }

    private void Attach(IMagnetic target, Rigidbody rb)
    {
        if (_attached.ContainsKey(rb)) return;

        var st = new AttachState
        {
            magnetic = target,
            rb = rb,
            prevKinematic = rb.isKinematic,
            prevUseGravity = rb.useGravity,
            prevConstraints = rb.constraints
        };

        foreach (var typeName in behaviourTypeNamesToDisable)
        {
            if (string.IsNullOrWhiteSpace(typeName)) continue;
            var type = System.Type.GetType(typeName);
            if (type == null) continue;

            var comps = rb.GetComponentsInParent(type, true);
            foreach (var c in comps)
                if (c is Behaviour b && b.isActiveAndEnabled)
                {
                    b.enabled = false;
                    st.disabledBehaviours.Add(b);
                }
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.position = magneticPoint.position;

        _attached[rb] = st;
    }

    private void DetachAndLaunch(AttachState st)
    {
        st.rb.isKinematic = st.prevKinematic;
        st.rb.useGravity = st.prevUseGravity;
        st.rb.constraints = st.prevConstraints;

        foreach (var b in st.disabledBehaviours)
            if (b != null) b.enabled = true;

        // 💥 Direction de lancement contrôlée : locale -> monde
        Vector3 launchDir = transform.TransformDirection(localLaunchDirection).normalized;

        // Sécurité : si la direction locale est nulle → fallback sur la normale à la surface
        if (launchDir == Vector3.zero)
            launchDir = (st.rb.position - magneticPoint.position).normalized;

        st.rb.AddForce(launchDir * launchImpulse, ForceMode.Impulse);

        _attached.Remove(st.rb);
    }

    private void DetachOnly(AttachState st)
    {
        st.rb.isKinematic = st.prevKinematic;
        st.rb.useGravity = st.prevUseGravity;
        st.rb.constraints = st.prevConstraints;

        foreach (var b in st.disabledBehaviours)
            if (b != null) b.enabled = true;

        _attached.Remove(st.rb);
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 fieldCenter = transform.TransformPoint(fieldOffset);
        Gizmos.color = (polarity == Polarity.Positive)
            ? new Color(1f, 0f, 0f, 0.3f)
            : new Color(0f, 0.4f, 1f, 0.3f);

        if (fieldShape == FieldShape.Sphere)
            Gizmos.DrawWireSphere(fieldCenter, magneticRange);
        else
        {
            Gizmos.matrix = Matrix4x4.TRS(fieldCenter, transform.rotation, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, boxSize);
        }

        if (magneticPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawSphere(magneticPoint.position, 0.12f);
        }

        // Visualisation de la direction de shoot
        Gizmos.color = Color.yellow;
        Vector3 dir = transform.TransformDirection(localLaunchDirection).normalized;
        Gizmos.DrawLine(magneticPoint.position, magneticPoint.position + dir * 2f);
        Gizmos.DrawSphere(magneticPoint.position + dir * 2f, 0.1f);
    }

    public void TogglePolarity()
    {
        polarity = (polarity == Polarity.Positive) ? Polarity.Negative : Polarity.Positive;
    }
}