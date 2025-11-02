using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class PolarityVisual : MonoBehaviour
{
    [Header("Couleurs")]
    [SerializeField] private Color positiveColor = new Color(1f, 0.2f, 0.2f); // Rouge
    [SerializeField] private Color negativeColor = new Color(0.2f, 0.4f, 1f); // Bleu

    [Header("Options")]
    [SerializeField] private bool applyToChildren = true; // Si les meshes sont dans les enfants
    [SerializeField] private bool useEmission = true;     // Brillance optionnelle
    [SerializeField] private bool pulseEffect = false;    // Effet "respiration" du matériau

    private IPolarityProvider polarityProvider;
    private Renderer[] renderers;
    private MaterialPropertyBlock mpb;

    private void Awake()
    {
        CacheRefs();
        ApplyNow();
    }

    private void OnEnable()
    {
        CacheRefs();
        ApplyNow();
    }

    private void OnValidate()
    {
        CacheRefs();
        ApplyNow();
    }

    private void Update()
    {
        ApplyNow();
    }

    private void CacheRefs()
    {
        // Récupère n’importe quel objet capable de fournir une polarité
        polarityProvider = GetComponent<IPolarityProvider>() ?? GetComponentInParent<IPolarityProvider>();

        // Récupère les renderers
        renderers = applyToChildren
            ? GetComponentsInChildren<Renderer>(includeInactive: true)
            : new Renderer[] { GetComponent<Renderer>() };

        if (mpb == null)
            mpb = new MaterialPropertyBlock();
    }

    private void ApplyNow()
    {
        if (polarityProvider == null || renderers == null || renderers.Length == 0)
            return;

        // Détermine la couleur cible
        Color baseColor = polarityProvider.GetPolarity() == Polarity.Positive ? positiveColor : negativeColor;

        // Petit effet de pulsation optionnel
        if (pulseEffect && Application.isPlaying)
        {
            float pulse = (Mathf.Sin(Time.time * 4f) * 0.2f) + 0.8f;
            baseColor *= pulse;
        }

        // Applique la couleur sur chaque Renderer
        foreach (var r in renderers)
        {
            if (r == null) continue;

            r.GetPropertyBlock(mpb);

            if (r.sharedMaterial.HasProperty("_BaseColor"))
                mpb.SetColor("_BaseColor", baseColor);
            else if (r.sharedMaterial.HasProperty("_Color"))
                mpb.SetColor("_Color", baseColor);

            if (useEmission && r.sharedMaterial.HasProperty("_EmissionColor"))
                mpb.SetColor("_EmissionColor", baseColor * 0.5f);

            r.SetPropertyBlock(mpb);
        }
    }
}
