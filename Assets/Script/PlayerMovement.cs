using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 5;
    [SerializeField] private int jumpForce = 8;
    [SerializeField] private int smallJump = 4;
    [SerializeField] private float gravity = 8.5f;
    [SerializeField] private int wallJump = 200;
    [SerializeField] private int wallJumpPush = 250;

    [Space]
    [SerializeField] private LayerMask wallMask;

    private bool groundedLastFrame = false;
    private int wallDirection;
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
                return true;
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
            if (walls[0].transform.position.x > rb.position.x && Input.GetAxis("Horizontal") > 0)
            {
                wallDirection = 1;
                return true;
            }
            else if(walls[0].transform.position.x < rb.position.x && Input.GetAxis("Horizontal") < 0)
            {
                wallDirection = -1;
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
                if (Input.GetAxis("Horizontal") > 0)
                    return false;
                else
                    return true;
            }
            else
            {
                if (Input.GetAxis("Horizontal") > 0)
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
        rb.AddForce(new Vector3(Input.GetAxis("Horizontal") * (airControl ? airSpeed : speed) - rb.velocity.x, 0, 0), ForceMode.Impulse);

        //Make user jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpDirection = Input.GetAxis("Horizontal");
            rb.velocity = new Vector3(rb.velocity.x, jumpForce + (movingPlateform != null ? movingPlateform.rb.velocity.y : 0) , rb.velocity.z);
        }
        //Make a small jump if user drop the button
        if (Input.GetButtonUp("Jump") && !isGrounded && rb.velocity.y > smallJump)
        {
            rb.velocity = new Vector3(rb.velocity.x, smallJump, rb.velocity.z);
        }

        //Move with the plateform
        if (movingPlateform != null)
            rb.AddForce(new Vector3(movingPlateform.rb.velocity.x, 0, 0), ForceMode.Impulse);

        //Apply more gravity
        if (rb.velocity.y < 1 && !isSliding)
        {
            rb.AddForce(new Vector3(0, gravity, 0), ForceMode.Acceleration);
        }

        //WallSlide
        if (isSliding)
        {
            rb.AddForce(new Vector3(0, Mathf.Abs(rb.velocity.y) + gravity / 3, 0), ForceMode.Acceleration);

            //Wall Jump
            if (Input.GetButtonDown("Jump"))
            {
                print("Wall jump");
                jumpDirection = -wallDirection;
                rb.AddForce(new Vector3(wallJumpPush * jumpDirection, wallJump, 0), ForceMode.Impulse);
            }
        }

        //////Wall jump
        //if (canWallJump && !isGrounded && Input.GetButtonDown("Jump"))
        //{
        //    print("Wall Jumping");
        //    //canCancel = false;
        //    jumpDirection = wallDirection;
        //    //rb.velocity = new Vector3(wallJumpPush * -wallDirection, wallJump, rb.velocity.z);
        //    rb.AddForce(new Vector3(wallJumpPush * -wallDirection, wallJump, 0), ForceMode.Acceleration);
        //}
    }
}
