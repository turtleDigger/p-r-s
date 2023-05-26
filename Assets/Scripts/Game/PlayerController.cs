using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : Actor
{
    public Texture2D[] cursorTexture;
    //public Material[] playerMaterial;
    public AudioClip shot, hurt;

    /*[SerializeField]*/ private bool isReloading, hasRapidFire, gameOver, isFlashing, onPause;
    /*[SerializeField]*/ private int life = -1, ammo = 6, grenadeAmmo = 0, rapidFireCountdown;
    private readonly int speed = 10, xLimit = 12;
    private float whenWasLastShot;
    private readonly float minimumTimeBetweenTwoShots = 0.125f, _volumeScale = 0.5f;
    private Vector2 hotSpot;
    private Vector3 target, orientation;
    public Transform gunTransform;
    private TextMeshProUGUI ammoText, grenadeAmmoText, lifeText, rapidFireCountdownText;
    private SkinnedMeshRenderer[] playerSkinnedMeshRenderer;
    private AudioSource playerSource;

    public delegate void GameOver();
    public static event GameOver OnGameOver;

    public delegate void Pause();
    public static event Pause PauseOn;

    void Awake()
    {
        _objectAnimator = transform.GetChild(0).GetComponent<Animator>();

        playerSkinnedMeshRenderer = new SkinnedMeshRenderer[2];
        playerSkinnedMeshRenderer[0] = transform.GetChild(0).GetChild(1).GetComponent<SkinnedMeshRenderer>();
        playerSkinnedMeshRenderer[1] = transform.GetChild(0).GetChild(2).GetComponent<SkinnedMeshRenderer>();
        _objectAnimator.SetBool("isRunning", true);

        hotSpot = new Vector2(cursorTexture[0].width / 2 , cursorTexture[0].height / 2);
        Cursor.SetCursor(cursorTexture[0], hotSpot, CursorMode.Auto);

        playerSource = GetComponent<AudioSource>();

        CreateColliderInfoForCoverMode();

        ammoText = GameObject.Find("Ammo Text").GetComponent<TextMeshProUGUI>();
        ammoText.SetText("Bullet " + ammo + " [R]");

        grenadeAmmoText = GameObject.Find("Grenade Ammo Text").GetComponent<TextMeshProUGUI>();
        grenadeAmmoText.SetText("Grenade " + grenadeAmmo);

        lifeText = GameObject.Find("Life Text").GetComponent<TextMeshProUGUI>();
        lifeText.SetText("Life " + life);

        rapidFireCountdownText = GameObject.Find("Rapid Fire Countdown Text").GetComponent<TextMeshProUGUI>();
        rapidFireCountdownText.gameObject.SetActive(false);

    }

    void Update()
    {
        if(!gameOver)
        {
            if(!onPause)
            {
                if (!_onCover)
                {
                    PlayerMove();
                    PlayerLimit();
                    // PlayerOrientation();
                    PlayerFire();
                }
                GetInOrOutCover();
            }
            PlayerPause();
        }
    }

    void PlayerPause()
    {
        if(Input.GetKeyUp(KeyCode.P))
        {
            PauseOn();
            if(Time.timeScale == 0)
            {
                onPause = true;
            }
            else
            {
                onPause = false;
            }
        }
    }

    void PlayerMove()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        transform.Translate(Vector3.right * Time.deltaTime * speed * horizontalInput, Space.World);
    }

    void PlayerLimit()
    {
        if (Math.Abs(transform.position.x) > xLimit)
        {
            transform.position = new Vector3(Math.Sign(transform.position.x) * xLimit, transform.position.y, transform.position.z);
        }
    }

    void PlayerOrientation()
    {        
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // if (plane.Raycast(ray, out float distance))
        // {
        //     Debug.DrawRay(ray.origin, ray.direction * distance, Color.green);

        if(Physics.Raycast(ray, out RaycastHit hit))
        {
            target = ray.GetPoint(hit.distance);
            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red);
        }
        // }

        // orientation = (target - transform.position).normalized;
        // orientation = new Vector3(orientation.x, 0, orientation.z);

        if(hit.collider.CompareTag("Ground"))
        {
            target = new Vector3(target.x, 2.3f, target.z);
        }

        // if (orientation != Vector3.zero)
        // {
        //     transform.rotation = Quaternion.LookRotation(orientation);
        // }
    }

    void PlayerFire()
    {
        if (hasRapidFire)
        {
            if (Input.GetMouseButton(0) && !isReloading)
            {
                SpawnBullet();
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0) && !isReloading)
            {
                SpawnBullet();
            }
        }

        if (Input.GetMouseButtonDown(1) && !isReloading)
        {
            SpawnGrenade();
        }

        if ((ammo == 0 || Input.GetKey(KeyCode.R)) && !isReloading)
        {
            StartCoroutine(ReloadRoutine());
        }
    }

    protected override void GetInOrOutCover()
    {
        if (Input.GetKey(KeyCode.Space) && _numberOfCovers > 0 && !_onCover)
        {
            ModifyColliderAndStance(Vector3.back);
        }

        if (Input.GetKeyUp(KeyCode.Space) && _onCover)
        {
            ModifyColliderAndStance(Vector3.forward);
        }
    }

    void SpawnBullet()
    {
        Vector3 bulletOffset;
        Quaternion bulletAngle;

        if (Time.time - whenWasLastShot > minimumTimeBetweenTwoShots && ammo != 0)
        {
            PlayerOrientation();
            bulletOffset = Vector3.zero/*1.5f * transform.up*/;

            GameObject pooledProjectile = ObjectPooler.SharedInstance.GetPooledObject(7);
            if (pooledProjectile != null)
            {
                pooledProjectile.SetActive(true);
                pooledProjectile.transform.position = gunTransform.position/*transform.position + bulletOffset*/;

                bulletAngle = Quaternion.LookRotation((target - pooledProjectile.transform.position).normalized);

                pooledProjectile.transform.rotation = bulletAngle;

                ammo--;
                ammoText.SetText("Bullet " + ammo + " [R]");
                
                VolumeScaleAdjustment(shot);

                whenWasLastShot = Time.time;
            }
        }
    }

    private void SpawnGrenade()
    {
        float distance;
        Vector3 grenadeOffset, grenadePosition;
        Quaternion grenadeAngle;

        if (Time.time - whenWasLastShot > minimumTimeBetweenTwoShots & grenadeAmmo != 0)
        {
            PlayerOrientation();
            GameObject pooledProjectile = ObjectPooler.SharedInstance.GetPooledObject(9);

            if (pooledProjectile != null)
            {
                distance = Vector3.Distance(target, transform.position);
                grenadeOffset = 1.5f * transform.up + transform.forward * 0.5f;
                grenadePosition = transform.position + grenadeOffset;
                grenadeAngle = Quaternion.LookRotation(((target + Vector3.up * 25) - transform.position).normalized);
                pooledProjectile.SetActive(true);
                pooledProjectile.transform.position = grenadePosition;
                pooledProjectile.transform.rotation = grenadeAngle;
                pooledProjectile.GetComponent<Rigidbody>().AddForce(pooledProjectile.transform.forward * (0.9f * distance), ForceMode.Impulse);
                grenadeAmmo--;
                grenadeAmmoText.SetText("Grenade " + grenadeAmmo);
                whenWasLastShot = Time.time;
            }
        }
    }

    public Vector2 GetHotSpot() => hotSpot;
    public Vector3 GetTarget() => target;
    public void SetHasCover(int i) => _numberOfCovers += i;
    public void StandWalkRun(bool isRunning)
    {
        _objectAnimator.SetBool("isRunning", isRunning);
    }
    public bool GetOnCover() => _onCover;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy Bullet") & !isFlashing)
        {
            other.gameObject.SetActive(false);

            VolumeScaleAdjustment(hurt);
            life--;
            lifeText.SetText("Life " + life);
            StartCoroutine(FlashingRoutine());

            if (life == 0)
            {
                gameOver = true;
                OnGameOver();
                _objectAnimator.SetBool("death", true);
                //playerAnimator.SetInteger("DeathType_int", Random.Range(1, 3));
            }
        }
    }

    void VolumeScaleAdjustment(AudioClip clip)
    {
        playerSource.PlayOneShot(clip, _volumeScale * DataManager.Instance.VolumeScaleFactor);
    }

    public void Life()
    {
        if(life < 3)
        {
            life++;
            lifeText.SetText("Life " + life);
        }
    }

    public void RapidFire()
    {
        StartCoroutine(RapidFireCountdownRoutine());
    }

    public void Grenade()
    {
        if(grenadeAmmo < 3)
        {
            grenadeAmmo++;
            grenadeAmmoText.SetText("Grenade " + grenadeAmmo);
        }
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        yield return new WaitForSeconds(2);
        ammo = hasRapidFire ? 30 : 6;
        ammoText.SetText("Bullet " + ammo + " [R]");
        isReloading = false;
    }

    private IEnumerator RapidFireCountdownRoutine()
    {
        if(!hasRapidFire)
        {
            ammo = 30;
            hasRapidFire = true;
            ammoText.SetText("Bullet " + ammo + " [R]");
            rapidFireCountdown += 30;
            rapidFireCountdownText.gameObject.SetActive(true);
            rapidFireCountdownText.SetText("" + rapidFireCountdown);
            while(rapidFireCountdown != 0)
            {
                yield return new WaitForSeconds(1);
                rapidFireCountdown--;
                rapidFireCountdownText.SetText("" + rapidFireCountdown);
            }
            rapidFireCountdownText.gameObject.SetActive(false);
            hasRapidFire = false;
            ammo = 6;
            ammoText.SetText("Bullet " + ammo + " [R]");
        }
        else
        {
            rapidFireCountdown = 30;
            rapidFireCountdownText.SetText("" + rapidFireCountdown);
        }
    }

    private IEnumerator FlashingRoutine()
    {
        isFlashing = true;

        for (int i = 0; i < 17; i++)
        {
            playerSkinnedMeshRenderer[0].enabled = playerSkinnedMeshRenderer[1].enabled = i % 2 == 0;
            yield return new WaitForSeconds(0.18f);
        }
        isFlashing = false;
        //playerSkinnedMeshRenderer.material = playerMaterial[1];
        //yield return new WaitForSeconds(0.1f);
        //playerSkinnedMeshRenderer.material = playerMaterial[0];
    }
}