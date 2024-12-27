using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy : MonoBehaviour
{
    //public Transform target0;
    public Player player;
    public enum Type { A, B, C };
    public Type enemyType;

    public int maxHealth;
    public int currentHealth;
    public int score;

    public bool isChase;
    public bool isAttack;
    public bool isDamage = true;
    public BoxCollider meleeArea;

    Rigidbody rigid;
    SphereCollider sphereCollider;
    Material mat;
    NavMeshAgent nav;
    Animator anim;

    public GameObject bullet;
    public Transform bulletPos;

    int n;

    //
    public AudioSource myAttackBSfx;
    public AudioClip AttackBSfx;

    public AudioSource myAttackCSfx;
    public AudioClip AttackCSfx;

    public AudioSource myEnemyDamageCSfx;
    public AudioClip EnemyDamageCSfx;

    public void EnemyDammageSound()
    {
        myEnemyDamageCSfx.PlayOneShot(EnemyDamageCSfx);
    }

    public void AttackBSound()
    {
        myAttackBSfx.PlayOneShot(AttackBSfx);
    }
    public void AttackCSound()
    {
        myAttackCSfx.PlayOneShot(AttackCSfx);
    }
    //

    //
    private GameObject getPlayer;
    //

    // Start is called before the first frame update
    void Start()
    {

        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        mat = GetComponentInChildren<SkinnedMeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        
        Invoke("ChaseStart", 2);

        //
        getPlayer = GameObject.Find("Player");
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        //

    }
    void ChaseStart()
    {
        isChase = true;
        anim.SetBool("isWalk", true);
    }
    // Update is called once per frame
    void Update()
    {
        Vector3 playerPosition = getPlayer.transform.position;

        if (nav.enabled)
        {
            nav.SetDestination(playerPosition);
            nav.isStopped = !isChase;
        }
    }

    public GameObject itemPrefab; // Prefab for dropping item

    private void OnDestroy()
    {
        n = Random.Range(1, 2); // Percentage of Item to drop is 25 % 
        if (itemPrefab != null && n ==1)
        {
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Melee")
        {
            EnemyDammageSound();
            Weapon weapon = other.GetComponent<Weapon>();
            currentHealth -= weapon.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
        else if (other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            currentHealth -= bullet.damage;
            Vector3 reactVec = transform.position - other.transform.position;
            StartCoroutine(OnDamage(reactVec));
        }
    }

    IEnumerator OnDamage(Vector3 reactVec)
    {
        mat.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if(currentHealth > 0)
        {
            mat.color = Color.white;
        }
        else
        {
            if (isDamage)
            {
                mat.color = Color.gray;
                gameObject.layer = 13;
                isChase = false;
                isAttack = false;
                nav.enabled = false;

                anim.SetTrigger("doDie");

                reactVec = reactVec.normalized;
                reactVec += Vector3.up;

                rigid.AddForce(reactVec * 4f, ForceMode.Impulse);

                player.score += score;

                Destroy(gameObject, 2f);
                isDamage = false;
            }
        }
    }
    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }
    void Targetting()
    {
        float targetRadius = 0f;
        float targetRange = 0f;

        switch (enemyType)
        {
            case Type.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case Type.B:
                targetRadius = 2f;
                targetRange = 20f;
                break;
            case Type.C:
                targetRadius = 1.5f;
                targetRange = 30f;
                break;
        }

        RaycastHit[] rayHits =
            Physics.SphereCastAll(transform.position, targetRadius,
                                    Vector3.forward, targetRange,
                                    LayerMask.GetMask("Player"));

        if (rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
            Debug.Log("Doing Attack Method!");
        }
    }
    IEnumerator Attack()
    {
        isChase = false;
        isAttack = true;
        anim.SetBool("isAttack", true);

        switch (enemyType)
        {
            case Type.A:
                yield return new WaitForSeconds(0.2f);
                meleeArea.enabled = true;

                yield return new WaitForSeconds(1f);
                meleeArea.enabled = false;
                
                yield return new WaitForSeconds(1f);
                
                break;

            case Type.B:
                //yield return new WaitForSeconds(0.1f);
                //rigid.AddForce(transform.forward * 500, ForceMode.Impulse);
                //meleeArea.enabled = true;
                //AttackBSound();

                //yield return new WaitForSeconds(0.5f);
                //rigid.velocity = Vector3.zero;
                //meleeArea.enabled = false;

                //yield return new WaitForSeconds(1f);
                yield return new WaitForSeconds(0.4f);
                GameObject instantBulletB = Instantiate(bullet, transform.position +
                    new Vector3(0f, 1.2f, 0f), transform.rotation); //transform.position
                Rigidbody rigidBulletB = instantBulletB.GetComponent<Rigidbody>();
                rigidBulletB.velocity = transform.forward * 15f;
                AttackBSound();

                break;

            case Type.C:
                yield return new WaitForSeconds(1f);
                GameObject instantBullet = Instantiate(bullet, transform.position + 
                    new Vector3(0f, 1.2f, 0f), transform.rotation); //transform.position
                Rigidbody rigidBullet = instantBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20f;
                AttackCSound();

                //yield return new WaitForSeconds(2f);
                break;
        }
        isChase = true;
        isAttack = false;
        anim.SetBool("isAttack", false);
    }
    void FixedUpdate()
    {
        Targetting();
        FreezeVelocity();
    }
}
