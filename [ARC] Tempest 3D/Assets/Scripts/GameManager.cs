using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public GameObject rotationPlane;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text gameOverText;
    public GameObject life;
    
    public int score = 0;
    private static int lives = 3; // Made static to access from TitleScript
    private int highScore;
    private float spawnZ = 200f;
    private GameObject playerInstance;
    private bool isGameOver = false;
    private float restartDelay = 3f; // Time to wait before returning to title screen

    void Start()
    {
        // Load high score at start
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Hide game over text initially
        if (highScoreText != null)
            highScoreText.gameObject.SetActive(false);
        
        // Hide game over text initially    
        if (gameOverText != null)
            gameOverText.gameObject.SetActive(false);
            
        // Spawn player at starting position with no rotation
        playerInstance = Instantiate(player, new Vector3(0, -4, -1), Quaternion.identity);
        
        // Start continuous enemy spawning
        StartCoroutine(SpawnEnemiesContinuously());
        
        // Create life indicators
        SpawnLifeIndicators();
        
        UpdateScoreDisplay();
    }

    // Static method to reset lives from TitleScript
    public static void ResetLives()
    {
        lives = 3;
    }

    // Separate method to spawn life indicators
    void SpawnLifeIndicators()
    {
        // Clear any existing life indicators first
        GameObject[] existingLives = GameObject.FindGameObjectsWithTag("Life");
        foreach (GameObject lifeObj in existingLives)
        {
            Destroy(lifeObj);
        }
        
        // Spawn new life indicators
        for (int i = 0; i < lives; i++)
        {
            Instantiate(life, new Vector3(i+6, 4, -1), Quaternion.identity).tag = "Life";
        }
    }

    IEnumerator SpawnEnemiesContinuously()
    {
        while (true)
        {
            SpawnSingleEnemy();
            // Get spawn delay from EnemyController
            yield return new WaitForSeconds(EnemyController.GetRandomSpawnDelay());
        }
    }

    void SpawnSingleEnemy()
    {
        // Calculate random angle around the circle
        float angle = Random.Range(0f, 360f);
        
        // Get center position from rotation plane
        Vector3 centerPos = rotationPlane.transform.position;
        
        // Calculate spawn position around the center using fixed radius
        Vector3 spawnPosLocal = new Vector3(
            4 * Mathf.Cos(angle * Mathf.Deg2Rad),
            4 * Mathf.Sin(angle * Mathf.Deg2Rad),
            spawnZ
        );
        
        // Calculate direction vector pointing from the enemy toward the center
        Vector3 directionToCenter = (centerPos - spawnPosLocal).normalized;
        
        // Create a rotation so the top of the enemy model faces the center
        // This uses LookRotation with the desired forward direction and up direction
        Quaternion rotationToCenter = Quaternion.LookRotation(Vector3.forward, directionToCenter);
        
        // Apply 180 degree Y rotation to ensure proper enemy facing direction
        Quaternion finalRotation = rotationToCenter * Quaternion.Euler(0, 180, 0);
        
        // Create the enemy - speed is determined within EnemyController
        GameObject newEnemy = Instantiate(enemy, spawnPosLocal, finalRotation);
    }

    public void AddScore()
    {
        score += 1;
        UpdateScoreDisplay();
        
        // Update high score if current score is higher
        if (score > highScore)
        {
            highScore = score;
            // Save high score immediately
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "SCORE: " + score;
        }
    }

    public void PlayerHit()
    {
        // Prevent hit processing if game over
        if (isGameOver) return;
        
        Debug.Log("Player hit: Lives remaining = " + lives);
        lives--;
        
        // Remove life indicator UI
        GameObject[] lifeIcons = GameObject.FindGameObjectsWithTag("Life");
        if (lifeIcons.Length > 0)
        {
            Destroy(lifeIcons[0]);
        }
        
        // Check for game over
        if (lives <= 0)
        {
            isGameOver = true;
            GameOver();
        }
    }
    
    void GameOver()
    {
        Debug.Log("Game Over triggered!");
        // Stop spawning enemies
        StopAllCoroutines();
        
        // Display high score message
        if (highScoreText != null)
        {
            highScoreText.gameObject.SetActive(true);
            highScoreText.text = "SCORE: " + score + "\nHIGH SCORE: " + highScore;
        }
        
        // Display game over message
        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(true);
            gameOverText.text = "GAME OVER";
        }
        
        // Update high score in PlayerPrefs
        if (score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
            PlayerPrefs.Save();
        }
        
        // Destroy the player
        if (playerInstance != null)
        {
            Destroy(playerInstance);
        }
        
        // Destroy all enemies
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
        
        Debug.Log("Returning to title screen in " + restartDelay + " seconds");
        // Return to title screen after delay
        StartCoroutine(ReturnToTitleAfterDelay());
    }
    
    IEnumerator ReturnToTitleAfterDelay()
    {
        yield return new WaitForSeconds(restartDelay);
        Debug.Log("Loading TitleScene");
        // Make sure the scene is in the build settings
        SceneManager.LoadScene("TitleScene");
    }
}