using Fusion;
using UnityEngine;

public class RifleScript : Weapon
{

    public override void Spawned()
    {
        this.Damage = 50;
        this.Ammo = 10;
    }

}
