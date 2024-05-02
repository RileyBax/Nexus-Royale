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
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Runner.IsServer && collision.tag == "Character" && collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent(out PlayerScript player))
        {
            player.AddNearbyWeapon(this.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Runner.IsServer && collision.tag == "Character" && collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent(out PlayerScript player))
        {
            player.RemoveNeabyWeapon(this.gameObject);
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
        if (this.gameObject.TryGetComponent(out BoxCollider2D collider)) {
            collider.enabled = false;
        }

    }

    void setCharacterNull(){

        character = null;
        if (this.gameObject.TryGetComponent(out BoxCollider2D collider))
        {
            collider.enabled = true;
        }
    }

    void getEquipped(GameObject bot){

        if(bot.activeSelf) bot.SendMessage("setIsEquipped", isEquipped);

    }

}
