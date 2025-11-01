using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class PolarityVisual : MonoBehaviour
{
    [Header("Colors")]
    [SerializeField] private Color positiveColor = new Color(1f, 0.2f, 0.2f); // Rouge
    [SerializeField] private Color negativeColor = new Color(0.2f, 0.4f, 1f); // Bleu

    [Header("Scope")]
    [SerializeField] private bool applyToChildren = false;   // Coche si ton mesh est sur des enfants
    [SerializeField] private bool onlyThisRoot = true;       // Evite d’attraper d’autres objets dans la hiérarchie

    [Header("Shader property names (auto)")]
    [SerializeField] private string colorProperty = "";      // Laisse vide → auto: _BaseColor puis _Color
    [SerializeField] private string emissionProperty = "";   // Optionnel: _EmissionColor si présent

    private IMagnetic magnetic;
    private Renderer[] rends;
    private MaterialPropertyBlock mpb;

    void OnEnable()
    {
        CacheRefs();
        ApplyNow();
    }

    void Awake()
    {
        CacheRefs();
        ApplyNow();
    }

    void OnValidate()
    {
        CacheRefs();
        ApplyNow();
    }

    void Update()
    {
        // Met à jour en live (si la polarité change pendant le jeu)
        ApplyNow();
    }

    private void CacheRefs()
    {
        // Trouve l'IMagnetic sur cet objet (ou parent immédiat)
        magnetic = GetComponent<IMagnetic>() ?? GetComponentInParent<IMagnetic>();

        // Limite le scope pour ne pas toucher d'autres objets
        if (applyToChildren)
        {
            if (onlyThisRoot)
                rends = GetComponentsInChildren<Renderer>(includeInactive: true);
            else
                rends = GetComponentsInChildren<Renderer>(true); // (équivalent)
        }
        else
        {
            var r = GetComponent<Renderer>();
            rends = (r != null) ? new Renderer[] { r } : System.Array.Empty<Renderer>();
        }

        if (mpb == null) mpb = new MaterialPropertyBlock();
    }

    private void ApplyNow()
    {
        if (magnetic == null || rends == null || rends.Length == 0) return;

        // Choix de la couleur selon la polarité
        var col = (magnetic.GetPolarity() == Polarity.Positive) ? positiveColor : negativeColor;

        // Pour chaque renderer → on pousse une couleur via MPB (sans toucher au material)
        foreach (var r in rends)
        {
            if (r == null) continue;

            // Trouve la bonne propriété couleur pour ce renderer
            string colProp = ResolveColorProperty(r);
            if (string.IsNullOrEmpty(colProp)) continue;

            r.GetPropertyBlock(mpb);
            mpb.SetColor(colProp, col);

            // Emission si disponible (sans changer le material/keywords)
            string emisProp = ResolveEmissionProperty(r);
            if (!string.IsNullOrEmpty(emisProp))
            {
                // Petite émission douce (facultatif)
                mpb.SetColor(emisProp, col * 0.5f);
            }

            r.SetPropertyBlock(mpb);
        }
    }

    private string ResolveColorProperty(Renderer r)
    {
        if (!string.IsNullOrEmpty(colorProperty)) return colorProperty;

        // Essaie les noms usuels (URP/HDRP/Standard)
        var mat = r.sharedMaterial;
        if (mat == null) return null;

        if (mat.HasProperty("_BaseColor")) return "_BaseColor";
        if (mat.HasProperty("_Color")) return "_Color";
        // Ajoute d'autres props si besoin (ex: "_BaseColorMap" etc.)
        return null;
    }

    private string ResolveEmissionProperty(Renderer r)
    {
        if (!string.IsNullOrEmpty(emissionProperty)) return emissionProperty;

        var mat = r.sharedMaterial;
        if (mat == null) return null;

        if (mat.HasProperty("_EmissionColor")) return "_EmissionColor";
        return null;
    }
}
