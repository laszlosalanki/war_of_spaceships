using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 15;
    public GameObject bullet;
    static List<GameObject> createdBullets;

    public static int points = 0;
    public static int lives = 0;

    static bool isSlowBoosterOn = false;
    static bool isPlayerSpeedBoosterOn = false;

    public AudioSource shoot, beingShotSound;

    void Start()
    {
        // Init
        createdBullets = new List<GameObject>();
        shoot.playOnAwake = false;


        var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        beingShotSound = allGameObjects.Where(x => x.CompareTag("BeingShotByEnemy")).First().GetComponent<AudioSource>();
    }

    void Update()
    {
        // User interactions
        if (Input.GetKey(KeyCode.A) && transform.position.x > -95)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }
        else if (Input.GetKey(KeyCode.D) && transform.position.x < 95)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            createdBullets.Add(Instantiate(bullet, transform.position, bullet.transform.rotation));
            shoot.Play();
        }

        // Game over, if no lives left
        if (lives == 0)
        {
            EnvController.isGameOn = false;
        }

        if (isSlowBoosterOn)
        {
            StartCoroutine(SlowEnemyBoosterProcess());
        }
        if (isPlayerSpeedBoosterOn)
        {
            StartCoroutine(PlayerSpeedBoosterProcess());
        }
    }

    // Destroy the player's bullet
    public static void DestroyBullet(GameObject bullet)
    {
        createdBullets.Remove(bullet);
        Destroy(bullet);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Being shot by an enemy
        if (other.gameObject.CompareTag("EnemyBullet"))
        {
            beingShotSound.Play();
            lives--;
            EnvController.ChangeTextStatus();
            EnemyController.DestroyBullet(other.gameObject);
        }
    }

    public static void SlowEnemyBooster() // gem_2
    {
        if (!isSlowBoosterOn)
            isSlowBoosterOn = true;
    }

    public static void PlayerSpeedBooster() // gem_13
    {
        if (!isPlayerSpeedBoosterOn)
            isPlayerSpeedBoosterOn = true;
    }

    public static void LifeBooster() // gem_9
    {
        lives++;
        EnvController.ChangeTextStatus();
    }

    IEnumerator SlowEnemyBoosterProcess()
    {
        EnvController.SlowDownEnemy();
        yield return new WaitForSeconds(5);
        EnvController.ResetEnemySpeed();
        isSlowBoosterOn = false;
    }

    IEnumerator PlayerSpeedBoosterProcess()
    {
        speed = 80;
        yield return new WaitForSeconds(5);
        speed = 55;
        isPlayerSpeedBoosterOn = false;
    }
}
