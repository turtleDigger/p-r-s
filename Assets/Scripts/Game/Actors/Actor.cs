using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public abstract class Actor : MonoBehaviour
{
    protected bool _onCover/*, isReloading, hasRapidFire, isFlashing*/;
    protected int /*life, ammo, grenade, rapidFireCountdown,*/ _numberOfCovers;
    // float speed, whenWasLastShot, minimumTimeBetweenTwoShots, volumeScale;
    // protected Vector3 target, orientation;
    protected Vector3[] _sizeForCover, _centerForCover;
    // Transform gunTranform;
    protected BoxCollider _objectCollider;
    // AudioSource objectAudioSource;
    protected Animator _objectAnimator;
    // SkinnedMeshRenderer objectSMR;
    // Material[] objectMaterials;
    
    // Cette méthode stocke les valeurs size et center du BoxCollider du gameObject.
    // Puis elle crée en conséquences les valeurs size et center que devra prendre le BoxCollider du gameObject quand il sera en mode couverture.
    protected void CreateColliderInfoForCoverMode()
    {
        _sizeForCover = new Vector3[2];
        _centerForCover = new Vector3[2];

        _objectCollider = GetComponent<BoxCollider>();

        _sizeForCover[0] = _objectCollider.size;
        _centerForCover[0] = _objectCollider.center;
        _sizeForCover[1] = new Vector3(_objectCollider.size.x, 1.1f, _objectCollider.size.z);
        _centerForCover[1] = new Vector3(_objectCollider.center.x, 0.05f, _objectCollider.center.z);
    }

    protected abstract void GetInOrOutCover();

    // Cette méthode modifie les valeurs du BoxCollider, de l'Animator et de la rotation du gameObject.
    // Il n'y a que deux valeurs possibles pour le BoxCollider et l'Animator régies par la valeur de _onCover.
    // Elle prend en paramètre un Vector3 qui détermine la direction dans laquelle pointe le gameObject.
    protected void ModifyColliderAndStance(Vector3 orientation)
    {
        _onCover = !_onCover;

        int stanceMode = Convert.ToByte(_onCover);

        _objectCollider.center = _centerForCover[stanceMode];
        _objectCollider.size = _sizeForCover[stanceMode];

        _objectAnimator.SetBool("isCrouch", _onCover);

        transform.rotation = Quaternion.LookRotation(orientation);
    }
}
