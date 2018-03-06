using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private const string Horizontal = "Horizontal";
    [SerializeField] private const string Jump = "Jump";

    [Space]
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 5;
    [SerializeField] private int jumpForce = 8;
    [SerializeField] private int smallJump = 4;
    [SerializeField] private float gravity = 8.5f;
    [SerializeField] private int wallJump = 200;
    [SerializeField] private int wallJumpPush = 250;
    [SerializeField] private float wallJumpSpeed;

    [Space]
    [SerializeField] private LayerMask wallMask;

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
        Collider[] walls = Physics.OverlapSphere(rb.position, 1, wallMask);
        if (walls.Length > 0)
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
        bool isGrounded = IsGrounded();
        bool isSliding = IsSliding();
        bool airControl = AirControl(isGrounded);

        //Check if we have left the plateform
        if (movingPlateform != null && !isGrounded && !groundedLastFrame && !isSliding)
            movingPlateform = null;

        //Move user with horizontal axis input
        if (!isSliding)
        {
            if(!wallJumped)
                rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * (airControl ? airSpeed : speed) - rb.velocity.x, 0, 0), ForceMode.Impulse);
            else
            {
                if(jumpDirection > 0 && Input.GetAxis(Horizontal) > 0)
                {
                    //good direction
                    rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * airSpeed / 2 - rb.velocity.x, 0, 0), ForceMode.Impulse);
                }
                else if(jumpDirection > 0 && Input.GetAxis(Horizontal) < 0)
                {
                    //reverse direction
                    rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * airSpeed - rb.velocity.x, 0, 0), ForceMode.Acceleration);
                }
                else if(jumpDirection < 0 && Input.GetAxis(Horizontal) > 0)
                {
                    //reverse direction
                    rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * airSpeed - rb.velocity.x, 0, 0), ForceMode.Acceleration);
                }
                else if(jumpDirection < 0 && Input.GetAxis(Horizontal) < 0)
                {
                    //good direction
                    rb.AddForce(new Vector3(Input.GetAxis(Horizontal) * airSpeed / 2 - rb.velocity.x, 0, 0), ForceMode.Impulse);
                }
            }
        }

        //Make user jump
        if (Input.GetButtonDown(Jump) && isGrounded)
        {
            jumpDirection = Input.GetAxis(Horizontal);
            rb.velocity = new Vector3(rb.velocity.x, jumpForce + (movingPlateform != null ? movingPlateform.rb.velocity.y : 0) , rb.velocity.z);
        }
        //Make a small jump if user drop the button
        if (Input.GetButtonUp(Jump) && !isGrounded && rb.velocity.y > smallJump && !wallJumped)
            rb.velocity = new Vector3(rb.velocity.x, smallJump, rb.velocity.z);

        //Move with the plateform
        if (movingPlateform != null)
            rb.AddForce(new Vector3(movingPlateform.rb.velocity.x, 0, 0), ForceMode.Impulse);

        //Apply more gravity
        if (rb.velocity.y < 1 && !isSliding)
            rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);

        //WallSlide
        if (wallJumpTimer > 0)
        {
            //Wall Jump
            if (Input.GetButtonDown(Jump) && !isGrounded)
            {
                wallJumped = true;
                jumpDirection = -wallDirection;
                rb.AddForce(new Vector3(wallJumpPush * jumpDirection, wallJump, 0), ForceMode.Impulse);
            }
            else if(isSliding)
                rb.AddForce(new Vector3(0, Mathf.Abs(rb.velocity.y) + gravity / 3, 0), ForceMode.Acceleration);
        }
    }
}
