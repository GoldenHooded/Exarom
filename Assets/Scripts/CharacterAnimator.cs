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
        // Obtener la velocidad del personaje en el plano XZ
        Vector3 velocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        velocity.Normalize();

        // Obtener la dirección del personaje en el plano XZ
        Vector3 characterDirection = new Vector3(transform.forward.x*2, 0f, transform.forward.z*2);
        characterDirection.Normalize();

        // Calcular la diferencia entre la velocidad y la dirección
        Vector2 difference = new Vector2(velocity.x - characterDirection.x, velocity.z - characterDirection.z);

        // Pasar la diferencia al blend tree
        cameraAnim.SetFloat("YDir", difference.y);
        cameraAnim.SetFloat("XDir", difference.x);
    }

    public void Jump()
    {
        cameraAnim.SetTrigger("Jump");
    }
}
