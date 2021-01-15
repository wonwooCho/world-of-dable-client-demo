using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PlayerManager
{
    private static PlayerManager instance = null;
    public static PlayerManager Instance { get { return instance == null ? instance = new PlayerManager() : instance; } }

    private List<PlayerInfo> playerInfoList = new List<PlayerInfo>();
    private Dictionary<string, CharacterBase> playerScriptMap = new Dictionary<string, CharacterBase>();

    public string MyId { get; private set; }

    public void SetMyId(string info)
    {
        MyId = info;
    }

    public PlayerInfo GetMe()
    {
        PlayerInfo me = playerInfoList.Find(x => x.id.Equals(MyId));
        return me;
    }

    public void AddPlayerInfoList(PlayerInfo player)
    {
        playerInfoList.Add(player);

        Debug.Log("Current player count: " + playerInfoList.Count);
    }

    public void RemovePlayer(string id)
    {
        if (id.Equals(MyId)) return;

        int targetIndex = playerInfoList.FindIndex(x => x.id.Equals(id));
        if (-1 < targetIndex)
        {
            playerInfoList.RemoveAt(targetIndex);

            GameObject.Destroy(playerScriptMap[id].gameObject);
            playerScriptMap.Remove(id);
        }
        
        Debug.Log("Current player count: " + playerInfoList.Count);
    }

    public CharacterBase CreatePlayerObject(PlayerInfo info)
    {
        Object prefab =
            info.characteRace == ECharacterRace.Footman ?
                Resources.Load("Prefabs/Characters/FootmanPrefab", typeof(GameObject)) :
                Resources.Load("Prefabs/Characters/OcWarriorPrefab", typeof(GameObject));
        
        if (!prefab)
        {
            Debug.LogErrorFormat("[PlayerManager::CreateOtherPlayerPrefabs] prefab is null. player id: {0}", info.id);
            return null;
        }

        GameObject playerInstance = GameObject.Instantiate(prefab, info.moveInfo.position, Quaternion.Euler(Vector3.zero)) as GameObject;
        CharacterBase playerScript = playerInstance.GetComponent<CharacterBase>();
        playerScript.SetPlayerInfo(info);
        playerScriptMap.Add(info.id, playerScript);

        return playerScript;
    }

    public void CreateOtherPlayerPrefabs()
    {
        foreach(PlayerInfo info in playerInfoList)
        {
            if (info.id != MyId)
            {
                CreatePlayerObject(info);
            }
        }
    }

    public void ProcessOnPlayerInput(JSONObject inData)
    {
        string id = inData["id"].ToString().Replace("\"", "");
        string actionStr = inData["action"].ToString().Replace("\"", "");

        ECHaracterAction action = ECHaracterAction.Idle;
        if (System.Enum.TryParse(actionStr, out action))
        {
            switch (action)
            {
                case ECHaracterAction.Idle:
                {
                    playerScriptMap[id]?.Idle();
                    break;
                }

                case ECHaracterAction.Attack:
                {
                    playerScriptMap[id]?.TryAttack();
                    break;
                }

                case ECHaracterAction.Jump:
                {
                    playerScriptMap[id]?.TryJump();
                    break;
                }

                case ECHaracterAction.Move:
                {
                    CharacterBase player = playerScriptMap[id];
                    if (!player) return;

                    PlayerInfo info = player.Info;
                    string isMoving = inData["isMoving"].ToString().Replace("\"", "");
                    info.moveInfo.isMoving = bool.Parse(isMoving);

                    if (info.moveInfo.isMoving)
                    {
                        string x = inData["x"].ToString().Replace("\"", "");
                        string y = inData["y"].ToString().Replace("\"", "");
                        string z = inData["z"].ToString().Replace("\"", "");
                        string x_dir = inData["x_dir"].ToString().Replace("\"", "");
                        string y_dir = inData["y_dir"].ToString().Replace("\"", "");
                        string z_dir = inData["z_dir"].ToString().Replace("\"", "");
                        string angle = inData["angle"].ToString().Replace("\"", "");

                        info.moveInfo.position.x = float.Parse(x);
                        info.moveInfo.position.y = float.Parse(y);
                        info.moveInfo.position.z = float.Parse(z);
                        info.moveInfo.dir.x = float.Parse(x_dir);
                        info.moveInfo.dir.y = float.Parse(y_dir);
                        info.moveInfo.dir.z = float.Parse(z_dir);
                        info.moveInfo.angle = float.Parse(angle);

                        player.StopAdjustCharacterPos();
                    }
                    else
                    {
                        string x = inData["x"].ToString().Replace("\"", "");
                        string y = inData["y"].ToString().Replace("\"", "");
                        string z = inData["z"].ToString().Replace("\"", "");

                        info.moveInfo.position.x = float.Parse(x);
                        info.moveInfo.position.y = float.Parse(y);
                        info.moveInfo.position.z = float.Parse(z);

                        player.AdjustCharacterPos();
                    }

                    break;
                }
            }
        }
    }
}
