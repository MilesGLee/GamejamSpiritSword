using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    [SerializeField] private GameObject visual;
    [SerializeField] private GameObject selectedVisual;
    [SerializeField] private GameObject deathParticle;
    [SerializeField] private int health;
    [SerializeField] private int damage;
    private NavMeshAgent nav;
    public Transform player;
    [SerializeField] private bool ranged;
    [SerializeField] private GameObject projectile;

    void Start()
    {
        nav = GetComponent<NavMeshAgent>();
        visual.SetActive(true);
        selectedVisual.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (ranged == true) 
        {
            InvokeRepeating("SpawnProjectile", 5f, 5f);
        }
    }

    void Update()
    {
        nav.SetDestination(player.position);

        selectedVisual.SetActive(false);

        if (health <= 0)
            Death();
        
    }

    void SpawnProjectile() 
    {
        Instantiate(projectile, transform.position, Quaternion.identity);
    }

    public void OnSelected()
    {
        visual.SetActive(true);
        selectedVisual.SetActive(true);
    }

    void Death() 
    {
        Instantiate(deathParticle, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword") 
        {
            Death();
        }
        if (other.tag == "Player") 
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Application.LoadLevel("main_menu");
        }
    }
}
