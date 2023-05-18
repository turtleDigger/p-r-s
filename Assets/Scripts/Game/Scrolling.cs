using System.Collections;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    private GameManager gameManagerScript;
    void OnEnable() => GameManager.OnScrolling += StartCoroutineScroll;
    void OnDisable() => GameManager.OnScrolling -= StartCoroutineScroll;
    void Awake() => gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
    void StartCoroutineScroll() => StartCoroutine(Scroll());

    private IEnumerator Scroll()
    {
        while(gameManagerScript.GetIsScrolling())
        {
            transform.Translate(Vector3.forward * Time.deltaTime * GameManager.unitBySecond, Space.World);
            yield return new WaitForEndOfFrame();
        }
    }
}