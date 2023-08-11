using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using Mirror;

public class Engine : NetworkBehaviour
{

  public List<Player> playerList = new List<Player>();
  public NetworkManager networkManager; // Assign in the Inspector


  void FixedUpdate()
  {
    // Check if this instance is running on the server
    if (!isServer) return;

    Tick();
  }

  void Tick(){
    //networkManager.GetComponents<Player>;
    // foreach (Player player in networkManager.GetComponents<>)
    // {
    //   player.HandleMovement(); // You would define this method in your Player class
    //   Debug.Log("updating movement");
    // }
  }

  void Start(){
    Debug.Log("Starting");
    networkManager.StartHost();
    //networkManager.StartClient();
    //networkManager.OnClientConnect();
  }


  void OnApplicationQuit()
  {
    //
  }

}