using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update ()
    {
        rb.AddForce(new Vector3(Input.GetAxis("Horizontal") , 0, 0));
	}
}
