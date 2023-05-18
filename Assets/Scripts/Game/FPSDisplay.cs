using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    private Text _framePerSecondText;
    private int lastFrameIndex;
    private float[] frameDeltaTimeArray;

    void Awake()
    {
        frameDeltaTimeArray = new float[50];
    }

    void Start()
    {
        _framePerSecondText = GetComponent<Text>();
    }

    void Update()
    {
        frameDeltaTimeArray[lastFrameIndex] = Time.deltaTime;
        lastFrameIndex = (lastFrameIndex + 1) % frameDeltaTimeArray.Length;
        _framePerSecondText.text = "" + Mathf.RoundToInt(CalculateFPS());
    }

    float CalculateFPS()
    {
        float total = 0f;
        foreach(float frameDeltaTime in frameDeltaTimeArray)
        {
            total += frameDeltaTime;
        }
        return frameDeltaTimeArray.Length / total;
    }
}
