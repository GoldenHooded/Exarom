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

    private Transform cameraTransform;

    private Rigidbody rb;

    private Vector2 input;

    private Vector2 characterDirection;

    private void Update()
    {
        // Obtener el input horizontal y vertical
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        input = new Vector2(horizontalInput, verticalInput).normalized;

        // Calcular el valor del blend tree en base a la dirección y el input
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
            cameraAnim.SetBool("Run", Input.GetKey(KeyCode.LeftShift)); 
        }
        else
        {
            cameraAnim.SetBool("Walk", false);
            cameraAnim.SetBool("Run", false);
        }
    }

    private void CalculateBlendTreeValues()
    {
        // Obtener la dirección del personaje en el plano XZ
        Vector3 characterDirection = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0f) * Vector3.forward;
        characterDirection.y = 0f;
        characterDirection.Normalize();

        // Calcular los valores del blend tree en base a la dirección y el input
        float yDirValue = Vector2.Dot(input, new Vector2(characterDirection.x, characterDirection.z));
        float xDirValue = Vector2.Dot(input, new Vector2(-characterDirection.z, characterDirection.x));

        // Asignar los valores al blend tree en el Animator
        cameraAnim.SetFloat("YDir", yDirValue);
        cameraAnim.SetFloat("XDir", xDirValue);
    }

    public void Jump()
    {
        cameraAnim.SetTrigger("Jump");
    }
}
