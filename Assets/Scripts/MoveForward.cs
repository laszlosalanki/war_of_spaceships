using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    public float speed = 15;

    void Start()
    {
        if (gameObject.CompareTag("EnemyBullet"))
        {
            transform.Rotate(new Vector3(0f, 0f, 0f));
        }
    }
    void Update()
    {
        if (gameObject.CompareTag("Bullet"))
        {
            transform.Translate(Vector3.up * Time.deltaTime * speed);
        }
        else if (gameObject.CompareTag("EnemyBullet"))
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else if (gameObject.CompareTag("Obstacle"))
        {
            transform.Translate(Vector3.back * Time.deltaTime * speed);
        }
        else if (gameObject.CompareTag("SlowSpeedBooster"))
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else if (gameObject.CompareTag("LifeBooster"))
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else if (gameObject.CompareTag("PlayerSpeedBooster"))
        {
            transform.Translate(Vector3.down * Time.deltaTime * speed);
        }
        else
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }
    }
}
