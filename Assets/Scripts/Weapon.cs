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

    public override void FixedUpdateNetwork()
    {
        if (FireCooldown >= 0) FireCooldown -= Runner.DeltaTime;
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

    void setEquipped(bool e)
    {
        IsEquipped = e;
    }

    void setCharacter(GameObject c)
    {
        Player = c;
    }

    void setCharacterNull()
    {
        Player = null;
    }

    void getEquipped(GameObject bot)
    {
        bot.SendMessage("setIsEquipped", IsEquipped);
    }
}
