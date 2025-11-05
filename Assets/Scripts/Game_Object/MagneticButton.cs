using UnityEngine;

public class MagneticButton : MonoBehaviour
{
    [Header("Button Settings")]
    [SerializeField] private MagneticAnchor[] targets;     // Les MagneticAnchors à inverser
    [SerializeField] private Light feedbackLight;          // Light de feedback visuel
    [SerializeField] private float lightFlashDuration = 0.3f; // Durée du flash
    [SerializeField] private Color activeColor = Color.green;
    [SerializeField] private Color idleColor = Color.white;

    [Header("Collision Settings")]
    [SerializeField] private bool oneUseOnly = false;      // Le bouton ne fonctionne qu’une fois ?
    private bool isUsed = false;

    private void Start()
    {
        if (feedbackLight != null)
            feedbackLight.color = idleColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isUsed) return;

        IMagnetic magnetic = other.GetComponentInParent<IMagnetic>();
        if (magnetic == null) return;

        // ⚡ Le joueur ou un objet magnétique active le bouton
        ToggleTargets();

        // Feedback visuel
        if (feedbackLight != null)
            StartCoroutine(LightFlash());

        if (oneUseOnly)
            isUsed = true;
    }

    private void ToggleTargets()
    {
        foreach (var anchor in targets)
        {
            if (anchor == null) continue;
            anchor.TogglePolarity();
        }
    }

    private System.Collections.IEnumerator LightFlash()
    {
        feedbackLight.color = activeColor;
        feedbackLight.intensity = 3f;

        yield return new WaitForSeconds(lightFlashDuration);

        feedbackLight.color = idleColor;
        feedbackLight.intensity = 1f;
    }
}
