using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ObstacleController : MonoBehaviour
{
    AudioSource shootObstacleSound, collisionObstacleSound;

    void Start()
    {

        var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        shootObstacleSound = allGameObjects.Where(x => x.CompareTag("PlayerShootsObstacle")).First().GetComponent<AudioSource>();
        collisionObstacleSound = allGameObjects.Where(x => x.CompareTag("ObstacleCollisionSound")).First().GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            collisionObstacleSound.Play();
            PlayerController.lives--;
            EnvController.ChangeTextStatus();
            EnvController.DestroyObstacle(gameObject);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            shootObstacleSound.Play();
            PlayerController.DestroyBullet(other.gameObject);
        }
    }
}
