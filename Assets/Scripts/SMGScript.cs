using Fusion;
using UnityEngine;

public class SMGScript : NetworkBehaviour
{
    [SerializeField] GameObject BulletPrefab;
    private float fireRate = 0.0f;
    private bool isEquipped = false;
    GameObject character;
    private int damage = 18;
    private int ammo = 30;

    public override void FixedUpdateNetwork()
    {
        if (fireRate >= 0) fireRate -= Runner.DeltaTime;
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
            fireRate = 0.2f;

            NetworkObject bulletObject = Runner.Spawn(BulletPrefab, transform.position);
            BulletScript bullet = bulletObject.GetComponent<BulletScript>();
            if (bullet != null)
            {
                bullet.init(new BulletInit(0, character, this.gameObject, damage));
            }

        }

    }

    void setEquipped(bool e){

        isEquipped = e;

    }

    void setCharacterNull(){

        character = null;

    }

    void setCharacter(GameObject c){

        character = c;

    }

    void getEquipped(GameObject bot){

        if(bot.activeSelf) bot.SendMessage("setIsEquipped", isEquipped);

    }

}
