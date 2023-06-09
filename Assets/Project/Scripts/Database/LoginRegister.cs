using System.Collections;
using Photon.Pun;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine;

public class LoginRegister : MonoBehaviour
{
    public static LoginRegister Instance;

    [SerializeField]
    private TMP_InputField _emailInput;

    [SerializeField]
    private TMP_InputField _passwordInput ;

    [SerializeField]
    NetworkPlayerSync _networkPlayerSync;

    [SerializeField]
    PanelControl _panelControl;

    [SerializeField]
    private TMP_InputField _username ;

    [SerializeField]
    TextMeshProUGUI _popUp;
    [SerializeField]
    TextMeshProUGUI _errorText;

    [SerializeField]
    GameObject _loadingTxt;

    [SerializeField]
    PlayFabSharedSettings playFabSharedSettings;

    public void RegisterButton()
    {
        if (_passwordInput.text.Length < 6)
        {
            Debug.Log("Password Too Short");
            return;
        }
        var request =
            new RegisterPlayFabUserRequest()
            {
                DisplayName = _username.text,
                Email = _emailInput.text,
                Password = _passwordInput.text,
                Username = _username.text,
                RequireBothUsernameAndEmail = false
            };
        PlayFabClientAPI.RegisterPlayFabUser (
            request,
            OneRegisterSucces,
            OnError
        );
    }
    public void LoginButton()
    {
        var request =
            new LoginWithEmailAddressRequest()
            {
                Email = _emailInput.text,
                Password = _passwordInput.text,
                InfoRequestParameters =
                    new GetPlayerCombinedInfoRequestParams {
                        GetPlayerProfile = true
                    }
            };
        PlayFabClientAPI.LoginWithEmailAddress (
            request,
            OnLoginSucces,
            OnError
        );
    }
    public void ResetPasswordButton()
    {
        var request =
            new SendAccountRecoveryEmailRequest()
            {
                Email =_emailInput.text,
                TitleId = playFabSharedSettings.TitleId
            };
        PlayFabClientAPI.SendAccountRecoveryEmail (
            request,
            OnPasswordReset,
            OnError
        );
    }
    void OnError(PlayFabError error)
    {
        Debug.Log(error.ErrorMessage);
        Debug.Log(error.GenerateErrorReport());
        _errorText.text =error.ErrorMessage;
    }
    void OneRegisterSucces(RegisterPlayFabUserResult result)
    {
        Debug.Log("Register and Logged Succesful" + result);

        _emailInput.text = "";
        _passwordInput.text = "";
        _panelControl.LoginPanel();       
    }
    void OnLoginSucces(LoginResult result)
    {
        Debug.Log("Login Succesful" + result);
        string name = null;
        if (result.InfoResultPayload != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
            PhotonNetwork.NickName = name;
            PlayerPrefs.SetString("Username", name);
        }
        _loadingTxt.SetActive(true);
        _popUp.text = "Welcome" + " " + PlayerPrefs.GetString("Username");
        StartCoroutine(OnLoginNetwork());
    }
    void OnPasswordReset(SendAccountRecoveryEmailResult result)
    {
        Debug.Log("Send Recovery Succesful" + result);
    }
    IEnumerator OnLoginNetwork()
    {
        yield return new WaitForSeconds(2f);
        PhotonManager._networkManager.Login();
    }
     public void OnUpdateCustomDataSuccess(UpdateUserDataResult result)
    {
        Debug.Log("Custom data updated!");
    }

    public void OnUpdateCustomDataError(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }
    private void Start(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    public void SetEmail(string email){
        this._emailInput.text = email;
    }

    public void SetPassword(string password){
        this._passwordInput.text = password;
    }

    public void SetUsername(string username){
        this._username.text = username;
    }

    public string GetUsername(){
        return this._username.text;
    }

    public void FillTextComponents(string email, string password)
    {
        _emailInput.text = email;
        _passwordInput.text = password;
    }
}
