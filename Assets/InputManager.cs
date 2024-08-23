using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;

    public static InputManager Instance
    {
        get { return _instance; }
    }
    private PlayerController playerControls;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
        Cursor.visible = false;
        
        playerControls = new PlayerController();
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public Vector2 GetPlayerMovement()
    {
        return playerControls.Player.Movement.ReadValue<Vector2>();
    }
    public Vector2 GetMouseDelta()
    {
        return playerControls.Player.Look.ReadValue<Vector2>();
    }

    public bool PlayerJumpedThisFrame()
    {
        return playerControls.Player.Jump.triggered;
    }

    public bool getInteraction()
    {
        return playerControls.Player.interact.triggered;
    }

    public bool getRun()
    {
        return playerControls.Player.Run.ReadValue<float>() > 0.5f;
    }

    public bool getCrouch()
    {
        return playerControls.Player.Crouch.ReadValue<float>() > 0.5f;
    }

    public bool getLanter()
    {
        return playerControls.Player.Lanter.triggered;
    }
}
