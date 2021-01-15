using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class GameSceneInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (!NetworkManager.Instance.IsLogedIn)
        {
            SceneManager.LoadScene("LoginScene");
            return;
        }

        PlayerInfo myInfo = PlayerManager.Instance.GetMe();

        CharacterBase player = InitPlayerCharacter(myInfo);
        if (!player)
        {
            Debug.LogError("[GameSceneInitializer] Init failed. player is null. Stop running...");
            QuitApp();
        }

        if (!InitCamera(player.transform))
        {
            Debug.LogError("[GameSceneInitializer] InitCamera failed. Stop running...");
            QuitApp();
        }

        if (!InitPlayerInputManager(player))
        {
            Debug.LogError("[GameSceneInitializer] PlayerInputManager Init failed. Stop running...");
            QuitApp();
        }

        InitOtherPlayersCharacter();
    }

    private void QuitApp()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private bool InitPlayerInputManager(CharacterBase player)
    {
        Object prefab = Resources.Load("Prefabs/Managers/PlayerInputManager", typeof(GameObject));
        if (!prefab)
        {
            Debug.LogError("[GameSceneInitializer::InitPlayerInputManager] failed. prefab is null.");
            return false;
        }

        GameObject obj = Instantiate(prefab, new Vector3(0, 100, 0), Quaternion.Euler(Vector3.zero)) as GameObject;
        PlayerInputManager manager = obj.GetComponent<PlayerInputManager>();

        if (!manager)
        {
            Debug.LogError("[GameSceneInitializer::InitPlayerInputManager] failed. PlayerInputManager is null.");
            return false;
        }

        return manager.Init(player);
    }

    private CharacterBase InitPlayerCharacter(PlayerInfo info)
    {
        return PlayerManager.Instance.CreatePlayerObject(info);
    }

    private void InitOtherPlayersCharacter()
    {
        PlayerManager.Instance.CreateOtherPlayerPrefabs();
    }

    private bool InitCamera(Transform player)
    {
        Object mainCamPrefab = Resources.Load("Prefabs/Cameras/MainCamera", typeof(GameObject));
        Object tpCamPrefab = Resources.Load("Prefabs/Cameras/ThirdPersonCamera", typeof(GameObject));
        if (!mainCamPrefab || !tpCamPrefab)
        {
            Debug.LogError("[GameSceneInitializer::InitCamera] failed. mainCamPrefab or tpCamPrefab is null.");
            return false;
        }

        GameObject mainCam = Instantiate(mainCamPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;
        GameObject tpCam = Instantiate(tpCamPrefab, Vector3.zero, Quaternion.Euler(Vector3.zero)) as GameObject;

        CinemachineBrain brainCam = mainCam.GetComponent<CinemachineBrain>();
        CinemachineFreeLook freelockCam = tpCam.GetComponent<CinemachineFreeLook>();
        if (!brainCam || !freelockCam)
        {
            Debug.LogError("[GameSceneInitializer::InitCamera] failed. brainCam or freelockCam is null.");
            return false;
        }

        freelockCam.Follow = player;
        freelockCam.LookAt = player;
        return true;
    }
}
