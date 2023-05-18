using UnityEngine;

public class Cleaner : MonoBehaviour
{
    private Vector3 TeleportValue = Vector3.forward * 200;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tutorial"))
        {
            Destroy(other.gameObject);
        }
        if(other.CompareTag("Cover") | other.tag.Contains("Bullet") | other.tag.Contains("Bonus") | other.CompareTag("Red Barrel"))
        {
            other.gameObject.SetActive(false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            other.transform.position += TeleportValue;
        }
    }
}
