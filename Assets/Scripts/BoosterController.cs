using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoosterController : MonoBehaviour
{
    AudioSource extraLifeSound, enemySlowerSound, playerSpeedSound;

    void Start()
    {
        var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        extraLifeSound = allGameObjects.Where(x => x.CompareTag("ExtraLifeBoosterSound")).First().GetComponent<AudioSource>();
        enemySlowerSound = allGameObjects.Where(x => x.CompareTag("EnemySlowerBoosterSound")).First().GetComponent<AudioSource>();
        playerSpeedSound = allGameObjects.Where(x => x.CompareTag("PlayerSpeedBoosterSound")).First().GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (gameObject.CompareTag("SlowSpeedBooster"))
            {
                enemySlowerSound.Play();
                PlayerController.SlowEnemyBooster();
            }
            else if (gameObject.CompareTag("LifeBooster"))
            {
                extraLifeSound.Play();
                PlayerController.LifeBooster();
            }
            else if (gameObject.CompareTag("PlayerSpeedBooster"))
            {
                playerSpeedSound.Play();
                PlayerController.PlayerSpeedBooster();
            }

            EnvController.DestroyBooster(gameObject);
        }
    }
}
