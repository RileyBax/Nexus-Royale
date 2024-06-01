using Fusion;
using Fusion.Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class User
{
    public String id;
    public String username;
    public String password;
    public int credits;
}
public class MenuManager : MonoBehaviour
{
    public User user;
    [SerializeField] private NetworkRunner _runnerPrefab;
    private NetworkRunner _runner;
    public int selectedPlayer = 1;
    public AudioManager am;

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

        Destroy(am.gameObject);

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

    public void SelectPlayer(int selectedPlayer)
    {
        this.selectedPlayer = selectedPlayer;
    }

    public Button[] buttons;
    public TMP_Text CurrentCreditsText;

    private Button currentlyEquippedButton;
    private bool[] isPurchased;

    async void Start()
    {

        am = Instantiate(Resources.Load("Prefabs/Audio Manager")).GetComponent<AudioManager>();
        am.PlayMusic("Menu");

        isPurchased = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            
            if (i < 2)
            {
                buttons[i].GetComponentInChildren<Text>().text = "Equip";
                int index = i; 
                buttons[i].onClick.AddListener(() => OnEquipButtonClicked(buttons[index]));
            }
            
            else
            {
                buttons[i].GetComponentInChildren<Text>().text = "100 credits";
                int index = i; 
                buttons[i].onClick.AddListener(() => OnPurchaseButtonClicked(buttons[index], index));
            }
        }

    }

    public void updateUser(User user)
    {
        this.user = user;
        Debug.Log(this.user);
        updateCreditsText(this.user.credits);
    }

    void updateCreditsText(int credits)
    {
        Debug.Log(credits); 
        CurrentCreditsText.text = $"{credits}";
    }

    void OnEquipButtonClicked(Button clickedButton)
    {
        
        if (currentlyEquippedButton != null)
        {
            currentlyEquippedButton.GetComponentInChildren<Text>().text = "Equip";
        }

        
        clickedButton.GetComponentInChildren<Text>().text = "Equipped";
        currentlyEquippedButton = clickedButton;
    }

    void OnPurchaseButtonClicked(Button clickedButton, int index)
    {
        
        if (!isPurchased[index])
        {
            isPurchased[index] = true;
            clickedButton.GetComponentInChildren<Text>().text = "Equip";
            clickedButton.onClick.RemoveAllListeners(); 
            clickedButton.onClick.AddListener(() => OnEquipButtonClicked(clickedButton)); 
        }
        else
        {
            
            OnEquipButtonClicked(clickedButton);
        }
    }

    public void Quit(){

        Application.Quit();

    }

}
