using System.Collections;
using UnityEngine;

public class RandomEnemyPlacementTest : MonoBehaviour
{
    public int mapSize = 13;
    public int[,] map;
    public GameObject[] prefab;

    void Start()
    {
        //PlaceEnemySpawnTest();

        InvokeRepeating("SpawnTest", 3, 2);
    }

    void SpawnTest()
    {
        StartCoroutine(Spawn());
    }

    void PlaceEnemySpawnTest()
    {
        map = new int[,]
        {
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 },
            { 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9 }
        };

        PlaceEnemyBlocs23Middle(2, 2, new int[] { 1, 1, 1 });
        PlaceEnemyBlocs23Middle(3, 3, new int[] { 1, 1, 1 });
        //PlaceEnemyBlocs012(0, 3, new int[] { 0, 0, 0 });
        //PlaceEnemyBlocs012(1, 3, new int[] { 0, 0, 0 });
        //PlaceEnemyBlocs012(2, 3, new int[] { 0, 0, 0 });
        //PlaceEnemyBlocs3Middle(3, 0); // Valeur possible (0/1/2/3, 0/1/2)
        //PlaceEnemyBlocs3LeftRight(3, 0); // Valeur possible (0/1/2/3, 0/1/2/3)
        //PlaceEnemyBlocs2Middle(2, 0); // Valeur possible (0/1/2, 0/1)

