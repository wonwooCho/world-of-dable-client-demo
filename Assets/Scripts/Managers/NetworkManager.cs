using System.Collections;
using System.Collections.Generic;
using SocketIO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(SocketIOComponent))]
public class NetworkManager : MonoBehaviour
{
    private static NetworkManager instance = null;
    public static NetworkManager Instance
    {
        get
        {
            if (!instance)  
            {  
                instance = GameObject.Find("NetworkManager")?.GetComponent<NetworkManager>() ?? null;  
                if (!instance)
                {
                    Object prefab = Resources.Load("Prefabs/Managers/NetworkManager", typeof(GameObject));
                    if (!prefab)
                    {
                        Debug.LogError("There needs to be one active NetworkManager script on a GameObject in your scene.");
                        return null;
                    }

                    GameObject obj = Instantiate(prefab, new Vector3(0, 100, 0), Quaternion.Euler(Vector3.zero)) as GameObject;
                    instance = obj.GetComponent<NetworkManager>();
                }
            }
            return instance;
        }
    }
    
    private SocketIOComponent socketComponent;

    public bool IsLogedIn { get; private set; } = false;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        
        socketComponent = GetComponent<SocketIOComponent>();
    }

    public void ConnectToServer()
    {
        socketComponent.On(SocketEventConstant.LOGIN, OnLoginCallback);
        socketComponent.On(SocketEventConstant.LOGIN_OTHER, OnLoginOtherCallback);
        socketComponent.On(SocketEventConstant.LOGOUT, OnLogoutCallback);

        socketComponent.On(SocketEventConstant.PLAYER_INPUT_ACTION, OnPlayerInputAction);

        socketComponent.On(SocketEventConstant.REQUEST_AD_DATA, OnRequestAdData);
        socketComponent.Connect();
    }

    ////////////////////////////////////////////////////////////////////
    // callback
    ////////////////////////////////////////////////////////////////////
    private System.Action<AdDataList> adDataListCallback;
    
    private void OnLoginCallback(SocketIOEvent args)
    {
        string myId = args.data["data"]["id"].ToString().Replace("\"", "");
        string playerList = args.data["data"]["playerList"].ToString();
        Debug.LogFormat("Connected! my id is {0}", myId);
        Debug.LogFormat("playerList is {0}", playerList);

        PlayerInfoList parsedPlayers = JsonUtility.FromJson<PlayerInfoList>(playerList);
        PlayerManager.Instance.SetMyId(myId);
        parsedPlayers.playerList.ForEach(player => {
            PlayerManager.Instance.AddPlayerInfoList(player);
        });

        IsLogedIn = true;
        Debug.Log("Now Load game scene...");
        SceneManager.LoadScene("GameScene");
    }

    private void OnLoginOtherCallback(SocketIOEvent args)
    {
        string playerId = args.data["data"]["playerInfo"].ToString().Replace("\"", "");
        PlayerInfo playerInfo = JsonUtility.FromJson<PlayerInfo>(args.data["data"]["playerInfo"].ToString());

        PlayerManager.Instance.AddPlayerInfoList(playerInfo);
        PlayerManager.Instance.CreatePlayerObject(playerInfo);
    }

    private void OnLogoutCallback(SocketIOEvent args)
    {
        string playerId = args.data["data"]["id"].ToString().Replace("\"", "");
        PlayerManager.Instance.RemovePlayer(playerId);

        Debug.LogFormat("Player {0} disconnected.", playerId);
    }

    private void OnPlayerInputAction(SocketIOEvent args)
    {
        JSONObject data = args.data["data"];
        PlayerManager.Instance.ProcessOnPlayerInput(data);
    }

    private void OnRequestAdData(SocketIOEvent args)
    {
        string adList = args.data["adData"].ToString();
        // Debug.Log("ad data returned: " + adList);

        AdDataList parsedAdDatas = JsonUtility.FromJson<AdDataList>(adList);
        adDataListCallback?.Invoke(parsedAdDatas);
    }

    ////////////////////////////////////////////////////////////////////
    // reqeust
    ////////////////////////////////////////////////////////////////////
    private Dictionary<string, string> sendData = new Dictionary<string, string>();
    public void RequestAdData(System.Action<AdDataList> callback)
    {
        adDataListCallback = callback;
        
        sendData.Clear();
        sendData.Add("data", "mocked_data");

        JSONObject data = new JSONObject();

        socketComponent.Emit(SocketEventConstant.REQUEST_AD_DATA, new JSONObject(sendData));
    }

    public void SendPlayerAction(ECHaracterAction action)
    {
        sendData.Clear();
        sendData.Add("id", PlayerManager.Instance.MyId);
        sendData.Add("action", action.ToString());

        socketComponent.Emit(SocketEventConstant.PLAYER_INPUT_ACTION, new JSONObject(sendData));
    }

    public void SendPlayerMoving(PlayerMoveInfo moveInfo)
    {
        sendData.Clear();
        sendData.Add("id", PlayerManager.Instance.MyId);
        sendData.Add("action", ECHaracterAction.Move.ToString());
        sendData.Add("x", moveInfo.position.x.ToString());
        sendData.Add("y", moveInfo.position.y.ToString());
        sendData.Add("z", moveInfo.position.z.ToString());
        sendData.Add("x_dir", moveInfo.dir.x.ToString());
        sendData.Add("y_dir", moveInfo.dir.y.ToString());
        sendData.Add("z_dir", moveInfo.dir.z.ToString());
        sendData.Add("angle", moveInfo.angle.ToString());
        sendData.Add("isMoving", moveInfo.isMoving.ToString());

        socketComponent.Emit(SocketEventConstant.PLAYER_INPUT_ACTION, new JSONObject(sendData));
    }

    public void SendPlayerEndMove(PlayerMoveInfo moveInfo)
    {
        sendData.Clear();
        sendData.Add("id", PlayerManager.Instance.MyId);
        sendData.Add("action", ECHaracterAction.Move.ToString());
        sendData.Add("x", moveInfo.position.x.ToString());
        sendData.Add("y", moveInfo.position.y.ToString());
        sendData.Add("z", moveInfo.position.z.ToString());
        // sendData.Add("x_dir", moveInfo.dir.x.ToString());
        // sendData.Add("y_dir", moveInfo.dir.y.ToString());
        // sendData.Add("z_dir", moveInfo.dir.z.ToString());
        // sendData.Add("angle", moveInfo.angle.ToString());
        sendData.Add("isMoving", moveInfo.isMoving.ToString());

        socketComponent.Emit(SocketEventConstant.PLAYER_INPUT_ACTION, new JSONObject(sendData));
    }
}
