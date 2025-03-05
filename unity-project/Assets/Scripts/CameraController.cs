using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public GameObject player;           // Reference to the player object
    public GameObject rotationPlane;    // Reference to the center plane
    public float tiltAngle = 0.05f;   // Maximum tilt angle in degrees

    void LateUpdate()
    {
        if (player == null || rotationPlane == null) return;

        Vector3 playerPos = player.transform.position;
        Vector3 centerPos = rotationPlane.transform.position;
        
        // Calculate relative position
        float xDiff = playerPos.x - centerPos.x;
        float yDiff = playerPos.y - centerPos.y;
        
        // Calculate tilt angles
        float tiltZ = xDiff * tiltAngle; // Tilt based on x-axis position
        float tiltX = -yDiff * tiltAngle;  // Tilt based on y-axis position
        
        // Apply the tilt (keeping Y rotation at 0)
        transform.rotation = Quaternion.Euler(tiltX, 0, tiltZ);
    }
}
