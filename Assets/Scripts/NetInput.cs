using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct NetInput : INetworkInput
{
    public Vector2 MousePos;
    public int WeaponChange;
    public bool PickupWeapon;
    public Vector2 Velocity;
}
