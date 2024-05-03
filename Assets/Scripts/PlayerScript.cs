using Fusion;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    private GameObject weapon;
    private List<GameObject> nearbyWeapons;
    private UnityEngine.UI.Image[] inventoryUI;
    private GameObject[] inventory;
    [SerializeField] private GameObject hud;
    // Camera target (the main camera) reference
    [SerializeField] private Transform camTarget;
    private int selectedWeapon = 0;
    private int health = 100;
    private bool isEquipped;

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

            if (data.WeaponChange != 0)
            {
                changeWeapon(data.WeaponChange - 1);

                for (int i = 0; i < inventoryUI.Length; i++)
                {

                    if (i != selectedWeapon) inventoryUI[i].color = Color.grey;
                    else inventoryUI[i].color = Color.blue;

                }
            }

            if (data.PickupWeapon && !isEquipped && nearbyWeapons.Count > 0)
            {
                PickupWeapon(nearbyWeapons[0]);
            }

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

    public GameObject getWeapon(){

        return weapon;

    }

    public void PickupWeapon(GameObject weapon)
    {
            // For the bots
            // weapon.gameObject.SendMessage("getEquipped", weapon);

            if (inventory[selectedWeapon] != null)
            {

                inventory[selectedWeapon].SendMessage("setEquipped", false);
                inventory[selectedWeapon].SendMessage("setCharacterNull");

            }

                inventoryUI[selectedWeapon].sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                inventory[selectedWeapon] = weapon.gameObject;
                weapon.gameObject.SendMessage("setEquipped", true);
                weapon.gameObject.SendMessage("setCharacter", this.gameObject);
                // can change above to remove equipped boolean but dont want to

        this.weapon = weapon;

        nearbyWeapons.Remove(weapon);
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

    // Swap weapon between those in inventory
    public void changeWeapon(int selected){

        // fix weapon angle jitter on swap

        selectedWeapon = selected;

        if(weapon != null) weapon.gameObject.SetActive(false);
        if(inventory[selectedWeapon] != null) {

            weapon = inventory[selectedWeapon];
            weapon.gameObject.SetActive(true);

        }
        else weapon = null;

    }

    // Updates health, called from bullet script
    public void updateHealth(int damage){

        health -= damage;
        if(health <= 0) Runner.Despawn(this.GetComponent<NetworkObject>());

    }

    void setIsEquipped(bool e){

        isEquipped = e;

    }

}
