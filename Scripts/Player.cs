using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager manager;
    public GameObject[] weapons;
    public bool[] hasWeapons;

    private float Speed = 0.035f;
    private float JumpPower = 0.15f;
    private float Gravity = 0.5f;
    float yVelocity;

    public int ammo;
    public int coin;
    public int health;
    public int hasGrenades;
    public int score;

    public int maxAmmo;
    public int maxCoin;
    public int maxHealth;
    public int maxHasGrenades;

    public float mouseSensitivity = 2.0f;
    public float currentCameraRotationX = 0;

    Vector3 moveVec = Vector3.zero;
    Vector3 dodgeVec;

    float hAxis; // Speed of forward
    float vAxis; // Speed of side
    bool walkDown;
    bool jumpDown;
    bool isJump;
    bool isDodge = false;
    bool isSwap;
    bool fDown;
    bool eDown;
    bool isFireReady;
    bool isReload;
    bool isDamage;
    bool isDead;

    bool sDown0;
    bool sDown1;
    bool sDown2;

    public bool isShop;

    public Camera playerCamera; // Camera
    public Animator animator; // Animator
    CharacterController cc; // Controller
    GameObject nearObject;
    public Weapon equipWeapon;
    Rigidbody rigid;
    MeshRenderer[] meshs;

    int equipWeaponIndex = -1;
    float fireDelay;

    private float verticalRotation = -1;

    //
    public float rotationSpeed = 5.0f; // 카메라 회전 속도
    public LayerMask aimLayerMask; // 에임 대상 레이어 마스크
    public Transform gunTransform; // 총기 위치
    //

    //
    public AudioSource audioSource;

    public AudioClip jumpSound;

    public AudioClip walkingSound;

    public AudioClip runningSound;

    public AudioSource myreloadSfs;
    public AudioClip reloadSfs;

    public void ReloadSound()
    {
        myreloadSfs.PlayOneShot(reloadSfs);
    }
    //

    void Start()
    {
        score = 0;

        cc = GetComponent<CharacterController>();

        playerCamera = GetComponentInChildren<Camera>();

        //Cursor.lockState = CursorLockMode.Locked; // lock mouse cursor 
        animator = GetComponentInChildren<Animator>();

        //
        //playerCamera = GetComponent<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        //

        meshs = GetComponentsInChildren<MeshRenderer>();

        //
        audioSource = GetComponent<AudioSource>();
        //
    }
    void Update() // Activate 60 times per a second
    {
        rigid = GetComponent<Rigidbody>();
        GetInput();

        if (health > 0)
        {
            if (cc.isGrounded && jumpDown)
            {
                playerJump();
                animator.SetBool("isJump", !isJump);
                animator.SetTrigger("doJump");
                isJump = true;


                if (cc.isGrounded)
                {
                    isJump = false;
                    animator.SetBool("isJump", isJump);
                }
            }

            if (cc.isGrounded)
            {
                if (cc.velocity.magnitude > 0)
                {
                    if (!audioSource.isPlaying)
                    {
                        audioSource.clip = !walkDown ? runningSound : walkingSound;
                        audioSource.Play();
                    }
                }
                else
                {
                    audioSource.Stop();
                }

                if (Input.GetButtonDown("Jump"))
                {
                    audioSource.clip = jumpSound;
                    audioSource.Play();
                }
            }
            else
            {
                audioSource.Stop();
            }

            playerMove();
            playerMouse();
            playerDodge();
            playerTurn();
            interaction();
            swap();
            //Attack();
            Reload();
            //playerTurn();
        }
        else
        {
            isDead = true;
            OnDie();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    void GetInput()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        walkDown = Input.GetKey(KeyCode.LeftControl);
        jumpDown = Input.GetKeyDown(KeyCode.Space); // Input.GetButtonDown("Jump");
        sDown0 = Input.GetKeyDown(KeyCode.Alpha1);
        sDown1 = Input.GetKeyDown(KeyCode.Alpha2);
        sDown2 = Input.GetKeyDown(KeyCode.Alpha3);
        fDown = Input.GetKey(KeyCode.Mouse0);
        eDown = Input.GetKeyDown(KeyCode.E);
    }

    void playerMove()
    {
        moveVec.x = hAxis;
        moveVec.y = 0;
        moveVec.z = vAxis;

        moveVec = transform.TransformDirection(moveVec);
        moveVec *= Speed * (walkDown ? 0.3f : 1f);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            moveVec = dodgeVec;
        }

        // Walk and Run
        animator.SetBool("isRun", moveVec != Vector3.zero);
        animator.SetBool("isWalk", walkDown);

        yVelocity -= Gravity * Time.deltaTime;
        moveVec.y = yVelocity;

        cc.Move(moveVec);
    }
    void playerTurn()
    {
        ////Player Turn Eyesight
        //transform.LookAt(transform.position + moveVec);

        //if (fDown)
        //{
        //    Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        //    RaycastHit rayHit;
        //    if (Physics.Raycast(ray, out rayHit, 100))
        //    {
        //        Vector3 nextVec = rayHit.point - transform.position;
        //        nextVec.y = 0;
        //        transform.LookAt(transform.position + nextVec);
        //    }
        //}
    }
    public void playerJump()
    {
        yVelocity = JumpPower;
    }
    public void playerDodge()
    {
        if(cc.isGrounded && Input.GetKeyDown(KeyCode.LeftShift) && !isDodge)
        {
            isDodge = true;
            dodgeVec = moveVec;
            Speed *= 1.5f;
            animator.SetTrigger("doDodge");

            Invoke("DodgeOut", 0.6f);
        }
    }
    public void DodgeOut()
    {
        Speed *= 0.67f;
        isDodge = false;
    }
    void playerMouse()
    {
        // player changing of perspective with a mouse
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        ////
        //verticalRotation += mouseY;
        //verticalRotation = Mathf.Clamp(verticalRotation, -90, 90);

        //playerCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        //transform.rotation *= Quaternion.Euler(0, mouseX, 0);
        ////

        //
        Vector3 rotation = transform.localEulerAngles;
        rotation.y += mouseX;
        rotation.x += mouseY;
        transform.localEulerAngles = rotation;

        // 에임 레이캐스트
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, aimLayerMask))
        {
            // 에임이 대상에 맞았을 때 총기 위치 조정
            gunTransform.LookAt(hit.point);
        }
        //else
        //{
        //    // 에임이 대상을 벗어났을 때 기본 총기 위치로 되돌림
        //    guntransform.localrotation = quaternion.identity;
        //}
        Attack();
        //
    }
    void swap()
    {
        if ((sDown0) && (!hasWeapons[0] || equipWeaponIndex == 0))
        {
            return;
        }
        if ((sDown1) && (!hasWeapons[1] || equipWeaponIndex == 1))
        {
            return;
        }
        if ((sDown2) && (!hasWeapons[2] || equipWeaponIndex == 2))
        {
            return;
        }

        int weaponIndex = -1;
        if (sDown0) weaponIndex = 0;
        if (sDown1) weaponIndex = 1;
        if (sDown2) weaponIndex = 2;

        if ((sDown0 || sDown1 || sDown2))
        {
            if(equipWeapon != null)
            {
                equipWeapon.gameObject.SetActive(false);
            }
            equipWeaponIndex = weaponIndex;
            equipWeapon = weapons[weaponIndex].GetComponent<Weapon>();
            equipWeapon.gameObject.SetActive(true);

            animator.SetTrigger("doSwap");
            isSwap = true;

            Invoke("swapOut", 0.4f);
        }
    }
    void swapOut()
    {
        isSwap = false;
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon" )
        {
            nearObject = null;
        }
        else if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShop = false;
            nearObject = null;
        }
    }
    void interaction()
    {
        if (nearObject != null)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapons[weaponIndex] = true;

                Destroy(nearObject); 
            }
            else if (nearObject.tag == "Shop")
            {
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShop = true;
            }
        }
    }
    void Attack()
    {
        if (equipWeapon == null)
        {
            return;
        }
        fireDelay += Time.deltaTime;
        isFireReady = equipWeapon.rate < fireDelay;

        if (fDown && isFireReady && !isSwap && !isDodge && !isShop) //  && isFireReady && !isSwap && !isDodge
        {
            equipWeapon.Use();
            animator.SetTrigger(equipWeapon.type == Weapon.Type.Melee ? "doSwing" : "doShot");
            fireDelay = 0;
        }
    }
    void Reload()
    {
        if(equipWeapon == null)
        {
            return;
        }
        if (equipWeapon.type == Weapon.Type.Melee)
        {
            return;
        }
        if (ammo <= 0)
        {
            return;
        }
        if (eDown && cc.isGrounded && !isDodge && !isSwap && isFireReady && !isShop)
        {
            animator.SetTrigger("doReload");
            ReloadSound();
            isReload = true;
            Invoke("ReloadOut", 0.5f);
        }
    }
    void ReloadOut()
    {
        int reAmmo = ammo < equipWeapon.maxAmmo ? ammo : equipWeapon.maxAmmo;
        ammo -= reAmmo;
        ammo += equipWeapon.curAmmo;
        equipWeapon.curAmmo = reAmmo;
        isReload = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch (item.type)
            {
                case Item.Type.Ammo:
                    {
                        ammo += item.value;
                        if (ammo > maxAmmo)
                        {
                            ammo = maxAmmo;
                        }
                        break;
                    }
                case Item.Type.Coin:
                    {
                        coin += item.value;
                        if (coin > maxCoin)
                        {
                            coin = maxCoin;
                        }
                        break;
                    }
                case Item.Type.Heart:
                    {
                        health += item.value;
                        if (health > maxHealth)
                        {
                            health = maxHealth;
                        }
                        break;
                    }
                case Item.Type.Grenade:
                    {
                        hasGrenades += item.value;
                        if (hasGrenades > maxHasGrenades)
                        {
                            hasGrenades = maxHasGrenades;
                        }
                        break;
                    }
            }
            Destroy(other.gameObject);
        }
        else if (other.tag == "EnemyBullet")
        {
            if (!isDamage)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                StartCoroutine(OnDamage());
            }
        }
    }
    IEnumerator OnDamage()
    {
        isDamage = true;
        foreach(MeshRenderer mesh in meshs)
        {
            if(health >= 0.5f * health)
            {
                /*mesh.material.color = Color.yellow;*/ // When player have equal and greater than 50 percent of HP, it is yellow
                mesh.material.color = Color.red;
            }
            //else
            //{
            //    mesh.material.color = Color.red; // Otherwise, it is red
            //}
        }
        yield return new WaitForSeconds(1f); // Player will not get damage for 1 second

        isDamage = false;
        foreach(MeshRenderer mesh in meshs)
        {
            mesh.material.color = Color.white;
        }
    }

    void OnDie()
    {
        manager.GameOver();
    }

    //void FreezRotation()
    //{
    //    rigid.angularVelocity = Vector3.zero;
    //}
    //void FixedUpdate()
    //{
    //    FreezRotation();    
    //}
}
