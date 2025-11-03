using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(CapsuleCollider))]
public class LaserHazard : MonoBehaviour
{
    [Header("Logic")]
    [SerializeField] private Polarity laserPolarity = Polarity.Positive;
    [SerializeField] private bool isActive = true;
    [SerializeField] private LayerMask victimLayers = ~0;

    [Header("Beam Visual")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Beam Settings")]
    [SerializeField] private float beamWidth = 0.1f;
    [SerializeField] private float beamOffset = 0f; // décalage optionnel du faisceau

    [Header("Colors")]
    [SerializeField] private Color positiveColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color negativeColor = new Color(0.2f, 0.4f, 1f);

    [Header("Pulse Effect")]
    [SerializeField] private float pulseSpeed = 5f;
    [SerializeField] private float pulseIntensity = 0.25f;

    private CapsuleCollider capsule;

    //───────────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        capsule = GetComponent<CapsuleCollider>();
        capsule.isTrigger = true;
        UpdatePointsFromCollider();
        SetupLineRenderer();
    }

    private void OnValidate()
    {
        capsule = GetComponent<CapsuleCollider>();
        UpdatePointsFromCollider();
        SetupLineRenderer();
        UpdateLaserColor();
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        if (lineRenderer == null || capsule == null)
            return;

        // 🧱 Empêche toute exécution sur un prefab (hors scène)
        if (!Application.isPlaying && !gameObject.scene.IsValid())
            return;

        UpdatePointsFromCollider();
        UpdateLaserVisual();
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void UpdatePointsFromCollider()
    {
        if (capsule == null) return;

        // Crée ou récupère les points
        if (startPoint == null)
        {
            GameObject s = transform.Find("StartPoint")?.gameObject ?? new GameObject("StartPoint");
            s.transform.SetParent(transform);
            startPoint = s.transform;
        }

        if (endPoint == null)
        {
            GameObject e = transform.Find("EndPoint")?.gameObject ?? new GameObject("EndPoint");
            e.transform.SetParent(transform);
            endPoint = e.transform;
        }

        // Axe selon la direction du collider
        Vector3 axis = Vector3.forward;
        switch (capsule.direction)
        {
            case 0: axis = Vector3.right; break;
            case 1: axis = Vector3.up; break;
            case 2: axis = Vector3.forward; break;
        }

        Vector3 center = capsule.center;
        float half = capsule.height / 2f;

        // Position locale des extrémités
        startPoint.localPosition = center - axis * (half + beamOffset);
        endPoint.localPosition = center + axis * (half + beamOffset);
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void SetupLineRenderer()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            if (lineRenderer == null)
            {
                var beam = new GameObject("Beam");
                beam.transform.SetParent(transform);
                beam.transform.localPosition = Vector3.zero;
                lineRenderer = beam.AddComponent<LineRenderer>();
            }
        }

        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = beamWidth;
        lineRenderer.endWidth = beamWidth;

        // 🔧 Crée un matériau de base uniquement en mode Play
        if (Application.isPlaying)
        {
            if (lineRenderer.material == null)
            {
                var mat = new Material(Shader.Find("Unlit/Color"));
                lineRenderer.material = mat;
            }
        }
        else
        {
            // 🧱 En mode Éditeur : utiliser sharedMaterial (ne crée pas d'instance)
            if (lineRenderer.sharedMaterial == null)
            {
                var mat = new Material(Shader.Find("Unlit/Color"));
                lineRenderer.sharedMaterial = mat;
            }
        }
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void UpdateLaserVisual()
    {
        lineRenderer.enabled = isActive;
        if (!isActive) return;

        // Positions du faisceau
        lineRenderer.SetPosition(0, startPoint.position);
        lineRenderer.SetPosition(1, endPoint.position);

        // Couleur selon polarité
        UpdateLaserColor();
    }

    private void UpdateLaserColor()
    {
        if (lineRenderer == null)
            return;

        // 🧱 Bloque la logique si le prefab n’est pas instancié dans une scène
        if (!Application.isPlaying && !gameObject.scene.IsValid())
            return;

        Color baseColor = (laserPolarity == Polarity.Positive) ? positiveColor : negativeColor;

        float pulse = Application.isPlaying
            ? (Mathf.Sin(Time.time * pulseSpeed) * pulseIntensity) + (1f - pulseIntensity)
            : 1f;

        Color pulsedColor = baseColor * pulse;

        lineRenderer.startColor = pulsedColor;
        lineRenderer.endColor = pulsedColor;

        // ✅ Matériau sûr selon le mode
        if (Application.isPlaying)
        {
            if (lineRenderer.material != null)
                lineRenderer.material.color = pulsedColor;
        }
        else
        {
            if (lineRenderer.sharedMaterial != null)
                lineRenderer.sharedMaterial.color = pulsedColor;
        }
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void OnTriggerEnter(Collider other)
    {
        TryKill(other);
    }

    private void OnTriggerStay(Collider other)
    {
        TryKill(other);
    }

    private void TryKill(Collider other)
    {
        if (!isActive) return;
        if (((1 << other.gameObject.layer) & victimLayers) == 0) return;

        var player = other.GetComponentInParent<PolarityManager>();
        if (player == null) return;

        if (player.GetPolarity() == laserPolarity)
        {
            if (PlayerRespawnManager.Instance != null)
                PlayerRespawnManager.Instance.KillPlayer();
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
                );
        }
    }

    //───────────────────────────────────────────────────────────────────────────────
    private void OnDrawGizmos()
    {
        Gizmos.color = (laserPolarity == Polarity.Positive) ? Color.red : Color.blue;
        if (capsule != null)
        {
            UpdatePointsFromCollider();
            Gizmos.DrawLine(transform.TransformPoint(startPoint.localPosition),
                            transform.TransformPoint(endPoint.localPosition));
        }
    }
}
