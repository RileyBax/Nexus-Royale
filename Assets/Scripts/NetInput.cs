using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetInput : INetworkInput
{
    public float weaponAngle;
    public bool pickupWeapon;
    public Vector2 Velocity;
}
