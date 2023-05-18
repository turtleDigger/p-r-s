using System;
using System.Collections;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    private bool isShaking;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    public bool GetIsShaking()
    {
        return isShaking;
    }

    private IEnumerator CameraShakeCoroutine()
    {
        isShaking = true;

        int i, j, loop = 3, frame = 1;
        float magnitude = 0.1f, step = 0.01f;

        startPosition = new Vector3(startPosition.x, startPosition.y, transform.position.z);

        while (Math.Abs(magnitude) > step)
        {
            for (i = 0; i < loop; i++)
            {
                for (j = 0; j <= frame; j++)
                {
                    yield return null;
                }
                transform.Translate(Vector3.right * magnitude);
                transform.Rotate(Vector3.forward * magnitude);
            }

            magnitude *= -1;
            step *= -1;

            for (i = 0; i < loop; i++)
            {
                for (j = 0; j <= frame; j++)
                {
                    yield return null;
                }
                transform.Translate(Vector3.right * magnitude);
                transform.Rotate(Vector3.forward * magnitude);
            }

            magnitude -= step;

            if(frame < loop)
            {
                frame++;
            }
        }

        transform.position = new Vector3(startPosition.x, startPosition.y, transform.position.z);

        isShaking = false;
    }
}
