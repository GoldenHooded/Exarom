using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
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

    private Transform cameraTransform;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    private void Update()
    {


        if (climbManager.onClimbMode)
        {
            planing = false;
        }

        if (planing)
        {
            Vector3 input = GetInput();

            Vector3 velocity = input * planingSpeed;

            velocity.y = gravity;

            mover.Move(velocity * Time.deltaTime, simpleCharacterController.moveContacts, out simpleCharacterController.contactCount);
            simpleCharacterController.RotateTowards(input * lerpInputSpeed, planingRotSpeed);
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
