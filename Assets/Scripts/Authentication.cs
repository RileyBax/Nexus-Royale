using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;



public class Authentication : MonoBehaviour
{
    public MenuManager MenuManager;
    public GameObject MenuPage;
    public GameObject OpeningPage;

    // Login Page
    public GameObject LoginPage;
    public TMP_InputField LoginUsernameField;
    public TMP_InputField LoginPasswordField;
    public TMP_Text LoginErrorText;

    // Register Page
    public GameObject RegisterPage;
    public TMP_InputField RegisterUsernameField;
    public TMP_InputField RegisterPasswordField;
    public TMP_InputField RegisterConfirmField;
    public TMP_Text RegisterErrorText;

    public class AuthParams
    {

        
        public string username, password, ns, db, sc;

        public AuthParams(string username, string password)
        {
            this.username = username.ToUpper();
            this.password = password;
            this.ns = "nexus";
            this.db = "nexus";
            this.sc = "user";
        }
    }

    public async Task Login(string username, string password) {
        string json = JsonUtility.ToJson(new AuthParams(username, password));
        var request = new UnityWebRequest("https://nexus.ryanfolio.live/signin", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        try
        {
            await request.SendWebRequestAsync();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    // Just assuming that it is failing because of bad credentials...
                    LoginErrorText.text = "Credentials Incorrect!";
                    return;
                }
          
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
        String q = $"SELECT * OMIT password FROM user WHERE username = \"{username}\"";
        User[] users = await Surreal.Query<User>(q);
        if(users.Length == 0)
        {
            LoginErrorText.text = "User Not Found!";
            return;
        }
        Debug.Log(users[0].unlockedSkins[0]);
        PlayerInfo.ID = users[0].id;
        PlayerInfo.Username = users[0].username;
        MenuManager.updateUser(users[0]);
        LoginPage.SetActive(false);
        MenuPage.SetActive(true);
    }

    public async void LoginHandler()
    {
        string username = "", password = "";

        if (LoginUsernameField.text == "")
        {
            LoginErrorText.text = "Username is required!";
            return;
        }
        if (LoginPasswordField.text == "")
        {
            LoginErrorText.text = "Password is required!";
            return;
        }

        username = LoginUsernameField.text.ToUpper();
        password = LoginPasswordField.text;


        await Login(username, password);
    }

    public async Task Register(string username, string password)
    {
        string json = JsonUtility.ToJson(new AuthParams(username, password));
        var request = new UnityWebRequest("https://nexus.ryanfolio.live/signup", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        try
        {
            await request.SendWebRequestAsync();

            if (request.result != UnityWebRequest.Result.Success)
            {
                // Just assuming that it is failing because of bad credentials...
                LoginErrorText.text = "Credentials Incorrect!";
                return;
            }

            RegisterPage.SetActive(false);
            LoginPage.SetActive(true);

        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }

        
    }

    public async void RegisterHandler()
    {
        string username = "", password = "", confirmPassword = "";

        if (RegisterUsernameField.text == "")
        {
            RegisterErrorText.text = "Username is required!";
            return;
        }
        if (RegisterPasswordField.text == "")
        {
            RegisterErrorText.text = "Password is required!";
            return;
        }
        if (RegisterConfirmField.text == "")
        {
            RegisterErrorText.text = "Please confirm your password.";
            return;
        }

        username = RegisterUsernameField.text;
        password = RegisterPasswordField.text;
        confirmPassword = RegisterConfirmField.text;

        if (!password.Equals(confirmPassword))
        {
            RegisterErrorText.text = "Passwords do not match!";
            return;
        }

        await Register(username, password);
    }


}
