using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private GameObject weaponPrefab;
    [Networked, Capacity(20)] private NetworkDictionary<PlayerRef, PlayerScript> Players => default;

    public override void Spawned()
    {
        Debug.Log("Spawned");
        if (HasStateAuthority) {
        Runner.Spawn(weaponPrefab, new Vector3(0, 0, 0));
        Runner.Spawn(weaponPrefab, new Vector3(5, 5, 0));
            Runner.Spawn(weaponPrefab, new Vector3(-5, -10, 0));
        }
    }

    public void PlayerJoined(PlayerRef player)
    {
        if (HasStateAuthority)
        {
            NetworkObject playerObject = Runner.Spawn(playerPrefab, new Vector3(0,0, 0), Quaternion.identity, player);
            Players.Add(player, playerObject.GetComponent<PlayerScript>());
        }
    }

    public void PlayerLeft(PlayerRef player) {
        if (!HasStateAuthority)
        {
            return;
        }

        if (Players.TryGet(player, out PlayerScript playerScript)) {
            Players.Remove(player);
            Runner.Despawn(playerScript.Object);
        }
    }
}
