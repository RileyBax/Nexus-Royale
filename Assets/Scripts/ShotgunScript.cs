using Fusion;

public class ShotgunScript : Weapon
{
    public override void Spawned()
    {
        this.FireRate = 1.5f;
        this.Damage = 18;
        this.Ammo = 12;
    }

    public override void FireWeapon(){

        if(FireCooldown <= 0.0f){
            FireCooldown = FireRate;

            NetworkObject bulletObject = Runner.Spawn(BulletPrefab, transform.position);
            BulletScript bullet = bulletObject.GetComponent<BulletScript>();
            if (bullet != null)
            {
                bullet.init(new BulletInit(-10, this.Player, this.gameObject, this.Damage));
            }
            bulletObject = Runner.Spawn(BulletPrefab, transform.position);
            bullet = bulletObject.GetComponent<BulletScript>();
            if (bullet != null)
            {
                bullet.init(new BulletInit(0, this.Player, this.gameObject, this.Damage));
            }
            bulletObject = Runner.Spawn(BulletPrefab, transform.position);
            bullet = bulletObject.GetComponent<BulletScript>();
            if (bullet != null)
            {
                bullet.init(new BulletInit(10, this.Player, this.gameObject, this.Damage));
            }

        }

    }

}
