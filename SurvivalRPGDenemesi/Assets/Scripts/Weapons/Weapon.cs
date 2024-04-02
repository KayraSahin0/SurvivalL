using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private Player player;
    private AudioSource audioS;

    [HideInInspector] public Animator anim;
    [HideInInspector] public Slot slotEquippedOn;
    public GameObject bulletHolePrefab;
    public ItemSO weaponData;
    public bool isAutomatic;
    [Space]
    public Transform shootPoint;
    public LayerMask shootableLabels;

    [Header("Aiming")]
    public float aimSpeed = 10;
    public Vector3 hipPos;
    public Vector3 aimPos;
    public bool isAiming;

    [HideInInspector] public bool isReloading;
    [HideInInspector] public bool hasTakenOut;

    private float currentFireRare;
    private float fireRate;

    void Start()
    {
        player = GetComponentInParent<Player>();
        anim = GetComponentInChildren<Animator>();
        audioS = GetComponentInChildren<AudioSource>();

        transform.localPosition = hipPos;

        fireRate = weaponData.fireRate;
        currentFireRare = weaponData.fireRate;
    }

    
    void Update()
    {
        UpdateAnimations();

        if (weaponData.itemType == ItemSO.ItemType.Weapon)
        {
            if (currentFireRare < fireRate)
                currentFireRare += Time.deltaTime;

            if (Input.GetKeyDown(KeyCode.R) && !player.windowHandler.inventory.opened)
                Start_Reload();

            UpdateAiming();

            if (player.windowHandler.inventory.opened)
                return;

            if (isAutomatic)
            {
                if (Input.GetButton("Fire1"))
                {
                    if (!weaponData.shotgunFire)
                        Shoot();
                    else
                        ShotgunShoot();
                }
            }
            else
            {
                if (Input.GetButtonDown("Fire1"))
                {
                    if (!weaponData.shotgunFire)
                        Shoot();
                    else
                        ShotgunShoot();
                }

            }
        }
        else if (weaponData.itemType == ItemSO.ItemType.MeleeWeapon)
        {
            if (currentFireRare < fireRate)
                currentFireRare += Time.deltaTime;

            if (Input.GetButton("Fire1"))
                Swing();
        }
    }

    #region Fire Weapon Functions

    public void Shoot()
    {
        if (currentFireRare < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0 || player.windowHandler.inventory.opened)
            return;

        RaycastHit hit;
        Vector3 shootDir = shootPoint.forward;

        if (isAiming)
        {
            shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
        }
        else
        {
            shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
        }

        if(Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLabels))
        {
            GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            Debug.Log($"Hitted: {hit.transform.name}");
        }

        anim.CrossFadeInFixedTime("Shoot_BASE", 0.015f);
        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil), Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil));
        audioS.PlayOneShot(weaponData.shootSound);
        currentFireRare = 0;
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }

    public void ShotgunShoot()
    {
        if (currentFireRare < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0 || player.windowHandler.inventory.opened)
            return;

        for (int i = 0; i < weaponData.pelletPerShot; i++)
        {
            RaycastHit hit;
            Vector3 shootDir = shootPoint.forward;

            if (isAiming)
            {
                shootDir.x += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
                shootDir.y += Random.Range(-weaponData.aimSpread, weaponData.aimSpread);
            }
            else
            {
                shootDir.x += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
                shootDir.y += Random.Range(-weaponData.hipSpread, weaponData.hipSpread);
            }

            if (Physics.Raycast(shootPoint.position, shootDir, out hit, weaponData.range, shootableLabels))
            {
                GameObject bulletHole = Instantiate(bulletHolePrefab, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
                Debug.Log($"Hitted: {hit.transform.name}");
            }
        }
        

        anim.CrossFadeInFixedTime("Shoot_BASE", 0.015f);
        GetComponentInParent<CameraLook>().RecoilCamera(Random.Range(-weaponData.horizontalRecoil, weaponData.horizontalRecoil), Random.Range(weaponData.minVerticalRecoil, weaponData.maxVerticalRecoil));
        audioS.PlayOneShot(weaponData.shootSound);
        currentFireRare = 0;
        slotEquippedOn.stackSize--;
        slotEquippedOn.UpdateSlot();
    }

    public void Start_Reload()
    {
        if (isReloading || slotEquippedOn.stackSize >= weaponData.magSize || player.running || !hasTakenOut || !CheckForBullets(weaponData.bulletData, weaponData.magSize))
            return;

        audioS.PlayOneShot(weaponData.reloadSound);

        anim.CrossFadeInFixedTime("Reload_BASE", 0.015f);
        isReloading = true;
    }

    public void Finish_Reload()
    {
        isReloading = false;
        TakeBullets(weaponData.bulletData, weaponData.magSize);
    }

    

    public void UpdateAiming()
    {
        if(Input.GetButton("Fire2") && !player.running && !isReloading && !player.windowHandler.inventory.opened)
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, aimPos, aimSpeed * Time.deltaTime);
            isAiming = true;
        }
        else
        {
            transform.localPosition = Vector3.Slerp(transform.localPosition, hipPos, aimSpeed * Time.deltaTime);
            isAiming = false;
        }
    }

    private bool CheckForBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int amountfound = 0;

        for (int b = 0; b < inventory.inventorySlots.Length; b++)
        {
            if (!inventory.inventorySlots[b].IsEmpty)
            {
                if (inventory.inventorySlots[b].data == bulletData)
                {
                    amountfound += inventory.inventorySlots[b].stackSize;
                }
            }
        }


        if (amountfound < 1)
            return false;

        return true;
    }

    public void TakeBullets(ItemSO bulletData, int magSize)
    {
        InventoryManager inventory = GetComponentInParent<Player>().GetComponentInChildren<InventoryManager>();

        int ammoToReload = weaponData.magSize - slotEquippedOn.stackSize;
        int ammoInTheInventory = 0;

        // CHECK FOR THE BULLETS
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].IsEmpty)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    ammoInTheInventory += inventory.inventorySlots[i].stackSize;
                }
            }
        }


        int ammoToTakeFromInventory = (ammoInTheInventory >= ammoToReload) ? ammoToReload : ammoInTheInventory;


        // TAKE THE BULLETS FROM THE INVENTORY
        for (int i = 0; i < inventory.inventorySlots.Length; i++)
        {
            if (!inventory.inventorySlots[i].IsEmpty && ammoToTakeFromInventory > 0)
            {
                if (inventory.inventorySlots[i].data == bulletData)
                {
                    if (inventory.inventorySlots[i].stackSize <= ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize += inventory.inventorySlots[i].stackSize;
                        ammoToTakeFromInventory -= inventory.inventorySlots[i].stackSize;
                        inventory.inventorySlots[i].Clean();
                    }
                    else if (inventory.inventorySlots[i].stackSize > ammoToTakeFromInventory)
                    {
                        slotEquippedOn.stackSize = weaponData.magSize;
                        inventory.inventorySlots[i].stackSize -= ammoToTakeFromInventory;
                        ammoToTakeFromInventory = 0;
                        inventory.inventorySlots[i].UpdateSlot();
                    }
                }
            }
        }


        slotEquippedOn.UpdateSlot();

    }

    #endregion

    #region Melee Weapon Functions

    public void Swing()
    {
        if (currentFireRare < fireRate || isReloading || !hasTakenOut || player.running || slotEquippedOn.stackSize <= 0)
            return;

        anim.SetTrigger("Swing");
        currentFireRare = 0;
    }

    public void CheckForHit()
    {
        RaycastHit hit;

        if (Physics.SphereCast(shootPoint.position, 0.2f, shootPoint.forward, out hit, weaponData.range, shootableLabels))
        {
            Hit();
        }
        else
            Miss();
    }

    public void Miss()
    {
        anim.SetTrigger("Miss");
    }

    public void Hit()
    {
        anim.SetTrigger("Hit");
    }

    public void ExecuteHit()
    {
        RaycastHit hit;

        if (Physics.SphereCast(shootPoint.position, 0.2f, shootPoint.forward, out hit, weaponData.range, shootableLabels))
        {
            
        }
    }

    #endregion

    public void UpdateAnimations()
    {
        anim.SetBool("Running", player.running);
    }

    public void Equip(Slot slot)
    {
        gameObject.SetActive(true);
        GetComponentInParent<CameraFOV_Handler>().weapon = this;
        slotEquippedOn = slot;
        transform.localPosition = hipPos;
    }
    
    public void UnEquip()
    {
        GetComponentInParent<CameraFOV_Handler>().weapon = null;
        gameObject.SetActive(false);
        slotEquippedOn = null;
    }


    
}
