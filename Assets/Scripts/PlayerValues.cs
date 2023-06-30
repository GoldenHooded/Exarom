using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerValues : MonoBehaviour
{
    [SerializeField] private Image staminaCont;
    [SerializeField] private Image staminaBar;
    [SerializeField] private float staminaSpeedAlpha;

    private float staminaAlphaReal = 1;
    [Range(0, 1)] [SerializeField] private float staminaAlpha = 1;
    [SerializeField] private float staminaSpeedChange = 10;

    [Space(10)]
    [Range(0, 1)] public float stamina = 1;
    private float realStamina = 1;
    public bool canRecoverStamina = false;


    private bool staminaTrigger = false;
    void Update()
    {
        staminaAlphaReal = Mathf.Lerp(staminaAlphaReal, staminaAlpha, Time.deltaTime * staminaSpeedAlpha);

        staminaBar.color = new Color(staminaBar.color.r, staminaBar.color.g, staminaBar.color.b, staminaAlphaReal);
        staminaCont.color = new Color(staminaCont.color.r, staminaCont.color.g, staminaCont.color.b, staminaAlphaReal);

        realStamina = Mathf.Lerp(realStamina, stamina, Time.deltaTime * staminaSpeedChange);
        staminaBar.fillAmount = realStamina;

        if (stamina != 1)
        {
            staminaAlpha = 1;
        }
        else
        {
            staminaAlpha = 0;
        }
    }
}
