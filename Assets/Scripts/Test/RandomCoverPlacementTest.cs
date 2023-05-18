using System.Collections;
using UnityEngine;

public class RandomCoverPlacementTest : MonoBehaviour
{
    public int mapSize = 13;
    public int[,] map;
    public GameObject[] prefab;
    
    void Start()
    {
        //PlaceCoverSpawnTest();

        InvokeRepeating("SpawnTest", 3, 2);
    }

    void SpawnTest()
    {
        StartCoroutine(Spawn());
    }

    void PlaceCoverSpawnTest()
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

        PlaceCover(Random.Range(1, 4));

        SpawnMap();
    }

    void PlaceCover(int coverLevelEarn)
    {
        bool atLeastOne;

        for(int i = 12; i > 12 - coverLevelEarn; i--)
        {
            do
            {
                atLeastOne = false;

                for (int j = 0; j < 3; j++)
                {
                    map[i, j + 1] = Random.Range(0, 2) == 0 ? (map[i, 11 - j] = 0) : (map[i, 11 - j] = 9);
                    atLeastOne |= map[i, j + 1] == 0;
                }
            }
            while (!(atLeastOne));

            do
            {
                atLeastOne = false;

                map[i, 0 + 5] = Random.Range(0, 2) == 0 ? (map[i, 0 + 7] = 0) : (map[i, 0 + 7] = 9);
                map[i, 0 + 6] = Random.Range(0, 2) == 0 ? 0 : 9;
                atLeastOne |= map[i, 0 + 5] == 0 | map[i, 0 + 6] == 0;

            }
            while (!(atLeastOne));
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
        PlaceCoverSpawnTest();

        yield return new WaitForSeconds(1);

        GameObject[] covers = GameObject.FindGameObjectsWithTag("Cover");
        foreach (GameObject cover in covers)
        {
            Destroy(cover.gameObject);
        }
    }
}

/*void PlacePlayerCover(int coverLevelEarn)
{
    bool atLeastOne;

    for (int i = 12; i > 12 - coverLevelEarn; i--)
    {
        atLeastOne = false;

        while (!atLeastOne)
        {
            for (int j = 0; j < 3; j++)
            {
                if (Random.Range(0, 2) == 0)
                {
                    map[i, j + 1] = 0;
                    map[i, 11 - j] = 0;
                    atLeastOne = true;
                }
                else
                {
                    map[i, j + 1] = 9;
                    map[i, 11 - j] = 9;
                }
            }
        }

        atLeastOne = false;

        while (!atLeastOne)
        {
            if (Random.Range(0, 2) == 0)
            {
                map[i, 0 + 5] = 0;
                map[i, 0 + 7] = 0;
                atLeastOne = true;
            }
            else
            {
                map[i, 0 + 5] = 9;
                map[i, 0 + 7] = 9;
            }
            if (Random.Range(0, 2) == 0)
            {
                map[i, 0 + 6] = 0;
                atLeastOne = true;
            }
            else
            {
                map[i, 0 + 6] = 9;
            }

        }
    }
}*/