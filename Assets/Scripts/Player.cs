using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {
  [Header("Ship parameters")]
  [SerializeField] private float shipAcceleration = 10f;
  [SerializeField] private float shipMaxVelocity = 10f;
  [SerializeField] private float shipRotationSpeed = 180f;
  [SerializeField] private float bulletSpeed = 8f;

  [Header("Object references")]
  [SerializeField] private Transform bulletSpawn;
  [SerializeField] private Rigidbody2D bulletPrefab;
  [SerializeField] private ParticleSystem destroyedParticles;

  //[SerializeField] public GameManager gameManager;

  private Rigidbody2D shipRigidbody;
  public bool isAlive = true;
  private bool isAccelerating = false;



  public GameObject objectToRespawn; // The object to respawn
    public Vector3 respawnPosition; // The position to respawn the object
    public float respawnTime = 3f; // Time to wait before respawning

  private void Start() {
    // Get a reference to the attached RigidBody2D.
    shipRigidbody = GetComponent<Rigidbody2D>();
  }

  private void Update() {
    if (isAlive) {
      HandleShipAcceleration();
      HandleShipRotation();
      HandleShooting();
    }
  }

  private void FixedUpdate() {
    if (isAlive && isAccelerating) {
      // Increase velocity upto a maximum.
      shipRigidbody.AddForce(shipAcceleration * transform.up);
      shipRigidbody.linearVelocity = Vector2.ClampMagnitude(shipRigidbody.linearVelocity, shipMaxVelocity);
    }
  }

  private void HandleShipAcceleration() {
    // Are we accelerating?
    isAccelerating = Input.GetKey(KeyCode.UpArrow);
  }

  private void HandleShipRotation() {
    // Ship rotation.
    if (Input.GetKey(KeyCode.LeftArrow)) {
      transform.Rotate(shipRotationSpeed * Time.deltaTime * transform.forward);
    } else if (Input.GetKey(KeyCode.RightArrow)) {
      transform.Rotate(-shipRotationSpeed * Time.deltaTime * transform.forward);
    }
  }

  private void HandleShooting() {
    // Shooting.
    if (Input.GetKeyDown(KeyCode.Space)) {

      Rigidbody2D bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

      // Inherit velicity only in the forward direction of ship.
      Vector2 shipVelocity = shipRigidbody.linearVelocity;
      Vector2 shipDirection = transform.up;
      float shipForwardSpeed = Vector2.Dot(shipVelocity, shipDirection);

      // Don't want to inherit in the opposite direction, else we'll get stationary bullets.
      if (shipForwardSpeed < 0) { 
        shipForwardSpeed = 0; 
      }

      bullet.linearVelocity = shipDirection * shipForwardSpeed;

      // Add force to propel bullet in direction the player is facing.
      bullet.AddForce(bulletSpeed * transform.up, ForceMode2D.Impulse);
    }
  }

private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.CompareTag("Asteroid")) {
            HandlePlayerHit();
        }
    }

    private void HandlePlayerHit() {
        isAlive = false;

        // Show the destroyed effect.
        Instantiate(destroyedParticles, transform.position, Quaternion.identity);

        // Deactivate the player GameObject
        gameObject.SetActive(false);

        // Get a reference to the GameManager
        GameManager gameManager = FindObjectOfType<GameManager>();
        gameManager.RemoveLife(); // Remove a life

        // Check if player is out of lives
        if (gameManager.livesRemaining == 0) {
            gameManager.GameOver();
        } else {
            // Start the respawn coroutine from the GameManager
            gameManager.StartCoroutine(gameManager.HandleRespawn(this, respawnTime));
        }
    }
  }

