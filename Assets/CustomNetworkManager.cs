using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public Engine engine;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        GameObject player = Instantiate(playerPrefab, engine.spawnPoint, Quaternion.identity);

        player.name = $"{playerPrefab.name} [connId={conn.connectionId}]";
        NetworkServer.AddPlayerForConnection(conn, player);

        // Once the player is added, add it to your engine
        Character playerComponent = player.GetComponent<Character>();
        engine.playerList.Add(playerComponent);

    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Character player = conn.identity.GetComponent<Character>();
        engine.playerList.Remove(player);

        base.OnServerDisconnect(conn);
    }
}
