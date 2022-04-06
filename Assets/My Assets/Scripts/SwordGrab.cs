using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordGrab : MonoBehaviour
{
    [SerializeField] private Transform owner;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            other.GetComponent<PlayerSword>().GrabSword(owner);
        }
    }
}
