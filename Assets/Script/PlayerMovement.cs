using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private int speed = 10;
    [SerializeField] private int airSpeed = 5;
    [SerializeField] private int jumpForce = 400;

    private float jumpDelay = 0;
    private bool isGrounded = true;
    private bool airControl = false;
    private float jumpDirection;

    private Rigidbody rb;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
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

        rb.MovePosition(rb.position + new Vector3(Input.GetAxis("Horizontal"), 0, 0) * (airControl ?  airSpeed : speed) * Time.deltaTime);

        if(jumpDelay == 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(rb.position, Vector3.down, out hit, 1))
            {
                if(hit.collider.tag != "Player" || hit.collider.tag != "Projectile")
                    isGrounded = true;
            }
        }
        else
        {
            jumpDelay--;
        }


        if (Input.GetAxis("Jump") > 0 && isGrounded)
        {
            print(Input.GetAxis("Jump"));
            jumpDirection = Input.GetAxis("Horizontal");
            isGrounded = false;
            jumpDelay = Mathf.Round(750 * Time.deltaTime);
            rb.AddForce(new Vector3(0, jumpForce * Input.GetAxis("Jump"), 0), ForceMode.Acceleration);
        }

        airControl = false;
	}
}
