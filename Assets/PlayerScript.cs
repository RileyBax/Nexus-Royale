using System;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private GameObject weapon;
    private Vector2 weaponPos;
    private Vector3 mousePos;
    private float angle;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        weapon = GameObject.Find("Weapon");
        weaponPos = new Vector2();

    }

    // Update is called once per frame
    void Update(){

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        angle = -(float) Math.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 1.55f;

        weaponPos.x = (float) (transform.position.x + Math.Sin(angle) * 1);
        weaponPos.y = (float) (transform.position.y + Math.Cos(angle) * 1);

        weapon.transform.position = weaponPos;
        weapon.transform.up = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

    }

    void FixedUpdate()
    {
        
        rb.MovePosition(rb.position + new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * Time.deltaTime * 5);

    }
}
