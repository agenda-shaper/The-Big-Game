using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using System.IO;


[System.Serializable]
public class Item
{
    public int id;
    public int count;
    public int typeId;

    public Item(int id, int count, int typeId)
    {
        this.id = id;
        this.count = Mathf.Clamp(count, 1, 255);
        this.typeId = typeId;
    }
}

public class Inventory
{
    public Item[] slots;

    public Inventory(int numberOfSlots)
    {
        slots = new Item[numberOfSlots];
    }

    public void AddItemToSlot(int slotIndex, Item item)
    {
        if (slotIndex >= 0 && slotIndex < slots.Length)
        {
            slots[slotIndex] = item;
        }
        else
        {
            Debug.LogError("Slot index out of range.");
        }
    }
    public void AddSlots(int numberOfSlotsToAdd)
    {
        Item[] newSlots = new Item[slots.Length + numberOfSlotsToAdd];
        slots.CopyTo(newSlots, 0);
        slots = newSlots;
    }
}
public class PlayerVitalStats
{
    public int Health { get; private set; } = 255;
    public int Hunger { get; private set; } = 255;
    public int Cold { get; private set; } = 255;
    public int Stamina { get; private set; } = 255;
    public int Radiation { get; private set; } = 255;

    // Other vital stats-related methods here...
}
public class Player
{    
    public GameObject UnityObject { get; private set; }

    public Inventory Inventory { get; private set; }
    public PlayerVitalStats VitalStats { get; private set; }
    public string Name { get; private set; }
    public int Level { get; private set; }
    public Clan clan { get; private set; }




    public Player(string name, GameObject unityObject)
    {
        UnityObject = unityObject;
        Name = name;
        Level = 0; // Starting level
        Inventory = new Inventory(8);
        VitalStats = new PlayerVitalStats();
        clan = null;
    }


    public void LevelUp(int levelsToIncrease)
    {
        Level += levelsToIncrease;
    }
    public void SetLocation(float x, float y, float z)
    {
        if (UnityObject != null)
        {
            UnityObject.transform.position = new Vector3(x, y, z);
        }
        else
        {
            Debug.LogError("UnityObject is not assigned. Cannot set location.");
        }
    }

}

public class Clan
{
    public string Name { get; private set; }
    public List<Player> Members { get; private set; }
    public Clan(string name)
    {
        Name = name;
        Members = new List<Player>();
    }

    public void AddMember(Player player)
    {
        if (!Members.Contains(player))
        {
            Members.Add(player);
        }
    }

    public void RemoveMember(Player player)
    {
        Members.Remove(player);
    }

    // Other clan-related methods here...
}
public class Tile
{
    public Vector3 Position { get; private set; }
    public GameObject TileObject { get; private set; }
    public Material Material { get; private set; }

    public Tile(Vector3 position, GameObject prefab, Material material)
    {
        Position = position;
        Material = material;

        // Instantiate the tile object using the provided prefab
        TileObject = GameObject.Instantiate(prefab, position, Quaternion.identity);

        // If the material is provided, apply it to the tile object
        if (Material != null)
        {
            MeshRenderer renderer = TileObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.material = Material;
            }
        }
    }

    // Method to change the material of the tile
    public void SetMaterial(Material newMaterial)
    {
        Material = newMaterial;
        MeshRenderer renderer = TileObject.GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = Material;
        }
    }

    // Any other tile-related methods can go here...
}
public class engine : MonoBehaviour
{

    public GameObject mainPlayerPrefab; 
    public GameObject World;

    public List<Player> players = new List<Player>();
    public List<Clan> clans = new List<Clan>();
    public Vector3 spawnPoint = new Vector3(0, 0, 0);

    WebSocket websocket;




public void SpawnMainPlayer(string name)
{

    // Instantiate the player at the current position of this object with no rotation
    GameObject mainPlayer = Instantiate(mainPlayerPrefab, spawnPoint, Quaternion.identity);

    Player player = new Player(name, mainPlayer);
    players.Add(player);



}

async void StartGame(string playerName)
{
  if (websocket.State == WebSocketState.Open)
  {
    // Create a JSON object with action code and player name
    string message = JsonUtility.ToJson(new { action = 10, name = playerName });
    await websocket.SendText(message);
    Debug.Log("Start game message sent!");
  }
}




async void SendRotation(float rotation)
{
    using (MemoryStream ms = new MemoryStream())
    using (BinaryWriter writer = new BinaryWriter(ms))
    {
        writer.Write((byte)6); // action code for rotation
        writer.Write(rotation); // write the rotation as a floating-point number
        await websocket.Send(ms.ToArray());
    }
}

async void SendMovement(int movingTo)
{
    using (MemoryStream ms = new MemoryStream())
    using (BinaryWriter writer = new BinaryWriter(ms))
    {
        writer.Write((byte)2); // action code for movement
        writer.Write((byte)movingTo); // write the movingTo as a byte
        await websocket.Send(ms.ToArray());
    }
}

async void SendShiftState(int shiftState)
{
    using (MemoryStream ms = new MemoryStream())
    using (BinaryWriter writer = new BinaryWriter(ms))
    {
        writer.Write((byte)7); // action code for shift state
        writer.Write((byte)shiftState); // write the shift state as a byte
        await websocket.Send(ms.ToArray());
    }
}

  // Start is called before the first frame update
  async void Start()
  {
    websocket = new WebSocket("ws://localhost:3000");

    websocket.OnOpen += () =>
    {
      Debug.Log("Connection open!");
    };

    websocket.OnError += (e) =>
    {
      Debug.Log("Error! " + e);
    };

    websocket.OnClose += (e) =>
    {
      Debug.Log("Connection closed!");
    };

    websocket.OnMessage += (bytes) =>
    {
      Debug.Log("OnMessage!");
      Debug.Log(bytes);

      // getting the message as a string
      // var message = System.Text.Encoding.UTF8.GetString(bytes);
      // Debug.Log("OnMessage! " + message);
    };

    // Keep sending messages at every 0.3s
    InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

    // waiting for messages
    await websocket.Connect();
  }

  void Update()
  {
    #if !UNITY_WEBGL || UNITY_EDITOR
      websocket.DispatchMessageQueue();
    #endif
  }

  async void SendWebSocketMessage()
  {
    if (websocket.State == WebSocketState.Open)
    {
      // Sending bytes
      await websocket.Send(new byte[] { 10, 20, 30 });

      // Sending plain text
      await websocket.SendText("plain text message");
    }
  }

  private async void OnApplicationQuit()
  {
    await websocket.Close();
  }

}