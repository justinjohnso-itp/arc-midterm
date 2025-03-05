using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // Configuration variables as public instance variables
    public float minSpawnDelay = 0.3f;
    public float maxSpawnDelay = 2.0f;
    public float minSpeed = -100f;
    public float maxSpeed = -50f;
    
    // Instance variables
    private float initialSpeed;
    private float currentSpeed;
    private Vector3 targetPosition = Vector3.zero;
    private float spawnZ = 200f;
    private GameManager gameManager;
    
    // Static reference to access the values from other scripts
    private static EnemyController instance;
    
    void Awake()
    {
        // Set up the static instance if this is the first enemy
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        // Initialize speed randomly within our configured range
        initialSpeed = Random.Range(minSpeed, maxSpeed);
        currentSpeed = initialSpeed;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Static helper methods to access instance values
    public static float GetRandomSpawnDelay()
    {
        if (instance != null)
        {
            return Random.Range(instance.minSpawnDelay, instance.maxSpawnDelay);
        }
        return 1f; // Default fallback
    }
    
    // This method allows individual enemies to have different speeds
    public void SetSpeed(float speed)
    {
        initialSpeed = speed;
        currentSpeed = speed;
    }

    void Update()
    {
        // Calculate logarithmic speed reduction based on Z position
        float distanceRatio = Mathf.Abs(transform.position.z) / spawnZ;
        
        // Updated formula to reduce to 20% of initial speed
        float speedMultiplier = 0.2f + 0.8f * Mathf.Log10(1f + 9f * distanceRatio);
        currentSpeed = initialSpeed * speedMultiplier;
        
        // Move along global Z axis with adjusted speed
        transform.position += new Vector3(0, 0, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.5f)
        {
            Destroy(gameObject);
        }
    }
}
