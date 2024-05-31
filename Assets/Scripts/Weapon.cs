using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class Weapon : NetworkBehaviour
{
    [SerializeField]
    protected GameObject BulletPrefab;
    [SerializeField]
    protected float FireRate = 1.0f;
    protected float FireCooldown = 0.0f;
    protected bool IsEquipped = false;
    protected GameObject Player;
    protected int Damage = 18;
    protected int Ammo = 30;
    [SerializeField] SpriteRenderer sr;
    [Networked] bool flip {get; set;}
    //[SerializeField] AudioManager am;
    [Networked] bool clientVisible {get; set;}

    void Start(){

        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        //am = GameObject.Find("Audio Manager(Clone)").GetComponent<AudioManager>();
        clientVisible = true;

    }

    public override void FixedUpdateNetwork()
    {
        if (FireCooldown >= 0) FireCooldown -= Runner.DeltaTime;

        if(Player != null){

            if(transform.position.x - Player.transform.position.x < 0 && !sr.flipY) flip = true;
            else if(transform.position.x - Player.transform.position.x > 0 && sr.flipY) flip = false;

        }

    }

    // visual update for clients
    void FixedUpdate(){

        sr.flipY = flip;
        if(!clientVisible && this.transform.GetChild(0).position.z >= -10) this.transform.GetChild(0).position -= new Vector3(0,0,20);
        else if(clientVisible && this.transform.GetChild(0).position.z <= 10) this.transform.GetChild(0).position += new Vector3(0,0,20);

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (Runner.IsServer && collision.tag == "Character" && collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent(out Player player))
        {
            player.AddNearbyWeapon(this.gameObject);
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (Runner.IsServer && collision.tag == "Character" && collision.attachedRigidbody != null && collision.attachedRigidbody.TryGetComponent(out Player player) && collision.gameObject.activeSelf)
        {
            player.RemoveNeabyWeapon(this.gameObject);
        }
    }

    public virtual void FireWeapon()
    {

        if (FireCooldown <= 0.0f)
        {
            FireCooldown = FireRate;

            NetworkObject bulletObject = Runner.Spawn(BulletPrefab, transform.position);
            BulletScript bullet = bulletObject.GetComponent<BulletScript>();
            if (bullet != null)
            {
                bullet.init(new BulletInit(0, Player, this.gameObject, Damage));
            }

        }
    }

    public void SetVisible(bool visible)
    {
        this.clientVisible = visible;
        this.gameObject.SetActive(visible);
        sr.enabled = visible;
    }

    public void setEquipped(bool e)
    {
        IsEquipped = e;
    }

    public void SetPlayer(GameObject p)
    {
        Player = p;
    }

    public void RemovePlayer()
    {
        Player = null;
    }

    void getEquipped(GameObject bot)
    {
        bot.SendMessage("setIsEquipped", IsEquipped);
    }

    public bool GetEquipped(){
        return IsEquipped;
    }

    public float GetFireRate(){

        return FireRate;

    }

}
