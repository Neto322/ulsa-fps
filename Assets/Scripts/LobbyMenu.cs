using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using TMPro;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField]
    Button btnStartServer;
    [SerializeField]
    Button btnStartHost;
    [SerializeField]
    Button btnStartClient;
    [SerializeField]
    TMP_InputField tmpInfUsername;

    void Awake()
    {
        btnStartServer.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartServer();
        });

        btnStartHost.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartHost();
            gameObject.SetActive(false);
        });

        btnStartClient.onClick.AddListener(()=>{
            NetworkManager.Singleton.StartClient();
            gameObject.SetActive(false);
        });
        tmpInfUsername.onValueChanged.AddListener(value =>{
            //Debug.Log(value);
            Gamemanager.instance.currentUsername = value;
        });
    }
}
