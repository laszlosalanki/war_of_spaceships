using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    void Update()
    {
        // Destroy unnecessary bullets
        if (gameObject.transform.position.z > 100)
        {
            PlayerController.DestroyBullet(gameObject);
        }
        else if (gameObject.transform.position.z < -18)
        {
            EnemyController.DestroyBullet(gameObject);
        }
    }
}
