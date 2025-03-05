using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Reference to the central point around which the player orbits
    public GameObject rotationPlane;
    
    // Controls the orbital movement speed
    public float rotationSpeed = 300.0f;
    
    // Bullet prefab that will be instantiated when firing
    public GameObject bulletPrefab;
    
    // Material for neon connections
    public Material neonYellowMaterial;
    
    // Thickness of connection beams
    public float connectionBeamRadius = 0.03f;
    
    // Number of vertices in the rotation plane (should match NeonGridRenderer.beamCount)
    public int vertexCount = 12;
    
    private AudioSource shootSound;
    
    // Stores horizontal input value for movement
    private Vector2 horizontal;
    
    private GameManager gameManager;
    
    // Reference to the connection cylinders
    private GameObject[] connectionCylinders = new GameObject[2];
    
    // Store last position to detect movement
    private Vector3 lastPosition;
    
    // Cache vertex positions
    private Vector3[] vertexPositions;

    void Start()
    {
        shootSound = GetComponent<AudioSource>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        // Initialize vertex positions cache
        CalculateVertexPositions();
        
        // Initialize connection cylinders
        UpdateConnectionCylinders();
        
        // Store initial position
        lastPosition = transform.position;
    }
    
    void CalculateVertexPositions()
    {
        vertexPositions = new Vector3[vertexCount];
        Vector3 centerPos = rotationPlane.transform.position;
        float radius = 4.5f; // Should match the radius in NeonGridRenderer
        
        for (int i = 0; i < vertexCount; i++)
        {
            float angle = i * (360f / vertexCount);
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            vertexPositions[i] = new Vector3(x, y, 0) + centerPos;
        }
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
            
            // Update connections when player moves
            if (Vector3.Distance(transform.position, lastPosition) > 0.01f)
            {
                UpdateConnectionCylinders();
                lastPosition = transform.position;
            }
        }

        if (Input.GetKeyDown(KeyCode.Q))
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
    
    void UpdateConnectionCylinders()
    {
        // Find the two closest vertices
        int[] closestIndices = FindTwoClosestVertices();
        
        // Destroy existing cylinders
        for (int i = 0; i < connectionCylinders.Length; i++)
        {
            if (connectionCylinders[i] != null)
            {
                Destroy(connectionCylinders[i]);
            }
        }
        
        // Create new cylinders to the closest vertices
        for (int i = 0; i < 2; i++)
        {
            Vector3 vertexPos = vertexPositions[closestIndices[i]];
            connectionCylinders[i] = CreateBeam(transform.position, vertexPos, connectionBeamRadius, neonYellowMaterial);
        }
    }
    
    int[] FindTwoClosestVertices()
    {
        // Initialize arrays to track distances and indices
        float[] distances = new float[vertexCount];
        int[] indices = new int[vertexCount];
        
        // Calculate distances to all vertices
        for (int i = 0; i < vertexCount; i++)
        {
            distances[i] = Vector3.Distance(transform.position, vertexPositions[i]);
            indices[i] = i;
        }
        
        // Simple bubble sort to find the two closest
        for (int i = 0; i < vertexCount - 1; i++)
        {
            for (int j = 0; j < vertexCount - i - 1; j++)
            {
                if (distances[j] > distances[j + 1])
                {
                    // Swap distances
                    float tempDist = distances[j];
                    distances[j] = distances[j + 1];
                    distances[j + 1] = tempDist;
                    
                    // Swap indices
                    int tempIndex = indices[j];
                    indices[j] = indices[j + 1];
                    indices[j + 1] = tempIndex;
                }
            }
        }
        
        // Return the indices of the two closest vertices
        return new int[] { indices[0], indices[1] };
    }
    
    GameObject CreateBeam(Vector3 start, Vector3 end, float thickness, Material material)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        Vector3 center = start + (direction / 2);
        
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.transform.position = center;
        
        // Orient cylinder to point from start to end
        beam.transform.up = direction.normalized;
        
        // Scale cylinder to span the distance
        beam.transform.localScale = new Vector3(thickness * 2, distance / 2, thickness * 2);
        
        // Remove collider to avoid physics interactions
        Destroy(beam.GetComponent<Collider>());
        
        // Apply material
        Renderer renderer = beam.GetComponent<Renderer>();
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        return beam;
    }
}
