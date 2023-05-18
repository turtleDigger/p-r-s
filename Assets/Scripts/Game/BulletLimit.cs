using UnityEngine;

public class BulletLimit : MonoBehaviour
{
    void OnEnable() => GameManager.OnScrolling += Teleport;
    void OnDisable() => GameManager.OnScrolling -= Teleport;
    private void Teleport() => transform.position += Vector3.forward * GameManager.mapOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Bullet"))
        {
            other.gameObject.SetActive(false);
        }
    }
}