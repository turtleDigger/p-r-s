using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cover : MonoBehaviour
{
    public Material[] coverMaterial;
    public AudioClip hurt;
    private TextMesh coverText;
    private bool justCreated = true, isDying = false;
    private int life;
    private readonly float _volumeScale = 0.25f;
    private AudioSource coverAudioSource;
    [SerializeField] private MeshRenderer[] coverMeshRenderer;
    private ParticleSystem coverParticle;
    private Light particleLight;
    private Rigidbody coverRigidbody;
    private BoxCollider[] coverBoxColliders;
    private GameManager gameManagerScript;
    private PlayerController playerScript;
    private List<GameObject> onCoverGuys;

    void OnEnable()
    {
        life = 8;

        if(!justCreated)
        {
            isDying = false;
            
            coverRigidbody.useGravity = true;

            foreach (Collider coverCollider in coverBoxColliders)
            {
                coverCollider.enabled = true;
            }
            coverMeshRenderer[0].enabled = true;
            coverMeshRenderer[1].enabled = true;
        }
    }

    void OnDisable()
    {
        if(!justCreated)
        {
            onCoverGuys.Clear();
            coverRigidbody.velocity = Vector3.zero;
            coverRigidbody.angularVelocity = Vector3.zero; 
        }
    }

    void Start()
    {
        justCreated = false;
        coverText = transform.GetChild(1).GetComponent<TextMesh>();
        coverAudioSource = GetComponent<AudioSource>();
        //coverMeshRenderer = transform.GetChild(2).GetComponent<MeshRenderer>();
        coverParticle = GetComponentInChildren<ParticleSystem>();
        particleLight = transform.GetChild(0).GetChild(0).GetComponent<Light>();
        particleLight.enabled = false;
        coverRigidbody = GetComponent<Rigidbody>();
        coverBoxColliders = GetComponents<BoxCollider>();
        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();
        onCoverGuys = new List<GameObject>();
    }

    public void SetLife(int amount) => life = amount;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onCoverGuys.Add(other.gameObject);
            playerScript.SetHasCover(1);
        }

        if (other.CompareTag("Enemy"))
        {
            onCoverGuys.Add(other.gameObject);
            other.GetComponent<Enemy>().SetHasCover(1);
        }

        if (other.CompareTag("Enemy Bullet"))
        {
            if(onCoverGuys.Count != 0)
            {
                if(!(onCoverGuys[0].name == "Enemy(Clone)"))
                {
                    BulletThing(other);
                }
            }
            else
            {
                BulletThing(other);
            }
        }
        if(other.CompareTag("Player Bullet"))
        {
            if(Vector3.Distance(playerScript.transform.position, transform.position) > 6)
            {
                BulletThing(other);
            }
        }
    }

    private void BulletThing(Collider other)
    {
        StartCoroutine(FlashingingRoutine(other));
        life--;
        coverAudioSource.PlayOneShot(hurt, _volumeScale * DataManager.Instance.VolumeScaleFactor);
        if (life == 0)
        {
            StartCoroutine(DeathCoroutine());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            onCoverGuys.Remove(other.gameObject);
            playerScript.SetHasCover(-1);
        }
        if (other.CompareTag("Enemy"))
        {
            onCoverGuys.Remove(other.gameObject);
            //other.GetComponent<Enemy>().SetHasCover(-1);
        }
    }

    private IEnumerator FlashingingRoutine(Collider other)
    {
        coverParticle.transform.position = other.transform.position;
        coverParticle.transform.rotation = other.transform.rotation * Quaternion.Euler(0, 180, 0);

        coverParticle.Play();

        particleLight.enabled = true;

        other.gameObject.SetActive(false);

        coverMeshRenderer[0].material = coverMaterial[1];
        coverMeshRenderer[1].material = coverMaterial[1];
        yield return new WaitForSeconds(0.1f);
        coverMeshRenderer[0].material = coverMaterial[0];
        coverMeshRenderer[1].material = coverMaterial[0];

        particleLight.enabled = false;
    }

    public void ExplosionForce(Vector3 position, int power)
    {
        float distance = Vector3.Distance(position, transform.position);
        Vector3 forceDirection = (transform.position- position).normalized;

        coverRigidbody.AddForce(forceDirection * ((power * 500) / distance), ForceMode.Impulse);
    }

    public bool GetIsDying()
    {
        return isDying;
    }
    public void Death() => StartCoroutine(DeathCoroutine());
    private IEnumerator DeathCoroutine()
    {
        isDying = true;
        coverText.gameObject.SetActive(true);
        coverText.text = "Argh !";
        yield return new WaitForSeconds(1);
        coverText.text = "Je meurs…";
        yield return new WaitForSeconds(1);
        coverText.gameObject.SetActive(false);

        foreach (GameObject onCoverGuy in onCoverGuys)
        {
            if (onCoverGuy != null)
            {
                if (onCoverGuy.CompareTag("Player"))
                {
                    playerScript.SetHasCover(-1);
                }
            }
            if (onCoverGuy.activeInHierarchy && onCoverGuy.CompareTag("Enemy"))
            {
                onCoverGuy.GetComponent<Enemy>().SetHasCover(-1);
            }
        }

        //coverAudioSource.PlayOneShot(hurt, 0.5f);
        gameManagerScript.UpdateScoreText(10, transform.position);
        coverRigidbody.useGravity = false;

        foreach (Collider coverCollider in coverBoxColliders)
        {
            coverCollider.enabled = false;
        }
        coverMeshRenderer[0].enabled = false;
        coverMeshRenderer[1].enabled = false;

        for (int i = 0; i < 7; i++)
        {
            yield return new WaitForSeconds(0.18f);

            if (i % 2 == 0)
            {
                coverMeshRenderer[0].enabled = false;
                coverMeshRenderer[1].enabled = false;
            }
            else
            {
                coverMeshRenderer[0].enabled = true;
                coverMeshRenderer[1].enabled = true;
            }
        }
        gameObject.SetActive(false);
    }
}