        SpawnMap();
    }

    void PlaceEnemyBlocs23Middle(int who, int lvl, int[] voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber, forIValue = 0, forJLimite = 0, maxLevel = 0, jOffsetLeft = 0, jOffsetRight, lastIvalue;

        switch(who)
        {
            case 2:
                forIValue = 5;
                forJLimite = 2;
                maxLevel = 2;
                jOffsetLeft = 5;
                break;
            case 3:
                forIValue = 2;
                forJLimite = 3;
                maxLevel = 3;
                jOffsetLeft = 4;
                break;
        }
        lastIvalue = forIValue - (maxLevel - 1);
        jOffsetRight = jOffsetLeft + (forJLimite-1) * 2;

        for (int i = forIValue; i > forIValue - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != forIValue;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < forJLimite; j++)
                {
                    if(i == lastIvalue)
                    {
                        if(Random.Range(0, 2) == 0)
                        {
                            draw = 0;
                        }
                        else
                        {
                            draw = 2;
                        }
                    }
                    else
                    {
                        draw = Random.Range(0, 3);
                    }

                    switch (draw)
                    {
                        case 0:
                            if(i == lastIvalue)
                            {
                                if (j < forJLimite - 1)
                                {
                                    map[i, j + jOffsetLeft] = 0;
                                    map[i + maxLevel, j + jOffsetLeft] = 0;

                                    map[i, jOffsetRight - j] = 0;
                                    map[i + maxLevel, jOffsetRight - j] = 0;
                                }
                                else
                                {
                                    map[i, j + jOffsetLeft] = 0;
                                    map[i + maxLevel, j + jOffsetLeft] = 0;
                                }
                            }
                            else
                            {
                                if (j < forJLimite - 1)
                                {
                                    map[i, j + jOffsetLeft] = 0;

                                    map[i, jOffsetRight - j] = 0;
                                }
                                else
                                {
                                    map[i, j + jOffsetLeft] = 0;
                                }
                            }
                            atLeastOne = true;
                            break;
                        case 1:
                            if (j < forJLimite - 1)
                            {
                                map[i, j + jOffsetLeft] = 1;
                                map[i, jOffsetRight - j] = 1;
                            }
                            else
                            {
                                map[i, j + jOffsetLeft] = 1;
                            }
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            if(i == lastIvalue)
                            {
                                if (j < forJLimite - 1)
                                {
                                    map[i, j + jOffsetLeft] = 9;
                                    map[i + maxLevel, j + jOffsetLeft] = 9;

                                    map[i, jOffsetRight - j] = 9;
                                    map[i + maxLevel, jOffsetRight - j] = 9;
                                }
                                else
                                {
                                    map[i, j + jOffsetLeft] = 9;
                                    map[i + maxLevel, j + jOffsetLeft] = 9;
                                }
                            }
                            else
                            {
                                if (j < forJLimite - 1)
                                {
                                    map[i, j + jOffsetLeft] = 9;

                                    map[i, jOffsetRight - j] = 9;
                                }
                                else
                                {
                                    map[i, j + jOffsetLeft] = 9;
                                }
                            }
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal[-i + forIValue])));
        }
    }

    void PlaceEnemyBlocs012(int who, int lvl, int[] voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber, forIValue = 0, forJLimite = 0, jOffset = 0;

        switch (who)
        {
            case 0:
                forIValue = 7;
                forJLimite = 3;
                break;
            case 1:
                forIValue = 7;
                forJLimite = 3;
                jOffset = 3;
                break;
            case 2:
                forIValue = 4;
                forJLimite = 5;
                break;
        }

        for (int i = forIValue; i < forIValue + lvl; i++)
        {
            do
            {
                atLeastOneEnemy = i != forIValue;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < forJLimite; j++)
                {
                    draw = i == (forIValue + 1) ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 1;
                            atLeastOneEnemy = true;
                            atLeastOne = true;
                            break;
                        default:
                            map[i, j + jOffset] = map[i, 12 - j - jOffset] = 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal[i - forIValue])));
        }
    }

    void PlaceEnemyBlocs3Middle(int lvl, int voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber;

        for (int i = 2; i > 2 - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != 2;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < 3; j++)
                {
                    draw = i == 0 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);
                    switch (draw)
                    {
                        case 0:
                            map[i, j + 4] = i == 0
                                ? j < 2 ? (map[i + 3, j + 4] = map[i, 8 - j] = map[i + 3, 8 - j] = 0) : (map[i + 3, j + 4] = 0)
                                : j < 2 ? (map[i, 8 - j] = 0) : 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j + 4] = j < 2 ? (map[i, 8 - j] = 1) : 1;
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            map[i, j + 4] = i == 0
                                ? j < 2 ? (map[i + 3, j + 4] = map[i, 8 - j] = map[i + 3, 8 - j] = 9) : (map[i + 3, j + 4] = 9)
                                : j < 2 ? (map[i, 8 - j] = 9) : 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal)));
        }
    }

    void PlaceEnemyBlocs3LeftRight(int lvl, int voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber;

        for (int i = 2; i > 2 - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != 2;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < 4; j++)
                {
                    draw = i == 0 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j] = i == 0 ? (map[i, 12 - j] = map[i + 3, j] = map[i + 3, 12 - j] = 0) : (map[i, 12 - j] = 0);
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j] = map[i, 12 - j] = 1;
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            map[i, j] = i == 0 ? (map[i, 12 - j] = map[i + 3, j] = map[i + 3, 12 - j] = 9) : (map[i, 12 - j] = 9);
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal)));
        }
    }

    void PlaceEnemyBlocs2Middle(int lvl, int voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber;

        for (int i = 5; i > 5 - lvl; i--)
        {
            do
            {
                atLeastOneEnemy = i != 5;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < 2; j++)
                {
                    draw = i == 4 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j + 5] = i == 4 ? j == 0 ? (map[6, j + 5] = map[i, 7] = map[6, 7] = 0) : (map[6, j + 5] = 0) : j == 0 ? (map[i, 7] = 0) : 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j + 5] = j == 0 ? (map[i, 7] = 1) : 1;
                            atLeastOne = true;
                            atLeastOneEnemy = true;
                            break;
                        default:
                            map[i, j + 5] = i == 4 ? j == 0 ? (map[6, j + 5] = map[i, 7] = map[6, 7] = 9) : (map[6, j + 5] = 9) : j == 0 ? (map[i, 7] = 9) : 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal)));
        }
    }

    // Pour me rappeler du sens du code :ahi: :sueur:
    /*
        switch (draw)
        {
            case 0:
                if(i == 4)
                {
                    if (j == 0)
                    {
                        map[i, j + 5] = 0;
                        map[6, j + 5] = 0;
                        map[i, 7] = map[6, 7] = 0;
                    }
                    else
                    {
                        map[i, j + 5] = 0;
                        map[6, j + 5] = 0;
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        map[i, j + 5] = 0;
                        map[i, 7] = 0;
                    }
                    else
                    {
                        map[i, j + 5] = 0;
                    }
                }
                atLeastOne = true;
                break;
            case 1:
                if (j == 0)
                {
                    map[i, j + 5] = 1;
                    map[i, 7] = 1;
                }
                else
                {
                    map[i, j + 5] = 1;
                }
                atLeastOne = true;
                atLeastOneEnemy = true;
                break;
            default:
                if (i == 4)
                {
                    if (j == 0)
                    {
                        map[i, j + 5] = 9;
                        map[6, j + 5] = 9;
                        map[i, 7] = 9;
                        map[6, 7] = 9;
                    }
                    else
                    {
                        map[i, j + 5] = 9;
                        map[6, j + 5] = 9;
                    }
                }
                else
                {
                    if (j == 0)
                    {
                        map[i, j + 5] = 9;
                        map[i, 7] = 9;
                    }
                    else
                    {
                        map[i, j + 5] = 9;
                    }
                }
                voidNumber++;
                break;
        }
    */

    void PlaceEnemyBlocs2LeftRight(int lvl, int voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int draw, voidNumber;

        for (int i = 4; i < 4 + lvl; i++)
        {
            do
            {
                atLeastOneEnemy = i != 4;
                atLeastOne = false;
                voidNumber = 0;

                for (int j = 0; j < 5; j++)
                {
                    draw = i == 5 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                    switch (draw)
                    {
                        case 0:
                            map[i, j] = map[i, 12 - j] = 0;
                            atLeastOne = true;
                            break;
                        case 1:
                            map[i, j] = map[i, 12 - j] = 1;
                            atLeastOneEnemy = true;
                            atLeastOne = true;
                            break;
                        default:
                            map[i, j] = map[i, 12 - j] = 9;
                            voidNumber++;
                            break;
                    }
                }
            }
            while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal)));
        }
    }

    void PlaceEnemyBlocs1(int lvl, int voidNumberGoal)
    {
        bool atLeastOneEnemy, atLeastOne;
        int prefabNumber, voidNumber;

        for (int k = 0; k < 2; k++)
        {
            for (int i = 7; i < 7 + lvl; i++)
            {
                do
                {
                    atLeastOneEnemy = i != 7;
                    atLeastOne = false;
                    voidNumber = 0;

                    for (int j = 0; j < 3; j++)
                    {
                        prefabNumber = i == 8 ? Random.Range(0, 2) == 0 ? 0 : 2 : Random.Range(0, 3);

                        switch (prefabNumber)
                        {
                            case 0:
                                map[i, j + (3 * k)] = map[i, 12 - j - (3 * k)] = 0;
                                atLeastOne = true;
                                break;
                            case 1:
                                map[i, j + (3 * k)] = map[i, 12 - j - (3 * k)] = 1;
                                atLeastOneEnemy = true;
                                atLeastOne = true;
                                break;
                            default:
                                map[i, j + (3 * k)] = map[i, 12 - j - (3 * k)] = 9;
                                voidNumber++;
                                break;
                        }
                    }
                }
                while (!(atLeastOneEnemy && atLeastOne && (voidNumber >= voidNumberGoal)));
            }
        }
    }

    void SpawnMap()
    {
        for (int i = 0; i < mapSize; i++)
        {
            for (int j = 0; j < mapSize; j++)
            {
                if (map[i, j] < prefab.Length)
                {
                    SpawnThings(i, j, map[i, j]);
                }
            }
        }
    }

    GameObject SpawnThings(int i, int j, int prefabNum)
    {
        return Instantiate
        (
            prefab[prefabNum],
            new Vector3
            (
                ((j * 2) - (mapSize - 1)),
                1,
                ((((mapSize - 1) * 2) + 2) - i * 2)
            ),
            prefab[prefabNum].transform.rotation
        );
    }

    IEnumerator Spawn()
    {
        PlaceEnemySpawnTest();

        yield return new WaitForSeconds(1);

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        foreach (GameObject cover in covers)
        {
            Destroy(cover.gameObject);
        }
    }
}
