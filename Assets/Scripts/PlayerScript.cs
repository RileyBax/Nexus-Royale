using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    public Weapons Weapons;
    private List<GameObject> nearbyWeapons;
    private UnityEngine.UI.Image[] inventoryUI;
    private GameObject[] inventory;
    [SerializeField] private GameObject hud;
    // Camera target (the main camera) reference
    [SerializeField] private Transform camTarget;
    private int Health = 100;
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer sr;

    // Is the player ready to play
    private bool isReady;

    private Rigidbody2D rigidBody;

    // Start is called before the first frame update
    void Start()
    {
        this.nearbyWeapons = new List<GameObject>();
        this.inventoryUI = new UnityEngine.UI.Image[3];
        this.inventory = new GameObject[3];

        hud = GameObject.Find("HUD");

        for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = hud.transform.GetChild(0).GetChild(i).GetComponent<UnityEngine.UI.Image>();

    }

    public override void Spawned()
    {
        if (HasInputAuthority)
        {
            CameraFollower.Singleton.SetTarget(camTarget);
            Weapons = new Weapons();
        }
    }

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput data))
        {
            rigidBody.velocity = data.Velocity * 5;

            // Handles animations Host looks fine, other players only see their own movement animations
            animator.SetFloat("x", rigidBody.velocity.x);
            animator.SetFloat("y", rigidBody.velocity.y);
            if(rigidBody.velocity.x > 1 || rigidBody.velocity.x < -1) animator.SetBool("side", true);
            else animator.SetBool("side", false);

            if(rigidBody.velocity.x > 1 && sr.flipX == true){
                sr.flipX = false;
            }
            else if (rigidBody.velocity.x < -1 && sr.flipX == false){
                sr.flipX = true;
            }

            if(rigidBody.velocity.x < 1 && rigidBody.velocity.x > -1 && rigidBody.velocity.y < 1 && rigidBody.velocity.y > -1) animator.SetBool("stopped", true);
            else animator.SetBool("stopped", false);

            if (data.WeaponChange != 0)
            {
                Weapons.ChangeWeapon(data.WeaponChange);
            }

            if (data.PickupWeapon && nearbyWeapons.Count > 0)
            {
                PickupWeapon(nearbyWeapons[0]);
            }

            Weapon weapon = Weapons.GetSelectedWeapon();
            if (weapon != null)
            {
                Rigidbody2D rb = weapon.GetComponent<Rigidbody2D>();
                float angle = -(float)Math.Atan2(data.MousePos.y - transform.position.y, data.MousePos.x - transform.position.x) + 1.55f;

                Vector3 weaponPos = Vector3.zero;

                weaponPos.x = (float)(transform.position.x + Math.Sin(angle) * 1);
                weaponPos.y = (float)(transform.position.y + Math.Cos(angle) * 1);

                weapon.transform.position = weaponPos;
                weapon.transform.up = new Vector2(data.MousePos.x - transform.position.x, data.MousePos.y - transform.position.y);
                
                if (data.FireWeapon)
                {
                    weapon.SendMessage("FireWeapon", new Vector3(data.MousePos.x, data.MousePos.y, 0));
                }
            }

        }

        double ping = Runner.GetPlayerRtt(PlayerRef.None) * 1000;

        if (hud != null)
        {

        hud.transform.GetChild(1).GetComponent<TMP_Text>().text = ping.ToString();
        }

    }

    public void PickupWeapon(GameObject w)
    {
        Weapon weapon = w.GetComponent<Weapon>();
        Weapons.PickupWeapon(weapon);
        weapon.SetPlayer(this.gameObject);

        nearbyWeapons.Remove(w);
    }

    public void AddNearbyWeapon(GameObject weapon)
    {
        nearbyWeapons.Add(weapon);
        Debug.Log(nearbyWeapons.Count);
    }

    public void RemoveNeabyWeapon(GameObject weapon)
    {
        nearbyWeapons.Remove(weapon);
    }

    // Updates health, called from bullet script
    public void updateHealth(int damage){

        Health -= damage;
        if(Health <= 0) Runner.Despawn(this.GetComponent<NetworkObject>());

    }

}
