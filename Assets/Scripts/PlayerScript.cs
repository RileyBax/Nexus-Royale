using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private GameObject weapon;
    private Vector2 weaponPos;
    private Vector3 mousePos;
    private float angle;
    private UnityEngine.UI.Image[] inventoryUI = new UnityEngine.UI.Image[3];
    private GameObject[] inventory = new GameObject[3];
    [SerializeField] private GameObject hud;
    private int selectedWeapon = 0;
    private int health = 100;
    private bool isEquipped;
    private float damageTimer;
    private Color baseColor = new Color(0.7f, 0.8f, 0.5f, 255);
    private SpriteRenderer sr;
    public float zoneDamageTimer = 2.0f;
    public bool insideZone = true;
    private float healTimer;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        weaponPos = new Vector2();
        hud = GameObject.Find("HUD");
        sr = GetComponent<SpriteRenderer>();

        for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = hud.transform.GetChild(0).GetChild(i).GetComponent<UnityEngine.UI.Image>();

        GameObject.Find("Main Camera").SendMessage("setPlayer", this.gameObject);

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

            if(Input.GetMouseButton(0)) weapon.SendMessage("FireWeapon");

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

    void FixedUpdate()
    {
        
        // Moves player in input direction

        rb.MovePosition(rb.position + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * 5);

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

}
