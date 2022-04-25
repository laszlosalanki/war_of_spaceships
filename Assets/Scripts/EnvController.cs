using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnvController : MonoBehaviour
{
    public GameObject enemy;
    public GameObject player;
    public GameObject[] obstacles;
    public GameObject[] boosters;

    static List<GameObject> createdEnemies;
    static List<GameObject> createdObstacles;
    static List<GameObject> createdBoosters;

    System.Random r;

    Coroutine enemyGenerator;
    Coroutine obstacleGenerator;
    Coroutine boosterGenerator;

    int obstaclesLeft;
    int enemiesLeft;
    int boostersLeft;

    static bool shouldUpdateTexts = false;
    static bool isEnemySlowedDown = false;
    static bool shouldResetEnemySpeed = false;

    // GUI
    public TextMeshProUGUI pointsText;
    public TextMeshProUGUI livesText;
    public TextMeshProUGUI enemiesLeftText;
    public GameObject gameOverPanel;
    public GameObject mainMenuPanel;

    // Rules
    Dictionary<Difficulty, int> enemyNumbers;
    Dictionary<Difficulty, int> obstacleNumbers;
    Dictionary<Difficulty, int> boosterNumbers;
    Dictionary<Difficulty, int> lifeCount;
    Dictionary<Difficulty, int[]> enemyShootInterval;
    Dictionary<Difficulty, int[]> enemySpawnInterval;
    Dictionary<Difficulty, int[]> obstacleSpawnInterval;

    // Background music
    public AudioSource backroundMusic;

    AudioSource startSound;

    // Enum for the available difficulties
    enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    // The chosen difficulty
    static Difficulty actualDifficulty;

    public static bool isGameOn = true;
    static bool isGameWon = false;

    void Start()
    {
        // Initialize
        createdEnemies = new List<GameObject>();
        createdObstacles = new List<GameObject>();
        createdBoosters = new List<GameObject>();
        r = new System.Random();

        var allGameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        startSound = allGameObjects.Where(x => x.CompareTag("StartSound")).First().GetComponent<AudioSource>();

        // Set the number of enemies for each difficulty level
        enemyNumbers = new Dictionary<Difficulty, int>()
        {
            {Difficulty.Easy, 10},
            {Difficulty.Medium, 20},
            {Difficulty.Hard, 30}
        };

        // Set the number of lives for each difficulty level
        lifeCount = new Dictionary<Difficulty, int>()
        {
            {Difficulty.Easy, 3},
            {Difficulty.Medium, 2},
            {Difficulty.Hard, 1}
        };

        // Set the interval of enemy shooting
        enemyShootInterval = new Dictionary<Difficulty, int[]>()
        {
            {Difficulty.Easy, new int[]{3, 6}},
            {Difficulty.Medium, new int[]{2, 5}},
            {Difficulty.Hard, new int[]{1, 4}}
        };

        // Set the number of obstacles for each difficulty level
        obstacleNumbers = new Dictionary<Difficulty, int>()
        {
            {Difficulty.Easy, 4},
            {Difficulty.Medium, 6},
            {Difficulty.Hard, 8}
        };

        // Set the number of boosters for each difficulty level
        boosterNumbers = new Dictionary<Difficulty, int>()
        {
            {Difficulty.Easy, 3},
            {Difficulty.Medium, 2},
            {Difficulty.Hard, 1}
        };

        // Set the interval of enemy creation
        enemySpawnInterval = enemyShootInterval;

        // Set the interval of obstacle creation
        obstacleSpawnInterval = new Dictionary<Difficulty, int[]>() 
        {
            {Difficulty.Easy, new int[]{6, 9}},
            {Difficulty.Medium, new int[]{4, 7}},
            {Difficulty.Hard, new int[]{2, 5}}
        };

        mainMenuPanel.SetActive(true);
        backroundMusic.Play();
    }

    void Update()
    {
        if (!backroundMusic.isPlaying)
            backroundMusic.Play();

        if (!mainMenuPanel.activeInHierarchy)
        {
            // Update the GUI, if necessary
            if (shouldUpdateTexts)
            {
                pointsText.SetText($"Points: {PlayerController.points}");
                livesText.SetText($"Lives: {PlayerController.lives}");

                shouldUpdateTexts = false;
            }

            if (isEnemySlowedDown)
            {
                enemy.GetComponent<MoveForward>().speed = 2.5f;
                var actInstanceOfEnemies = createdEnemies;
                foreach (var e in actInstanceOfEnemies)
                    e.GetComponent<MoveForward>().speed = 2.5f;
                isEnemySlowedDown = false;
            }

            if (shouldResetEnemySpeed)
            {
                enemy.GetComponent<MoveForward>().speed = 4.0f;
                var actInstanceOfEnemies = createdEnemies;
                foreach (var e in actInstanceOfEnemies)
                    e.GetComponent<MoveForward>().speed = 4.0f;
                shouldResetEnemySpeed = false;
            }

            // If the player managed to win the level
            if (PlayerController.lives > 0 && enemiesLeft == 0 && obstaclesLeft == 0 && createdEnemies.Count == 0)
                isGameWon = true;

            if (isGameWon)
                isGameOn = false;

            // Stop the game
            if (!isGameOn)
            {
                StopCoroutine(enemyGenerator);
                StopCoroutine(obstacleGenerator);
                StopCoroutine(boosterGenerator);
                player.GetComponent<PlayerController>().enabled = false;
                EnemyController.StopShooting();

                foreach (var e in createdEnemies)
                {
                    e.GetComponent<MoveForward>().enabled = false;
                }

                foreach (var o in createdObstacles)
                {
                    o.GetComponent<MoveForward>().enabled = false;
                }

                foreach (var b in createdBoosters)
                {
                    b.GetComponent<MoveForward>().enabled = false;
                }

                if (isGameWon)
                {
                    // Navigate to game won Mars scene
                    SceneManager.LoadScene("Mars Landscape 3D Overview");
                }
                else
                {
                    gameOverPanel.SetActive(true);
                }
            }
        }
    }

    // Creates enemies in a random x position within a random time based on the difficulty
    IEnumerator CreateRandomEnemy()
    {
        int rndTime;
        float rndPos;
        while (enemiesLeft > 0)
        {
            rndTime = r.Next(enemySpawnInterval[actualDifficulty][0], enemySpawnInterval[actualDifficulty][1]);
            rndPos = GenerateRandomXPosition();

            var actuallyCreatedObstacles = createdObstacles;
            foreach (var o in actuallyCreatedObstacles)
                if (o.transform.position.x == rndPos)
                    continue;

            yield return new WaitForSeconds(rndTime);

            createdEnemies.Add(Instantiate(enemy, new Vector3(rndPos, 0f, 100f), enemy.transform.rotation));

            enemiesLeftText.SetText($"Enemies left: {enemiesLeft}");

            enemiesLeft--;
        }
    }

    // Creates obstacles in a random x position within a random time based on the difficulty
    IEnumerator CreateRandomObstacle()
    {
        int rndTime;
        float rndPos;
        while (obstaclesLeft > 0)
        {
            rndTime = r.Next(obstacleSpawnInterval[actualDifficulty][0], obstacleSpawnInterval[actualDifficulty][1]);
            rndPos = GenerateRandomXPosition();

            var actuallyCreatedEnemies = createdEnemies;
            foreach (var e in actuallyCreatedEnemies)
                if (e.transform.position.x == rndPos)
                    continue;

            yield return new WaitForSeconds(rndTime);

            int obstacleID = r.Next(0, 2);

            createdObstacles.Add(Instantiate(obstacles[obstacleID], new Vector3(rndPos, 0f, 100f), obstacles[obstacleID].transform.rotation));

            obstaclesLeft--;
        }
    }

    // Creates boosters in a random x position within a random time based on the difficulty
    IEnumerator CreateRandomBooster()
    {
        int rndTime;
        float rndPos;
        while (boostersLeft > 0)
        {
            rndTime = r.Next(5, 16);
            rndPos = GenerateRandomXPosition();

            var actuallyCreatedBoosters = createdBoosters;
            foreach (var b in actuallyCreatedBoosters)
                if (b.transform.position.x == rndPos)
                    continue;

            yield return new WaitForSeconds(rndTime);

            int boosterID = r.Next(0, 3);

            createdBoosters.Add(Instantiate(boosters[boosterID], new Vector3(rndPos, 0f, 100f), boosters[boosterID].transform.rotation));

            boostersLeft--;
        }
    }

    float GenerateRandomXPosition()
    {
        int xShouldBeNegative = r.Next(0, 2);
        return (xShouldBeNegative == 1) ? ((float)r.NextDouble() * 95) * -1f : (float)r.NextDouble() * 95;
    }

    public void TryAgain()
    {
        PlayerController.lives = lifeCount[actualDifficulty];
        PlayerController.points = 0;
        ChangeTextStatus();
        player.GetComponent<PlayerController>().enabled = true;
        EnemyController.shootingInterval = enemyShootInterval[actualDifficulty];
        EnemyController.ContinueShooting();

        foreach (var e in createdEnemies)
        {
            Destroy(e);
        }

        foreach (var o in createdObstacles)
        {
            Destroy(o);
        }

        foreach (var b in createdBoosters)
        {
            Destroy(b);
        }

        createdEnemies.Clear();
        createdObstacles.Clear();
        createdBoosters.Clear();

        gameOverPanel.SetActive(false);

        StartGame();

        isGameOn = true;
        isGameWon = false;
    }

    public void ToMainMenu()
    {
        PlayerController.lives = lifeCount[actualDifficulty];
        PlayerController.points = 0;
        EnemyController.shootingInterval = enemyShootInterval[actualDifficulty];
        EnemyController.ContinueShooting();
        player.GetComponent<PlayerController>().enabled = true;
        ChangeTextStatus();

        foreach (var e in createdEnemies)
        {
            Destroy(e);
        }

        foreach (var o in createdObstacles)
        {
            Destroy(o);
        }

        foreach (var b in createdBoosters)
        {
            Destroy(b);
        }

        createdEnemies.Clear();
        createdObstacles.Clear();
        createdBoosters.Clear();

        gameOverPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    void StartGame()
    {
        enemiesLeft = enemyNumbers[actualDifficulty];
        obstaclesLeft = obstacleNumbers[actualDifficulty];
        boostersLeft = boosterNumbers[actualDifficulty];
        PlayerController.lives = lifeCount[actualDifficulty];
        EnemyController.shootingInterval = enemyShootInterval[actualDifficulty];
        PlayerController.points = 0;
        EnemyController.ContinueShooting();

        player.GetComponent<PlayerController>().enabled = true;

        shouldUpdateTexts = true;
        isGameWon = false;
        isGameOn = true;

        // Start enemy generation
        enemyGenerator = StartCoroutine(CreateRandomEnemy());

        // Start obstacle generation
        obstacleGenerator = StartCoroutine(CreateRandomObstacle());

        // Start booster generation
        boosterGenerator = StartCoroutine(CreateRandomBooster());
    }

    public void Easy()
    {
        startSound.Play();
        actualDifficulty = Difficulty.Easy;
        StartGame();
        mainMenuPanel.SetActive(false);
    }

    public void Medium()
    {
        startSound.Play();
        actualDifficulty = Difficulty.Medium;
        StartGame();
        mainMenuPanel.SetActive(false);
    }

    public void Hard()
    {
        startSound.Play();
        actualDifficulty = Difficulty.Hard;
        StartGame();
        mainMenuPanel.SetActive(false);
    }

    // If called, the GUI texts should be updated in the next frame
    public static void ChangeTextStatus()
    {
        shouldUpdateTexts = true;
    }

    // Function to destroy an automatically created enemy
    public static void DestroyEnemy(GameObject enemy)
    {
        createdEnemies.Remove(enemy);
        Destroy(enemy);
    }

    public static void DestroyObstacle(GameObject obstacle)
    {
        createdObstacles.Remove(obstacle);
        Destroy(obstacle);
    }

    public static void DestroyBooster(GameObject booster)
    {
        createdBoosters.Remove(booster);
        Destroy(booster);
    }

    public void SetDifficulty(int diff)
    {
        actualDifficulty = (Difficulty)diff;
    }

    public int GetDifficulty()
    {
        return (int)actualDifficulty;
    }

    public static void SlowDownEnemy()
    {
        isEnemySlowedDown = true;
    }

    public static void ResetEnemySpeed()
    {
        shouldResetEnemySpeed = true;
    }
}
