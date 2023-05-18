using System.Collections;
using UnityEngine;
using TMPro;

public class ScoreFading : MonoBehaviour
{
    private bool justCreated = true;
    private Vector3 startScale;
    private TextMeshPro text;

    void OnEnable()
    {
        if(!justCreated)
        {
            transform.localScale = startScale;
            StartCoroutine(ScoreCoroutine());
        }
    }
    void Start()
    {
        justCreated = false;
        startScale = transform.localScale;
        text = GetComponent<TextMeshPro>();
        StartCoroutine(ScoreCoroutine());
    }

    private IEnumerator ScoreCoroutine()
    {
        int loopMod = 1, loop = 255 / loopMod; // loopMod's possible values : 1, 3, 5, 15, 17, 51 or 85
        float positionTarget = 1, scaleTarget = 0.2f;
        Vector3 goingUp = Vector3.up * (positionTarget / loop);
        goingUp /= Time.deltaTime;
        Vector3 growingUp = new Vector3(scaleTarget / loop, scaleTarget / loop, 0);
        growingUp /= Time.deltaTime;

        for (int i = 0; i < loop; i++)
        {
            text.color -= (new Color(0, 0, 0, 1.0f / loop)) / Time.deltaTime;
            text.transform.Translate(goingUp);
            text.transform.localScale += growingUp;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }
}
