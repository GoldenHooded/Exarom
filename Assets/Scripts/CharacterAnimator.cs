using MenteBacata.ScivoloCharacterController;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator cameraAnim;

    [SerializeField] private CharacterMover mover;

    void LateUpdate()
    {
        cameraAnim.SetBool("Air", !mover.isInWalkMode);
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            cameraAnim.SetBool("Walk", true);
            cameraAnim.SetBool("Run", Input.GetKey(KeyCode.LeftShift)); 
        }
        else
        {
            cameraAnim.SetBool("Walk", false);
            cameraAnim.SetBool("Run", false);
        }
    }
}
