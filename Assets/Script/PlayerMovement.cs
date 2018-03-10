﻿using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public string Horizontal;
    public string Vertical;
    public KeyCode JumpKey = KeyCode.None;
    public KeyCode HookKey = KeyCode.None;
    public KeyCode DashKey = KeyCode.None;
    public KeyCode UltKey = KeyCode.None;

    [Space]
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 7;
    [SerializeField] private int jumpForce = 8;
    [SerializeField] private int smallJump = 4;
    [SerializeField] private float gravity = -10;
    [SerializeField] private int wallJump = 4;
    [SerializeField] private int wallJumpPush = 10;
    [SerializeField] private int smallJumpPush = 5;
    [SerializeField] private float pushSpeed = 0.2f;
    [SerializeField] private float dashSpeed = 15;

    [Space]
    [SerializeField] private GameObject smallProjectile;


    [Space]
    [Space]
    [SerializeField] private LayerMask playerMask;

    [Space]
    public bool setuped = false;

    private float velocity = 0;

    private bool groundedLastFrame = false;
    private int wallDirection;
    private int wallJumpTimer = 0;
    private bool wallJumped;
    private float jumpDirection;

    private MovingElement movingPlateform;

    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private bool IsGrounded()
    {
        if (Mathf.Abs(RelativeVelocity().y) < 0.1f)
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
        CapsuleCollider col = GetComponent<CapsuleCollider>();

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
        if (JumpKey == KeyCode.None || Horizontal == null)
            return;

        bool isGrounded = IsGrounded();
        bool isSliding = IsSliding();
        bool isPushing = IsPushing();
        bool airControl = AirControl(isGrounded);

        //Check if we have left the plateform
        if (movingPlateform != null && !isGrounded && !groundedLastFrame && !isSliding)
            movingPlateform = null;


        if (-1 < velocity && velocity < 1)
            velocity = 0;

        if (velocity > 0)
            velocity -= 0.5f;
        else if (velocity < 0)
            velocity += 0.5f;

        float horizontalVel = Input.GetAxis(Horizontal);

        if (wallJumped)
        {
            if (-0.3 < horizontalVel && horizontalVel < 0.3)
                horizontalVel = 0.6f * jumpDirection;
            if (Mathf.Sign(horizontalVel) == Mathf.Sign(horizontalVel))
                horizontalVel /= 2;
        }

        if(!isSliding || (isSliding && Mathf.Sign(wallDirection) != Mathf.Sign(Input.GetAxis(Horizontal))))
            rb.AddForce(new Vector3(horizontalVel * (isPushing ? pushSpeed : 1) * (airControl ? airSpeed : speed) - (rb.velocity.x - velocity), 0, 0), ForceMode.Impulse);

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
        if (rb.velocity.y < 1 && !isSliding)
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
                rb.AddForce(new Vector3(wallJumpPush * jumpDirection, wallJump, 0), ForceMode.Impulse);
            }
            else if(isSliding)
                rb.AddForce(new Vector3(0, Mathf.Abs(rb.velocity.y) + gravity / 2, 0), ForceMode.Acceleration);
        }

        //Dash and small attack
        if (Input.GetKeyDown(DashKey))
        {
            horizontalVel = Input.GetAxis(Horizontal);
            float verticalVel = Input.GetAxis(Vertical);

            //if(horizontalVel ) //if horizontal and vertical vel are null, set horizontal to 0.80

            rb.velocity = new Vector3(dashSpeed * horizontalVel, dashSpeed * verticalVel, 0);
            velocity = dashSpeed * Input.GetAxis(Horizontal);
            //Instantiate(smallProjectile, rb.position)
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "DeathZone")
            Die();
    }

    private void Die()
    {
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
