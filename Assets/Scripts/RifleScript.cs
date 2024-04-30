using Fusion;
using System;
using UnityEngine;

public class RifleScript : NetworkBehaviour
{

    [SerializeField] GameObject bullet;
    private float fireRate = 0.0f;
    private bool isEquipped = false;
    GameObject character;

    // Update is called once per frame
    void Update()
    {
        
       // if(fireRate >= 0) fireRate -= Time.deltaTime;

    }

    public override void FixedUpdateNetwork()
    {
        if (character != null && isEquipped)
        {
            Vector2 weaponPos = Vector2.zero;

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Angle to mouse position
            float angle = -(float)Math.Atan2(mousePos.y - transform.position.y, mousePos.x - transform.position.x) + 1.55f;

            // Rotates weapon towards mouse
                weaponPos.x = (float)(character.transform.position.x + Math.Sin(angle) * 1);
                weaponPos.y = (float)(character.transform.position.y + Math.Cos(angle) * 1);

                this.transform.position = weaponPos;
                this.transform.up = new Vector2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);

            if (Input.GetMouseButton(0)) this.FireWeapon();

            transform.position = character.transform.position;
        }
    }

    void FireWeapon(){

        if(fireRate <= 0.0f){

            fireRate = 1.0f;

            Instantiate(bullet).SendMessage("init", new BulletInit(0, character, this.gameObject));

        }

    }

    void setEquipped(bool e){

        isEquipped = e;

    }

    void setCharacter(GameObject c){

        character = c;
        //Runner.Despawn(this.Object);

    }

    void setCharacterNull(){

        character = null;

    }

    void getEquipped(GameObject bot){

        if(bot.activeSelf) bot.SendMessage("setIsEquipped", isEquipped);

    }

}
