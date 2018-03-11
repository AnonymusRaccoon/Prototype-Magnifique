using UnityEngine;

public class SmallProjectile : MonoBehaviour
{
    [HideInInspector] public PlayerMovement sender;
    [SerializeField] private float pushForce = 1;
    
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Rigidbody rb = GetComponent<Rigidbody>();
            PlayerMovement pMovement = collision.gameObject.GetComponent<PlayerMovement>();

            pMovement.velocity = rb.velocity.x * pushForce;
            collision.gameObject.GetComponent<Rigidbody>().velocity = rb.velocity * pushForce;

            sender.SmallProjectileHit(pMovement);
        }

        Destroy(gameObject);
    }
}
