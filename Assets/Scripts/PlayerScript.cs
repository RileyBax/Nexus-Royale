using Fusion;
using System;
using UnityEngine;

public class PlayerScript : NetworkBehaviour
{
    private GameObject weapon;
    private Vector2 weaponPos;
    private Vector3 mousePos;
    private float angle;
    private UnityEngine.UI.Image[] inventoryUI = new UnityEngine.UI.Image[3];
    private GameObject[] inventory = new GameObject[3];
    [SerializeField] private GameObject hud;
    // Camera target (the main camera) reference
    [SerializeField] private Transform camTarget;
    private int selectedWeapon = 0;
    private int health = 100;
    private bool isEquipped;
    private float damageTimer;
    private Color baseColor = new Color(0.7f, 0.8f, 0.5f, 255);
    private SpriteRenderer sr;
    private float zoneDamageTimer = 2.0f;
    private bool insideZone = true;
    private float healTimer;
    private TextMeshProUGUI ammoText;

    // Is the player ready to play
    private bool isReady;

    private NetworkCharacterController _characterController;

    // Start is called before the first frame update
    void Start()
    {

        weaponPos = new Vector2();
        hud = GameObject.Find("HUD");
        ammoText = hud.transform.Find("Inventory").transform.Find("Ammo").GetComponent<TextMeshProUGUI>();
        sr = GetComponent<SpriteRenderer>();

        for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = hud.transform.GetChild(0).GetChild(i).GetComponent<UnityEngine.UI.Image>();

    }

    // Update is called once per frame
    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Angle to mouse position
        angle = -(float) Math.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 1.55f;

        // Rotates weapon towards mouse
        if(weapon != null){

            weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
            weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

            weapon.transform.position = weaponPos;
            weapon.transform.up = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

            if(Input.GetMouseButton(0)) {
                weapon.SendMessage("FireWeapon");
                updateAmmo();
            }

        }

        // Inventory selection
        if(Input.anyKeyDown){

            Int32.TryParse(Input.inputString, out int inputNum);

            if(inputNum >= 1 && inputNum <= 3) changeWeapon(inputNum - 1);

        }

        // Sets selected inventory slot colour
        for(int i = 0; i < inventoryUI.Length; i++){
            
            if(i != selectedWeapon) inventoryUI[i].color = Color.grey;
            else inventoryUI[i].color = Color.blue;

        }

        if(damageTimer > 0.0f) {

            sr.color = baseColor - new Color(0, 1.5f * damageTimer, damageTimer);
            damageTimer -= Time.deltaTime;

        }

        if(healTimer > 0.0f){

            sr.color = baseColor - new Color(healTimer, 0, healTimer);
            healTimer -= Time.deltaTime;

        }

        if(!insideZone && zoneDamageTimer <= 0.0f){

            zoneDamageTimer = 2.0f;
            updateHealth(10);

        }
        else if(!insideZone && zoneDamageTimer > 0.0f) zoneDamageTimer -= Time.deltaTime;

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
        _characterController = GetComponent<NetworkCharacterController>();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetInput data))
        {
            _characterController.transform.position += data.Position * Time.deltaTime * 50;
        }
    }

    // Handles collisions and weapon equip
    void OnTriggerStay2D(Collider2D col){

        if(col.tag.Equals("Weapon")){

            col.gameObject.SendMessage("getEquipped", transform.gameObject);

            if(Input.GetKey(KeyCode.E) && !isEquipped){

                weapon = col.gameObject;

                if(inventory[selectedWeapon] != null) {
                    
                    inventory[selectedWeapon].SendMessage("setEquipped", false);
                    inventory[selectedWeapon].SendMessage("setCharacterNull");

                }

                inventoryUI[selectedWeapon].sprite = weapon.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
                inventory[selectedWeapon] = col.gameObject;
                col.gameObject.SendMessage("setEquipped", true);
                col.gameObject.SendMessage("setCharacter", transform.gameObject);
                updateAmmo();

            }

        }

    }

    void OnTriggerExit2D(Collider2D col){

        if(col.tag.Equals("Game Manager")) insideZone = false;

    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.tag.Equals("Game Manager")) insideZone = true;
        
        if(col.tag.Equals("Health") && health < 100) {

            updateHealth(-(Math.Min(100 - health, 50)));
            Destroy(col.gameObject);
            
        }

        if(col.tag.Equals("Ammo") && inventory[selectedWeapon] != null) {

            inventory[selectedWeapon].SendMessage("addAmmo", 10);
            updateAmmo();
            Destroy(col.gameObject);

        }

    }

    public GameObject getWeapon(){

        return weapon;

    }

    // Swap weapon between those in inventory
    public void changeWeapon(int selected){

        // fix weapon angle jitter on swap

        selectedWeapon = selected;

        if(weapon != null) weapon.gameObject.SetActive(false);
        if(inventory[selectedWeapon] != null) {

            weapon = inventory[selectedWeapon];
            weapon.gameObject.SetActive(true);
            updateAmmo();

        }
        else weapon = null;

    }

    // Updates health, called from bullet script
    public void updateHealth(int damage){

        health -= damage;
        if(health <= 0) transform.gameObject.SetActive(false);

        if(damage > 0) damageTimer = 0.5f;
        else healTimer = 0.5f;

    }

    void setIsEquipped(bool e){

        isEquipped = e;

    }

    // To stop gamemanager throwing not found exception
    void setZone(){}

    void updateAmmo(){

        inventory[selectedWeapon].SendMessage("getAmmo");

    }

    void setAmmo(string ammo){

        ammoText.text = ammo;

    }

}
