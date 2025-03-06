using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScript : MonoBehaviour
{
    private int highScore;
    public TMP_Text highScoreText;
    public TMP_Text instructionsText;
    
    void Start()
    {
        // Load high score from PlayerPrefs
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
        
        // Reset lives when starting a new game
        GameManager.ResetLives();
    }

    void Update()
    {
        // Any key press starts the game
        if (Input.anyKeyDown)
        {
            // Load the main game scene
            SceneManager.LoadScene("TempestScene");
        }
    }
}
