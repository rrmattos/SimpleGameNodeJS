using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class MenuController : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputUsername;
    [SerializeField] private TMP_InputField inputPassword;
    [SerializeField] private TMP_InputField inputEmail;
    [SerializeField] private TMP_InputField inputTokenPass;
    [SerializeField] private TMP_InputField inputNewPass;
    [SerializeField] private TMP_InputField inputConfirmPass;
    [SerializeField] private List<CanvasGroup> canvasGroups = new();
    [SerializeField] private GameObject userScorePrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private Transform menuWindow;
    //private Tilemap menuTilemap;
    private APIClient api;
    private List<UserScore> userScores = new();

    private void Awake()
    {
        //menuTilemap = menuWindow.GetComponent<Tilemap>();
        api = FindObjectOfType<APIClient>();

        if (GlobalChecker.CurrentToken == "")
        {
            menuWindow.gameObject.SetActive(true);
    
            canvasGroups[0].alpha = 1;
            canvasGroups[0].interactable = true;
            canvasGroups[0].blocksRaycasts = true;
        }
    }

    private void OnDisable()
    {
        CloseWindow();
    }

    #region --- API Calls ---
    public async void Register()
    {
        if (string.IsNullOrEmpty(inputUsername.text) || string.IsNullOrEmpty(inputPassword.text) ||
            string.IsNullOrEmpty(inputEmail.text))
        {
            FeedbackController.Instance.ShowMessage("Please fill in all fields!", new Color32(173, 38, 39, 255));
            return;
        }
        
        long responseCode = await api.RegisterUser(inputUsername.text, inputPassword.text, inputEmail.text);
        if (responseCode == 503 || responseCode == 404 || responseCode == 0)
        {
            FeedbackController.Instance.ResetMessage();
            FeedbackController.Instance.ShowMessage("It was not possible to communicate with the api or database, starting game in test mode...", new Color32(243, 175, 15, 255));
            GlobalChecker.CurrentToken = "teste";
            CloseWindow();
        }
    }

    public async void Login()
    {
        if (string.IsNullOrEmpty(inputUsername.text) || string.IsNullOrEmpty(inputPassword.text))
        {
            FeedbackController.Instance.ShowMessage("Username or password field is empty", new Color32(173, 38, 39, 255));
            return;
        }
        
        long responseCode = await api.LoginUser(inputUsername.text, inputPassword.text);
        if(responseCode == 200) CloseWindow();
        if (responseCode == 503 || responseCode == 404 || responseCode == 0)
        {
            FeedbackController.Instance.ResetMessage();
            FeedbackController.Instance.ShowMessage("It was not possible to communicate with the api or database, starting game in test mode...", new Color32(243, 175, 15, 255));
            GlobalChecker.CurrentToken = "teste";
            CloseWindow();
        }
    }
    
    public async void ForgotPassword()
    {
        if (string.IsNullOrEmpty(inputUsername.text) || string.IsNullOrEmpty(inputEmail.text))
        {
            FeedbackController.Instance.ShowMessage("Username or email field is empty!", new Color32(173, 38, 39, 255));
            return;
        }
        
        long responseCode = await api.RecoverPass(inputUsername.text, inputEmail.text);
        if(responseCode == 200) ChangeCanvasGroup(1);
        if (responseCode == 503 || responseCode == 404 || responseCode == 0)
        {
            FeedbackController.Instance.ResetMessage();
            FeedbackController.Instance.ShowMessage("It was not possible to communicate with the api or database, starting game in test mode...", new Color32(243, 175, 15, 255));
            GlobalChecker.CurrentToken = "teste";
            CloseWindow();
        }
    }
    
    public async void VerifyPasswordToken()
    {
        if (string.IsNullOrEmpty(inputTokenPass.text))
        {
            FeedbackController.Instance.ShowMessage("Please insert your code!", new Color32(173, 38, 39, 255));
            return;
        }
        
        long responseCode = await api.VerifyResetToken(inputTokenPass.text);
        if(responseCode == 200) ChangeCanvasGroup(2);
    }
    
    public void ChangePassword()
    {
        if (inputNewPass.text != inputConfirmPass.text)
        {
            FeedbackController.Instance.ShowMessage("The passwords must be the same!", new Color32(173, 38, 39, 255));
            return;
        }
        
        if (string.IsNullOrEmpty(inputNewPass.text) && string.IsNullOrEmpty(inputConfirmPass.text))
        {
            FeedbackController.Instance.ShowMessage("The password fields cannot be empty!", new Color32(173, 38, 39, 255));
            return;
        }
        
        api.CallChangePassword(inputNewPass.text);
        ChangeCanvasGroup(0);
    }

    public void ListScore()
    {
        ChangeCanvasGroup(5);
        PopulateScrollView();
    }
    #endregion
    
    #region --- Menu Functions ---
    public void ChangeCanvasGroup(int _toIndex)
    {
        CanvasGroup group = null;
        foreach (CanvasGroup currentGroup in canvasGroups)
        {
            if (currentGroup.alpha > 0)
            {
                group = currentGroup;
            }
        }
        
        try { group.alpha = 0; } catch
        {
            Debug.Log("All canvas groups are invisible!");
            return;
        }
        group.interactable = false;
        group.blocksRaycasts = false;
        
        group = canvasGroups[_toIndex];
        group.alpha = 1;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    public void GoToExternalLink(string _url)
    {
        Application.OpenURL(_url);
    }
    
    private async void PopulateScrollView()
    {
        foreach (Transform child in contentTransform)
        {
            Destroy(child.gameObject);
        }
        
        userScores = await APIClient.Instance.GetUserScores();

        // Verificar se a lista foi populada corretamente
        if (userScores == null || userScores.Count == 0)
        {
            Debug.LogWarning("Nenhum score encontrado.");
            return;
        }

        // Ordenar a lista por pontuação decrescente
        userScores = userScores.OrderByDescending(_u => _u.score).ToList();

        float prefabHeight = 83;
        float totalHeight = userScores.Count * prefabHeight;
        
        RectTransform contentRectTransform = contentTransform.GetComponent<RectTransform>();
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, totalHeight);
        
        for (int i = 0; i < userScores.Count; i++)
        {
            GameObject newPrefab = Instantiate(userScorePrefab, contentTransform);
            
            TextMeshProUGUI[] textComponents = newPrefab.GetComponentsInChildren<TextMeshProUGUI>();
            textComponents[0].text = userScores[i].username;
            textComponents[2].text = userScores[i].score.ToString();

            RectTransform rectTransform = newPrefab.GetComponent<RectTransform>();
            if (i == 0)
                rectTransform.localPosition = new Vector2(287, -42);
            else
                rectTransform.localPosition = new Vector2(287, -42 - (i * prefabHeight));
        }
    }

    public void OpenWindow(int _index)
    {
        if (GlobalChecker.CurrentToken == "") return;
        
        menuWindow.gameObject.SetActive(true);
        
        canvasGroups[_index].alpha = 1;
        canvasGroups[_index].interactable = true;
        canvasGroups[_index].blocksRaycasts = true;
    }
    
    public void CloseWindow()
    {
        menuWindow.gameObject.SetActive(false);
        foreach (CanvasGroup currentGroup in canvasGroups)
        {
            if (currentGroup.alpha > 0)
            {
                currentGroup.alpha = 0;
                currentGroup.interactable = false;
                currentGroup.blocksRaycasts = false;
            }
        }
    }
    #endregion
}

