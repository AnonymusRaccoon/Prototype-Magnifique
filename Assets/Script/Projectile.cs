using UnityEngine;

public class Projectile : MonoBehaviour
{
    [HideInInspector] public PlayerMovement sender;
    [SerializeField] private float pushForce = 1;
    [SerializeField] private float defaultForce = 10;
    [SerializeField] private bool swapLocation = false;


    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            PlayerMovement pMovement = collision.gameObject.GetComponent<PlayerMovement>();

            pMovement.velocity = rb.velocity.x * pushForce + rb.velocity.normalized.x * defaultForce;
            collision.gameObject.GetComponent<Rigidbody>().velocity = rb.velocity * pushForce + rb.velocity.normalized * defaultForce;

            if(swapLocation)
            {
                Vector3 colPosition = pMovement.transform.position;
                pMovement.transform.position = sender.transform.position;
                sender.transform.position = colPosition;
            }

            sender.ProjectileHit(pMovement);
        }

        Destroy(gameObject);
    }
}
