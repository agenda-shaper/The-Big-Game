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
        engine.AddPlayer(playerComponent);

        GameObject LocalPlayer = Instantiate(engine.localPlayerPrefab, engine.spawnPoint, Quaternion.Euler(90, 0, 0));

        playerComponent.localPlayer = LocalPlayer.GetComponent<LocalPlayer>();
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        Character player = conn.identity.GetComponent<Character>();
        engine.playerList.Remove(player);

        base.OnServerDisconnect(conn);
    }
}
