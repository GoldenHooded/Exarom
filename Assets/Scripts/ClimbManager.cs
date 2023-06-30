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

    private void Update()
    {
        if (!canClimb) onClimbMode = false;
        if (onClimbMode) { characterController.canMove = false; characterController.canRotate = false; }
        if (playerValues.stamina <= 0) { canClimb = false; }

        if (onClimbMode)
        {
            playerValues.stamina -= staminaSpentSpeed * Time.deltaTime;
        }
    }
}
