using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObject : MonoBehaviour, IMagnetic, IEnergySource
{
    [Header("Magnetic Settings")]
    [SerializeField] private Polarity polarity = Polarity.Positive;

    [Header("Energy Settings")]
    [SerializeField] private bool isActiveEnergySource = true;
    [SerializeField] private float power = 1f;
    [SerializeField] private float fieldRange = 5f;
    public float GetFieldRange() => fieldRange;

    public Polarity GetPolarity() => polarity;

    public bool EmitsField() => false;

    public void ApplyMagneticForce(IMagnetic other)
    {
        // Les objets passifs ne génèrent pas de champ
    }
    public bool IsActiveEnergy() => isActiveEnergySource;
    public float GetPower() => power;
}
