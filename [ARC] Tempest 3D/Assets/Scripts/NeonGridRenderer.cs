using UnityEngine;
using System.Collections.Generic;

public class NeonGridRenderer : MonoBehaviour
{
    public GameObject rotationPlane;
    public Material yellowNeonMaterial;
    public float beamRadius = 0.05f;  // Reduced radius for thinner beams
    public float beamLength = 200f;
    public int beamCount = 12;
    
    private List<GameObject> beams = new List<GameObject>();
    
    void Start()
    {
        if (rotationPlane == null)
        {
            Debug.LogError("Rotation plane not assigned!");
            return;
        }
        
        if (yellowNeonMaterial == null)
        {
            Debug.LogError("Neon material not assigned!");
            return;
        }
        
        GenerateGrid();
    }
    
    void GenerateGrid()
    {
        // Clean up any existing beams
        foreach (GameObject beam in beams)
        {
            if (beam != null)
                Destroy(beam);
        }
        beams.Clear();
        
        // Get rotation plane center
        Vector3 centerPos = rotationPlane.transform.position;
        float radius = 4.5f; // The radius around the center to place beams
        
        // Create beams in a circle, extending along Z axis
        for (int i = 0; i < beamCount; i++)
        {
            float angle = i * (360f / beamCount);
            float x = radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            float y = radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            
            // Create a Z-axis beam from near to far
            Vector3 nearPoint = new Vector3(x, y, 0) + centerPos;
            Vector3 farPoint = new Vector3(x, y, beamLength) + centerPos;
            
            // Create Z-axis beam - all use yellow material
            CreateBeam(nearPoint, farPoint, beamRadius, yellowNeonMaterial);
        }
    }
    
    void CreateBeam(Vector3 start, Vector3 end, float thickness, Material material)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        Vector3 center = start + (direction / 2);
        
        GameObject beam = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        beam.transform.position = center;
        
        // Orient cylinder along the Z axis
        beam.transform.up = direction.normalized;
        
        // Scale cylinder to span the distance
        beam.transform.localScale = new Vector3(thickness * 2, distance / 2, thickness * 2);
        beam.transform.parent = transform;
        
        // Remove collider to avoid physics interactions
        Destroy(beam.GetComponent<Collider>());
        
        // Apply material
        Renderer renderer = beam.GetComponent<Renderer>();
        renderer.material = material;
        renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        // Add to list for cleanup
        beams.Add(beam);
    }
    
    // Method to regenerate the grid (can be called from inspector or other scripts)
    [ContextMenu("Regenerate Grid")]
    public void RegenerateGrid()
    {
        GenerateGrid();
    }
    
    // Clean up when destroyed
    void OnDestroy()
    {
        foreach (GameObject beam in beams)
        {
            if (beam != null)
                Destroy(beam);
        }
    }
}
