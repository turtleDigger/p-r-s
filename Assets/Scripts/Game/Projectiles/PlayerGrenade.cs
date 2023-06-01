using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrenade : MonoBehaviour
{
    public AudioClip explosionAudio, clickAudio;
    public Material[] grenadeMaterial;

    private AudioSource playerGrenadeSource;
    private bool isExploding, justCreated = true;
    private readonly float _volumeScale = 1;
    private ParticleSystem sparks;
    private MeshRenderer grenadeMeshRenderer;
    private Rigidbody grenadeRigidbody;
    private BoxCollider grenadeCollider;
    private Light littleExplosionLight, flashingLight;

    void OnEnable()
    {
        isExploding = false;

        if(!justCreated)
        {
            grenadeCollider.enabled = true;
            grenadeCollider.size = Vector3.one;
            grenadeCollider.isTrigger = false;
            grenadeRigidbody.useGravity = true;
            grenadeMeshRenderer.enabled = true;
        }
        else
        {
            playerGrenadeSource = GetComponent<AudioSource>();
            sparks = transform.GetChild(0).GetComponent<ParticleSystem>();
            littleExplosionLight = transform.GetChild(1).GetComponent<Light>();
            flashingLight = transform.GetChild(2).GetComponent<Light>();
            grenadeMeshRenderer = GetComponent<MeshRenderer>();
            grenadeRigidbody = GetComponent<Rigidbody>();
            grenadeCollider = GetComponent<BoxCollider>();
            littleExplosionLight.enabled = false;
            flashingLight.enabled = false;
        }
        StartCoroutine(GrenadeExplosionCoroutine());
    }

    void Start()
    {
        justCreated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isExploding)
        {
            if(other.CompareTag("Enemy"))
            {
                Enemy enemyScript = other.GetComponent<Enemy>();
                enemyScript.ExplosionForce(transform.position, 1);
                if(!enemyScript.GetIsDying())
                {
                    enemyScript.Death();
                }
            }
            else if (other.CompareTag("Cover"))
            {
                Cover coverScript = other.GetComponent<Cover>();
                coverScript.ExplosionForce(transform.position, 1);
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
    private IEnumerator GrenadeExplosionCoroutine()
    {
        for (int i = 1; i < 24; i++)
        {
            yield return new WaitForSeconds(0.2f);
            if(i % 2 == 0)
            {
                VolumeScaleAdjustment(clickAudio);
                grenadeMeshRenderer.material = grenadeMaterial[1];
                flashingLight.enabled = true;
            }
            else
            {
                VolumeScaleAdjustment(clickAudio);
                grenadeMeshRenderer.material = grenadeMaterial[0];
                flashingLight.enabled = false;
            }
        }
        grenadeMeshRenderer.enabled = false;
        sparks.Play();
        littleExplosionLight.enabled = true;
        VolumeScaleAdjustment(explosionAudio);
        isExploding = true;
        grenadeRigidbody.useGravity = false;
        grenadeCollider.isTrigger = true;
        grenadeCollider.size = Vector3.one * 13;

        // Apparition et disparition progressive de la lumière
        int numberFrameIntensityUp = 25, numberFrameIntensityDown = 80;
        float intensityTarget = 5.0f;

        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForEndOfFrame();
            littleExplosionLight.intensity += (intensityTarget / numberFrameIntensityUp);
        }
        // Quelque frame pour laisser le temps de calcul au système de physique.
        grenadeCollider.enabled = false;

        for (int i = 0; i < (numberFrameIntensityUp - 10); i++)
        {
            yield return new WaitForEndOfFrame();
            littleExplosionLight.intensity += (intensityTarget / numberFrameIntensityUp);
        }
        //Debug.Log(LittleExplosionLight.intensity);

        for(int i = 0 ; i < numberFrameIntensityDown ; i++)
        {
            yield return new WaitForEndOfFrame();
            littleExplosionLight.intensity -= (intensityTarget / numberFrameIntensityDown);
        }
        littleExplosionLight.enabled = false;
        //Debug.Log(LittleExplosionLight.intensity);

        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }

    void VolumeScaleAdjustment(AudioClip clip)
    {
        playerGrenadeSource.PlayOneShot(clip, _volumeScale * DataManager.Instance.VolumeScaleFactor);
    }
}
