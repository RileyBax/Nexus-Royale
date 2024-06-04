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
    public bool[] unlockedSkins;
}
public class MenuManager : MonoBehaviour
{
    public User user;
    [SerializeField] private NetworkRunner _runnerPrefab;
    private NetworkRunner _runner;
    public int selectedPlayer = 1;
    public AudioManager am;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private TMP_InputField UsernameField;
    public TMP_Text ErrorMessage;

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
        PlayerInfo.ID = user.id;
        PlayerInfo.Username = user.username;
        PlayerInfo.Skin = selectedPlayer;
        PlayerInfo.MusicVolume = am.mVolume;
        PlayerInfo.SoundVolume = am.sVolume;

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

    void Start()
    {

        am = Instantiate(Resources.Load("Prefabs/Audio Manager")).GetComponent<AudioManager>();
        am.PlayMusic("Menu");

        isPurchased = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            if (user.unlockedSkins[i])
            {
                buttons[i].GetComponentInChildren<TMP_Text>().text = "Equip";
                buttons[i].onClick.AddListener(() => OnEquipButtonClicked(buttons[index], index));
            }

            else
            {
                buttons[i].GetComponentInChildren<TMP_Text>().text = "100 credits";
                buttons[i].onClick.AddListener(() => OnPurchaseButtonClicked(buttons[index], index));
            }

        }
    }

    public void updateUser(User user)
    {
        this.user = user;
        this.UsernameField.text = user.username;
        updateCreditsText(this.user.credits);
    }

    void updateCreditsText(int credits)
    {
        Debug.Log(credits); 
        CurrentCreditsText.text = $"{credits}";
    }

    void OnEquipButtonClicked(Button clickedButton, int index)
    {
        
        if (currentlyEquippedButton != null)
        {
            ErrorMessage.text = "";
            currentlyEquippedButton.GetComponentInChildren<TMP_Text>().text = "Equip";
        }

        ErrorMessage.text = "";
        clickedButton.GetComponentInChildren<TMP_Text>().text = "Equipped";
        currentlyEquippedButton = clickedButton;
        SelectPlayer(index + 1);
    }

    void OnPurchaseButtonClicked(Button clickedButton, int index)
    {
        
        if (!isPurchased[index])
        {
            if (user.credits < 100)
            {
                ErrorMessage.text = "Not enough credits!";
            }
            else
            {
                ErrorMessage.text = "";

                isPurchased[index] = true;
                clickedButton.GetComponentInChildren<TMP_Text>().text = "Equip";
                user.credits -= 100;
                updateCreditsText(this.user.credits);
                UpdateDB(index);
                OnEquipButtonClicked(buttons[index], index);
                clickedButton.onClick.RemoveAllListeners();
                clickedButton.onClick.AddListener(() => OnEquipButtonClicked(clickedButton, index));
            }
        }
        else
        {
            
            OnEquipButtonClicked(clickedButton, index);
        }
    }

    public async void OnUpdateUsernameButtonClicked()
    {
        string username = this.UsernameField.text;

        if (string.IsNullOrEmpty(username))
        {
            this.UsernameField.text = PlayerInfo.Username;
            return;
        }

        username = username.ToUpper();

        string query = $"UPDATE {PlayerInfo.ID} SET username = \"{username}\" RETURN id, username;";
        User[] users = await Surreal.Query<User>(query);
        if (users.Length > 0)
        {
            PlayerInfo.Username = users[0].username;
        }
    }

    public void Quit(){

        Application.Quit();

    }

    void Update(){

        am.SFXVolume(SFXSlider.value);
        am.MusicVolume(musicSlider.value);

    }

    public async void UpdateDB(int index)
    {
        String send = $"UPDATE user SET credits = {user.credits}, unlockedSkins[{index}] = true WHERE username = \"{user.username}\"; ";
        User[] users = await Surreal.Query<User>(send);

    }

}
