using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetInput : INetworkInput
{
    public float WeaponAngle;
    public bool PickupWeapon;
    public Vector2 Velocity;
}
