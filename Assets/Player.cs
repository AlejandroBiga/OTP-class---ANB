
using PSXShaderKit;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;

    [Header("Player Settings")]
    public float playerSpeed = 2.0f;
    [SerializeField] private float crouchSpeed = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    private InputManager inputManager;
    private Transform cameraTransform;
    private Transform playerBody;
    private staminaC staminaController;

    [Header("Booleans")]
    private bool groundedPlayer;
    public bool isWalking = false;
    public bool isRunning = false;
    private bool isCrouching = false;

    [Header("Crouch")]
    private float crouchCooldown = 0.5f;
    private float crouchTimer = 0f;

    [Header("Animator")]
    private Animator animator;
    public Transform headTransform;
    public Vector3 standingCameraOffset;
    public Vector3 crouchingCameraOffset;

    [Header("Camera Bobbing")]
    public float bobbingAmount = 0.05f;
    public float bobbingSpeed = 0.2f;
    public float standingBobbingAmount = 0.05f;
    public float crouchingBobbingAmount = 0.03f;
    private Vector3 originalHeadStandingPosition;
    private Vector3 originalHeadCrouchingPosition;
    private float bobbingOffset = 0f;
    private Vector2 movement;

    private void Start()
    {
        
        controller = GetComponent<CharacterController>();
        inputManager = InputManager.Instance;
        cameraTransform = Camera.main.transform;
        playerBody = transform.GetChild(0);
        animator = playerBody.GetComponent<Animator>();
        originalHeadStandingPosition = headTransform.localPosition;
        originalHeadCrouchingPosition = crouchingCameraOffset;
        staminaController = GetComponent<staminaC>();
    }

    void Update()
    {
        
        UpdateGroundedStatus();
        MovePlayer();
        UpdateAnimation();

        Quaternion cameraRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        playerBody.rotation = cameraRotation;

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        movement = inputManager.GetPlayerMovement();

        if (movement.magnitude > 0 && groundedPlayer)
        {
            ApplyBobbing();
        }
        else
        {
            ResetHeadPosition();
        }
    }

    void ApplyBobbing()
    {
        float currentBobbingSpeed = isCrouching ? bobbingSpeed * 0.5f : bobbingSpeed;
        bobbingOffset = Mathf.Sin(Time.time * currentBobbingSpeed) * GetBobbingAmount();
        Vector3 newPosition = isCrouching ? originalHeadCrouchingPosition : originalHeadStandingPosition;
        newPosition.y += bobbingOffset;
        headTransform.localPosition = newPosition;
    }

    void ResetHeadPosition()
    {
        Vector3 originalPosition = isCrouching ? originalHeadCrouchingPosition : originalHeadStandingPosition;
        headTransform.localPosition = originalPosition;
    }

    float GetBobbingAmount()
    {
        return isCrouching ? crouchingBobbingAmount : standingBobbingAmount;
    }

    void UpdateGroundedStatus()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }
    }

    void MovePlayer()
    {
        Vector2 movement = inputManager.GetPlayerMovement();
        bool isRunningInput = inputManager.getRun();
        bool isCrouchingInput = inputManager.getCrouch();

        if (isCrouchingInput && !isRunningInput && crouchTimer <= 0f)
        {
            ToggleCrouch();
            crouchTimer = crouchCooldown;
        }

        if (!isCrouching)
        {
            crouchTimer -= Time.deltaTime;
        }

        float speed = isCrouching ? crouchSpeed : playerSpeed;
        // se fija la direccion de la puta camara
        Vector3 moveDirection = CalculateMoveVector(movement);
        // define que mierda es atras
        bool walkingBackwards = movement.y < 0;
        //no me deja usar el puto input de correr si estoy yendo para atras o estoy agachado lpm DIOOOSSSSS
        if ((isRunningInput && !isCrouching) && movement.magnitude > 0 && !walkingBackwards)
        {
            if (staminaController.playerStamina > 0)
            {
                isRunning = true;
                staminaController.Sprinting();
                speed *= 2.0f;
            }
            else
            {
                isRunning = false;
                staminaController.weAreSprinting = false;
            }
        }
        else
        {
            isRunning = false;
            staminaController.weAreSprinting = false;
        }

        moveDirection = Vector3.ProjectOnPlane(moveDirection, Vector3.up).normalized;
        controller.Move(moveDirection * Time.deltaTime * speed);

        if (isCrouching && !isCrouchingInput)
        {
            ToggleCrouch();
        }
    }

    Vector3 CalculateMoveVector(Vector2 movement)
    {
        Vector3 cameraForward = cameraTransform.forward;
        cameraForward.y = 0f; 
        Vector3 moveDirection = Quaternion.LookRotation(cameraForward) * new Vector3(movement.x, 0f, movement.y);
        return moveDirection;
    }

    void UpdateAnimation()
    {
        Vector2 movement = inputManager.GetPlayerMovement();
        bool isRunningInput = inputManager.getRun();

        bool movingForward = movement.y > 0;
        bool movingBackward = movement.y < 0;
        bool movingLeft = movement.x < 0;
        bool movingRight = movement.x > 0;
        bool movingSideways = Mathf.Abs(movement.x) > 0 && Mathf.Abs(movement.y) <= 0.1f;
        bool canRun = isRunningInput && movement.magnitude > 0 && groundedPlayer && staminaController.playerStamina > 0;

        if (!canRun || !movingForward)
        {
            isRunning = false;
        }
        else
        {
            isRunning = true;
        }
        if (isRunning && !staminaController.hasRegenerated)
        {
            isRunning = false;
        }

        // Actualizar las animaciones
        animator.SetBool("walking", movement.magnitude > 0 && !isCrouching && (movingForward || movingBackward || movingSideways));
        animator.SetBool("runing", isRunning && !isCrouching && movingForward);
        animator.SetBool("crouchidle", isCrouching);
        animator.SetBool("crouchwalk", movement.magnitude > 0 && isCrouching);
        animator.SetBool("walkleft", movingLeft && !movingSideways);
        animator.SetBool("walkright", movingRight && !movingSideways);
        animator.SetBool("walkstrictleft", movingLeft && !movingForward && !movingBackward);
        animator.SetBool("walkstrictright", movingRight && !movingForward && !movingBackward);
        animator.SetBool("walkbacking", movingBackward && !movingLeft && !movingRight);

        if (isRunning && movingForward)
        {
            animator.SetBool("runleft", movingLeft);
            animator.SetBool("runright", movingRight);
        }
        else
        {
            animator.SetBool("runleft", false);
            animator.SetBool("runright", false);
        }
    }

    public void ToggleCrouch()
    {
        isCrouching = !isCrouching;

        float targetHeight = isCrouching ? 1f : 2.12f;
        Vector3 targetCenter = isCrouching ? new Vector3(0f, -0.55f, 0f) : new Vector3(0f, 0f, 0f);
        Vector3 targetCameraOffset = isCrouching ? crouchingCameraOffset : standingCameraOffset;

        StartCoroutine(AdjustCharacterHeight(targetHeight, targetCenter, targetCameraOffset));
    }

    private IEnumerator AdjustCharacterHeight(float targetHeight, Vector3 targetCenter, Vector3 targetCameraOffset)
    {
        float originalHeight = controller.height;
        Vector3 originalCenter = controller.center;
        Vector3 originalCameraOffset = headTransform.localPosition;

        float duration = 0.4f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            controller.height = Mathf.Lerp(originalHeight, targetHeight, t);
            controller.center = Vector3.Lerp(originalCenter, targetCenter, t);
            headTransform.localPosition = Vector3.Lerp(originalCameraOffset, targetCameraOffset, t);

            yield return null;
            elapsedTime += Time.deltaTime;
        }

        controller.height = targetHeight;
        controller.center = targetCenter;
        headTransform.localPosition = targetCameraOffset;
    }


    //void CheckJump()
    //{
    //if (inputManager.PlayerJumpedThisFrame() && groundedPlayer)
    //{
    //playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
    //}
    //}




}
