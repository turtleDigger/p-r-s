using UnityEngine;

public class GoToTarget : MonoBehaviour
{
    private const int speed = 20;

    void Update() => BulletMove();

    void BulletMove()
    {
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
    }
}
