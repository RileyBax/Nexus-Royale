using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletScript : MonoBehaviour
{

    private Rigidbody2D rb;
    private GameObject player;
    private GameObject weapon;
    private float timer;

    // Start is called before the first frame update
    void Start(){

        player = GameObject.Find("Player");
        rb = GetComponent<Rigidbody2D>();
        weapon = player.GetComponent<PlayerScript>().getWeapon();
        timer = 5.0f;

        transform.position = weapon.transform.position + (weapon.transform.position - player.transform.position);
        transform.up = weapon.transform.up;

    }

    // Update is called once per frame
    void Update()
    {
        
        rb.transform.Translate(0, 10 * Time.deltaTime, 0);
        timer -= Time.deltaTime;

        if(timer <= 0) Destroy(transform.root.gameObject);

    }


}
