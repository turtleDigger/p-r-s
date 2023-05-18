using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chill : MonoBehaviour
{
    private Animator noShoesGuyAnimator;
    void Start()
    {
        noShoesGuyAnimator = GetComponent<Animator>();
        noShoesGuyAnimator.SetBool("isCrouch", true);
    }
}
