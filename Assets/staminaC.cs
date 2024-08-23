using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class staminaC : MonoBehaviour
{
    [Header("Stamina Main Parameters")]
    public float playerStamina = 100.0f;
    [SerializeField] private float maxStamina = 100.0f;
    [HideInInspector] public bool hasRegenerated = true;
    [HideInInspector] public bool weAreSprinting = false;

    [Header("Stamina Regen Parameters")]
    [Range(0, 50)][SerializeField] private float staminaDrain = 0.5f;
    [Range(0, 50)][SerializeField] private float staminaRegen = 0.5f;

    [Header("Stamina Speed Parameters")]
    [SerializeField] private float slowedRunSpeed = 4;
    [SerializeField] private float normalRunSpeed = 8;

    [Header("Stamina UI Elements")]
    [SerializeField] private Image staminaProgressUI = null;
    [SerializeField] private CanvasGroup sliderCanvasGroup = null;

    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (playerStamina < maxStamina)
        {
            playerStamina += staminaRegen * Time.deltaTime;
            UpdateStamina(1);
        }

        playerStamina = Mathf.Clamp(playerStamina, 0, maxStamina);

        if (playerStamina >= maxStamina)
        {
            player.playerSpeed = normalRunSpeed;
            sliderCanvasGroup.alpha = 0;
            hasRegenerated = true;
        }
    }

    public void Sprinting()
    {
        if (hasRegenerated)
        {
            weAreSprinting = true;
            playerStamina -= staminaDrain * Time.deltaTime;
            UpdateStamina(1);

            if (playerStamina <= 0)
            {
                hasRegenerated = false;
                player.playerSpeed = slowedRunSpeed;
                sliderCanvasGroup.alpha = 0;
            }
        }
        else
        {
            weAreSprinting = false;
        }
    }

    void UpdateStamina(int value)
    {
        staminaProgressUI.fillAmount = playerStamina / maxStamina;

        if (value == 1)
        {
            sliderCanvasGroup.alpha = 1;
        }
    }
}
