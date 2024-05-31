using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerManager
{
    private static List<PlayerRef> _tempSpawnPlayers = new List<PlayerRef>();
    private static List<Player> _tempSpawnedPlayers = new List<Player>();

    public static void UpdatePlayerConnections(NetworkRunner runner, Action<PlayerRef> spawnPlayer, Action<PlayerRef, Player> despawnPlayer)
    {
        _tempSpawnPlayers.Clear();
        _tempSpawnedPlayers.Clear();

        // 1. Get all connected players, marking them as pending spawn.
        _tempSpawnPlayers.AddRange(runner.ActivePlayers);

        // 2. Get all player objects with component of type T.
        runner.GetAllBehaviours(_tempSpawnedPlayers);

        for (int i = 0; i < _tempSpawnedPlayers.Count; ++i)
        {
            Player player = _tempSpawnedPlayers[i];
            PlayerRef playerRef = player.Object.InputAuthority;

            // 3. Remove PlayerRef of existing player object from pending spawn list.
            _tempSpawnPlayers.Remove(playerRef);

            // 4. If a player is not valid (disconnected) execute the despawn callback.
            if (runner.IsPlayerValid(playerRef) == false)
            {
                try
                {
                    despawnPlayer(playerRef, player);
                }
                catch (Exception exception)
                {
                    Debug.LogException(exception);
                }
            }
        }

        // 5. Execute spawn callback for all players pending spawn (recently connected).
        for (int i = 0; i < _tempSpawnPlayers.Count; ++i)
        {
            try
            {
                spawnPlayer(_tempSpawnPlayers[i]);
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
        }

        // 6. Cleanup
        _tempSpawnPlayers.Clear();
        _tempSpawnedPlayers.Clear();
    }
}
