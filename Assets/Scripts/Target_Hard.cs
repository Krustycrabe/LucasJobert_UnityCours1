using System;
using UnityEngine;

public class Target_Hard : MonoBehaviour
{
    [SerializeField] private int _targetValue = 1;
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Player_Collect>() != null)
        {
            other.gameObject.GetComponent<Player_Collect>().UpdateScore(_targetValue);
            Destroy(gameObject);
        }
        
    }
}
