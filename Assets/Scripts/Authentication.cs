using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


public class Authentication : MonoBehaviour
{
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

    public IEnumerator Login(string username, string password) {
        string json = JsonUtility.ToJson(new AuthParams(username, password));
        var request = new UnityWebRequest("https://nexus.ryanfolio.live/signin", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // Just assuming that it is failing because of bad credentials...
            LoginErrorText.text = "Credentials Incorrect!";
            yield break;
        }

        LoginPage.SetActive(false);
        MenuPage.SetActive(true);
    }

    public void LoginHandler()
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

        username = LoginUsernameField.text;
        password = LoginPasswordField.text;


        StartCoroutine(Login(username, password));
    }

    public IEnumerator Register(string username, string password)
    {
        string json = JsonUtility.ToJson(new AuthParams(username, password));
        var request = new UnityWebRequest("https://nexus.ryanfolio.live/signup", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            // Just assuming that it is failing because of bad credentials...
            RegisterErrorText.text = "Something went wrong.";
            yield break;
        }

        RegisterPage.SetActive(false);
        LoginPage.SetActive(true);
    }

    public void RegisterHandler()
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

        StartCoroutine(Register(username, password));
    }


}
