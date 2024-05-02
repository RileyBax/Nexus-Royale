using Fusion;
using Fusion.Addons.Physics;
using UnityEngine;

public class BulletScript : NetworkBehaviour
{

    private NetworkRigidbody2D rb;
    private GameObject character;
    private GameObject weapon;
    private float timer;
    private int angle = 0;
    private int damage;

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        // Moves on local Y axis until destroyed
        rb.transform.Translate(0, 10 * Runner.DeltaTime, 0);
        timer -= Runner.DeltaTime;

        if (timer <= 0) Destroy(transform.root.gameObject);

    }

    // Initializes bullet, called from weapon script
    public void init(BulletInit bInit)
    {

        angle = bInit.angle;
        character = bInit.character;
        weapon = bInit.weapon;
        damage = bInit.damage;

        Debug.Log(angle);
        rb = GetComponent<NetworkRigidbody2D>();
        timer = 3.0f;

        transform.position = weapon.transform.position + (weapon.transform.position - character.transform.position);

        transform.up = weapon.transform.up;

        transform.Rotate(0, 0, angle);

    }

    void OnTriggerStay2D(Collider2D col)
    {

        // causing error when bullet hits but character is not active
        if (isCharacter(col) && col.gameObject.activeSelf)
        {
            col.SendMessage("updateHealth", damage);
            Runner.Despawn(this.GetComponent<NetworkObject>());
        }
        else if (col.tag == "Object") Destroy(transform.root.gameObject);

    }

    bool isCharacter(Collider2D col)
    {

        if (col.tag.Equals("Character") || col.tag.Equals("Player")) return true;

        return false;

    }

}