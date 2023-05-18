using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler SharedInstance;
    public List<GameObject> [] pooledObjects;
    public GameObject [] objectToPool;
    public int [] amountToPool;

    void Awake()
    {
        SharedInstance = this;
    }

    void Start()
    {
        FillingPools();
    }

    public void FillingPools()
    {
        // Initialisation du tableau de listes
        pooledObjects = new List<GameObject>[objectToPool.Length];

        for (int i = 0; i < objectToPool.Length; i++)
        {
            // Initialisation des listes
            pooledObjects[i] = new List<GameObject>();

            for (int j = 0; j < amountToPool[i]; j++)
            {
                GameObject obj = (GameObject)Instantiate(objectToPool[i]);
                obj.SetActive(false);
                // Remplissage
                pooledObjects[i].Add(obj);

                // Rangement
                switch (i)
                {
                    case 0:
                        obj.transform.SetParent(GameObject.Find("Covers").transform);
                        break;
                    case 1:
                        obj.transform.SetParent(GameObject.Find("Enemies").transform);
                        break;
                    case 2:
                        obj.transform.SetParent(GameObject.Find("Bonuses").transform);
                        break;
                    case 3:
                        obj.transform.SetParent(GameObject.Find("Bonuses").transform);
                        break;
                    case 4:
                        obj.transform.SetParent(GameObject.Find("Bonuses").transform);
                        break;
                    case 5:
                        obj.transform.SetParent(GameObject.Find("Red Barrels").transform);
                        break;
                    case 6:
                        obj.transform.SetParent(GameObject.Find("Scores").transform);
                        break;
                    case 7:
                        obj.transform.SetParent(GameObject.Find("Player Bullets").transform);
                        break;
                    case 8:
                        obj.transform.SetParent(GameObject.Find("Enemy Bullets").transform);
                        break;
                    case 9:
                        obj.transform.SetParent(GameObject.Find("Player Grenades").transform);
                        break;
                    default:
                        obj.transform.SetParent(this.transform);
                        break;
                }
            }
        }
    }

    public GameObject GetPooledObject(int who)
    {
        for (int i = 0; i < pooledObjects[who].Count; i++)
        {
            if (!pooledObjects[who][i].activeInHierarchy)
            {
                return pooledObjects[who][i];
            }
        }
        return null;
    }
}
