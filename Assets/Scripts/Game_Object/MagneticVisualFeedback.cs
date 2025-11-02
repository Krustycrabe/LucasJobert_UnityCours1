using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class MagneticVisualFeedback : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("L'objet doit implémenter IMagnetic ou IPolarityProvider")]
    [SerializeField] private Renderer[] renderers;

    private IPolarityProvider polarityProvider;

    [Header("Couleurs magnétiques")]
    [SerializeField] private Color positiveColor = new Color(1f, 0.2f, 0.2f);
    [SerializeField] private Color negativeColor = new Color(0.2f, 0.4f, 1f);
    [SerializeField] private Color neutralColor = Color.white;

    [Header("Blend & Emission")]
    [SerializeField] private float blendSpeed = 5f;
    [SerializeField] private float baseEmission = 0.4f;
    [SerializeField] private float pulseIntensity = 0.8f;
    [SerializeField] private float pulseSpeed = 5f;

    private Color currentColor;
    private Color targetColor;
    private MaterialPropertyBlock mpb;

    private bool isActive = false;
    private float pulseTimer;

    private void Awake()
    {
        polarityProvider = GetComponent<IPolarityProvider>() ?? GetComponentInParent<IPolarityProvider>();
        if (renderers == null || renderers.Length == 0)
            renderers = GetComponentsInChildren<Renderer>();
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        UpdateTargetColor();
        AnimateColor();
        ApplyColor();
    }

    /// <summary>
    /// Choisit la couleur cible en fonction de la polarité
    /// </summary>
    private void UpdateTargetColor()
    {
        if (polarityProvider == null)
        {
            targetColor = neutralColor;
            return;
        }

        switch (polarityProvider.GetPolarity())
        {
            case Polarity.Positive:
                targetColor = positiveColor;
                break;
            case Polarity.Negative:
                targetColor = negativeColor;
                break;
            default:
                targetColor = neutralColor;
                break;
        }
    }

    /// <summary>
    /// Transition fluide entre les couleurs + gestion du pulse
    /// </summary>
    private void AnimateColor()
    {
        currentColor = Color.Lerp(currentColor, targetColor, Time.deltaTime * blendSpeed);

        if (isActive)
        {
            pulseTimer += Time.deltaTime * pulseSpeed;
            float pulse = (Mathf.Sin(pulseTimer) + 1f) * 0.5f * pulseIntensity + baseEmission;
            currentColor *= (1f + pulse * 0.3f);
        }
    }

    /// <summary>
    /// Applique la couleur sur tous les renderers
    /// </summary>
    private void ApplyColor()
    {
        foreach (var r in renderers)
        {
            if (!r) continue;

            r.GetPropertyBlock(mpb);

            if (r.sharedMaterial.HasProperty("_BaseColor"))
                mpb.SetColor("_BaseColor", currentColor);
            else if (r.sharedMaterial.HasProperty("_Color"))
                mpb.SetColor("_Color", currentColor);

            if (r.sharedMaterial.HasProperty("_EmissionColor"))
                mpb.SetColor("_EmissionColor", currentColor * (isActive ? 1.5f : 0.8f));

            r.SetPropertyBlock(mpb);
        }
    }

    /// <summary>
    /// Appelé par les scripts qui veulent activer le feedback (ex: quand ils attirent/repoussent)
    /// </summary>
    public void SetActiveState(bool active)
    {
        isActive = active;
        if (!active) pulseTimer = 0f;
    }
}
