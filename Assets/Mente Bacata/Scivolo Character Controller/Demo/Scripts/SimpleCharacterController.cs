//#define MB_DEBUG

using MenteBacata.ScivoloCharacterController;
using System.Collections.Generic;
using UnityEngine;

namespace MenteBacata.ScivoloCharacterControllerDemo
{
    public class SimpleCharacterController : MonoBehaviour
    {
        public bool canMove = true;

        public bool canRotate = true;

        public float slowMoveSpeed = 2f;

        public float normalMoveSpeed = 5f;

        public float fastMoveSpeed = 8f;

        public float realMoveSpeed = 5f;

        public float jumpSpeed = 8f;
        
        public float rotationSpeed = 720f;

        public float gravity = -25f;

        public float hardJumpTresshold = -5;

        public CharacterMover mover;

        public GroundDetector groundDetector;

        public MeshRenderer groundedIndicator;

        public CharacterAnimator characterAnimator;

        private const float minVerticalSpeed = -20f;

        // Allowed time before the character is set to ungrounded from the last time he was safely grounded.
        private const float timeBeforeUngrounded = 0.02f;

        // Speed along the character local up direction.
        public float verticalSpeed = 0f;

        // Time after which the character should be considered ungrounded.
        private float nextUngroundedTime = -1f;

        private Transform cameraTransform;

        private int jumpsLeft = 2;
        
        [HideInInspector] public MoveContact[] moveContacts = CharacterMover.NewMoveContactArray;

        [HideInInspector] public int contactCount;

        private bool isOnMovingPlatform = false;

        private MovingPlatform movingPlatform;

        public Vector3 velocity;

        [SerializeField] private ClimbManager climbManager;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            cameraTransform = Camera.main.transform;
            mover.canClimbSteepSlope = true;
        }

        private void Update()
        {
            if (climbManager.onClimbMode) //GRAVITY
            {
                gravity = 0f;
                mover.isInWalkMode = false;
            }
            else
            {
                gravity = -22.575f;
            }

            float deltaTime = Time.deltaTime;
            Vector3 movementInput = GetMovementInput();

            velocity = realMoveSpeed * movementInput;

            bool groundDetected = DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo);

            SetGroundedIndicatorColor(isGrounded);

            isOnMovingPlatform = false;

            

            if (canMove)
            {
                if (isGrounded && Input.GetButtonDown("X") /*|| jumpsLeft > 0 && Input.GetButtonDown("X")*/) //<== Double Jump
                {
                    GetComponent<CharacterAnimator>().Jump();
                    verticalSpeed = jumpSpeed;
                    nextUngroundedTime = -1f;
                    jumpsLeft--;
                    isGrounded = false;
                }

                if (isGrounded)
                {
                    jumpsLeft = 2;
                    mover.isInWalkMode = true;
                    verticalSpeed = 0f;

                    if (groundDetected)
                        isOnMovingPlatform = groundInfo.collider.TryGetComponent(out movingPlatform);
                }
                else
                {
                    mover.isInWalkMode = false;

                    BounceDownIfTouchedCeiling();

                    verticalSpeed += gravity * deltaTime;

                    if (verticalSpeed < minVerticalSpeed)
                        verticalSpeed = minVerticalSpeed;

                    velocity += verticalSpeed * transform.up;
                }

                mover.Move(velocity * deltaTime, moveContacts, out contactCount);
            }

            if (canRotate)
            {
                RotateTowards(velocity);
            }

            if (verticalSpeed < hardJumpTresshold)
            {
                characterAnimator.anim.SetBool("HardLand", true);
            }

            if (climbManager.onClimbMode)
            {
                verticalSpeed = 0;
            }
        }

        private void LateUpdate()
        {
            if (isOnMovingPlatform)
                ApplyPlatformMovement(movingPlatform);
        }

        private Vector3 GetMovementInput()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float y = Input.GetAxisRaw("Vertical");

            Vector2 xy = new Vector2(x, y).normalized;

            Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, transform.up).normalized;
            Vector3 right = Vector3.Cross(transform.up, forward);

            return xy.x * right + xy.y * forward;
        }

        private bool DetectGroundAndCheckIfGrounded(out bool isGrounded, out GroundInfo groundInfo)
        {
            bool groundDetected = groundDetector.DetectGround(out groundInfo);

            if (groundDetected)
            {
                if (groundInfo.isOnFloor && verticalSpeed < 0.1f)
                    nextUngroundedTime = Time.time + timeBeforeUngrounded;
            }
            else
                nextUngroundedTime = -1f;

            isGrounded = Time.time < nextUngroundedTime;
            return groundDetected;
        }

        private void SetGroundedIndicatorColor(bool isGrounded)
        {
            if (groundedIndicator != null)
                groundedIndicator.material.color = isGrounded ? Color.green : Color.blue;
        }

        public void RotateTowards(in Vector3 direction)
        {
            Vector3 flatDirection = Vector3.ProjectOnPlane(direction, transform.up);

            if (flatDirection.sqrMagnitude < 1E-06f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(flatDirection, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void RotateTowards(in Vector3 direction, float speed)
        {
            Vector3 flatDirection = Vector3.ProjectOnPlane(direction, transform.up);

            if (flatDirection.sqrMagnitude < 1E-06f)
                return;

            Quaternion targetRotation = Quaternion.LookRotation(flatDirection, transform.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, speed * Time.deltaTime);
        }

        private void ApplyPlatformMovement(MovingPlatform movingPlatform)
        {
            GetMovementFromMovingPlatform(movingPlatform, out Vector3 movement, out float upRotation);

            transform.Translate(movement, Space.World);
            transform.Rotate(0f, upRotation, 0f, Space.Self);
        }

        private void GetMovementFromMovingPlatform(MovingPlatform movingPlatform, out Vector3 movement, out float deltaAngleUp)
        {
            movingPlatform.GetDeltaPositionAndRotation(out Vector3 platformDeltaPosition, out Quaternion platformDeltaRotation);
            Vector3 localPosition = transform.position - movingPlatform.transform.position;
            movement = platformDeltaPosition + platformDeltaRotation * localPosition - localPosition;

            platformDeltaRotation.ToAngleAxis(out float platformDeltaAngle, out Vector3 axis);
            float axisDotUp = Vector3.Dot(axis, transform.up);

            if (-0.1f < axisDotUp && axisDotUp < 0.1f)
                deltaAngleUp = 0f;
            else
                deltaAngleUp = platformDeltaAngle * Mathf.Sign(axisDotUp);
        }
        
        private void BounceDownIfTouchedCeiling()
        {
            for (int i = 0; i < contactCount; i++)
            {
                if (Vector3.Dot(moveContacts[i].normal, transform.up) < -0.7f)
                {
                    verticalSpeed = -0.25f * verticalSpeed;
                    break;
                }
            }
        }
    }
}
