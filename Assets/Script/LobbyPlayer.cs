using UnityEngine;
using UnityEngine.Networking;

public class LobbyPlayer : NetworkLobbyPlayer
{
    public override void OnClientEnterLobby()
    {
        base.OnClientEnterLobby();

        if (!GetComponent<PlayerMovement>().setuped)
        {

            NetworkManager netManager = GameObject.Find("GameManager").GetComponent<NetworkManager>();

            int playerNumber = 0;

            if (netManager.ControllerP1 != 0 && GameObject.Find("LobbyPlayer (1)") == null)
                playerNumber = 1;
            if (netManager.ControllerP2 != 0 && GameObject.Find("LobbyPlayer (2)") == null)
                playerNumber = 2;
            if (netManager.ControllerP3 != 0 && GameObject.Find("LobbyPlayer (3)") == null)
                playerNumber = 3;
            if (netManager.ControllerP4 != 0 && GameObject.Find("LobbyPlayer (4)") == null)
                playerNumber = 4;

            if (playerNumber == 0)
                playerNumber = 1;

            gameObject.name = "LobbyPlayer (" + playerNumber + ")";
            netManager.SetupPlayerController(gameObject.GetComponent<PlayerMovement>(), playerNumber);
        }
        else
            LobbyEntered();
    }

    private void LobbyEntered()
    {
        GetComponent<PlayerMovement>().enabled = true;
        GetComponent<CapsuleCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;

        transform.position = new Vector3(0, 2, 0);
        GetComponent<Rigidbody>().velocity = new Vector3(0, 10, 0);
    }

    public void GameEntered()
    {
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
    }
}
