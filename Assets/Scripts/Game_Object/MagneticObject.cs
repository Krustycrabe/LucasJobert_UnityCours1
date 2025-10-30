using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MagneticObject : MonoBehaviour, IMagnetic
{
    [SerializeField] private Polarity polarity = Polarity.Positive;

    public Polarity GetPolarity() => polarity;

    public bool EmitsField() => false;

    public void ApplyMagneticForce(IMagnetic other)
    {
        // Les objets passifs ne génèrent pas de champ
    }
}
