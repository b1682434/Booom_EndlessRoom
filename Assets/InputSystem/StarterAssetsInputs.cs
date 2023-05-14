using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 按键按下事件
/// </summary>
public delegate void ButtonStartDelegate();

public class StarterAssetsInputs : MonoBehaviour
{
    [Header("Character Input Values")]
    public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool esc;
    public bool backPack;
    public float scroll;
    public bool confirm;
    public bool inspectItem;

    [Header("Movement Settings")]
    public bool analogMovement;

    [Header("Mouse Cursor Settings")]
    public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    /**************** 输入事件 ****************/
    public ButtonStartDelegate OnScrollUpStart;
    public ButtonStartDelegate OnScrollDownStart;
    public ButtonStartDelegate OnInspectItemStart;


#if ENABLE_INPUT_SYSTEM

	public void OnEsc(InputValue value)
    {
        esc = value.isPressed;
    }

	public void OnConfirm(InputValue value)
    {
		confirm = value.isPressed;
    }

	public void OnScroll(InputValue value)
    {
        var newScrollDelta = Mathf.Clamp(value.Get<float>(), -1, 1);
        // 只有从零变化到非零会触发事件
        if (scroll == 0.0f)
        {
            if (newScrollDelta > 0.0f)
            {
                OnScrollUpStart();
            }

            if (newScrollDelta < 0.0f)
            {
                OnScrollDownStart();
            }
        }

		scroll = newScrollDelta;
    }
    
	public void OnBackPack(InputValue value)
    {
		backPack = value.isPressed;
    }

    public void OnMove(InputValue value)
    {
        MoveInput(value.Get<Vector2>());
    }

    public void OnLook(InputValue value)
    {
        if (cursorInputForLook)
        {
            LookInput(value.Get<Vector2>());
        }
    }

    public void OnJump(InputValue value)
    {
        JumpInput(value.isPressed);
    }

    public void OnSprint(InputValue value)
    {
        SprintInput(value.isPressed);
    }

    public void OnInspectItem(InputValue value)
    {
        // 只有按下时会触发一次
        if (inspectItem == false && value.isPressed)
        {
            OnInspectItemStart();
        }

        inspectItem = value.isPressed;
    }
#endif


    public void MoveInput(Vector2 newMoveDirection)
    {
        move = newMoveDirection;
    }

    public void LookInput(Vector2 newLookDirection)
    {
        look = newLookDirection;
    }

    public void JumpInput(bool newJumpState)
    {
        jump = newJumpState;
    }

    public void SprintInput(bool newSprintState)
    {
        sprint = newSprintState;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        SetCursorState(cursorLocked);
    }

    private void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}