using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlaneManager : MonoBehaviour
{
    public bool planing;

    [SerializeField] private float planingSpeed;

    [SerializeField] private float planingRotSpeed;

    [SerializeField] private float gravity;

    [SerializeField] private ClimbManager climbManager;

    [SerializeField] private CharacterMover mover;

    [SerializeField] private SimpleCharacterController simpleCharacterController;

    [SerializeField] private CharacterIK characterIK;

    [SerializeField] private CharacterAnimator characterAnimator;

    [SerializeField] private LayerMask layerMask;

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {
        if (characterAnimator.anim.GetBool("Air") && Input.GetButtonDown("X") && !planing)
        {
            planing = true;
        } 
        else if (planing && Input.GetButtonDown("X"))
        {
            planing = false;
        }

        characterAnimator.anim.SetBool("Brace", planing);

        if (climbManager.onClimbMode)
        {
            planing = false;
        }

        if (planing)
        {
            simpleCharacterController.verticalSpeed = 0;

            Vector3 input = GetInput();

            Vector3 velocity = input * planingSpeed;

            velocity.y = gravity;

            mover.Move(velocity * Time.deltaTime, simpleCharacterController.moveContacts, out simpleCharacterController.contactCount);
            simpleCharacterController.RotateTowards(input * lerpInputSpeed, planingRotSpeed);
        }

        Ray ray = new Ray(transform.position + Vector3.up * 0.5f, Vector3.down);
        bool hit = Physics.Raycast(ray, out RaycastHit hitInfo, 2.5f, layerMask);
        if (hit)
        {
            planing = false;
        }
    }

    private Vector3 currentInput;
    [SerializeField] private float lerpInputSpeed;

    private Vector3 GetInput()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        input = input.normalized;

        Vector3 realInput = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z) * input.y + new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z) * input.x;

        currentInput = Vector3.Lerp(currentInput, realInput, lerpInputSpeed * Time.deltaTime);

        return currentInput;
    }
}
