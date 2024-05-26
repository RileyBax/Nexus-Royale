using ExitGames.Client.Photon;
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
    [Networked] private int Health {get; set;}
    private int lastHealth = 100;
    [SerializeField] SpriteRenderer sr;

    // Is the player ready to play
    private bool isReady;

    private Rigidbody2D rigidBody;
    [SerializeField] private Sprite[] spriteIdle;
    [SerializeField] private Sprite[] spriteWalk;
    [Networked] private int selectedSprite {get; set;} // 1 - 10
    private float frameTime = 0;
    private int frame;
    private int startFrame;
    public bool isMoving = false;
    public bool up = false;
    public bool down = true;
    public bool side = false;
    private float damageTimer;
    private Color baseColor = new Color(1, 1, 1, 1);
    [SerializeField] NetworkObject deathEmitter;

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

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

        Health = 100;

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

            RpcUpdateSpriteState(data);

        }

        double ping = Runner.GetPlayerRtt(PlayerRef.None) * 1000;

        if (hud != null)
        {

        hud.transform.GetChild(1).GetComponent<TMP_Text>().text = ping.ToString();
        }

        RpcUpdateSprite();

    }

    void FixedUpdate(){

        if(lastHealth > Health) {
            damageTimer = 1.0f;
        }
        lastHealth = Health;

        if(Health <= 0) {

            Instantiate(deathEmitter, this.transform.position, Quaternion.identity);
            Runner.Despawn(this.GetComponent<NetworkObject>());
            transform.gameObject.SetActive(false);

        }

    }

    public void PickupWeapon(GameObject w)
    {
        Weapon weapon = w.GetComponent<Weapon>();
        if(!weapon.GetEquipped()){
            Weapons.PickupWeapon(weapon);
            weapon.SetPlayer(this.gameObject);
            weapon.setEquipped(true);

            nearbyWeapons.Remove(w);
        }
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

    }

    [Rpc]
    void RpcUpdateSprite(){

        // Visual feed back for health change
        if(damageTimer > 0.0f) {

            sr.color = baseColor - new Color(0, damageTimer, damageTimer, 0);
            damageTimer -= Runner.DeltaTime;

        }

        frameTime += Runner.DeltaTime;

        if(isMoving == false){

            if(frameTime >= 1){

                frame += 3;
                frameTime = 0.5f;

            }
            
            sr.sprite = spriteIdle[frame % 6];

        }
        else{

            if(frameTime >= 1){

                frame += 3;
                frameTime = 0.7f;

            }

            sr.sprite = spriteWalk[frame % 12];

        }

    }

    [Rpc]
    void RpcUpdateSpriteState(NetInput data){

        // Handles animations states

        if(data.Velocity.x == 0.0f && data.Velocity.y == 0.0f) isMoving = false;
        else isMoving = true;

        if(data.Velocity.y >= 1 && up != true){
            frame = 1;
            startFrame = frame;
            up = true;
            down = false;
            side = false;
        }
        else if(data.Velocity.y <= -1 && down != true){
            frame = 0;
            startFrame = frame;
            up = false;
            down = true;
            side = false;
        }
        else if(data.Velocity.x >= 1 && data.Velocity.y >= -0.1 && data.Velocity.y <= 0.1 && side != true 
        || data.Velocity.x <= -1 && data.Velocity.y >= -0.1 && data.Velocity.y <= 0.1 && side != true){
            frame = 2;
            startFrame = frame;
            up = false;
            down = false;
            side = true;
        }

        if(data.Velocity.x <= -1 && sr.flipX != true) sr.flipX = true;
        else if(data.Velocity.x >= 1 && sr.flipX != false) sr.flipX = false;

    }

    public void setSprite(int s){

        selectedSprite = s;

        spriteIdle = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " idle");
        spriteWalk = Resources.LoadAll<Sprite>("Sprites/" + selectedSprite + " walk");

    }

}
