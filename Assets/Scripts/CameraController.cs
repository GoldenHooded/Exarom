using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform horizontalTransform;
    public float horizontalSpeed = 2f;
    public float verticalSpeed = 2f;

    private float _rotationX = 0f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        // Obtener los valores del input del mouse
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotar el objeto horizontalmente según el input del mouse en el eje X
        horizontalTransform.Rotate(Vector3.up, mouseX * horizontalSpeed);

        // Rotar la cámara verticalmente según el input del mouse en el eje Y
        _rotationX -= mouseY * verticalSpeed;
        _rotationX = Mathf.Clamp(_rotationX, -90f, 90f);

        transform.localRotation = Quaternion.Euler(_rotationX, 0f, 0f);
    }
}
