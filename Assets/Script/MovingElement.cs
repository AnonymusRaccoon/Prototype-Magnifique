using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MovingElement : MonoBehaviour
{
    [SerializeField] private Vector3 position1;
    [SerializeField] private Vector3 position2;
    [Space]
    [SerializeField] private float speed = 1;

    private Vector3 middlePosition;
    private float intensityX;
    private float intensityY;

    [HideInInspector] public Rigidbody rb;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = true;

        middlePosition = (position1 + position2) / 2;
        rb.position = middlePosition;

        intensityX = position1.x - rb.position.x;
        intensityY = position1.y - rb.position.y;
    }

    private void FixedUpdate ()
    {
        rb.MovePosition(new Vector3(middlePosition.x + Mathf.Sin(Time.time * speed) * intensityX, middlePosition.y + Mathf.Sin(Time.time * speed) * intensityY, rb.position.z));
    }
}
