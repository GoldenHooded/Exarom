using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System;
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

    [HideInInspector] public bool topClimb;
    [HideInInspector] public bool finishedTC;

    [Space(10)]
    public Vector3 transformDesiredPos;
    [SerializeField] private float speedChangePos;
    [SerializeField] private float minAngle = 50f;

    [Space(10)]
    public bool braced;
    public Vector3 destintationBraced;
    [SerializeField] private float bracedSpeed;

    private void Update()
    {
        rayOffset = realRayOffset * transform.up;

        if (!canClimb) onClimbMode = false;
        if (onClimbMode) { characterController.canMove = false; characterController.canRotate = false; }
        if (playerValues.stamina <= 0) { canClimb = false; onClimbMode = false; topClimb = false; } 
        else { canClimb = true; } //<== Add conditions in order to enable climb mode

        if (!topClimb && !braced)
        {
            if (onClimbMode)
            {
                playerValues.stamina -= staminaSpentSpeed * Time.deltaTime;

                ClimbMove();

                characterAnimator.anim.SetBool("HardLand", false);

                if (Input.GetButtonDown("X"))
                {
                    characterAnimator.anim.SetTrigger("Brace");
                }
            }
            else
            {
                firstTime = true;
            }

            CheckClimb();
            tcTrigger = false;
        }
        else if (topClimb)
        {
            TopClimb();
            SemiCheckClimb();
        } 
        else if (braced)
        {
            Braced();
            CheckClimb();
        }
    }

    private void Braced()
    {
        transform.position = Vector3.Lerp(transform.position, destintationBraced, Time.deltaTime * bracedSpeed);

        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);

        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);

        if (!ray1)
        {
            braced = false;
        }
    }

    private void FixedUpdate()
    {
        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);
        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);
        if (onClimbMode && ray1 && !topClimb)
        {
            AdjustRotation(hit);
        }
    }

    private void SemiCheckClimb()
    {
        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);
        Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.blue);
        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);

        if (!ray1)
        {
            Invoke("StopClimb", 1f);
        }
    }

    private void StopClimb()
    {
        onClimbMode = false;
    }

    private Vector3 finalPos;
    private bool tcTrigger;
    private void TopClimb()
    {
        if (!tcTrigger) 
        { 
            characterAnimator.anim.SetTrigger("TopClimb");
            tcTrigger = true;
        }

        if (finishedTC)
        {
            transform.position = Vector3.Lerp(transform.position, finalPos, Time.deltaTime * 20);
            if (Vector3.Distance(transform.position, finalPos) < 0.2f)
            {
                topClimb = false;
                onClimbMode = false;
            }
        }
    }

    public bool keyTrigger;
    public bool firstTime = true;
    private bool triggerNotClimb = false;
    private void CheckClimb()
    {
        Ray forwardRay = new Ray(transform.position + rayOffset, transform.forward);
        Ray forwardRay2 = new Ray(transform.position + rayOffset * 1.8f, new Vector3(transform.forward.x * 2, 0, transform.forward.z * 2));
        Ray downRay = new Ray(transform.position, - transform.up);
        Ray upRay = new Ray(transform.position + rayOffset * 2.5f + new Vector3(transform.forward.x, 0, transform.forward.z) * 0.6f, Vector3.down);
        Debug.DrawRay(forwardRay.origin, forwardRay.direction, Color.blue);
        Debug.DrawRay(forwardRay2.origin, forwardRay2.direction, Color.blue);
        Debug.DrawRay(upRay.origin, upRay.direction, Color.blue);
        Debug.DrawRay(downRay.origin, downRay.direction, Color.red);

        bool ray1 = Physics.Raycast(forwardRay, out RaycastHit hit, 1, climbable);
        bool ray2 = Physics.Raycast(forwardRay2, out RaycastHit hit2, 2.5f, climbable);
        bool ray5 = Physics.Raycast(forwardRay2, out RaycastHit hit5, 2, climbable);
        bool ray3 = Physics.Raycast(downRay, 0.325f, climbable);
        bool ray4 = Physics.Raycast(upRay, out RaycastHit hit4, 1, climbable);

        Ray forwardRay3_L = new Ray(transform.position + rayOffset - transform.right * 0.6f, transform.forward);
        Ray forwardRay4_R = new Ray(transform.position + rayOffset + transform.right * 0.6f, transform.forward);

        bool left = Physics.Raycast(forwardRay3_L, 1, climbable);
        bool right = Physics.Raycast(forwardRay4_R, 1, climbable);

        bool bool1 = left || right;

        if (ray1 && ray2 && hit.collider == hit2.collider && bool1 && Vector3.Angle(hit.normal, Vector3.up) > minAngle ||
            ray1 && ray2 && hit.collider != hit2.collider && bool1 && Vector3.Angle(hit.normal, Vector3.up) > minAngle && Vector3.Angle(hit.normal, Vector3.up) == Vector3.Angle(hit2.normal, Vector3.up))
        {
            if (!hit.collider || hit.collider && !hit.collider.CompareTag("Unclimbable"))
            {
                if (!onClimbMode && canClimb)
                {
                    if (Input.GetKey(KeyCode.W) && !keyTrigger)
                    {
                        keyTrigger = true;

                        Invoke("CheckKeyTrigger", 0.2f);
                    }
                    else
                    {
                        keyTrigger = false;
                    }
                } 
            }
        }
        else if (ray1 && !ray2 && onClimbMode && ray4 || ray1 && ray2 && onClimbMode && ray4 && hit.collider != hit2.collider)
        {
            finalPos = hit4.point;
            topClimb = true;
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