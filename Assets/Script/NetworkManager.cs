using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : NetworkLobbyManager
{
    [Space]
    public int ControllerP1;
    public int ControllerP2;
    public int ControllerP3;
    public int ControllerP4;


    private void Start()
    {
        CreateGame();
    }

    private void CreateGame()
    {
        networkAddress = "localhost";
        networkPort = 4444;
        StartHost();

        foreach (string s in Input.GetJoystickNames())
            print(s);
    }

    private void Update()
    {
        //Check if a new controller or a keyboard is used and add a player
        if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.Space))
        {
            int Controller = 0;

            if (Input.GetKeyDown(KeyCode.Joystick1Button0))
                Controller = 1;
            if (Input.GetKeyDown(KeyCode.Joystick2Button0))
                Controller = 2;
            if (Input.GetKeyDown(KeyCode.Joystick3Button0))
                Controller = 3;
            if (Input.GetKeyDown(KeyCode.Joystick4Button0))
                Controller = 4;
            if (Input.GetKeyDown(KeyCode.Joystick5Button0))
                Controller = 5;
            if (Input.GetKeyDown(KeyCode.Joystick6Button0))
                Controller = 6;
            if (Input.GetKeyDown(KeyCode.Joystick7Button0))
                Controller = 7;
            if (Input.GetKeyDown(KeyCode.Joystick8Button0))
                Controller = 8;
            if (Input.GetKeyDown(KeyCode.Space))
                Controller = 9;

            CheckControllerToAdd(Controller);
        }
        //Check if user want to remove his player
        if ((Input.GetKey(KeyCode.JoystickButton1) && Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5)) || Input.GetKey(KeyCode.Escape))
        {
            int Controller = 0;

            if (Input.GetKey(KeyCode.Joystick1Button1) && Input.GetKey(KeyCode.Joystick1Button4) && Input.GetKey(KeyCode.Joystick1Button5))
                Controller = 1;
            if (Input.GetKey(KeyCode.Joystick2Button1) && Input.GetKey(KeyCode.Joystick2Button4) && Input.GetKey(KeyCode.Joystick2Button5))
                Controller = 2;
            if (Input.GetKey(KeyCode.Joystick3Button1) && Input.GetKey(KeyCode.Joystick3Button4) && Input.GetKey(KeyCode.Joystick3Button5))
                Controller = 3;
            if (Input.GetKey(KeyCode.Joystick4Button1) && Input.GetKey(KeyCode.Joystick4Button4) && Input.GetKey(KeyCode.Joystick4Button5))
                Controller = 4;
            if (Input.GetKey(KeyCode.Joystick5Button1) && Input.GetKey(KeyCode.Joystick5Button4) && Input.GetKey(KeyCode.Joystick5Button5))
                Controller = 5;
            if (Input.GetKey(KeyCode.Joystick6Button1) && Input.GetKey(KeyCode.Joystick6Button4) && Input.GetKey(KeyCode.Joystick6Button5))
                Controller = 6;
            if (Input.GetKey(KeyCode.Joystick7Button1) && Input.GetKey(KeyCode.Joystick7Button4) && Input.GetKey(KeyCode.Joystick7Button5))
                Controller = 7;
            if (Input.GetKey(KeyCode.Joystick8Button1) && Input.GetKey(KeyCode.Joystick8Button4) && Input.GetKey(KeyCode.Joystick8Button5))
                Controller = 8;
            if (Input.GetKeyDown(KeyCode.Escape))
                Controller = 9;

            CheckControllerToRemove(Controller);
        }
        //Check for ready calls

    }

    private void CheckControllerToAdd(int Controller)
    {
        if (Controller == ControllerP1 || Controller == ControllerP2 || Controller == ControllerP3 || Controller == ControllerP4)
            return;


        if (ControllerP1 == 0)
        {
            ControllerP1 = Controller;
            AddLocalPlayer(1);
            return;
        }
        if (ControllerP2 == 0)
        {
            ControllerP2 = Controller;
            AddLocalPlayer(2);
            return;
        }
        if (ControllerP3 == 0)
        {
            ControllerP3 = Controller;
            AddLocalPlayer(3);
            return;
        }
        if (ControllerP4 == 0)
        {
            ControllerP4 = Controller;
            AddLocalPlayer(4);
            return;
        }
    }

    private void AddLocalPlayer(int player)
    {
        if (player == 1 && GameObject.Find("LobbyPlayer (1)") != null)
        {
            SetupPlayerController(GameObject.Find("LobbyPlayer (1)").GetComponent<PlayerMovement>(), 1);
            return;
        }

        TryToAddPlayer();
    }

    private void CheckControllerToRemove(int Controller)
    {
        if (Controller == 0)
            return;

        if(Controller == ControllerP1)
        {
            ControllerP1 = 0;
            RemoveLocalPlayer(1);
            return;
        }
        if (Controller == ControllerP2)
        {
            ControllerP2 = 0;
            RemoveLocalPlayer(2);
            return;
        }
        if (Controller == ControllerP3)
        {
            ControllerP3 = 0;
            RemoveLocalPlayer(3);
            return;
        }
        if (Controller == ControllerP4)
        {
            ControllerP4 = 0;
            RemoveLocalPlayer(4);
            return;
        }
    }

    private void RemoveLocalPlayer(int player)
    {
        LobbyPlayer lobbyPlayer = GameObject.Find("LobbyPlayer (" + player + ")").GetComponent<LobbyPlayer>();
        lobbyPlayer.RemovePlayer();
    }

    public void SetupPlayerController(PlayerMovement pMovement, int player)
    {
        if(player == 1)
        {
            if (ControllerP1 == 0)
                return;

            if(ControllerP1 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP1;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button0");
            }
        }
        else if (player == 2)
        {
            if (ControllerP2 == 0)
                return;

            if (ControllerP2 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP2;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button0");
            }
        }
        else if (player == 3)
        {
            if (ControllerP3 == 0)
                return;

            if (ControllerP3 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP3;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button0");
            }
        }
        else if (player == 4)
        {
            if (ControllerP4 == 0)
                return; 

            if (ControllerP4 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP4;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button0");
            }
        }

        pMovement.gameObject.transform.position = new Vector3(0, 2, 0);
        pMovement.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 10, 0);
    }
}
