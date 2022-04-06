using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSword : MonoBehaviour
{
    [SerializeField] private SwordBehavior _sb;
    [SerializeField] private Transform cam;
    [SerializeField] private Transform orientation;
    [Header("Summon Sword Variables")]
    [SerializeField] private float summonMaxDistance;
    [SerializeField] private LayerMask enemyLayerMask;
    [SerializeField] private LayerMask summonLayerMask;
    [SerializeField] private GameObject summonReference;
    private GameObject currentSummonReference;
    private bool currentSummonCheck;
    [Header("Connecting to Sword Variables")]
    public bool connectedToSword;
    private bool swordGrabbed;
    private Transform grabbed;
    [SerializeField] private Transform grabPoint;
    [Header("Attacking Sword Variables")]
    private bool check1, check2, attackCheck, attack2Check, attack3Check;
    private float attackCooldown, attack2Cooldown, attack3Cooldown, currentAttackCooldown, currentAttack2Cooldown, currentAttack3Cooldown;
    [SerializeField] private Image attackImage, attack2Image, attack3Image;
    [SerializeField] private GameObject enemyTag, shield;


    void Start()
    {
        attackCooldown = 2;
        attack2Cooldown = 20;
        attack3Cooldown = 10;
    }

    void Update()
    {
        SummonSword();
        ConnectToSword();
        AttackSword();
        Attack2Sword();
        Attack3Sword();
        Quaternion rot = cam.rotation;
        rot.x = 0;
        rot.z = 0;
        orientation.rotation = rot;
        if (swordGrabbed == true) 
        {
            grabbed.position = grabPoint.position;
            grabbed.rotation = grabPoint.rotation;
        }
    }

    void AttackSword()
    {
        if (check1 && !attackCheck) 
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, summonMaxDistance, enemyLayerMask))
            {
                hit.transform.GetComponent<EnemyBehavior>().OnSelected();
                if (Input.GetKeyDown(KeyCode.Mouse0)) 
                {
                    //_sb.findAttack = false;
                    _sb.SetDestination(hit.transform.gameObject, 0.5f);
                    StartCoroutine(AttackCooldown());
                    check1 = false;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            check1 = true;
        }
        if (Input.GetKeyUp(KeyCode.Alpha1))
        {
            check1 = false;
        }
    }

    void Attack2Sword()
    {
        if (check2 && !attack2Check)
        {
            GameObject[] gos;
            gos = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject go in gos)
            {
                if (go.GetComponentInChildren<Renderer>().isVisible)
                {
                    go.GetComponent<EnemyBehavior>().OnSelected();
                }
            }
            if (Input.GetKeyDown(KeyCode.Mouse0)) 
            {
                foreach (GameObject go in gos)
                {
                    if (go.GetComponentInChildren<Renderer>().isVisible)
                    {
                        Instantiate(enemyTag, go.transform);
                    }
                }
                StartCoroutine(Attack2Cooldown());
                check2 = false;
                _sb.findAttack = true;
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            check2 = true;
        }
        if (Input.GetKeyUp(KeyCode.Alpha2))
        {
            check2 = false;
        }
    }

    void Attack3Sword()
    {
        if (Input.GetKeyDown(KeyCode.Alpha3) && !attack3Check)
        {
            GameObject sh = Instantiate(shield, transform.position, Quaternion.identity);
            shield.GetComponent<ShieldBehavior>().target = _sb.transform;
            StartCoroutine(Attack3Cooldown());
        }
    }

    IEnumerator AttackCooldown() 
    {
        attackCheck = true;
        currentAttackCooldown = 2;
        InvokeRepeating("VisualizeAttackCD", 0, 0.01f);
        attackImage.fillAmount = 0;
        yield return new WaitForSeconds(attackCooldown);
        CancelInvoke("VisualizeAttackCD");
        attackCheck = false;
    }
    IEnumerator Attack2Cooldown()
    {
        attack2Check = true;
        currentAttack2Cooldown = 20;
        InvokeRepeating("VisualizeAttack2CD", 0, 0.01f);
        attack2Image.fillAmount = 0;
        yield return new WaitForSeconds(attack2Cooldown);
        CancelInvoke("VisualizeAttack2CD");
        attack2Check = false;
    }
    IEnumerator Attack3Cooldown()
    {
        attack3Check = true;
        currentAttack3Cooldown = 10;
        InvokeRepeating("VisualizeAttack3CD", 0, 0.01f);
        attack3Image.fillAmount = 0;
        yield return new WaitForSeconds(attack3Cooldown);
        CancelInvoke("VisualizeAttack3CD");
        attack3Check = false;
    }

    void VisualizeAttackCD() 
    {
        currentAttackCooldown -= 0.01f;
        float cd = 1 - (currentAttackCooldown / 2);
        attackImage.fillAmount = cd;
    }

    void VisualizeAttack2CD()
    {
        currentAttack2Cooldown -= 0.01f;
        float cd = 1 - (currentAttack2Cooldown / 20);
        attack2Image.fillAmount = cd;
    }

    void VisualizeAttack3CD()
    {
        currentAttack3Cooldown -= 0.01f;
        float cd = 1 - (currentAttack3Cooldown / 10);
        attack3Image.fillAmount = cd;
    }

    void ConnectToSword() 
    {
        if (connectedToSword == true)
        {
            transform.position = _sb.basket.position;
            swordGrabbed = false;
            grabbed = null;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                connectedToSword = false;
                Rigidbody playerRigidbody = GetComponent<Rigidbody>();
                playerRigidbody.AddForce(Vector2.up * 50 * 1.5f);
                playerRigidbody.AddForce(Vector3.up * 50 * 0.5f);

                //If jumping while in air, reset y velocity
                Vector3 vel = playerRigidbody.velocity;
                if (playerRigidbody.velocity.y > 0.5f)
                    playerRigidbody.velocity = new Vector3(vel.x, 0, vel.z);
                else if (playerRigidbody.velocity.y > 0)
                    playerRigidbody.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            }
        }
    }

    void SummonSword() 
    {
        if (currentSummonCheck == true) 
        {
            RaycastHit hit;
            if (Physics.Raycast(cam.position, cam.forward, out hit, summonMaxDistance, summonLayerMask))
            {
                Vector3 newPos = hit.point;
                RaycastHit hit2;
                if (Physics.Raycast(hit.point, hit.normal, out hit2, 2))
                    newPos += (hit2.point - hit.point).normalized;
                else
                    newPos += hit.normal * 2;
                currentSummonReference.transform.position = newPos;
                Quaternion newRot = cam.rotation;
                newRot.x = 0;
                newRot.z = 0;
                currentSummonReference.transform.rotation = newRot;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _sb.SetDestination(currentSummonReference, 1);
                    Destroy(currentSummonReference);
                    currentSummonCheck = false;
                    swordGrabbed = false;
                    if (grabbed != null)
                        grabbed.GetComponent<SwordBehavior>().startPosition = grabPoint.position;
                    grabbed = null;
                }
                if (Input.GetKeyDown(KeyCode.Mouse1))
                {
                    _sb.SetDestination(gameObject, 0.25f);
                    Destroy(currentSummonReference);
                    currentSummonCheck = false;
                    swordGrabbed = false;
                    if (grabbed != null)
                        grabbed.GetComponent<SwordBehavior>().startPosition = grabPoint.position;
                    grabbed = null;
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currentSummonCheck = true;
            currentSummonReference = Instantiate(summonReference, transform.position, Quaternion.identity);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (currentSummonReference != null)
                Destroy(currentSummonReference);
            if (currentSummonCheck == true)
                currentSummonCheck = false;

        }
        
        
    }

    public GameObject FindClosestEnemy(float dist)
    {
        GameObject[] gos;
        gos = GameObject.FindGameObjectsWithTag("Enemy");
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

    public void GrabSword(Transform trans) 
    {
        swordGrabbed = true;
        grabbed = trans;
    }
}
