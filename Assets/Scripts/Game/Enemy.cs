using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Actor
{
    public AudioClip shot, hurt;
    //public GameObject[] stepOfPath;

    private bool fear, gameOver, justCreated = true, isDying = false, isFlying = false;
    private int hasCover, enemyCount/*, enemyType, step, speed*/;
    private readonly float minimumTimeBetweenTwoShots = 0.5f, _volumeScale = 0.5f, _choirTerm = 0.05f;
    private readonly Vector3 childInit = new Vector3(0, -0.5f, 0);
    private SkinnedMeshRenderer[] enemySkinnedMeshRenderer;
    private AudioSource enemySource;
    private Rigidbody enemyRigidbody;
    private PlayerController playerScript;
    private GameManager gameManagerScript;

    void OnEnable()
    {
        PlayerController.OnGameOver += StopTactics;

        enemyCount = FindObjectsOfType<Enemy>().Length;

        if(!justCreated)
        {
            isFlying = false;
            isDying = false;
            fear = false;
            enemySkinnedMeshRenderer[0].enabled = enemySkinnedMeshRenderer[1].enabled = enemySkinnedMeshRenderer[2].enabled = true;
            _objectCollider.enabled = true;
            enemyRigidbody.useGravity = true;
        }
        StartTactics();
    }

    void OnDisable()
    {
        PlayerController.OnGameOver -= StopTactics;

        transform.GetChild(0).transform.localPosition = childInit;
        enemyRigidbody.velocity = Vector3.zero;
        enemyRigidbody.angularVelocity = Vector3.zero;

        hasCover = 0;

        StopTactics();
    }

    void Awake() // Awake() pour les ennemies spéciaux dans les maps tutoriels. Les BoxColliders ne sont affectées assez vite.
    {
        _objectAnimator = transform.GetChild(0).GetComponent<Animator>();
        enemySkinnedMeshRenderer = new SkinnedMeshRenderer[3];
        enemySkinnedMeshRenderer[0] = transform.GetChild(0).GetChild(3).GetComponent<SkinnedMeshRenderer>();
        enemySkinnedMeshRenderer[1] = transform.GetChild(0).GetChild(4).GetComponent<SkinnedMeshRenderer>();
        enemySkinnedMeshRenderer[2] = transform.GetChild(0).GetChild(5).GetComponent<SkinnedMeshRenderer>();

        enemySource = GetComponent<AudioSource>();

        enemyRigidbody = GetComponent<Rigidbody>();

        CreateColliderInfoForCoverMode();

        gameManagerScript = GameObject.Find("Game Manager").GetComponent<GameManager>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerController>();

        // if(enemyType == 1)
        // {
        //     StartCoroutine(GetAllStepOfPath());
        // }

        //InvokeRepeating("Tactics", Random.Range(1, 7), minimumTimeBetweenTwoShots * 3 + 2);
    }

    void Start()
    {
        justCreated = false;
    }

    // void Update()
    // {
    //     if (enemyType == 1 && !onCover && !gameManagerScript.GetIsScrolling() && !gameOver)
    //     {
    //         EnemyMove();
    //     }
    // }

    public void SetHasCover(int i) => hasCover += i;
    public void SetFear() => fear = true;
    void Tactics() => StartCoroutine(EnemyFire());

    void StartTactics()
    {
        gameOver = false;
        InvokeRepeating("Tactics", Random.Range(1, 7), minimumTimeBetweenTwoShots * 3 + 2);
    }

    void StopTactics()
    {
        gameOver = true;
        CancelInvoke("Tactics");
    }

    // void EnemyMove()
    // {
    //     Vector3 stepDirection = (stepOfPath[step].transform.position - transform.position).normalized;
    //     transform.Translate(stepDirection * Time.deltaTime * speed, Space.World);
    // }

    void EnemyOrientation()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 target, orientation;

        if (playerScript != null && !_onCover)
        {
            target = playerScript.transform.position;
            target = new Vector3(target.x, 0.5f, target.z);

            if (Math.Abs(horizontalInput) > 0)
            {
                if (horizontalInput < 0)
                {
                    target += 5 * Vector3.left;
                }
                else
                {
                    target += 5 * Vector3.right;
                }
            }
            orientation = (target - transform.position);
        }
        else
        {
            orientation = Vector3.back;
        }

        transform.rotation = Quaternion.LookRotation(orientation);
    }

    protected override void GetInOrOutCover()
    {
        if (_onCover && !fear)
        {
            ModifyColliderAndStance(Vector3.back);
        }
        else
        {
            ModifyColliderAndStance(Vector3.back);
        }
    }

    private IEnumerator EnemyFire()
    {
        Vector3 bulletOffset;
        Quaternion bulletAngle;

        if (_onCover)
        {
            GetInOrOutCover();
        }

        for(int i = 0; i < 3 && !gameOver && !fear; i++)
        {
            bulletAngle = transform.rotation;
            
            if (playerScript.GetOnCover())
            {
                bulletAngle *= Quaternion.Euler(1, 0, 0);
            }

            bulletOffset = 1.5f * transform.up;

            EnemyOrientation();

            GameObject pooledProjectile = ObjectPooler.SharedInstance.GetPooledObject(8);
            if (pooledProjectile != null)
            {
                pooledProjectile.SetActive(true);
                pooledProjectile.transform.position = transform.position + bulletOffset;
                pooledProjectile.transform.rotation = bulletAngle;
        
                VolumeScaleAdjustment(shot);

                yield return new WaitForSeconds(minimumTimeBetweenTwoShots);
            }
        }

        if (hasCover != 0)
        {
            GetInOrOutCover();
        }
    }

    // IEnumerator GetAllStepOfPath()
    // {
    //     yield return new WaitForSeconds(1);

    //     int stepNumber = GameObject.FindGameObjectsWithTag("Step").Length;
    //     stepOfPath = new GameObject[stepNumber];

    //     for (int i = 0; i < stepNumber; i++)
    //     {
    //         stepOfPath[i] = GameObject.Find("Step " + i);
    //     }
    // }

    public void ExplosionForce(Vector3 position, int power)
    {
        float distance = Vector3.Distance(position, transform.position);
        Vector3 forceDirection = (transform.position- position).normalized;

        enemyRigidbody.AddForce(forceDirection * ((power * 500) / distance), ForceMode.Impulse);

        isFlying = true;
    }
    public bool GetIsDying()
    {
        return isDying;
    }
    public void Death() => StartCoroutine(DeathCoroutine());

    void VolumeScaleAdjustment(AudioClip clip)
    {
        if (enemyCount > 8)
        {
            enemySource.PlayOneShot(clip, (_volumeScale - _choirTerm * 8) * DataManager.Instance.VolumeScaleFactor);
        }
        else
        {
            enemySource.PlayOneShot(clip, (_volumeScale - _choirTerm * (enemyCount - 1)) * DataManager.Instance.VolumeScaleFactor);
        }
    }

    private IEnumerator DeathCoroutine()
    {
        isDying = true;
        
        VolumeScaleAdjustment(hurt);

        gameManagerScript.UpdateScoreText(100, transform.position);
        StopTactics();

        if(isFlying)
        {
            yield return new WaitForSeconds(2);
        }

        enemyRigidbody.useGravity = false;
        _objectCollider.enabled = false;

        if(!isFlying)
        {
            _objectAnimator.SetBool("death", true);
        }
        yield return new WaitForSeconds(2);
        enemySkinnedMeshRenderer[0].enabled = enemySkinnedMeshRenderer[1].enabled = enemySkinnedMeshRenderer[2].enabled = false;

        for (int i = 1; i < 7; i++)
        {
            yield return new WaitForSeconds(0.18f);

            enemySkinnedMeshRenderer[0].enabled = enemySkinnedMeshRenderer[1].enabled = enemySkinnedMeshRenderer[2].enabled = i % 2 != 0;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        // if (other.CompareTag("Step"))
        // {
        //     if (step < stepOfPath.Length - 1)
        //     {
        //         step++;
        //     }
        //     else
        //     {
        //         step = 0;
        //     }
        // }

        if (other.CompareTag("Player Bullet"))
        {
            other.gameObject.SetActive(false);
            StartCoroutine(DeathCoroutine());
        }
    }

    private void OnMouseEnter()
    {
        Cursor.SetCursor(playerScript.cursorTexture[1], playerScript.GetHotSpot(), CursorMode.Auto);
    }

    private void OnMouseExit()
    {
        Cursor.SetCursor(playerScript.cursorTexture[0], playerScript.GetHotSpot(), CursorMode.Auto);
    }
}
