using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotgunScript : MonoBehaviour
{
    [SerializeField] GameObject bullet;
    private float fireRate = 0.0f;
    private bool isEquipped = false;
    GameObject character;
    private int damage = 18;
    private int ammo = 30;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
        if(fireRate >= 0) fireRate -= Time.deltaTime;

    }

    void FireWeapon(){

        if(fireRate <= 0.0f){

            fireRate = 1.5f;

            Instantiate(bullet).SendMessage("init", new BulletInit(0, character, this.gameObject, damage));
            Instantiate(bullet).SendMessage("init", new BulletInit(-10, character, this.gameObject, damage));
            Instantiate(bullet).SendMessage("init", new BulletInit(10, character, this.gameObject, damage));

        }

    }

    void setEquipped(bool e){

        isEquipped = e;

    }

    void setCharacter(GameObject c){

        character = c;

    }

    void setCharacterNull(){

        character = null;

    }

    void getEquipped(GameObject bot){

        bot.SendMessage("setIsEquipped", isEquipped);

    }

}
