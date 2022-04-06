using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemy1;
    public GameObject enemy2;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("SpawnEnemy", 5f, 5f);
    }

    public void SpawnEnemy() 
    {
        int rand = Random.Range(1, 3);
        if (rand == 1)
            Instantiate(enemy1, transform.position, Quaternion.identity);
        if (rand == 2)
            Instantiate(enemy2, transform.position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
