using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterIK : MonoBehaviour
{
    public bool preventMoving;

    public bool preventRotation;

    public float IKLeftFootWeight;

    public float IKRightFootWeight;

    public float modelYOffset;

    [SerializeField] private CharacterAnimator characterAnim;

    [SerializeField] private Transform modelTransform;

    [SerializeField] private Transform offsetTransform;

    [SerializeField] private float speed;

    [SerializeField] private Vector3 toGoPos;

    [SerializeField] private Transform playerTransform;

    [SerializeField] private SimpleCharacterController controller;

    [SerializeField] private ClimbManager climbManager;

    private bool toGoPosTrigger;

    public Vector3 topClimbPos;

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 leastY = modelTransform.position;

        characterAnim.anim.SetIKPositionWeight(AvatarIKGoal.LeftFoot, IKLeftFootWeight);
        characterAnim.anim.SetIKRotationWeight(AvatarIKGoal.LeftFoot, IKLeftFootWeight);

        if (Physics.Raycast(characterAnim.anim.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down, out RaycastHit hitInfo, characterAnim.distanceToGround + 1.35f, characterAnim.collisionLayer))
        {
            Vector3 footPos = hitInfo.point;
            footPos.y += characterAnim.distanceToGround;
            characterAnim.anim.SetIKPosition(AvatarIKGoal.LeftFoot, footPos);
            characterAnim.anim.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(transform.forward, hitInfo.normal));

            if (hitInfo.point.y < leastY.y)
            {
                leastY.y = hitInfo.point.y;
            }
        }

        characterAnim.anim.SetIKPositionWeight(AvatarIKGoal.RightFoot, IKRightFootWeight);
        characterAnim.anim.SetIKRotationWeight(AvatarIKGoal.RightFoot, IKRightFootWeight);

        if (Physics.Raycast(characterAnim.anim.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down, out RaycastHit hitInfo2, characterAnim.distanceToGround + 1.35f, characterAnim.collisionLayer))
        {
            Vector3 footPos = hitInfo2.point;
            footPos.y += characterAnim.distanceToGround;
            characterAnim.anim.SetIKPosition(AvatarIKGoal.RightFoot, footPos);
            characterAnim.anim.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(transform.forward, hitInfo2.normal));

            if (hitInfo2.point.y < leastY.y)
            {
                leastY.y = hitInfo2.point.y;
            }
        }

        toGoPos = leastY;

        if (preventMoving)
        {
            modelTransform.localPosition = Vector3.zero;
        }
        else
        {
            if (characterAnim.anim.GetBool("Walk") || characterAnim.anim.GetBool("Air"))
            {
                toGoPos = new Vector3(playerTransform.position.x, playerTransform.position.y - 0.01f, playerTransform.position.z);
                toGoPosTrigger = false; 
            }
            else if (!toGoPosTrigger)
            {
                toGoPos = new Vector3(playerTransform.position.x, playerTransform.position.y - 0.01f, playerTransform.position.z);
                Invoke("ToGoPosTrigger", 0.1f);
                Invoke("ToGoPosTrigger", 0.25f);

                toGoPosTrigger = true;
            }

            modelTransform.position = Vector3.Lerp(transform.position, toGoPos, speed * Time.deltaTime);
        }

        offsetTransform.localPosition = new Vector3(offsetTransform.localPosition.x, modelYOffset, offsetTransform.localPosition.z);

        if (climbManager.onClimbMode || lockedMove)
        {
            preventMoving = true;
            preventRotation = true;
        }
        else
        {
            preventMoving = false;
            preventRotation = false;
        }
    }

    private void Update()
    {
        if (climbManager.topClimb)
        {
            transform.localPosition = topClimbPos;
        }

        climbManager.finishedTC = finishedTopClimb;
    }

    private bool lockedMove;

    private Vector3 startPos;
    public void SaveStartPos()
    {
        startPos = playerTransform.position;
        finishedTopClimb = false;
    }

    private bool finishedTopClimb;

    public void FinishedClimb()
    {
        finishedTopClimb = true;
    }

    public void LockMove()
    {
        lockedMove = true;
    }

    public void UnlockMove()
    {
        lockedMove = false;
    }

    public void LockRotation()
    {
        preventRotation = true;
    }

    public void UnlockRotation()
    {
        preventRotation = false;
    }

    private void ToGoPosTrigger()
    {
        toGoPos = new Vector3(playerTransform.position.x, playerTransform.position.y - 0.01f, playerTransform.position.z);
    }

    public void ResetHardJump()
    {
        characterAnim.anim.SetBool("HardLand", false);
    }
}
