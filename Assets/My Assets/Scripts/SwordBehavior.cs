using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordBehavior : MonoBehaviour
{
    private Vector3 targetPosition;
    private Quaternion targetRotation;
    public Vector3 startPosition;
    private float t;
    private float timeToReachTarget;
    public Transform basket;
    public bool findAttack;
    private GameObject target;

    void Start()
    {
        startPosition = targetPosition = transform.position;
        targetRotation = transform.rotation;
    }

    void Update()
    {
        if (target != null && target.tag != "Player")
            targetPosition = target.transform.position;
        UpdatePosition();
        if (GameObject.FindGameObjectWithTag("TaggedEnemy") == null)
        {
            findAttack = false;
        }
        if (findAttack) 
        {
            GameObject enemy = FindClosestEnemy(Mathf.Infinity);
            SetDestination(enemy.transform, 0.25f);
        }
    }

    void UpdatePosition() 
    {
        t += Time.deltaTime / timeToReachTarget;
        transform.position = Vector3.Lerp(startPosition, targetPosition, t);
        Quaternion rot = targetRotation;
        rot.x = 0;
        rot.z = 0;
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, 5f * Time.deltaTime);
    }

    public void SetDestination(GameObject destination, float time)
    {
        t = 0;
        startPosition = transform.position;
        target = destination;
        timeToReachTarget = time;
        targetPosition = destination.transform.position;
        targetRotation = destination.transform.rotation;
    }

    public void SetDestination(Transform destination, float time)
    {
        t = 0;
        startPosition = transform.position;
        timeToReachTarget = time;
        targetPosition = destination.transform.position;
        targetRotation = Quaternion.LookRotation(destination.position);
    }

    public GameObject FindClosestEnemy(float dist)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("TaggedEnemy");
        GameObject closest = null;
        float distance = dist;
        Vector3 position = transform.position;
        foreach (GameObject go in gos)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                closest = go;
                distance = curDistance;
            }
        }
        return closest;
    }
}
