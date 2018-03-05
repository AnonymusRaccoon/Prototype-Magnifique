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
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private bool isGrounded = true;
    private bool canCancel = false;
    private bool canWallJump = false;
    private int wallDirection;
    private float jumpDirection;
    private bool airControl = false;

    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (Physics.OverlapSphere(rb.position, 1.5f, groundMask).Length > 0)
        {
            isGrounded = true;
            canCancel = false;
        }
        else
            isGrounded = false;

        Collider[] walls = Physics.OverlapSphere(rb.position, 1, wallMask);
        if (walls.Length > 0)
        {
            canWallJump = true;
            if(walls[0].transform.position.x > rb.position.x)
                wallDirection = -1;
            else
                wallDirection = 1;
        }
        else
        {
            canWallJump = false;
        }


        if (!isGrounded)
        {
            if (jumpDirection > 0)
            {
                if (Input.GetAxis("Horizontal") > 0)
                    airControl = false;
                else
                    airControl = true;
            }
            else
            {
                if (Input.GetAxis("Horizontal") > 0)
                    airControl = true;
                else
                    airControl = false;
            }
        }

        rb.MovePosition(rb.position + new Vector3(Input.GetAxis("Horizontal") * (airControl ? airSpeed : speed) * Time.fixedDeltaTime - rb.velocity.x, 0, 0));
        airControl = false;


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpDirection = Input.GetAxis("Horizontal");
            isGrounded = false;
            rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
            canCancel = true;
        }
        if (Input.GetButtonUp("Jump") && !isGrounded && rb.velocity.y > smallJump && canCancel)
        {
            canCancel = false;
            rb.velocity = new Vector3(rb.velocity.x, smallJump, rb.velocity.z);
        }


        if (rb.velocity.y < 1)
        {
            rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - gravity * Time.fixedDeltaTime, rb.velocity.z);
        }


        if (canWallJump && !isGrounded && Input.GetButtonDown("Jump"))
        {
            print("Wall Jumping");
            canCancel = false;
            jumpDirection = wallDirection;
            rb.velocity = new Vector3(wallJumpPush * -wallDirection, wallJump, rb.velocity.z);
            //rb.AddForce(new Vector3(wallJumpPush * -wallDirection, wallJump, 0), ForceMode.Acceleration);
        }
    }
}
