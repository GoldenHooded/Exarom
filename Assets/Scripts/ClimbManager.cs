using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbManager : MonoBehaviour
{
    public bool onClimbMode;
    public bool canClimb;
    [SerializeField] private float staminaSpentSpeed;

    [Space(10)]
    [SerializeField] private SimpleCharacterController characterController;
    [SerializeField] private PlayerValues playerValues;
    [SerializeField] private CharacterCapsule capsule;
    [SerializeField] private CharacterAnimator characterAnimator;
    [SerializeField] private CharacterMover characterMover;

    [Space(10)]
    [SerializeField] private float realRayOffset;
    private Vector3 rayOffset;
    [SerializeField] private LayerMask climbable;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float smoothSpeed;
    [SerializeField] private float smoothSpeedRotation;
    private Vector3 desiredPosition;
    private Vector3 desiredNormal;

    private void Update()
    {
        rayOffset = realRayOffset * transform.up;

        if (!canClimb) onClimbMode = false;
        if (onClimbMode) { characterController.canMove = false; characterController.canRotate = false; }
        if (playerValues.stamina <= 0) { canClimb = false; } 
        else { canClimb = true; } //<== Add conditions in order to enable climb mode

        if (onClimbMode)
        {
            playerValues.stamina -= staminaSpentSpeed * Time.deltaTime;

            ClimbMove();
        }
        else
        {
            firstTime = true;
        }

        CheckClimb();
    }

    private void FixedUpdate()
    {
        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);
        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);
        if (onClimbMode && ray1)
        {
            AdjustRotation(hit);
        }
    }

    public bool keyTrigger;
    public bool firstTime = true;
    private bool triggerNotClimb = false;
    private void CheckClimb()
    {
        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);
        Ray forwardRay2 = new Ray(transform.position + rayOffset * 1.8f, transform.forward);
        Ray downRay = new Ray(transform.position, - transform.up);
        Ray upRay = new Ray(transform.position + rayOffset * 2.5f + new Vector3(transform.forward.x, 0, transform.forward.z) * 0.6f, Vector3.down);
        Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.blue);
        Debug.DrawRay(forwardRay2.origin, forwardRay2.direction, Color.blue);
        Debug.DrawRay(upRay.origin, upRay.direction, Color.blue);
        Debug.DrawRay(downRay.origin, downRay.direction, Color.red);

        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);
        bool ray2 = Physics.Raycast(forwardRay2, out RaycastHit hit2, 1, climbable);
        bool ray3 = Physics.Raycast(downRay, out RaycastHit hit3, 0.325f, climbable);
        if (ray1 && ray2 && !hit.collider.CompareTag("Unclimbable") && !hit2.collider.CompareTag("Unclimbable"))
        {
            if (!onClimbMode && canClimb)
            {
                if (Input.GetKey(KeyCode.W) && !keyTrigger)
                {
                    keyTrigger = true;

                    Invoke("CheckKeyTrigger", 0.5f);
                }
                else
                {
                    keyTrigger = false;
                } 
            }
        } 
        else if (ray1 && !ray2)
        {
            onClimbMode = false; //Temporal
        }
        else
        {
            onClimbMode = false;
        }

        if (ray3 && !firstTime)
        {
            onClimbMode = false;
        }
        else if (!ray3 && firstTime)
        {
            firstTime = false;
        }

        if (onClimbMode)
        {
            triggerNotClimb = false;

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                onClimbMode = false;
            }
        }
        else if (!triggerNotClimb)
        {
            transform.forward = new Vector3(transform.forward.x, 0, transform.forward.z);
        }
    }
    
    private void AdjustRotation(RaycastHit hit)
    {
        desiredNormal = -hit.normal;
        transform.forward = Vector3.Lerp(transform.forward, desiredNormal, smoothSpeedRotation * Time.deltaTime);
        desiredPosition = (hit.point - rayOffset) + hit.normal * (capsule.Radius + capsule.contactOffset);

        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
    }

    private void CheckKeyTrigger()
    {
        if (keyTrigger)
        {
            onClimbMode = true;
        }
    }

    private void ClimbMove()
    {
        Ray forwardRay3_L = new Ray(transform.position + rayOffset - transform.right * 0.6f, transform.forward);
        Ray forwardRay4_R = new Ray(transform.position + rayOffset + transform.right * 0.6f, transform.forward);
        Debug.DrawRay(forwardRay3_L.origin, forwardRay3_L.direction, Color.red);
        Debug.DrawRay(forwardRay4_R.origin, forwardRay4_R.direction, Color.red);

        bool left = Physics.Raycast(forwardRay3_L, out RaycastHit hit, 1, climbable);
        bool right = Physics.Raycast(forwardRay4_R, out RaycastHit hit2, 1, climbable);

        Vector3 input = GetClimbMoveInput(left, right);
        Vector3 velocity = input * moveSpeed;
        characterController.mover.Move(velocity * Time.deltaTime, characterController.moveContacts, out characterController.contactCount);
    }

    private Vector3 GetClimbMoveInput(bool left, bool right)
    {
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        characterAnimator.anim.SetFloat("XInput", Input.GetAxis("Horizontal"));
        characterAnimator.anim.SetFloat("YInput", Input.GetAxis("Vertical"));

        if (!left)
        {
            x = Mathf.Clamp(x, 0, 1);
        }
        if (!right)
        {
            x = Mathf.Clamp(x, -1, 0);
        }

        return transform.TransformDirection(new Vector3(x, y).normalized);
    }
}