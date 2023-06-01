using System.Collections;
using UnityEngine;

public class Bonus : MonoBehaviour
{
    public AudioClip blipUn, blipDeux;
    private bool justCreated = true;
    private ParticleSystem spark;
    private TextMesh bonusText;
    private Light bonusLight;
    private MeshRenderer bonusMeshRenderer;
    private AudioSource bonusSource;
    private readonly float rotateSpeed = 90, _volumeScale = 0.5f;
    private PlayerController playerScript;

    void OnEnable()
    {
        if(!justCreated)
        {
            bonusMeshRenderer.enabled = true;
            bonusLight.enabled = true;
            foreach (Collider bonusCollider in gameObject.GetComponents<BoxCollider>())
            {
                bonusCollider.enabled = true;
            }
            // GetComponent<Rigidbody>().useGravity = true;
            bonusText.text = "Shoot me";
            bonusSource.PlayOneShot(blipUn, 0.5f);
            spark.Play();
        }
    }
    void Start()
    {
        justCreated = false;

        int children = transform.childCount;

        for (int i = 0; i < children; ++i)
        {
            switch(i)
            {
                case 0:
                    spark = transform.GetChild(i).GetComponent<ParticleSystem>();
                    break;
                case 1:
                    bonusText = transform.GetChild(i).GetComponent<TextMesh>();
                    break;
                case 2:
                    bonusLight = transform.GetChild(i).GetComponent<Light>();
                    break;
                default:
                    bonusMeshRenderer = transform.GetChild(i).GetComponent<MeshRenderer>();
                    break;

            }
        }
        if(bonusMeshRenderer == null)
        {
            bonusMeshRenderer = GetComponent<MeshRenderer>();
        }
        bonusSource = GetComponent<AudioSource>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        VolumeScaleAdjustment(blipUn);
        spark.Play();
    }

    void Update() => RotationAnimation();

    void RotationAnimation() => transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    private IEnumerator IsGetCoroutine()
    {
        // GetComponent<Rigidbody>().useGravity = false;
        foreach (Collider bonusCollider in gameObject.GetComponents<BoxCollider>())
        {
            bonusCollider.enabled = false;
        }
        bonusText.text = "Thank you";
        bonusLight.enabled = false;
        bonusMeshRenderer.enabled = false;
        VolumeScaleAdjustment(blipDeux);
        spark.Play();
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    void VolumeScaleAdjustment(AudioClip clip)
    {
        bonusSource.PlayOneShot(clip, _volumeScale * DataManager.Instance.VolumeScaleFactor);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CompareTagChoice();
        }
        if (other.CompareTag("Player Bullet"))
        {
            CompareTagChoice();
            other.gameObject.SetActive(false);
        }
    }

    private void CompareTagChoice()
    {
        if (tag.Contains("Life"))
        {
            StartCoroutine(IsGetCoroutine());
            playerScript.Life();
        }
        else if (tag.Contains("Rapid Fire"))
        {
            StartCoroutine(IsGetCoroutine());
            playerScript.RapidFire();
        }
        else if(tag.Contains("Grenade"))
        {
            StartCoroutine(IsGetCoroutine());
            playerScript.Grenade();
        }
    }
}
