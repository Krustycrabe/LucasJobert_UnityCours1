using UnityEngine;

public interface IMagnetic
{
    PolarityManager.Polarity Polarity { get; }
    Rigidbody Rb { get; }
    void OnMagneticInteraction(float force, Vector3 direction);
}
