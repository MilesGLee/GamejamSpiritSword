using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldBehavior : MonoBehaviour
{
    public Transform target;
    public float Timer;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Sword").transform;
        StartCoroutine(deathCount());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;
    }

    IEnumerator deathCount() 
    {
        yield return new WaitForSeconds(Timer);
        Destroy(gameObject);
    }
}
