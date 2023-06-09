using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class PhotonManager : MonoBehaviourPunCallbacks
{
    public static PhotonManager _networkManager;
    [SerializeField] string roomName;
    [SerializeField] GameObject pcObject;

    [SerializeField] GameObject game;
    [SerializeField] GameObject login;

    //[SerializeField] VoiceChatManager agoraChat; BURAYA DÖNECEZ
    [SerializeField] TMP_InputField a_chat;
    public int actorNmbr;
    public string name;
    public PhotonView photonView;
    Dictionary<string, Player> players;


    private void Awake()
    {
        PhotonNetwork.SendRate = 1;
        pcObject.SetActive(true);
    }
    void Start()
    {
        if (_networkManager == null)
        {
            _networkManager = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            PhotonNetwork.Destroy(gameObject);
        }
        PhotonNetwork.ConnectUsingSettings();
        actorNmbr = 1;
    }

    public override void OnConnectedToMaster()
    {
        DebugConsole("ConnectedToServer");
        PhotonNetwork.JoinLobby();
    }
    public override void OnJoinedLobby()
    {
        DebugConsole("JoinLobby");
    }
    public override void OnJoinedRoom()
    {
        DebugConsole("Odadayiz");

        login.SetActive(false);
        game.SetActive(true);

        //agoraChat.Join(); BURAYAR Döneceğiz
        CreatePlayer();
        Player player = PhotonNetwork.CurrentRoom.GetPlayer(actorNmbr);
        players = new Dictionary<string, Player>();
        players.Add(player.NickName, player);
        photonView.RPC("actorinc", RpcTarget.All, actorNmbr);
        name = a_chat.text;       
    }
    [PunRPC]
    public void actorinc(int actorNmbr)
    {
        actorNmbr += 1000;
    }
    public void Ban()
    {
        name = a_chat.text;
        photonView.RPC("KickPlayer", players[name]);
    }
    [PunRPC]
    public void KickPlayer()
    {
        PhotonNetwork.LeaveRoom();
    }
    private void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    private void CreateRoomSync(RoomOptions roomOptions)
    {
        PhotonNetwork.CreateRoom(roomName, roomOptions, TypedLobby.Default);
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 20,
            IsOpen = true,
            IsVisible = true
        };
        CreateRoomSync(roomOptions);
    }
    IEnumerator JoinRoom()
    {
        yield return null;
        PhotonNetwork.JoinRoom(roomName);
    }
    public void Login()
    {
        if (PhotonNetwork.InLobby)
        {
            DebugConsole("Waiting...");
            StartCoroutine(JoinRoom());
        }

    }
    public void CreatePlayer()
    {
        string gender = PlayerPrefs.GetString("Gender");
        Debug.Log(gender + "gender");
        GameObject[] points = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int value = Random.Range(0, points.Length);

        if (gender != null)
        {
            PhotonNetwork.Instantiate(gender, points[value].transform.position, Quaternion.identity);
            return;
        }
        else
        {
            Debug.Log(gender + "qqq");
        }
    }

    public void DebugConsole(string status)
    {
        Debug.Log(status);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (this.CanRecoverFromDisconnect(cause))
        {
            this.Recover();
        }
    }

    private bool CanRecoverFromDisconnect(DisconnectCause cause)
    {
        switch (cause)
        {
            // the list here may be non exhaustive and is subject to review
            case DisconnectCause.Exception:
            case DisconnectCause.ServerTimeout:
            case DisconnectCause.ClientTimeout:
            case DisconnectCause.DisconnectByServerLogic:
            case DisconnectCause.DisconnectByServerReasonUnknown:
                return true;
        }
        return false;
    }

    private void Recover()
    {
        if (!PhotonNetwork.ReconnectAndRejoin())
        {
            Debug.LogError("ReconnectAndRejoin failed, trying Reconnect");
            StartCoroutine(JoinRoom());
            if (!PhotonNetwork.Reconnect())
            {
                Debug.LogError("Reconnect failed, trying ConnectUsingSettings");
                if (!PhotonNetwork.ConnectUsingSettings())
                {
                    Debug.LogError("ConnectUsingSettings failed");
                }
            }
        }
    }
}