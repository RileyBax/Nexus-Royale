using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private GameObject character;
    private GameObject weapon;
    private float timer;
    private int angle = 0;
    private int damage = 30;

    // Start is called before the first frame update
    void Start(){

        rb = GetComponent<Rigidbody2D>();
        timer = 5.0f;

        transform.position = weapon.transform.position + (weapon.transform.position - character.transform.position);
        
        transform.up = weapon.transform.up;

        transform.Rotate(0, 0, angle);

    }

    // Update is called once per frame
    void Update()
    {
        
        rb.transform.Translate(0, 10 * Time.deltaTime, 0);
        timer -= Time.deltaTime;

        if(timer <= 0) Destroy(transform.root.gameObject);

    }

    void init(BulletInit bInit){

        angle = bInit.angle;
        character = bInit.character;
        weapon = bInit.weapon;

    }

    void OnTriggerEnter2D(Collider2D col){

        // causing error when bullet hits but character is not active
        if(col.tag == "Character" && col.gameObject.activeSelf) {
            col.SendMessage("updateHealth", damage);
        }

    }

}
