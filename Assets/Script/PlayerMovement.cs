using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Keybinds (changed at runtime)")]
    public string Horizontal;
    public string Vertical;
    public KeyCode JumpKey = KeyCode.None;
    public KeyCode HookKey = KeyCode.None;
    public KeyCode DashKey = KeyCode.None;
    public KeyCode UltKey = KeyCode.None;
    public KeyCode ChannelKey = KeyCode.None;
    public KeyCode ChannelKey2 = KeyCode.None;

    [Space]
    [Space]
    [Header("Movement settings")]
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 7;
    [SerializeField] private int jumpForce = 8;
    [SerializeField] private int smallJump = 4;
    [SerializeField] private float gravity = -10;
    [SerializeField] private int wallJump = 4;
    [SerializeField] private int wallJumpPush = 10;
    [SerializeField] private int smallJumpPush = 5;
    [SerializeField] private float pushSpeed = 0.2f;
    [SerializeField] private LayerMask playerMask;
    private bool groundedLastFrame = false;
    private int wallDirection;
    private int wallJumpTimer = 0;
    private bool wallJumped;
    private float jumpDirection;
    private MovingElement movingPlateform;
    [HideInInspector] public float velocity = 0;

    [Space]
    [Space]
    [Header("Hook variables")]
    [SerializeField] private float hookRange = 10;
    [SerializeField] private GameObject hookObject;
    [SerializeField] private float playerHookSpeed = 15;
    [SerializeField] private float springForce = Mathf.Infinity;
    [SerializeField] private float damperForce = Mathf.Infinity;
    [SerializeField] private float ropeSwing = 1;
    private GameObject hook;
    private HookType hookType = HookType.Wall;
    private GameObject objectHooked;
    private Vector3 hookPosition;
    private float hookLength;
    private Rigidbody rb;

    [Space]
    [Space]
    [Header("Dash variables")]
    [SerializeField] private float dashSpeed = 15;
    [SerializeField] private GameObject smallProjectile;
    [SerializeField] private float sProjSpeed = 2;
    private bool canDash = true;
    private float dashTime = 0;
    private Vector3 dashVelocity;

    [Space]
    [Space]
    [Header("Ult variables")]
    [SerializeField] private float energy = 0;
    public bool channeling = false;

    [SerializeField] private GameObject largeProjectile;
    [SerializeField] private float lProjSpeed = 4;

    [SerializeField] private GameObject swapProjectile;
    [SerializeField] private float SwapProjSpeed = 25;
    [SerializeField] private float largeDamper = 200;

    [Space]
    [Space]
    [Header("Game Information")]
    public bool setuped = false;
    [SerializeField] private bool gameIsRunning = false;
    public Vector3 topLeftDeath;
    public Vector3 bottomRightDeath;



    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private bool IsGrounded()
    {
        if (Mathf.Abs(RelativeVelocity().y) < 0.1f && hookType != HookType.Wall && hookType != HookType.LargeProjectile)
        {
            if (groundedLastFrame)
            {
                wallJumped = false;
                return true;
            }
            else
            {
                groundedLastFrame = true;
                return false;
            }
        }
        groundedLastFrame = false;
        return false;
    }

    private bool IsSliding()
    {
        List<RaycastHit> raycastHits = Physics.BoxCastAll(rb.position - new Vector3(0, 0.6f, 0), new Vector3(0.55f, 0, 1), Vector3.up, Quaternion.identity, 1.2f).ToList();
        List<RaycastHit> walls = new List<RaycastHit>();

        foreach(RaycastHit hit in raycastHits)
        {
            if (hit.collider.tag != "Player")
                walls.Add(hit);
        }

        if (walls.Count > 0)
        {
            wallJumpTimer = 10;

            if (walls[0].transform.position.x > rb.position.x && Input.GetAxis(Horizontal) > 0)
            {
                wallDirection = 1;
                return true;
            }
            else if(walls[0].transform.position.x < rb.position.x && Input.GetAxis(Horizontal) < 0)
            {
                wallDirection = -1;
                return true;
            }
        }
        wallJumpTimer--;
        return false;
    }

    private bool IsPushing()
    {
        List<RaycastHit> raycastHits = Physics.BoxCastAll(rb.position - new Vector3(0, 0.6f, 0), new Vector3(0.55f, 0, 1), Vector3.up, Quaternion.identity, 1.2f, playerMask).ToList();
        List<RaycastHit> players = new List<RaycastHit>();

        foreach (RaycastHit hit in raycastHits)
        {
            if (hit.collider.gameObject != gameObject)
                players.Add(hit);
        }

        if (players.Count > 0)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].transform.position.x > rb.position.x && Input.GetAxis(Horizontal) > 0)
                    return true;
                else if (players[i].transform.position.x < rb.position.x && Input.GetAxis(Horizontal) < 0)
                    return true;
            }
        }
        return false;
    }

    private bool AirControl(bool isGrounded)
    {
        if (!isGrounded)
        {
            if (jumpDirection > 0)
            {
                if (Input.GetAxis(Horizontal) > 0)
                    return false;
                else
                    return true;
            }
            else
            {
                if (Input.GetAxis(Horizontal) > 0)
                    return true;
                else
                    return false;
            }
        }
        return false;
    }

    public void OnCollisionEnter(Collision collision)
    {
        MovingElement movingElement = collision.gameObject.GetComponent<MovingElement>();
        if(movingElement != null)
        {
            movingPlateform = movingElement;
        }
    }

    private Vector3 RelativeVelocity()
    {
        return rb.velocity - PlateformVelocity();
    }

    private Vector3 PlateformVelocity()
    {
        if (movingPlateform != null)
            return movingPlateform.rb.velocity;
        else
            return Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (Horizontal == null || JumpKey == KeyCode.None)
            return;

        //Check if player is outside the map bondaries
        if (gameIsRunning && topLeftDeath.magnitude != 0 && (transform.position.x < topLeftDeath.x || transform.position.x > bottomRightDeath.x || transform.position.y > topLeftDeath.y || transform.position.y < bottomRightDeath.y))
            Die();

        bool isGrounded = IsGrounded();
        bool isSliding = IsSliding();
        bool isPushing = IsPushing();
        bool airControl = AirControl(isGrounded);

        //Check if we have left the plateform
        if (movingPlateform != null && !isGrounded && !groundedLastFrame && !isSliding)
            movingPlateform = null;

        //Reload dash
        if (isGrounded || isPushing)
            canDash = true;


        if (-1 < velocity && velocity < 1)
            velocity = 0;

        if (velocity > 0)
            velocity -= 0.5f;
        else if (velocity < 0)
            velocity += 0.5f;

        float horizontalVel = Input.GetAxis(Horizontal);

        //Change air velocity after wall jump
        if (wallJumped)
        {
            if (-0.3 < horizontalVel && horizontalVel < 0.3)
                horizontalVel = 0.6f * jumpDirection;
            if (Mathf.Sign(horizontalVel) == Mathf.Sign(horizontalVel))
                horizontalVel /= 2;
        }

        //Handle hook display
        if (hookType == HookType.Wall)
        {
            if(hook != null)
                hook.GetComponent<LineRenderer>().SetPosition(0, rb.position);
        }
        else if (hookType == HookType.SmallProjectile && objectHooked != null && hook != null)
        {
            hook.GetComponent<LineRenderer>().SetPositions(new Vector3[] { rb.position, objectHooked.transform.position });
        }
        else if(hookType == HookType.LargeProjectile)
        {
            if(objectHooked != null)
            {
                if(hook != null)
                    hook.GetComponent<LineRenderer>().SetPositions(new Vector3[] { rb.position, objectHooked.transform.position });

                hookPosition = objectHooked.transform.position;
                GetComponent<SpringJoint>().connectedAnchor = hookPosition;
            }
        }

        //Handle dash after user pressed the button
        if (dashTime > 0)
        {
            rb.velocity = dashVelocity;
            dashVelocity.y -= dashVelocity.y / 10;
            dashTime--;

            if(dashTime == 0)
            {
                //Create projectile
                GameObject proj = Instantiate(smallProjectile, rb.position + new Vector3(rb.velocity.x / 10, rb.velocity.y / 5, 0), Quaternion.identity);
                proj.name = "SmallProjectile";
                proj.GetComponent<Rigidbody>().velocity = new Vector3(dashVelocity.x * sProjSpeed, dashVelocity.y * sProjSpeed, 0);
                proj.GetComponent<Projectile>().sender = this;

                dashVelocity = Vector3.zero;
            }
        }

        //Make player move
        if(!isSliding || (isSliding && Mathf.Sign(wallDirection) != Mathf.Sign(Input.GetAxis(Horizontal))))
        {
            if (dashTime <= 0)
            {
                if (hookType != HookType.Wall && hookType != HookType.LargeProjectile)
                {
                    //Normal movement
                    if(!channeling)
                        rb.AddForce(new Vector3(horizontalVel * (isPushing ? pushSpeed : 1) * (airControl ? airSpeed : speed) - (rb.velocity.x - velocity), 0, 0), ForceMode.Impulse);
                }
                else
                {
                    //Rope swing movement
                    if (rb.position.y < hookPosition.y - hookLength / 2)
                        rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * ropeSwing, 0, 0));
                }
            }
        }

        //Channel energy
        if (channeling)
        {
            if (!Input.GetKey(ChannelKey) && !Input.GetKey(ChannelKey2))
            {
                StartCoroutine(CancelChanneling());
            }
            else
            {
                energy += Time.deltaTime;
                energy = Mathf.Clamp(energy, 0, 3);
            }
        }
        else if (Input.GetKey(ChannelKey) || Input.GetKey(ChannelKey2))
        {
            channeling = true;
        }

        //Cancel player movement and abilities if he is channeling
        if (channeling)
            return;

        //Make user jump
        if (Input.GetKey(JumpKey) && isGrounded)
        {
            jumpDirection = Input.GetAxis(Horizontal);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce + (movingPlateform != null ? movingPlateform.rb.velocity.y : 0) , rb.velocity.z);
        }
        //Make a small jump if user drop the button
        if (Input.GetKeyUp(JumpKey) && !isGrounded && rb.velocity.y > smallJump)
        {
            if(wallJumped && Mathf.Abs(rb.velocity.x) > smallJumpPush)
            {
                velocity = smallJumpPush * jumpDirection;
                rb.velocity = new Vector3(rb.velocity.x - (wallJumpPush - smallJumpPush) * jumpDirection, rb.velocity.y, rb.velocity.z);
            }
            else
                rb.velocity = new Vector3(rb.velocity.x, smallJump, rb.velocity.z);
        }

        //Move with the plateform
        if (movingPlateform != null)
        {
            rb.AddForce(new Vector3(movingPlateform.rb.velocity.x, 0, 0), ForceMode.Impulse);
        }

        //Apply more gravity
        if (rb.velocity.y < 1 && !isSliding && hookType == HookType.None)
            rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);

        //WallSlide
        if (wallJumpTimer > 0)
        {
            //Wall Jump
            if (Input.GetKeyDown(JumpKey) && !isGrounded)
            {
                wallJumped = true;
                jumpDirection = -wallDirection;
                velocity = wallJumpPush * jumpDirection;
                rb.AddForce(new   Vector3(wallJumpPush * jumpDirection, wallJump, 0), ForceMode.Impulse);
            }
            else if(isSliding)
                rb.AddForce(new Vector3(0, Mathf.Abs(rb.velocity.y) + gravity / 2, 0), ForceMode.Acceleration);
        }

        //Hook
        if (hookType != HookType.None)
        {
            if (!Input.GetKey(HookKey) || (objectHooked == null && hookType == HookType.SmallProjectile))
            {
                Destroy(hook);
                SpringJoint spring = GetComponent<SpringJoint>();
                if(spring != null)
                {
                    spring.connectedAnchor = new Vector3(0, 0, 0);
                    spring.connectedBody = null;
                    spring.spring = 0;
                    spring.damper = 0;
                    spring.maxDistance = 0;
                }
                hookType = HookType.None;
                hookPosition = Vector3.zero;
                hookLength = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(HookKey))
            {
                if (hook != null)
                    Destroy(hook);

                Vector3 direction = NormaliseMovement(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));

                RaycastHit hit;
                if (Physics.Raycast(rb.position, direction, out hit, hookRange))
                {
                    if (hit.collider.tag == "Player")
                    {
                        hit.collider.GetComponent<PlayerMovement>().Hooked(direction, rb.position);
                        hookType = HookType.Player;
                    }
                    else if(hit.collider.tag == "SmallProjectile")
                    {
                        hookType = HookType.SmallProjectile;
                        hook = Instantiate(hookObject, rb.position, Quaternion.identity);
                        hook.GetComponent<LineRenderer>().SetPositions(new Vector3[] { rb.position, hit.point });
                        objectHooked = hit.collider.gameObject;

                        SpringJoint spring = GetComponent<SpringJoint>();
                        spring.spring = springForce;
                        spring.damper = damperForce;
                        spring.maxDistance = Vector3.Distance(rb.position, hit.point);
                        spring.connectedBody = hit.collider.GetComponent<Rigidbody>();
                        StartCoroutine(CancelHook());
                    }
                    else if (hit.collider.tag == "LargeProjectile")
                    {
                        hookType = HookType.LargeProjectile;
                        hook = Instantiate(hookObject, rb.position, Quaternion.identity);
                        hook.GetComponent<LineRenderer>().SetPositions(new Vector3[] { rb.position, hit.point });
                        objectHooked = hit.collider.gameObject;

                        SpringJoint spring = GetComponent<SpringJoint>();
                        spring.connectedAnchor = hit.point;
                        spring.spring = springForce;
                        spring.damper = largeDamper;
                        spring.maxDistance = Vector3.Distance(rb.position, hit.point);

                        hookPosition = hit.point;
                        hookLength = Vector3.Distance(rb.position, hit.point);
                    }
                    else
                    {
                        hookType = HookType.Wall;
                        SpringJoint spring = GetComponent<SpringJoint>();
                        spring.connectedAnchor = hit.point;
                        spring.spring = springForce;
                        spring.damper = damperForce;
                        spring.breakForce = Mathf.Infinity;
                        spring.maxDistance = Vector3.Distance(rb.position, hit.point);

                        hook = Instantiate(hookObject, rb.position, Quaternion.identity);
                        hook.GetComponent<LineRenderer>().SetPositions(new Vector3[] { rb.position, hit.point });

                        hookPosition = hit.point;
                        hookLength = Vector3.Distance(rb.position, hit.point);
                    }
                }
                else
                {
                    print("Mised");
                }
            }
        }

        //Dash and small attack
        if (canDash && Input.GetKeyDown(DashKey))
        {
            Vector3 movement = NormaliseMovement(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));

            dashTime = 8;
            dashVelocity = new Vector3(dashSpeed * movement.x, dashSpeed * movement.y, 0);

            rb.velocity = new Vector3(dashSpeed * movement.x, dashSpeed * movement.y, 0);
            velocity = dashSpeed * movement.x;
            jumpDirection = movement.x;
            canDash = false;
        }

        //Ult
        if (Input.GetKeyDown(UltKey))
        {
            Vector3 direction = NormaliseMovement(Input.GetAxis(Horizontal), Input.GetAxis(Vertical));

            if (energy < 1)
            {
                //Boop
            }
            else if(energy < 3)
            {
                //Large projectile
                energy -= 1;

                GameObject proj = Instantiate(largeProjectile, rb.position + direction * 4, Quaternion.identity);
                proj.name = "LargeProjectile";
                proj.GetComponent<Rigidbody>().velocity = new Vector3(direction.x * lProjSpeed, direction.y * lProjSpeed, 0);
                proj.GetComponent<Projectile>().sender = this;
            }
            else if(energy == 3)
            {
                //Swap projectile
                energy -= 3;
                GameObject proj = Instantiate(swapProjectile, rb.position + direction, Quaternion.identity);
                proj.name = "SwapProjectile";
                proj.GetComponent<Rigidbody>().velocity = new Vector3(direction.x * SwapProjSpeed, direction.y * SwapProjSpeed, 0);
                proj.GetComponent<Projectile>().sender = this;
            }
        }
    }

    //Cancel hook on small projectile, if not, the rigidbody crash
    private IEnumerator CancelHook()
    {
        yield return new WaitForSeconds(0.7f);
        Destroy(hook);
        SpringJoint spring = GetComponent<SpringJoint>();
        if (spring != null)
        {
            spring.connectedAnchor = new Vector3(0, 0, 0);
            spring.connectedBody = null;
            spring.spring = 0;
            spring.damper = 0;
            spring.maxDistance = 0;
        }
        hookType = HookType.None;
        hookPosition = Vector3.zero;
        hookLength = 0;
    }

    //Cancel channeling after 0.5 seconds
    private IEnumerator CancelChanneling()
    {
        yield return new WaitForSeconds(0.5f);
        channeling = false;
    }

    Vector3 NormaliseMovement(float x, float y)
    {
        Vector3 input = new Vector3(x, y, 0);
        input.Normalize();

        if (input.x == 0 && input.y == 0)
            input.x = 1;

        return input;
    }

    public void Hooked(Vector3 direction, Vector3 attackPosition)
    {
        rb.velocity = direction * playerHookSpeed;
        velocity = direction.x * playerHookSpeed;
    }

    public void ProjectileHit(PlayerMovement victim)
    {
        canDash = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeathZone")
            Die();
    }

    private void Die()
    {
        gameIsRunning = false;

        if (gameObject.name == "GamePlayer (1)")
            GameObject.Find("GameManager").GetComponent<NetworkManager>().IsDead1 = true;
        if (gameObject.name == "GamePlayer (2)")
            GameObject.Find("GameManager").GetComponent<NetworkManager>().IsDead2 = true;
        if (gameObject.name == "GamePlayer (3)")
            GameObject.Find("GameManager").GetComponent<NetworkManager>().IsDead3 = true;
        if (gameObject.name == "GamePlayer (4)")
            GameObject.Find("GameManager").GetComponent<NetworkManager>().IsDead4 = true;
    }
}
