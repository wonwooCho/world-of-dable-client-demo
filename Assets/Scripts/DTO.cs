using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
    public string id;
    public ECharacterRace characteRace;
    public PlayerMoveInfo moveInfo;
}

[System.Serializable]
public class PlayerInfoList
{
    public List<PlayerInfo> playerList;
}

[System.Serializable]
public class AdData
{
    public string request_id;
    public string title;
    public string thumbnail;
    public string link;
    public string ad_mark;
}

[System.Serializable]
public class AdDataList
{
    public List<AdData> result;
}

[System.Serializable]
public class PlayerMoveInfo
{
    public Vector3 position;
    public Vector3 dir;
    public bool isMoving;
    public float angle;
}