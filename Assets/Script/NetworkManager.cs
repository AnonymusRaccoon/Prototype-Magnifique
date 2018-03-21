using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : NetworkLobbyManager
{
    [Space]
    [SerializeField] private bool gameIsRunning = false;

    [Space]
    public int ControllerP1;
    public int ControllerP2;
    public int ControllerP3;
    public int ControllerP4;

    [Space]
    public int selectedMap;

    [Space]
    [SerializeField] internal bool isDead1 = false;
    [SerializeField] internal bool isDead2 = false;
    [SerializeField] internal bool isDead3 = false;
    [SerializeField] internal bool isDead4 = false;
    public bool IsDead1
    {
        get
        {
            return isDead1;
        }
        set
        {
            isDead1 = value;
            PlayerDied(1);
        }
    }
    public bool IsDead2
    {
        get
        {
            return isDead2;
        }
        set
        {
            isDead2 = value;
            PlayerDied(2);
        }
    }
    public bool IsDead3
    {
        get
        {
            return isDead3;
        }
        set
        {
            isDead3 = value;
            PlayerDied(3);
        }
    }
    public bool IsDead4
    {
        get
        {
            return isDead4;
        }
        set
        {
            isDead4 = value;
            PlayerDied(4);
        }
    }


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
        if (gameIsRunning)
            return;

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
        if ((Input.GetKey(KeyCode.JoystickButton0) && Input.GetKey(KeyCode.JoystickButton4) && Input.GetKey(KeyCode.JoystickButton5)) || Input.GetKey(KeyCode.Return))
        {
            int Controller = 0;

            if (Input.GetKey(KeyCode.Joystick1Button0) && Input.GetKey(KeyCode.Joystick1Button4) && Input.GetKey(KeyCode.Joystick1Button5))
                Controller = 1;
            if (Input.GetKey(KeyCode.Joystick2Button0) && Input.GetKey(KeyCode.Joystick2Button4) && Input.GetKey(KeyCode.Joystick2Button5))
                Controller = 2;
            if (Input.GetKey(KeyCode.Joystick3Button0) && Input.GetKey(KeyCode.Joystick3Button4) && Input.GetKey(KeyCode.Joystick3Button5))
                Controller = 3;
            if (Input.GetKey(KeyCode.Joystick4Button0) && Input.GetKey(KeyCode.Joystick4Button4) && Input.GetKey(KeyCode.Joystick4Button5))
                Controller = 4;
            if (Input.GetKey(KeyCode.Joystick5Button0) && Input.GetKey(KeyCode.Joystick5Button4) && Input.GetKey(KeyCode.Joystick5Button5))
                Controller = 5;
            if (Input.GetKey(KeyCode.Joystick6Button0) && Input.GetKey(KeyCode.Joystick6Button4) && Input.GetKey(KeyCode.Joystick6Button5))
                Controller = 6;
            if (Input.GetKey(KeyCode.Joystick7Button0) && Input.GetKey(KeyCode.Joystick7Button4) && Input.GetKey(KeyCode.Joystick7Button5))
                Controller = 7;
            if (Input.GetKey(KeyCode.Joystick8Button0) && Input.GetKey(KeyCode.Joystick8Button4) && Input.GetKey(KeyCode.Joystick8Button5))
                Controller = 8;
            if (Input.GetKeyDown(KeyCode.Return))
                Controller = 9;

            CheckControllerToSetReady(Controller);
        }
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

    private void CheckControllerToSetReady(int Controller)
    {
        if (Controller == 0)
            return;

        if (Controller == ControllerP1 && !GameObject.Find("LobbyPlayer (1)").GetComponent<LobbyPlayer>().readyToBegin)
        {
            SetPlayerReady(1);
            return;
        }
        if (Controller == ControllerP2 && !GameObject.Find("LobbyPlayer (2)").GetComponent<LobbyPlayer>().readyToBegin)
        {
            SetPlayerReady(2);
            return;
        }
        if (Controller == ControllerP3 && !GameObject.Find("LobbyPlayer (3)").GetComponent<LobbyPlayer>().readyToBegin)
        {
            SetPlayerReady(3);
            return;
        }
        if (Controller == ControllerP4 && !GameObject.Find("LobbyPlayer (4)").GetComponent<LobbyPlayer>().readyToBegin)
        {
            SetPlayerReady(4);
            return;
        }
    }

    private void SetPlayerReady(int player)
    {
        GameObject.Find("LobbyPlayer (" + player + ")").GetComponent<LobbyPlayer>().SendReadyToBeginMessage();
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
                pMovement.Vertical = "Vertical_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
                pMovement.HookKey = KeyCode.LeftShift;
                pMovement.DashKey = KeyCode.LeftControl;
                pMovement.UltKey = KeyCode.F;
                pMovement.ChannelKey = KeyCode.A;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP1;
                pMovement.Vertical = "Vertical_J" + ControllerP1;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button0");
                pMovement.HookKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button2");
                pMovement.DashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button3");
                pMovement.UltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button1");
                pMovement.ChannelKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button4");
                pMovement.ChannelKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP1 + "Button5");
            }
        }
        else if (player == 2)
        {
            if (ControllerP2 == 0)
                return;

            if (ControllerP2 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.Vertical = "Vertical_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
                pMovement.HookKey = KeyCode.LeftShift;
                pMovement.DashKey = KeyCode.LeftControl;
                pMovement.UltKey = KeyCode.F;
                pMovement.ChannelKey = KeyCode.A;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP2;
                pMovement.Vertical = "Vertical_J" + ControllerP2;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button0");
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button0");
                pMovement.HookKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button2");
                pMovement.DashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button3");
                pMovement.UltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button1");
                pMovement.ChannelKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button4");
                pMovement.ChannelKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP2 + "Button5");
            }
        }
        else if (player == 3)
        {
            if (ControllerP3 == 0)
                return;

            if (ControllerP3 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.Vertical = "Vertical_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
                pMovement.HookKey = KeyCode.LeftShift;
                pMovement.DashKey = KeyCode.LeftControl;
                pMovement.UltKey = KeyCode.F;
                pMovement.ChannelKey = KeyCode.A;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP3;
                pMovement.Vertical = "Vertical_J" + ControllerP3;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button0");
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button0");
                pMovement.HookKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button2");
                pMovement.DashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button3");
                pMovement.UltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button1");
                pMovement.ChannelKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button4");
                pMovement.ChannelKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP3 + "Button5");
            }
        }
        else if (player == 4)
        {
            if (ControllerP4 == 0)
                return; 

            if (ControllerP4 == 9)
            {
                pMovement.Horizontal = "Horizontal_Keyboard";
                pMovement.Vertical = "Vertical_Keyboard";
                pMovement.JumpKey = KeyCode.Space;
                pMovement.HookKey = KeyCode.LeftShift;
                pMovement.DashKey = KeyCode.LeftControl;
                pMovement.UltKey = KeyCode.F;
                pMovement.ChannelKey = KeyCode.A;
            }
            else
            {
                pMovement.Horizontal = "Horizontal_J" + ControllerP4;
                pMovement.Vertical = "Vertical_J" + ControllerP4;
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button0");
                pMovement.JumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button0");
                pMovement.HookKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button2");
                pMovement.DashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button3");
                pMovement.UltKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button1");
                pMovement.ChannelKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button4");
                pMovement.ChannelKey2 = (KeyCode)System.Enum.Parse(typeof(KeyCode), "Joystick" + ControllerP4 + "Button5");
            }
        }

        pMovement.setuped = true;
        if (!gameIsRunning)
            pMovement.transform.position = new Vector3(player * 2, 2, 0);
        else
        {
            GameObject spawnPoint = GameObject.Find("SpawnPoint(Clone)");
            pMovement.transform.position = spawnPoint.transform.position;
            Destroy(spawnPoint);
        }
        pMovement.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 5, 0);
    }

    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        int playerNumber = int.Parse(lobbyPlayer.name.Substring(lobbyPlayer.name.IndexOf("(") + 1, 1));

        gamePlayer.name = "GamePlayer (" + playerNumber + ")";
        SetupPlayerController(gamePlayer.GetComponent<PlayerMovement>(), playerNumber);

        lobbyPlayer.GetComponent<LobbyPlayer>().GameEntered();

        return base.OnLobbyServerSceneLoadedForPlayer(lobbyPlayer, gamePlayer);
    }

    public IEnumerator SetDeathZone(Vector3 topLeft, Vector3 bottomRight)
    {
        yield return new WaitForSeconds(0.5f);
        foreach(GameObject foo in GameObject.FindGameObjectsWithTag("Player"))
        {
            PlayerMovement player = foo.GetComponent<PlayerMovement>();
            player.topLeftDeath = topLeft;
            player.bottomRightDeath = bottomRight;
        }
    }

    private void PlayerDied(int player)
    {
        int playerCount = 0;

        if (ControllerP1 != 0)
            playerCount++;
        if (ControllerP2 != 0)
            playerCount++;
        if (ControllerP3 != 0)
            playerCount++;
        if (ControllerP4 != 0)
            playerCount++;

        int dieCount = 0;

        if (IsDead1)
            dieCount++;
        if (IsDead2)
            dieCount++;
        if (IsDead3 && playerCount > 2)
            dieCount++;
        if (IsDead4 && playerCount > 3)
            dieCount++;

        if (dieCount >= playerCount)
        {
            ServerReturnToLobby();
        }
        else if (dieCount == playerCount - 1)
        {
            int playerAlive = 0;

            if (!IsDead1)
                playerAlive = 1;
            if (!IsDead2)
                playerAlive = 2;
            if (!IsDead3 && playerCount > 2)
                playerAlive = 3;
            if (!IsDead4 && playerCount > 3)
                playerAlive = 4;

            //Win screen
            print("Player " + playerAlive + " won!");
            StartCoroutine("ReturnToLobby");
            isDead1 = false;
            isDead2 = false;
            isDead3 = false;
            isDead4 = false;
        }
        else
        {
            //Player is dead
            print("Player " + player + " is dead.");
        }
    }

    private IEnumerator ReturnToLobby()
    {
        yield return new WaitForSeconds(2.5f);
        gameIsRunning = false;
        ServerReturnToLobby();
    }
}
