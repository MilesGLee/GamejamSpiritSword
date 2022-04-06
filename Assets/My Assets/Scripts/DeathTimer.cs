using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTimer : MonoBehaviour
{
    [SerializeField] private float Timer;

    private void Awake()
    {
        StartCoroutine(OnDeath());
    }

    IEnumerator OnDeath() 
    {
        yield return new WaitForSeconds(Timer);
        Destroy(gameObject);
    }
}
