using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBarrel : MonoBehaviour
{
    public AudioClip hitAudio, explosionAudio, clickAudio;

    private bool gonnaExplode, isExploding, justCreated = true;
    private readonly float _volumeScale = 1;
    private Vector3 baseCollider, explosionCollider;
    private AudioSource barrelSource;
    private ParticleSystem fireParticle, explosionParticle, smokeParticle;
    private MeshRenderer barrelMeshRenderer;
    private Light explosionLight;
    private Rigidbody barrelRigidbody;
    private BoxCollider barrelCollider;
    //private MainCamera mainCameraScript;

    void OnEnable()
    {
        if(!justCreated)
        {
            gonnaExplode = false;
            isExploding = false;
            barrelCollider.enabled = true;
            barrelCollider.size = baseCollider;
            barrelCollider.isTrigger = false;
            barrelRigidbody.useGravity = true;
            barrelMeshRenderer.enabled = true;
        }
    }

    void Start()
    {
        justCreated = false;

        baseCollider = Vector3.one;
        explosionCollider = Vector3.one * 8 - Vector3.up * 7 ;

        barrelSource = GetComponent<AudioSource>();

        int children = transform.childCount;

        for (int i = 0; i < children; ++i)
        {
            switch(i)
            {
                case 0:
                    fireParticle = transform.GetChild(i).GetComponent<ParticleSystem>();
                    break;
                case 1:
                    explosionParticle = transform.GetChild(i).GetComponent<ParticleSystem>();
                    break;
                case 2:
                    smokeParticle = transform.GetChild(i).GetComponent<ParticleSystem>();
                    break;
                case 3:
                    barrelMeshRenderer = transform.GetChild(i).GetComponent<MeshRenderer>();
                    break;
                case 4:
                    explosionLight = transform.GetChild(i).GetComponent<Light>();
                    explosionLight.enabled = false;
                    break;

            }
        }
        barrelRigidbody = GetComponent<Rigidbody>();
        barrelCollider = gameObject.GetComponent<BoxCollider>();
        //mainCameraScript = GameObject.Find("Main Camera").GetComponent<MainCamera>();
    }

    public bool GetIsExploding() => isExploding;

    public void Victimisation() => StartCoroutine(ExplosionCoroutine());

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Bullet"))
        {
            other.gameObject.SetActive(false);
            VolumeScaleAdjustment(hitAudio);

            if (!gonnaExplode)
            {
                StartCoroutine(GonnaExplodeCoroutine());
                gonnaExplode = true;
            }
        }

        // Peut-être ajouter plus tard la destruction de Bonus pour dissuader de toujours choisir cette solution
        if (isExploding)
        {
            if(other.CompareTag("Enemy"))
            {
                Enemy enemyScript = other.GetComponent<Enemy>();
                enemyScript.ExplosionForce(transform.position, 2);
                if(!enemyScript.GetIsDying())
                {
                    enemyScript.Death();
                }
            }
            else if (other.CompareTag("Cover"))
            {
                Cover coverScript = other.GetComponent<Cover>();
                coverScript.ExplosionForce(transform.position, 2);
                if(!coverScript.GetIsDying())
                {
                    coverScript.Death();
                }
            }
            else if(other.CompareTag("Red Barrel"))
            {
                RedBarrel redBarrelScript = other.GetComponent<RedBarrel>();
                if(!redBarrelScript.GetIsExploding())
                {
                    redBarrelScript.Victimisation();
                }
            }
        }
    }

    private IEnumerator GonnaExplodeCoroutine()
    {
        fireParticle.Play();
        StartCoroutine(ClickingSoundRoutine());

        for(int i = 0; i < 5; i++)
        {
            if (!isExploding)
            {
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                fireParticle.Stop();
                i = 5;
            }
        }

        if (!isExploding)
        {
            StartCoroutine(ExplosionCoroutine());
        }
        // else
        // {
        //     //Cas ou deux barils proches sont allumés en même temps.
        //     fire.Stop();
        //     barrelSource.Stop();
        // }
    }

    private IEnumerator ClickingSoundRoutine()
    {
        while(!isExploding)
        {
            VolumeScaleAdjustment(clickAudio);
            yield return new WaitForSeconds(0.2f);
        }
    }

    private IEnumerator ExplosionCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        if (!isExploding)
        {
            fireParticle.Stop();
        }

        barrelMeshRenderer.enabled = false;

        explosionParticle.Play();
        explosionLight.enabled = true;
        VolumeScaleAdjustment(explosionAudio);
        isExploding = true;

        barrelRigidbody.useGravity = false;
        barrelCollider.isTrigger = true;
        barrelCollider.size = explosionCollider;

        // if(!mainCameraScript.GetIsShaking())
        // {
        //     StartCoroutine(mainCameraScript.CameraShakeCoroutine());
        // }

        // Apparition et disparition progressive de la lumière
        int numberFrameIntensityUp = 25, numberFrameIntensityDown = 80;
        float intensityTarget = 5.0f;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
            explosionLight.intensity += (intensityTarget / numberFrameIntensityUp);
        }
        // Quelque frame pour laisser le temps de calcul au système de physique.
        barrelCollider.enabled = false;

        for (int i = 0; i < (numberFrameIntensityUp - 10); i++)
        {
            yield return new WaitForEndOfFrame();
            explosionLight.intensity += (intensityTarget / numberFrameIntensityUp);
        }
        // Debug.Log(explosionLight.intensity);

        for(int i = 0 ; i < numberFrameIntensityDown ; i++)
        {
            yield return new WaitForEndOfFrame();
            explosionLight.intensity -= (intensityTarget / numberFrameIntensityDown);
        }
        explosionLight.enabled = false;
        // Debug.Log(explosionLight.intensity);
        smokeParticle.Play();
        yield return new WaitForSeconds(6);
        smokeParticle.Stop();
        yield return new WaitForSeconds(6);
        gameObject.SetActive(false);
    }

    void VolumeScaleAdjustment(AudioClip clip)
    {
        barrelSource.PlayOneShot(clip, _volumeScale * DataManager.Instance.VolumeScaleFactor);
    }
}
