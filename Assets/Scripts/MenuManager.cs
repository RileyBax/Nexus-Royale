using Fusion;
using Fusion.Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private NetworkRunner _runnerPrefab;
    private NetworkRunner _runner;

    public void HostGame()
    {
        this.StartGame(GameMode.Host);
    }

    public void JoinGame()
    {
        this.StartGame(GameMode.Client);
    }


    public async void StartGame(GameMode mode)
    {
        var appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
        appSettings.FixedRegion = "au";
        appSettings.UseNameServer = true;

        var args = new StartGameArgs();

        args.GameMode = mode;
        args.SessionName = "Room 1";
        args.PlayerCount = 20;
        args.CustomPhotonAppSettings = appSettings;

        _runner = CreateRunner();
        var sceneManager = _runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        sceneManager.IsSceneTakeOverEnabled = false;

        var startGameResult = default(StartGameResult);
        startGameResult = await _runner.StartGame(args);

        if (startGameResult.Ok)
        {
            Debug.Log(_runner.LagCompensation);
        await _runner.LoadScene(SceneRef.FromIndex(1));
        }
    }

    private NetworkRunner CreateRunner()
    {
        return _runnerPrefab ? UnityEngine.Object.Instantiate(_runnerPrefab) : new GameObject("NetworkRunner", typeof(NetworkRunner)).GetComponent<NetworkRunner>();
    }
}
