using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SimulationBehaviour, IBeforeUpdate, INetworkRunnerCallbacks
{
    private NetInput accumulatedInput;
    private bool resetInput;

    public void BeforeUpdate()
    {
        // Reset the input
        if (resetInput)
        {
            resetInput = false;
            accumulatedInput = default;
        }

        // Player movement
        Vector2 movement = Vector2.zero;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        accumulatedInput.Velocity += movement * 5;

        // If they are trying to interact with something
        if (Input.GetKey(KeyCode.E))
        {
            accumulatedInput.PickupWeapon = true;
        }

        // If they are trying to change weapon slots
        accumulatedInput.WeaponChange = 0;

        if (Input.GetKey(KeyCode.Alpha1))
        {
            accumulatedInput.WeaponChange = 1;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            accumulatedInput.WeaponChange = 2;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            accumulatedInput.WeaponChange = 3;
        }

        // Get the moust position for aiming
        accumulatedInput.MousePos = Input.mousePosition;

        if (CameraFollower.Singleton != null)
        {
        Camera camera = CameraFollower.Singleton.GetCamera();

        accumulatedInput.MousePos = camera.ScreenToWorldPoint(Input.mousePosition);
        }

        // Fire Weapon
        accumulatedInput.FireWeapon = false;

        if (Input.GetMouseButton(0))
        {
            accumulatedInput.FireWeapon = true;
        }

    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        accumulatedInput.Velocity.Normalize();
        input.Set(accumulatedInput);
        resetInput = true;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
    }

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public async void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        
    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
}
