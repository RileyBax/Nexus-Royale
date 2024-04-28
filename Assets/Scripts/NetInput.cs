using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputButton
{
    Forward,
    Backward,
    Left,
    Right
}

public struct NetInput : INetworkInput
{
    public NetworkButtons Buttons;
    public Vector3 Position;
}
