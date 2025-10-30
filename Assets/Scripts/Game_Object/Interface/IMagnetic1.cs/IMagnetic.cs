using UnityEngine;

public enum Polarity
{
    Positive,
    Negative
}

public interface IMagnetic
{
    Polarity GetPolarity();
    bool EmitsField(); // True si l�objet �met un champ magn�tique
    void ApplyMagneticForce(IMagnetic other);
}