using UnityEngine;

public interface IPolarityProvider
{
    Polarity GetPolarity();
    float GetFieldRange();
}