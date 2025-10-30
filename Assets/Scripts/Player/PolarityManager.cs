using UnityEngine;

public class PolarityManager : MonoBehaviour
{
    public enum Polarity { Positive, Negative }
    public Polarity currentPolarity = Polarity.Positive;

    public float magneticForce = 20f;
    public float magneticRange = 10f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TogglePolarity();
        }

        ApplyMagneticForce();
    }

    void TogglePolarity()
    {
        currentPolarity = (currentPolarity == Polarity.Positive) ? Polarity.Negative : Polarity.Positive;
    }

    void ApplyMagneticForce()
    {
        Collider[] objects = Physics.OverlapSphere(transform.position, magneticRange);

        foreach (Collider obj in objects)
        {
            IMagnetic magnetic = obj.GetComponent<IMagnetic>();
            if (magnetic == null) continue;

            Vector3 dir = (magnetic.Rb.position - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, magnetic.Rb.position);
            float force = magneticForce / Mathf.Max(distance, 0.5f);

            if (magnetic.Polarity == currentPolarity)
                magnetic.Rb.AddForce(dir * force); // répulsion
            else
                magnetic.Rb.AddForce(-dir * force); // attraction

            magnetic.OnMagneticInteraction(force, dir);
        }
    }
}   