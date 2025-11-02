using UnityEngine;

[ExecuteAlways]
[DisallowMultipleComponent]
public class MagneticFieldAutoVisualizer : MonoBehaviour
{
    [Header("Base Settings")]
    [SerializeField] private Material baseFieldMaterial;
    [SerializeField] private bool useFlatDisk = true;
    [SerializeField] private float visualScaleMultiplier = 1f;

    [Header("Colors")]
    [SerializeField] private Color positiveColor = new(1f, 0.2f, 0.2f, 0.3f);
    [SerializeField] private Color negativeColor = new(0.2f, 0.4f, 1f, 0.3f);

    private IPolarityProvider polarityProvider;
    private IMagnetic magneticSource;
    private GameObject fieldVisual;
    private Renderer fieldRenderer;
    private MaterialPropertyBlock propBlock;

    private void OnEnable()
    {
        polarityProvider = GetComponent<IPolarityProvider>() ?? GetComponentInParent<IPolarityProvider>();
        magneticSource = GetComponent<IMagnetic>() ?? GetComponentInParent<IMagnetic>();

        CreateField();
        UpdateVisuals();
    }

    private void Update()
    {
        UpdateVisuals();
    }

    private void CreateField()
    {
        // Supprime l’ancien visuel si besoin
        Transform old = transform.Find("MagneticFieldVisual");
        if (old) DestroyImmediate(old.gameObject);

        // Crée un disque ou sphère pour représenter le champ
        fieldVisual = GameObject.CreatePrimitive(useFlatDisk ? PrimitiveType.Plane : PrimitiveType.Sphere);
        fieldVisual.name = "MagneticFieldVisual";
        fieldVisual.transform.SetParent(transform, false);
        DestroyImmediate(fieldVisual.GetComponent<Collider>());

        fieldRenderer = fieldVisual.GetComponent<Renderer>();
        fieldRenderer.sharedMaterial = baseFieldMaterial;
        propBlock = new MaterialPropertyBlock();

#if UNITY_EDITOR
        if (!Application.isPlaying)
            UnityEditor.EditorUtility.SetDirty(fieldRenderer);
#endif
    }

    private void UpdateVisuals()
    {
        if (!fieldRenderer || !baseFieldMaterial)
            return;

        if (propBlock == null)
            propBlock = new MaterialPropertyBlock();

        // 🎨 Couleur selon polarité
        Color targetColor = polarityProvider != null && polarityProvider.GetPolarity() == Polarity.Positive
            ? positiveColor
            : negativeColor;

        propBlock.SetColor("_BaseColor", targetColor);
        fieldRenderer.SetPropertyBlock(propBlock);

        // 🧲 Taille du champ basée sur le fieldRange réel
        float range = magneticSource != null ? magneticSource.GetFieldRange() : 3f;
        float scale = range * 2f * visualScaleMultiplier;

        fieldVisual.transform.localScale = useFlatDisk
            ? new Vector3(scale, 1f, scale)
            : new Vector3(scale, scale, scale);

        // 📍 Position
        fieldVisual.transform.localPosition = Vector3.zero;

        // 🔄 Si c’est un disque, on le garde bien à plat
        if (useFlatDisk)
        {
            fieldVisual.transform.rotation = Quaternion.identity;
            Vector3 pos = fieldVisual.transform.localPosition;
            pos.y = 0.05f; // léger décalage pour pas clipper le sol
            fieldVisual.transform.localPosition = pos;
        }
    }

    private void OnDisable()
    {
        if (fieldVisual != null && !Application.isPlaying)
            DestroyImmediate(fieldVisual);
    }
}
