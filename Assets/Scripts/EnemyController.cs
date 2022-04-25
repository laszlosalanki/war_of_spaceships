using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour
{
    System.Random r;
    public GameObject bullet;
    static List<GameObject> createdBullets;
    Coroutine randomShooting;
    public static int[] shootingInterval;

    static bool shouldShoot = true;
    AudioSource explosion, shootSound, enemyLeavesScreenSound;

    void Start()
    {
        r = new System.Random();
        createdBullets = new List<GameObject>();
        randomShooting = StartCoroutine(RandomShooting());

        var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        explosion = allGameObjects.Where(x => x.CompareTag("Explosion")).First().GetComponent<AudioSource>();
        shootSound = allGameObjects.Where(x => x.CompareTag("ShootSound")).First().GetComponent<AudioSource>(); ;
        enemyLeavesScreenSound = allGameObjects.Where(x => x.CompareTag("EnemyLeavesScreenSound")).First().GetComponent<AudioSource>(); ;
    }

    void Update()
    {
        // Hit the player, if an enemy left the screen
        if (gameObject.transform.position.z < -18)
        {
            enemyLeavesScreenSound.Play();
            HitPlayer();
        }

        if (!shouldShoot)
            StopCoroutine(randomShooting);
    }

    // Shoot a bullet in a random range based on difficulty
    IEnumerator RandomShooting()
    {
        int rndTime;
        while (gameObject)
        {
            rndTime = r.Next(shootingInterval[0], shootingInterval[1]);
            yield return new WaitForSeconds(rndTime);
            createdBullets.Add(Instantiate(bullet, transform.position + new Vector3(0f, 0f, -3f), bullet.transform.rotation));

            shootSound.Play();
        }
    }

    // Simulates an enemy shot
    void HitPlayer()
    {
        PlayerController.lives--;
        EnvController.ChangeTextStatus();
        EnvController.DestroyEnemy(gameObject);
    }

    // Simulates a player shot
    void HitEnemy(GameObject bullet)
    {
        explosion.Play();
        PlayerController.points++;
        EnvController.ChangeTextStatus();
        PlayerController.DestroyBullet(bullet);
        EnvController.DestroyEnemy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Hit the enemy, if the player shot it
        if (other.gameObject.CompareTag("Bullet"))
        {
            HitEnemy(other.gameObject);
        }
        // Hit the player, if there's a collision between the player and an enemy
        else if (other.gameObject.CompareTag("Player"))
        {
            HitPlayer();
        }
    }

    public static void StopShooting()
    {
        shouldShoot = false;
    }

    public static void ContinueShooting()
    {
        shouldShoot = true;
    }

    // Destroy an enemy bullet
    public static void DestroyBullet(GameObject bullet)
    {
        createdBullets.Remove(bullet);
        Destroy(bullet);
    }
}
