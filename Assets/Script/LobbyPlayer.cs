using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        NetworkManager netManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();

        int playerNumber = 0;

        if (netManager.ControllerP1 != 0)
            playerNumber = 1;
        if (netManager.ControllerP2 != 0)
            playerNumber = 2;
        if (netManager.ControllerP3 != 0)
            playerNumber = 3;
        if (netManager.ControllerP4 != 0)
            playerNumber = 4;

        if (playerNumber == 0)
            playerNumber = 1;

        gameObject.name = "LobbyPlayer (" + playerNumber + ")";
    }
}
