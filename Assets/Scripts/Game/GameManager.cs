using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    private bool isScrolling, gonnaScrolling, gameOver;
    private int enemyCount,
    score, timeScore, pickUpBonusScore, barrelBonusScore, coverBonusScore, barrelEarn, coverEarn,
    mapLevelUpScore, mapVoidDownScore, mapLevelUpCount = 0, mapVoidDownCount = 0,
    level = 4;
    private int [] mapInfo;
    private int[,] blocsLevel;
    public static readonly int mapSize = 13, mapOffset = 48, unitBySecond = 8, levelMax = -1, tutorialMaxLevel = 6;// levelMax = -1 pour infini.
    private PlayerController playerScript;
    private TextMeshProUGUI scoreText, gameOverText, countdownText;
    public Button restartButton, quitButton;
    private GameObject _pausePanel, _settings;
    public GameObject[] prefab;// [0]: Cover [1]: Enemy [2]: Life [3]: Rapid Fire [4]: Grenade [5]: Barrel [6]: Score [7]: Tutorial Text
    //public GameObject[] paths;

    public delegate void Scrolling();
    public static event Scrolling OnScrolling;

    void OnEnable()
    {
        PlayerController.OnGameOver += GameOver;
        PlayerController.PauseOn += Pause;
        if(SceneManager.GetActiveScene().name == "Game")
        {
            level = tutorialMaxLevel;
        }
    }

    void OnDisable()
    {
        PlayerController.OnGameOver -= GameOver;
        PlayerController.PauseOn -= Pause;
    }

    void GameOver()
    {
        gameOver = true;
        isScrolling = false;
        gameOverText.gameObject.SetActive(true);
        restartButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    void DesactivatePauseMenu()
    {
        _pausePanel.SetActive(false);
        _settings.SetActive(false);
    }

    void Pause()
    {
        if(Time.timeScale == 1)
        {
            Time.timeScale = 0;
            _pausePanel.SetActive(true);
            _settings.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            DesactivatePauseMenu();
        }
    }

    void Start()
    {
        DataManager.Instance.LoadSettings();

        _pausePanel = GameObject.Find("Pause Panel");
        _settings = GameObject.Find("Settings");
        DesactivatePauseMenu();

        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        scoreText = GameObject.Find("Score Text").GetComponent<TextMeshProUGUI>();
        scoreText.SetText("Score " + score);

        gameOverText = GameObject.Find("Game Over Text").GetComponent<TextMeshProUGUI>();
        gameOverText.gameObject.SetActive(false);

        countdownText = GameObject.Find("Countdown Text").GetComponent<TextMeshProUGUI>();
        countdownText.gameObject.SetActive(false);

        restartButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);

        blocsLevel = new int[,]
        {
            {1, 4, 4, 4},
            {0, 4, 4, 4},
            {0, 4, 4, 4},
            {0, 4, 4, 4},
            {0, 4, 4, 4},
            {0, 4, 4, 4}
        };

        mapInfo = new int[] {0, 0};
    }

    void Update()
    {
        enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount == 0 && level != levelMax && !gameOver)
        {
            if(SceneManager.GetActiveScene().name == "Tutorial" & level == tutorialMaxLevel)
            {
                SceneManager.LoadScene("Title Menu");
            }
            else
            {
                CountdownAction(2);
                SpawnMap(Maps());
                StopAllCoroutines();
                StartCoroutine(ScrollMap());
                level++;
            }
        }
    }

    void SpawnMap(int[,] map)
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if(map[i, j] < prefab.Length)
                {
                    SpawnThings(i, j, map[i, j]);
                }
            }
        }
    }

    GameObject SpawnThings(int i, int j, int prefabNum)
    {
        Vector3 offset;

        offset = prefabNum == 7 | (prefabNum > 1 & prefabNum < 5) ? Vector3.up*3 : Vector3.up * 0.4f;

        if (prefabNum < 6)
        {
            GameObject pooledObject = ObjectPooler.SharedInstance.GetPooledObject(prefabNum);
            if (pooledObject != null)
            {
                FillMapInfo(prefabNum);
                pooledObject.SetActive(true);
                pooledObject.transform.position = CoordinatesToWorldPosition(i, j) + offset;
                pooledObject.transform.rotation = ObjectPooler.SharedInstance.objectToPool[prefabNum].transform.rotation;
                if(i > 9)
                {
                    pooledObject.transform.rotation *= Quaternion.Euler(0, 180, 0);
                }
            }
            return pooledObject;
        }
        else
        {
            return Instantiate(prefab[prefabNum], CoordinatesToWorldPosition(i, j) + offset, prefab[prefabNum].transform.rotation);
        }
    }

    private void FillMapInfo(int prefabNum)
    {
        switch(prefabNum)
        {
            case 0:
                mapInfo[prefabNum]++;
                break;
            case 1:
                mapInfo[prefabNum]++;
                break;
        }
    }

    private Vector3 CoordinatesToWorldPosition(int i, int j)
    {
        return new Vector3
        (
            ((j * 2) - (mapSize - 1)),// x
            1,// y
            ((((mapSize - 1) * 2) + playerScript.transform.position.z + mapOffset + 2) - i * 2)// z
        );
    }

    public void UpdateScoreText(int points, Vector3 position)
    {
        int drawX, drawY, neededScore;
        Vector3 spawnPosition;

        neededScore = (((mapInfo[0] / 2) * 10) + (mapInfo[1] * 100));

        spawnPosition = position + (Vector3.up * 3);
        SpawnScore(spawnPosition, "" + points, Color.white);

        score += points;

        scoreText.SetText("Score " + score);

        if(level > tutorialMaxLevel)
        {
            pickUpBonusScore += points;

            if (pickUpBonusScore >= neededScore)
            {
                pickUpBonusScore = 0;
                SpawnBonus(position);
            }

            if(coverEarn < 3)
            {
                coverBonusScore += points;

                if (coverBonusScore >= ((neededScore * 2) / 3))
                {
                    coverBonusScore = 0;
                    coverEarn++;
                    spawnPosition = Vector3.forward * (playerScript.transform.position.z + 2 * coverEarn) + Vector3.up * 6;
                    SpawnScore(spawnPosition, "Cover " + coverEarn, Color.yellow);
                }
            }
            if(barrelEarn < 9)
            {
                barrelBonusScore += points;

                if(barrelBonusScore >= (neededScore / 2))
                {
                    barrelBonusScore = 0;
                    barrelEarn++;
                    spawnPosition = position + Vector3.up * 5;
                    SpawnScore(spawnPosition, "Red Barrel", Color.red);
                }
            }

            if (mapLevelUpCount < 17)
            {
                mapLevelUpScore += points;

                if (mapLevelUpScore >= 500 * mapLevelUpCount)
                {
                    mapLevelUpScore = 0;

                    do
                    {
                        drawX = Random.Range(0, 6);
                    }
                    while (blocsLevel[drawX, 0] > 2);

                    blocsLevel[drawX, 0]++;

                    mapLevelUpCount++;
                }
            }
            if (mapVoidDownCount < 72)
            {
                mapVoidDownScore += points;

                if (mapVoidDownScore >= 125 * mapVoidDownCount)
                {
                    mapVoidDownScore = 0;

                    do
                    {
                        drawX = Random.Range(0, 6);
                        drawY = Random.Range(1, 4);
                    }
                    while (blocsLevel[drawX, drawY] < 1);

                    blocsLevel[drawX, drawY]--;

                    mapVoidDownCount++;
                }
            }
        }
    }
    
    public void SpawnBonus(Vector3 position)
    {
        int i, j, index;
        Vector3 spawnPosition;

        index = BonusDraw();
        i = Random.Range(2, 5);
        j = Random.Range(4, 9);

        if(gonnaScrolling)
        {
            SpawnThings(i + (mapOffset / 3), j, index);
        }
        else
        {
            SpawnThings(i + (mapOffset / 2), j, index);
        }

        spawnPosition = position + Vector3.up * 4;
        if(index == 2)
        {
            SpawnScore(spawnPosition, "Life", Color.yellow);
        }
        else if(index == 3)
        {
            SpawnScore(spawnPosition, "Rapid Fire", Color.yellow);
        }
        else if(index == 4)
        {
            SpawnScore(spawnPosition, "Grenade", Color.yellow);
        }
    }

    private GameObject SpawnScore(Vector3 spawnPosition, string text, Color textColor)
    {
        GameObject pooledObject = ObjectPooler.SharedInstance.GetPooledObject(6);
        TextMeshPro ScoreText;
            if (pooledObject != null)
            {
                pooledObject.SetActive(true);
                pooledObject.transform.position = spawnPosition;
                pooledObject.transform.rotation = ObjectPooler.SharedInstance.objectToPool[6].transform.rotation;
                ScoreText = pooledObject.GetComponent<TextMeshPro>();
                ScoreText.SetText(text);
                ScoreText.color = textColor;
            }
            return pooledObject;
    }

        private IEnumerator ScrollMap()
    {
        isScrolling = true;
        playerScript.StandWalkRun(true);
        CountdownAction(0);

        OnScrolling();

        yield return new WaitForSeconds(mapOffset/unitBySecond);

        isScrolling = gonnaScrolling = false;
        playerScript.StandWalkRun(false);
        CountdownAction(1);

        StartCoroutine(CountdownRoutine());
    }
    private IEnumerator CountdownRoutine()
    {
        int minute;
        int second;
        //int hundredth;

        timeScore = (mapInfo[0] / 20) + (mapInfo[1] * 2); // en seconde
        minute = timeScore / 60;
        second = timeScore - minute * 60;
        //hundredth = 0;

        while(timeScore >= 0 & countdownText.gameObject.activeInHierarchy)
        {

            if(minute < 10)
            {
                countdownText.SetText("0" + minute + "'");
            }
            else
            {
                countdownText.SetText(minute + "'");
            }
            if(second < 10)
            {
                countdownText.SetText(countdownText.text + "0" + second + "\"");
            }
            else
            {
                countdownText.SetText(countdownText.text + second + "\"");
            }
            // if(hundredth < 10)
            // {
            //     countdownText.SetText(countdownText.text + "0" + hundredth);
            // }
            // else
            // {
            //     countdownText.SetText(countdownText.text + hundredth);
            // }

            // // Est-ce que ça vaut vraiment le coup de fais 100 opération par seconde pour afficher les centièmes ?
            // yield return new WaitForSeconds(0.01f);

            // if(hundredth == 0)
            // {
            //     hundredth = 100;
            //     if(second == 0)
            //     {
            //         second = 60;
            //         minute--;
            //     }
            //     countdownDuration--;
            //     second--;
            // }
            // hundredth--;

            yield return new WaitForSeconds(1);

            if(second == 0)
            {
                second = 60;
                minute--;
            }
            timeScore--;
            second--;
        }
    }
    
    private void CountdownAction(int action)
    {
        if(SceneManager.GetActiveScene().name == "Game")
        {
            switch(action)
            {
                case 0:
                    countdownText.gameObject.SetActive(false);
                    break;
                case 1:
                    countdownText.gameObject.SetActive(true);
                    break;
                case 2:
                    if(countdownText.text != "00'00\"" & countdownText.text != "")
                    {
                        gonnaScrolling = true;
                        UpdateScoreText(timeScore * 100, Vector3.forward * (playerScript.transform.position.z + 24));
                    }
                    break;
            }
        }
    }

    private int[,] Maps()
    {
        int[,] map;

        switch (level)
        {
            case 0:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 1, 9, 9, 9, 9, 9, 9, 9, 1, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 1, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("Move with [Q] and [D] or [LEFT] and [RIGHT]");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("Aim with [MOUSE] and fire with [LEFT CLICK]");
                break;
            case 1:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 1, 9, 1, 9, 1, 9, 1, 9, 9, 9 },
                    { 9, 9, 9, 9, 1, 9, 1, 9, 1, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 1, 9, 1, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 2, 9, 9, 9, 9, 9, 9, 9, 2, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };
                SpawnThings(13, 4, 3);
                SpawnThings(13, 6, 3);
                SpawnThings(13, 8, 3);
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("With rapid fire bonus hold [LEFT CLICK] to fire");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("You can pick up bonuses or shoot them");
                break;
            case 2:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 0, 9, 1, 9, 1, 9, 0, 9, 9, 9 },
                    { 9, 9, 9, 0, 0, 0, 0, 0, 0, 0, 9, 9, 9 },
                    { 0, 1, 0, 9, 9, 9, 9, 9, 9, 9, 0, 1, 0 },
                    { 0, 0, 0, 9, 9, 9, 9, 9, 9, 9, 0, 0, 0 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };
                SpawnThings(5, 3, 2);
                SpawnThings(5, 6, 3);
                SpawnThings(5, 9, 2);
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("Enemy cover can be destroyed…");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("… but you can wait for the right moment");
                break;
            case 3:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 0, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 0, 1, 0, 9, 9, 9, 9, 9 },
                    { 9, 9, 2, 9, 0, 1, 9, 1, 0, 9, 2, 9, 9 },
                    { 9, 9, 9, 9, 9, 0, 1, 0, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 0, 9, 0, 9, 0, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };

                SpawnThings(8, 4, 4);
                SpawnThings(8, 8, 4);
                SpawnThings(8, 6, 4);
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("Throw grenades with [RIGHT CLICK]…");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("… aiming is cryptic on purpose :-)");
                break;
            case 4:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 1, 1, 1, 9, 1, 1, 1, 9, 9, 9 },
                    { 9, 9, 9, 1, 1, 1, 9, 1, 1, 1, 9, 9, 9 },
                    { 9, 9, 9, 1, 1, 1, 9, 1, 1, 1, 9, 9, 9 },
                    { 9, 9, 2, 0, 0, 0, 5, 0, 0, 0, 2, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };
                SpawnThings(12, 5, 0).GetComponent<Cover>().SetLife(500);
                SpawnThings(12, 6, 0).GetComponent<Cover>().SetLife(500);
                SpawnThings(12, 7, 0).GetComponent<Cover>().SetLife(500);
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("Next to a cover, [SPACE] to get on cover…");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("… and yes, sometimes there is a red barrel");
                break;
            case 5:
                map = new int[,]
                {
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 1, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
                    { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
                };
                SpawnThings(12, 3, 7).GetComponent<TextMeshPro>().SetText("That's all…");
                SpawnThings(12, 9, 7).GetComponent<TextMeshPro>().SetText("… for the tutorial. [P] for pause by the way");
                break;
            default:
                mapInfo = new int[] {0, 0};
                map = GetComponent<MapGenerator>().GenerateMap(barrelEarn, coverEarn, blocsLevel);
                barrelBonusScore = 0;
                coverBonusScore = 0;
                barrelEarn = 0;
                coverEarn = 0;
                break;
        }

        return map;
    }

    public bool GetIsScrolling() => isScrolling;

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        SceneManager.LoadScene("Title Menu");
    }
    public int BonusDraw()
    {
        int start, end, draw;
        int[] chance = { 34, 33, 33}; // Le total doit faire 100.

        end = 0;
        draw = Random.Range(0, 100);

        for (int i = 0; i < 3; i++)
        {
            start = i == 0 ? -1 : end - 1;
            end = 0;
            for (int j = 0; j < i + 1; j++)
            {
                end += chance[j];
            }
            if (draw > start && draw < end)
            {
                switch (i)
                {
                    case 0:
                        return 2;
                    case 1:
                        return 3;
                    case 2:
                        return 4;
                }
            }
        }
        return 0;
    }
}