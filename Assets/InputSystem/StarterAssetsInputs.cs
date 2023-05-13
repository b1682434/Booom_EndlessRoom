using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

/// <summary>
/// 按键按下事件
/// </summary>
public delegate void ButtonStartDelegate();


public class StarterAssetsInputs : MonoBehaviour
{
    [Header("Character Input Values")] public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool esc;
    public bool debug1InputState;
    public bool inspectItem;
    public float mouseScroll;

    [Header("Movement Settings")] public bool analogMovement;

    [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    /**************** 输入事件 ****************/
    public ButtonStartDelegate OnMouseScrollUpStart;
    public ButtonStartDelegate OnMouseScrollDownStart;
    public ButtonStartDelegate OnInspectItemStart;


#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED

    public void OnEsc(InputValue value)
    {
        esc = value.isPressed;
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

    public void OnDebug1(InputValue value)
    {
        Debug1Input(value.isPressed);
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

    public void OnMouseScroll(InputValue value)
    {
        // 只有从零变化到非零会触发事件
        if (mouseScroll == 0)
        {
            var newScrollDelta = value.Get<float>();

            if (newScrollDelta < 0.0f)
            {
                OnMouseScrollUpStart();
            }

            if (newScrollDelta > 0.0f)
            {
                OnMouseScrollDownStart();
            }
        }

        mouseScroll = value.Get<float>();
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

    public void Debug1Input(bool newDebug1InputState)
    {
        debug1InputState = newDebug1InputState;
        // debug1InputStart = 
    }
}