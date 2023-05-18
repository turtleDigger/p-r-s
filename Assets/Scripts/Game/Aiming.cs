using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    private float targetYAngle, targetDistance;
    private Vector3 target, orientation, oldTarget;
    private Animator playerAnimator;
    private PlayerController playerScript;
    void Start()
    {
        playerAnimator = GetComponent<Animator>();
        playerScript = GetComponentInParent<PlayerController>();
    }

    private void OnAnimatorIK()
    {
        target = playerScript.GetTarget();
        orientation = (target - transform.position).normalized;
        targetYAngle = Quaternion.LookRotation(orientation).eulerAngles.y;
        targetYAngle = Quaternion.Angle(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, targetYAngle, 0));
        targetDistance = Vector3.Distance(transform.position, target);

        if(!playerScript.GetOnCover())
        {
            playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
            playerAnimator.SetLookAtWeight(1);

            if(targetYAngle < 70 & targetDistance > 2.9f)
            {
                playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, target);
                playerAnimator.SetLookAtPosition(target);
                oldTarget = target;
            }
            else
            {
                playerAnimator.SetIKPosition(AvatarIKGoal.RightHand, oldTarget);
                playerAnimator.SetLookAtPosition(oldTarget);
            }
        }
        // else
        // {
        //     playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
        //     playerAnimator.SetLookAtWeight(0);
        // }
    }
}
