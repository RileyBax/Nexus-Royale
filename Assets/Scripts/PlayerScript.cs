using System;
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
    [SerializeField] GameObject hud;
    private int selectedWeapon = 0;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        weaponPos = new Vector2();

        for(int i = 0; i < inventoryUI.Length; i++) inventoryUI[i] = hud.transform.GetChild(0).GetChild(i).GetComponent<UnityEngine.UI.Image>();

    }

    // Update is called once per frame
    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        angle = -(float) Math.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 1.55f;

        if(weapon != null){

            weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
            weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

            weapon.transform.position = weaponPos;
            weapon.transform.up = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

            if(Input.GetMouseButton(0)) weapon.SendMessage("FireWeapon");

        }

        if(Input.anyKeyDown){

            switch(Input.inputString){

                case "1":
                    changeWeapon(0);
                    break;
                case "2":
                    changeWeapon(1);
                    break;
                case "3":
                    changeWeapon(2);
                    break;

            }

        }

        for(int i = 0; i < inventoryUI.Length; i++){
            
            if(i != selectedWeapon) inventoryUI[i].color = Color.white;
            else inventoryUI[i].color = Color.blue;

        }

    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * 5);

    }
    
    void OnTriggerStay2D(Collider2D col){

        if(col.tag == "Weapon" && Input.GetKey(KeyCode.E)){

            weapon = col.gameObject;

            inventoryUI[selectedWeapon].sprite = Resources.Load<Sprite>("Rifle_Texture");
            inventory[selectedWeapon] = col.gameObject;

        }

    }

    public GameObject getWeapon(){

        return weapon;

    }

    public void changeWeapon(int selected){

        selectedWeapon = selected;

        if(weapon != null) weapon.gameObject.SetActive(false);
        if(inventory[selectedWeapon] != null) weapon.gameObject.SetActive(true);

    }

}
