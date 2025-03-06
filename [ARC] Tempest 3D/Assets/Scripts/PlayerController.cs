using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Reference to the central point around which the player orbits
    public GameObject rotationPlane;
    
    // Controls the orbital movement speed
    public float rotationSpeed = 300.0f;
    
    // Bullet prefab that will be instantiated when firing
    public GameObject bulletPrefab;
    
    private AudioSource shootSound;
    
    // Stores horizontal input value for movement
    private Vector2 horizontal;
    
    private GameManager gameManager;

    void Start()
    {
        shootSound = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        // Get input for horizontal movement (-1 to 1)
        horizontal.x = Input.GetAxisRaw("Horizontal");
        
        if (horizontal.x != 0f)
        {
            // Orbit the player around the central point using RotateAround
            Vector3 planePosition = rotationPlane.transform.position;
            transform.RotateAround(planePosition, Vector3.forward, rotationSpeed * horizontal.x * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBullet();
        }
    }

    void FireBullet()
    {
        // Calculate direction from center to player to determine bullet trajectory
        // This ensures bullets fire outward from the orbital circle
        Vector3 directionFromCenter = (transform.position - rotationPlane.transform.position).normalized;
        
        // Create rotation for the bullet to face away from the center
        Quaternion bulletRotation = Quaternion.LookRotation(directionFromCenter);
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, bulletRotation);
        
        if (shootSound != null) shootSound.Play();
    }

    // Called when another collider enters this object's trigger collider
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameManager.PlayerHit();
            Destroy(other.gameObject);
        }
    }
}
