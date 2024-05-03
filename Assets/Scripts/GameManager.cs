using Fusion;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private NetworkObject RiflePrefab;
    [SerializeField] private NetworkObject ShotgunPrefab;
    [SerializeField] private NetworkObject SMGPrefab;
    [SerializeField] private Tilemap Tilemap;

    [Networked, Capacity(20)] private NetworkDictionary<PlayerRef, PlayerScript> Players => default;

    public override void Spawned()
    {
        Debug.Log("Spawned");
        if (Runner.IsServer) {
            SpawnWeapons();
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

    private void SpawnWeapons()
    {
        Tilemap.CompressBounds();
        Vector3 tilemapSize = Tilemap.size;
        int x1 = -(int)(tilemapSize.x / 2);
        int x2 = (int)(tilemapSize.x / 2);
        int y1 = -(int)(tilemapSize.y / 2);
        int y2 = (int)(tilemapSize.y / 2);

        int maxRifles = 5;
        int maxShotguns = 20;
        int maxSMGs = 10;
        int spawned = 0;

        while (spawned < maxRifles)
        {
            var position = new Vector3(Random.Range(x1, x2), Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1,1,1)))
            {
                Runner.Spawn(RiflePrefab, position);
                spawned++;
            }
        }

        spawned = 0;

        while (spawned < maxShotguns)
        {
            var position = new Vector3(Random.Range(x1, x2), Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1, 1, 1)))
            {
                Runner.Spawn(ShotgunPrefab, position);
                spawned++;
            }
        }

        spawned = 0;

        while (spawned < maxSMGs)
        {
            var position = new Vector3(Random.Range(x1, x2), Random.Range(y1, y2));

            if (!Physics.CheckBox(position, new Vector3(1, 1, 1)))
            {
                Runner.Spawn(SMGPrefab, position);
                spawned++;
            }
        }

    }
}
