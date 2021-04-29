using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using System.IO;

public class Gamemanager : NetworkBehaviour
{
    public static Gamemanager instance;
    public string currentUsername;

    void Awake()
    {
        if(!instance)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

  public override void NetworkStart()
  {
    base.NetworkStart();
    /*foreach(MLAPI.Connection.NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
    {
        Debug.Log(client);
    }*/
  }

  /*public override void NetworkStart(Stream stream)
  {
    base.NetworkStart(stream);
  }*/

  public override void OnGainedOwnership()
  {
    base.OnGainedOwnership();
  }

  public override void OnLostOwnership()
  {
    base.OnLostOwnership();
  }
}
