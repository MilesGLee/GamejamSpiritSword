using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Transform target;
    float timer = 5;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
        StartCoroutine(deathTimer());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, Time.deltaTime * 10);
    }

    IEnumerator deathTimer() 
    {
        yield return new WaitForSeconds(timer);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == target) 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Application.LoadLevel("main_menu");
        }
        if (other.tag == "Shield") 
        {
            Destroy(gameObject);
        }
    }
}
