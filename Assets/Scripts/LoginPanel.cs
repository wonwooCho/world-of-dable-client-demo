using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] private Button sendMsg;

    void Start()
    {
        Screen.SetResolution(1920, 1280, FullScreenMode.Windowed);

        sendMsg.onClick.RemoveAllListeners();
        sendMsg.onClick.AddListener(() =>
        {
            NetworkManager.Instance.ConnectToServer();
        });
    }
}
