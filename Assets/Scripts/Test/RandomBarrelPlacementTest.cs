using System.Collections;
using TMPro;
using UnityEngine;

public class RandomBarrelPlacementTest : MonoBehaviour
{
    public int mapSize = 13, countdown = 1800, howManyRoll = 1800;
    public GameObject[] prefab;
    public GameObject[] textForExam;
    public int[] occurrences;
    public int[,] coordinates;
    public bool ORE;

    void Start()
    {
        occurrences = new int[9];

        coordinates = new int[,]
        {
            { 6, 6 },// Anthony : 1%
            { 3, 6 },// Rare : 3%
            { 0, 6 },// Peu commun : 18%
            { 6, 0 },// Peu commun : 18%
            { 6, 12 },// Peu commun : 18%
            { 3, 0 }, // Commun : 28%
            { 3, 12 },// Commun : 28%
            { 0, 0 },// Très commun : 50%
            { 0, 12 }// Très commun : 50%
        };

        if (ORE)
        {
            InitialisationForOccurrenceRateExam();
        }
        else
        {
            InvokeRepeating("SpawnTest", 3, 2);
        }
    }

    void Update()
    {
        if (ORE)
        {
            OccurrenceRateExam();
        }
    }

    void SpawnTest()
    {
        StartCoroutine(Spawn());
    }

    public void InitialisationForOccurrenceRateExam()
    {
        textForExam = new GameObject[9];

        for (int i = 0; i < 9; i++)
        {
            textForExam[i] = SpawnThings(coordinates[i, 0], coordinates[i, 1], 1);
            float temp = 0.0f;
            textForExam[i].GetComponent<TextMeshPro>().SetText(temp + "%");
        }
    }

    public void OccurrenceRateExam()
    {
        int index;

        if (countdown > 0)
        {
            index = IndexOfPositionDraw();

            occurrences[index]++;
            float temp = occurrences[index] / (howManyRoll / 100.0f);
            textForExam[index].GetComponent<TextMeshPro>().SetText(temp + "%");
        }
        countdown--;

        if (countdown == 0)
        {
            GetComponent<RandomBarrelPlacementTest>().enabled = false;
        }
    }

    public int IndexOfPositionDraw()
    {
        int start, end, draw, result;
        int[] chance = { 1, 3, 18, 28, 50 }; // Le total doit faire 100

        end = 0;
        draw = Random.Range(0, 100);

        for (int i = 0; i < 5; i++)
        {
            if (i == 0)
            {
                start = -1;
            }
            else
            {
                start = end - 1;
            }
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
                        return result = i;
                    case 1:
                        return result = i;
                    case 2:
                        return result = Random.Range(2, 5);
                    case 3:
                        return result = Random.Range(5, 7);
                    case 4:
                        return result = Random.Range(7, 9);
                }
            }
        }

        return 0;

        /*if (draw > -1 && draw < 1)
        {
            index = 0;
        }
        if (draw > 0 draw < 4)
        {
            index = 1;
        }
        if (draw > 3 && draw < 22)
        {
            index = Random.Range(2, 5);
        }
        if (draw > 21 && draw < 50)
        {
            index = Random.Range(5, 7);
        }
        if (draw > 49 && draw < 100)
        {
            index = Random.Range(7, 9);
        }*/
    }

    IEnumerator Spawn()
    {
        int index;

        index = IndexOfPositionDraw();
        SpawnThings(coordinates[index, 0], coordinates[index, 1], 0);
        string coordonnees = coordinates[index, 0] + "," + coordinates[index, 1];
        SpawnThings(coordinates[index, 0], coordinates[index, 1], 1).GetComponent<TextMeshPro>().SetText(coordonnees);

        occurrences[index]++;

        yield return new WaitForSeconds(1);
        Destroy(GameObject.FindWithTag("Red Barrel"));
        Destroy(GameObject.FindWithTag("Tutorial"));
    }

    GameObject SpawnThings(int i, int j, int prefabNum)
    {
        int fact;
        fact = prefabNum == 1 ? 1 : 0;

        return Instantiate
        (
            prefab[prefabNum],
            new Vector3
            (
                ((j * 2) - (mapSize - 1)),
                1,
                ((((mapSize - 1) * 2) + 2) - i * 2)
            )
            +
            Vector3.up * 3 * fact,
            prefab[prefabNum].transform.rotation
        );
    }
}
