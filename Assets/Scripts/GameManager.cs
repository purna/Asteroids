using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
  [SerializeField] private Asteroid asteroidPrefab;

   [SerializeField] private TMPro.TextMeshProUGUI livesText;
  
  public int asteroidCount = 0;

  private int level = 0;

    // Variables to handle the game state.
  private int startingLives = 3;
  public int livesRemaining;
  public float respawnCheckRadius = 2f; // Radius to check for asteroids

  private bool playing = true;

  

  public GameObject panel;

  
   void Start() {
    livesRemaining = startingLives;
    livesText.text = $"{livesRemaining}";
  }

  private void Update() {
    // If there are no asteroids left, spawn more!
    if (asteroidCount == 0) {
      // Increase the level.
      level++;

      // Spawn the correct number for this level.
      // 1=>4, 2=>6, 3=>8, 4=>10 ...
      int numAsteroids = 2 + (2 * level);
      for (int i = 0; i < numAsteroids; i++) {
        SpawnAsteroid();
      }
    }
  }

  private void SpawnAsteroid() {
    // How far along the edge.
    float offset = Random.Range(0f, 1f);
    Vector2 viewportSpawnPosition = Vector2.zero;

    // Which edge.
    int edge = Random.Range(0, 4);
    if (edge == 0) {
      viewportSpawnPosition = new Vector2(offset, 0);
    } else if (edge == 1) {
      viewportSpawnPosition = new Vector2(offset, 1);
    } else if (edge == 2) {
      viewportSpawnPosition = new Vector2(0, offset);
    } else if (edge == 3) {
      viewportSpawnPosition = new Vector2(1, offset);
    }

    // Create the asteroid.
    Vector2 worldSpawnPosition = Camera.main.ViewportToWorldPoint(viewportSpawnPosition);
    Asteroid asteroid = Instantiate(asteroidPrefab, worldSpawnPosition, Quaternion.identity);
    asteroid.gameManager = this;
  }

  public void GameOver() {
    StartCoroutine(Restart());
  }

  private IEnumerator Restart() {
    Debug.Log("Game Over");

    // Wait a bit before restarting.
    yield return new WaitForSeconds(2f);

    // Restart scene.
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    yield return null;
  }

  // Called from LoseLife whenever it detects a block has fallen off.
  public void RemoveLife() {
    // Update the lives remaining UI element.
    livesRemaining = Mathf.Max(livesRemaining - 1, 0);
    livesText.text = $"{livesRemaining}";
    // Check for end of game.
    if (livesRemaining == 0) {
      playing = false;
      panel.SetActive(true);

    }
  }

  public IEnumerator HandleRespawn(Player player, float respawnTime) {
        // Wait for the specified respawn time
        yield return new WaitForSeconds(respawnTime);

        // Check for nearby asteroids
        while (AreAsteroidsNearby(player.respawnPosition)) {
            Debug.Log("Asteroids nearby, waiting to respawn...");
            yield return new WaitForSeconds(0.5f); // Check again after a brief pause
        }

        // Respawn the player at the specified position
        player.transform.position = player.respawnPosition;

        // Reactivate the player GameObject
        player.gameObject.SetActive(true);
        player.isAlive = true; // Set the alive state to true

        Debug.Log("Player respawned");
    }

    private bool AreAsteroidsNearby(Vector3 position) {
        // Check for colliders within the respawn check radius
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, respawnCheckRadius);
        foreach (Collider2D collider in colliders) {
            if (collider.CompareTag("Asteroid")) {
                return true; // Found an asteroid nearby
            }
        }
        return false; // No asteroids nearby
    }
}
