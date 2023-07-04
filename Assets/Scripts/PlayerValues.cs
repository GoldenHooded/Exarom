using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValues : MonoBehaviour
{
    [SerializeField] private Image staminaCont;
    [SerializeField] private Image staminaBar;
    [SerializeField] private float staminaSpeedAlpha;

    private float staminaAlphaReal = 0;
    [Range(0, 1)] [SerializeField] private float staminaAlpha = 0;
    [SerializeField] private float staminaSpeedChange = 10;

    [Space(10)]
    [Range(0, 1)] public float stamina = 1;
    private float realStamina = 1;
    public bool canRecoverStamina = false;
    public float staminaRecoverSpeed;
    private bool recoveringStamina = false;
    [SerializeField] private bool infiniteStamina;

    private bool staminaTrigger = false;
    void Update()
    {
        if (infiniteStamina)
        {
            stamina = 1;
        }

        staminaAlphaReal = Mathf.Lerp(staminaAlphaReal, staminaAlpha, Time.deltaTime * staminaSpeedAlpha);

        staminaBar.color = new Color(staminaBar.color.r, staminaBar.color.g, staminaBar.color.b, staminaAlphaReal);
        staminaCont.color = new Color(staminaCont.color.r, staminaCont.color.g, staminaCont.color.b, staminaAlphaReal);

        realStamina = Mathf.Lerp(realStamina, stamina, Time.deltaTime * staminaSpeedChange);
        staminaBar.fillAmount = realStamina;

        if (stamina < 0.95)
        {
            staminaAlpha = 1;
        }
        else
        {
            staminaAlpha = 0;
        }

        if (stamina > 1)
        {
            stamina = 1;
        }

        if (canRecoverStamina && !staminaTrigger)
        {
            Invoke("StartRecovery", 1.5f);
            staminaTrigger = true;
        }
        else if (!canRecoverStamina)
        {
            staminaTrigger = false;
            recoveringStamina = false;
        }

        if (recoveringStamina && stamina < 1)
        {
            stamina += staminaRecoverSpeed * Time.deltaTime;
        }

        if (stamina >= 1)
        {
            recoveringStamina = false;
        }
    }

    private void StartRecovery()
    {
        recoveringStamina = true;
    }
}
