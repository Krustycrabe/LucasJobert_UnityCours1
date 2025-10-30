using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObject : MonoBehaviour, IMagnetic
{
    [SerializeField] private PolarityManager.Polarity polarity;
    private Rigidbody rb;

    public PolarityManager.Polarity Polarity => polarity;
    public Rigidbody Rb => rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMagneticInteraction(float force, Vector3 direction)
    {
        // Effet visuel ou particule (optionnel)
    }
}