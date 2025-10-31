using System.Linq;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    [Header("Energy Zones")]
    [SerializeField] private EnergyZone[] zones;
    [SerializeField] private int requiredActiveZones = 1;

    [Header("Door Settings")]
    [SerializeField] private float openSpeed = 3f;
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3, 0);

    private bool isOpen = false;
    private Vector3 closedPos;
    private Vector3 openPos;

    private void Start()
    {
        closedPos = transform.position;
        openPos = closedPos + openOffset;
    }

    private void Update()
    {
        // Compter le nombre de zones actives
        int activeZones = 0;
        foreach (var zone in zones)
        {
            if (zone != null && zone.IsZoneActive)
            {
                activeZones++;
                Debug.Log($"Zone active détectée : {zone.name}");
            }
        }

        Debug.Log($"Zones actives : {activeZones}/{requiredActiveZones}");

        // ✅ Condition explicite : si assez de zones actives → ouvrir
        if (activeZones >= requiredActiveZones && !isOpen)
        {
            OpenDoor();
        }
        else if (activeZones < requiredActiveZones && isOpen)
        {
            CloseDoor();
        }

        // Animation fluide vers la position cible
        Vector3 targetPos = isOpen ? openPos : closedPos;
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * openSpeed);
    }

    private void OpenDoor()
    {
        isOpen = true;
        Debug.Log("✅ PORTE OUVERTE !");
    }

    private void CloseDoor()
    {
        isOpen = false;
        Debug.Log("❌ PORTE FERMÉE !");
    }
}
