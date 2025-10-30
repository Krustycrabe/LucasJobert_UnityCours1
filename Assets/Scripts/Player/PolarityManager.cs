using Unity.Android.Gradle;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PolarityManager : MonoBehaviour
{
    public enum Polarity { Positive, Negative }
    public Polarity currentPolarity = Polarity.Positive;

    public float magneticForce = 20f;
    public float magneticRange = 10f;

    [Header("Materials")]
    public Material positiveMat;
    public Material negativeMat;

    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        UpdateMaterial();
    }
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
        UpdateMaterial();
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
    private void UpdateMaterial()
    {
        if (!rend) return;

        switch (currentPolarity)
        {
            case Polarity.Positive:
                rend.material = positiveMat;
                break;
            case Polarity.Negative:
                rend.material = negativeMat;
                break;
        }
    }
}