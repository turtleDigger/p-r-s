using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    protected bool _onCover/*, isReloading, hasRapidFire, isFlashing*/;
    protected int /*life, ammo, grenade, rapidFireCountdown,*/ _numberOfCovers;
    // float speed, whenWasLastShot, minimumTimeBetweenTwoShots, volumeScale;
    protected Vector3 /*target, orientation,*/ _colliderSize, _colliderCenter, _colliderCoverSize, _colliderCoverCenter;
    // Transform gunTranform;
    protected BoxCollider _objectCollider;
    // AudioSource objectAudioSource;
    protected Animator _objectAnimator;
    // SkinnedMeshRenderer objectSMR;
    // Material[] objectMaterials;

    protected void CreateColliderInfoForCoverMode()
    {
        _objectCollider = GetComponent<BoxCollider>();
        _colliderSize = _objectCollider.size;
        _colliderCenter = _objectCollider.center;
        _colliderCoverSize = new Vector3(_objectCollider.size.x, 1.1f, _objectCollider.size.z);
        _colliderCoverCenter = new Vector3(_objectCollider.center.x, 0.05f, _objectCollider.center.z);
    }

    protected abstract void GetInOrOutCover();

    protected void GetInCover()
    {
        _objectCollider.center = _colliderCoverCenter;
        _objectCollider.size = _colliderCoverSize;
        _onCover = true;
        _objectAnimator.SetBool("isCrouch", true);
        transform.rotation = Quaternion.LookRotation(Vector3.back);
    }

    protected void GetOutCover(Vector3 orientation)
    {
        _objectCollider.center = _colliderCenter;
        _objectCollider.size = _colliderSize;
        _onCover = false;
        _objectAnimator.SetBool("isCrouch", false);
        transform.rotation = Quaternion.LookRotation(orientation);
    }
}
