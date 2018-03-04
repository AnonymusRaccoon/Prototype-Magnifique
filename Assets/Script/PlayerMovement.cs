using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 5;
    [SerializeField] private int jumpForce = 8;
    [SerializeField] private int smallJump = 4;
    [SerializeField] private float gravity = 8.5f;

    [Space]
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask wallMask;

    private bool isGrounded = true;
    private bool canWallJump = false;
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
            isGrounded = true;
        else
            isGrounded = false;

        if (Physics.OverlapSphere(rb.position, 1, wallMask).Length > 0)
            canWallJump = true;
        else
            canWallJump = false;


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

        rb.MovePosition(rb.position + new Vector3(Input.GetAxis("Horizontal"), 0, 0) * (airControl ? airSpeed : speed) * Time.fixedDeltaTime);
        airControl = false;


        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            jumpDirection = Input.GetAxis("Horizontal");
            isGrounded = false;
            rb.velocity = new Vector3(0, jumpForce, 0);
        }
        if(Input.GetButtonUp("Jump") && !isGrounded && rb.velocity.y > smallJump)
        {
            rb.velocity = new Vector3(0, smallJump, 0);
        }


        if (rb.velocity.y < 1)
        {
            rb.velocity = new Vector3(0, rb.velocity.y - gravity * Time.fixedDeltaTime, 0);
        }
    }
}
