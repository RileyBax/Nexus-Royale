using Fusion;
using UnityEngine;

public class SMGScript : Weapon
{

    public override void Spawned()
    {
        this.FireRate = 0.2f;
        this.Damage = 18;
        this.Ammo = 30;
    }

}
