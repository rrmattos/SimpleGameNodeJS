using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class APIClient : MonoBehaviour
{
    public static APIClient Instance;

    private string token = "";
    private string tokenPass = "";
    private string sessionCookie;

    private APIClient() { } // Make the class constructor private

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region --- HTTP USER ---
        
    #region --- Functions ---
    public void CallChangePassword(string _newPassword)
    {
        StartCoroutine(ChangePassword(_newPassword));
    }
    #endregion
    
    #region --- Coroutines ---
    public async Task<long> RegisterUser(string _username, string _password, string _email)
    {
        string url = "http://localhost:3000/api/auth/register";
        User newUser = new User { username = _username, password = _password, email = _email };

        string json = JsonUtility.ToJson(newUser);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            FeedbackController.Instance.ShowMessage("User registered successfully", new Color32(32, 203, 35, 255));
            return 200;
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
            return request.responseCode;
        }
    }
    
    public async Task<long> LoginUser(string _username, string _password)
    {
        string url = "http://localhost:3000/api/auth/login";
        LoginData loginData = new LoginData { username = _username, password = _password };
        Debug.Log($"login: {_username} - senha: {_password}");
        string json = JsonUtility.ToJson(loginData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            // Parse the response to get the token
            TokenResponse tokenResponse = JsonUtility.FromJson<TokenResponse>(responseText);
            token = tokenResponse.token;
            GlobalChecker.CurrentToken = token;
            FeedbackController.Instance.ShowMessage("Login successful!", new Color32(32, 203, 35, 255));
            return 200;
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
            return request.responseCode;
        }
    }
    
    public async Task<long> RecoverPass(string _username, string _email)
    {
        Debug.Log(_username + " - " + _email);
        string url = "http://localhost:3000/api/forgot-password";
        RecoverData recoverData = new RecoverData { username = _username, email = _email };

        string json = JsonUtility.ToJson(recoverData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();
        
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string setCookieHeader = request.GetResponseHeader("Set-Cookie");
            if (!string.IsNullOrEmpty(setCookieHeader))
            {
                sessionCookie = setCookieHeader.Split(';')[0];
            }

            FeedbackController.Instance.ShowMessage("A verification code has been sent to your email. Please also check the spam folder!", new Color32(32, 203, 35, 255));
            return 200;
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
            return request.responseCode;
        }
    }
    
    public async Task<long> VerifyResetToken(string _token)
    {
        string url = "http://localhost:3000/api/verify-reset-token";
        TokenResponse tokenData = new TokenResponse { token = _token };

        string json = JsonUtility.ToJson(tokenData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        var operation = request.SendWebRequest();
        
        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Token válido.");
            tokenPass = _token;
            return 200;
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
            return request.responseCode;
        }
    }
    
    IEnumerator ChangePassword(string _newPassword)
    {
        string url = "http://localhost:3000/api/change-password";
        PasswordData passwordData = new PasswordData { newPassword = _newPassword };

        string json = JsonUtility.ToJson(passwordData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        
        //request.SetRequestHeader("Cookie", sessionCookie);
        request.SetRequestHeader("Authorization", "Bearer " + tokenPass);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            FeedbackController.Instance.ShowMessage("Password changed succefully!", new Color32(32, 203, 35, 255));
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
        }
    }
    #endregion
    
    #endregion
    
    #region --- HTTP SCORE ---
    
    #region --- Functions ---
    public void CallSaveScore(string _scoreText)
    {
        if (string.IsNullOrEmpty(token))
        {
            FeedbackController.Instance.ShowMessage("Error: User is not logged in!", new Color32(173, 38, 39, 255));
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            FeedbackController.Instance.ShowMessage("Error: No internet connection!", new Color32(173, 38, 39, 255));
            return;
        }

        StartCoroutine(SaveScore(_scoreText));
    }
    #endregion
        
    #region --- Coroutines ---
    IEnumerator SaveScore(string _scoreText)
    {
        string url = "http://localhost:3000/api/scores/";
        ScoreData scoreData = new ScoreData { score = int.Parse(_scoreText) };

        string json = JsonUtility.ToJson(scoreData);
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            FeedbackController.Instance.ShowMessage("Score saved successfully", new Color32(32, 203, 35, 255));
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
        }
    }
    
    public async Task<List<UserScore>> GetUserScores()
    {
        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            FeedbackController.Instance.ShowMessage("Error: No internet connection!", new Color32(173, 38, 39, 255));
            return null;
        }
        
        string url = "http://localhost:3000/api/scores/";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Content-Type", "application/json");

        var operation = request.SendWebRequest();

        while (!operation.isDone)
        {
            await Task.Yield();
        }

        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            UserScoresResponse userScoresResponse = JsonUtility.FromJson<UserScoresResponse>(responseText);
            return userScoresResponse.userScores;
        }
        else
        {
            FeedbackController.Instance.ShowMessage("Error: " + request.error, new Color32(173, 38, 39, 255));
            return null;
        }
    }
    // IEnumerator GetUserScores(Action<List<UserScore>> _callback)
    // {
    //     string url = "http://localhost:3000/api/scores/";
    //
    //     UnityWebRequest request = new UnityWebRequest(url, "GET");
    //     request.downloadHandler = new DownloadHandlerBuffer();
    //     request.SetRequestHeader("Content-Type", "application/json");
    //
    //     yield return request.SendWebRequest();
    //
    //     if (request.result == UnityWebRequest.Result.Success)
    //     {
    //         string responseText = request.downloadHandler.text;
    //         // Parse the response to get the list of users and scores
    //         UserScoresResponse userScoresResponse = JsonUtility.FromJson<UserScoresResponse>(responseText);
    //         _callback(userScoresResponse.userScores);
    //     }
    //     else
    //     {
    //         Debug.Log("Error: " + request.error);
    //         _callback(null);
    //     }
    // }
    #endregion
        
    #endregion
}