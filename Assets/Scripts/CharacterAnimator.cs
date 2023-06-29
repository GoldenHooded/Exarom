using MenteBacata.ScivoloCharacterController;
using MenteBacata.ScivoloCharacterControllerDemo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    [SerializeField] private Animator cameraAnim;

    [SerializeField] private CharacterMover mover;

    [SerializeField] private SimpleCharacterController characterController;

    [SerializeField] private CharacterCapsule characterCapsule;

    [SerializeField] private float normalHeight;

    [SerializeField] private float crouchedHeight;

    [SerializeField] float blendSmoothness = 5f;

    [SerializeField] LayerMask collisionLayer;

    private Transform cameraTransform;

    private Rigidbody rb;

    private Vector2 input;

    private Vector2 characterDirection;

    private void Update()
    {
        CalculateBlendTreeValues();
    }

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        cameraAnim.SetBool("Air", !mover.isInWalkMode);

        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
        {
            cameraAnim.SetBool("Walk", true);
        }
        else
        {
            cameraAnim.SetBool("Walk", false);
            cameraAnim.SetBool("Run", false);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            characterCapsule.Height = crouchedHeight;
            cameraAnim.SetBool("Crouched", true);
            characterController.realMoveSpeed = characterController.slowMoveSpeed;
        }
        else if (!Physics.Raycast(transform.position, transform.up, normalHeight - 0.1f, collisionLayer))
        {
            characterController.realMoveSpeed = characterController.normalMoveSpeed;
            cameraAnim.SetBool("Crouched", false);
            characterCapsule.Height = normalHeight;

            if (Input.GetKey(KeyCode.LeftShift))
            {
                characterController.realMoveSpeed = characterController.fastMoveSpeed;
                cameraAnim.SetBool("Run", true);
            }
            else
            {
                cameraAnim.SetBool("Run", false);
            }
        }
    }

    private void CalculateBlendTreeValues()
    {
        // Obtener la velocidad del personaje en el plano XZ
        Vector3 velocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        velocity.Normalize();

        // Obtener la dirección del personaje en el plano XZ
        Vector3 characterDirection = new Vector3(transform.forward.x, 0f, transform.forward.z);
        characterDirection.Normalize();

        // Calcular el ángulo entre la velocidad y la dirección
        float angle = Vector3.SignedAngle(characterDirection, velocity, Vector3.up);

        // Convertir el ángulo en una dirección de blend tree
        Vector2 blendDirection = AngleToBlendDirection(angle);

        // Interpolar suavemente entre la dirección actual y la dirección del blend tree
        Vector2 smoothBlendDirection = Vector2.Lerp(GetCurrentBlendDirection(), blendDirection, blendSmoothness * Time.deltaTime);

        // Pasar la diferencia al blend tree
        cameraAnim.SetFloat("YDir", smoothBlendDirection.y);
        cameraAnim.SetFloat("XDir", smoothBlendDirection.x);
    }

    private Vector2 GetCurrentBlendDirection()
    {
        float yDir = cameraAnim.GetFloat("YDir");
        float xDir = cameraAnim.GetFloat("XDir");
        return new Vector2(xDir, yDir);
    }

    private Vector2 AngleToBlendDirection(float angle)
    {
        const float forwardThreshold = 45f;
        const float backwardThreshold = 135f;

        if (angle >= -forwardThreshold && angle <= forwardThreshold)
        {
            return new Vector2(0f, 1f); // Hacia adelante
        }
        else if (angle >= backwardThreshold || angle <= -backwardThreshold)
        {
            return new Vector2(0f, -1f); // Hacia atrás
        }
        else if (angle > forwardThreshold && angle < backwardThreshold)
        {
            return new Vector2(1f, 0f); // Hacia la derecha
        }
        else
        {
            return new Vector2(-1f, 0f); // Hacia la izquierda
        }
    }

    public void Jump()
    {
        cameraAnim.SetTrigger("Jump");
    }
}
