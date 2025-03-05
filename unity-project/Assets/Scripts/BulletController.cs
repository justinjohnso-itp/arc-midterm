using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed = 300f;
    private GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        Destroy(gameObject, 3f);
    }

    void Update()
    {
        // Move along the global Z axis
        transform.position += Vector3.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            gameManager.AddScore();
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
