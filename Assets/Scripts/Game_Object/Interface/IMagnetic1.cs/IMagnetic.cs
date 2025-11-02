using UnityEngine;

public enum Polarity
{
    Positive,
    Negative
}

public interface IMagnetic : IPolarityProvider
{
    bool EmitsField(); // True si l’objet émet un champ magnétique
    void ApplyMagneticForce(IMagnetic other);
}