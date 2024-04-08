using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rb;
    public GameObject weapon;
    private Vector2 weaponPos;
    private Vector3 mousePos;
    private float angle;
    private bool hasWeapon;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        hasWeapon = false;
        weaponPos = new Vector2();

    }

    // Update is called once per frame
    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        angle = -(float) Math.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 1.55f;

        if(hasWeapon){

            weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
            weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

            weapon.transform.position = weaponPos;
            weapon.transform.up = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

            if(Input.GetMouseButtonDown(0)) weapon.SendMessage("FireWeapon");

        }

        

    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * 5);

    }

    void OnTriggerEnter2D(Collider2D col){

        if(col.tag == "Weapon"){

            weapon = col.gameObject;
            hasWeapon = true;

        }

    }

}
