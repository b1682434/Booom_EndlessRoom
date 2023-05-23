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
    [Header("Character Input Values")] public Vector2 move;
    public Vector2 look;
    public bool jump;
    public bool sprint;
    public bool esc;
    public bool backPack;
    public float scroll;
    public bool confirm;

    [Header("Movement Settings")] public bool analogMovement;

    [Header("Mouse Cursor Settings")] public bool cursorLocked = true;
    public bool cursorInputForLook = true;

    [Header("Inspection Mode")] public bool moveInspectionItem = false;
    public bool rotateInspectionItem = false;
    public Vector2 moveDeltaInInspectionMode;
    public Vector2 rotateDeltaInInspectionMode;

    public event ButtonStartDelegate OnEnterInspectionModeStart;
    public ButtonStartDelegate OnExitInspectionModeStart;

    public ButtonStartDelegate OnMoveInspectionItemStart;
    public ButtonStartDelegate OnMoveInspectionItemEnd;
    public ButtonStartDelegate OnRotateInspectionItemStart;
    public ButtonStartDelegate OnRotateInspectionItemEnd;

    /**************** 其他输入事件 ****************/
    public ButtonStartDelegate OnScrollUpStart;
    public ButtonStartDelegate OnScrollDownStart;
    public ButtonStartDelegate OnEscStart;


#if ENABLE_INPUT_SYSTEM

    public void OnEsc(InputValue value)
    {
        var oldEsc = esc;
        esc = value.isPressed;
        if (!oldEsc && esc)
        {
            OnEscStart?.Invoke();
        }
    }

    public void OnConfirm(InputValue value)
    {
        confirm = value.isPressed;
    }

    public void OnScroll(InputValue value)
    {
        var newScrollDelta = Mathf.Clamp(value.Get<float>(), -1, 1);
        // 只有从零变化到非零会触发事件，即按下时触发一次
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

    public void OnEnterInspectionMode(InputValue value)
    {
        // Debug.Log($"OnEnterInspectionMode called: {value.Get<Vector2>()}");
        // 按下时触发
        if (value.isPressed)
        {
            moveInspectionItem = false;
            rotateInspectionItem = false;
            moveDeltaInInspectionMode = Vector2.zero;
            rotateDeltaInInspectionMode = Vector2.zero;
            
            OnEnterInspectionModeStart?.Invoke();
        }
    }

    public void OnExitInspectionMode(InputValue value)
    {
        // 按下时触发
        if (value.isPressed)
        {
            OnExitInspectionModeStart?.Invoke();
        }
    }

    public void OnToggleMove(InputValue value)
    {
        // 按下和松开时触发
        moveInspectionItem = value.isPressed;

        if (moveInspectionItem)
        {
            OnMoveInspectionItemStart?.Invoke();
        }
        else
        {
            moveDeltaInInspectionMode = Vector2.zero;
            OnMoveInspectionItemEnd?.Invoke();
        }
    }

    public void OnMoveInspectionItem(InputValue value)
    {
        moveDeltaInInspectionMode = moveInspectionItem ? value.Get<Vector2>() : Vector2.zero;
    }

    public void OnToggleRotate(InputValue value)
    {
        // 按下和松开时触发
        rotateInspectionItem = value.isPressed;

        if (rotateInspectionItem)
        {
            OnRotateInspectionItemStart?.Invoke();
        }
        else
        {
            OnRotateInspectionItemEnd?.Invoke();
            rotateDeltaInInspectionMode = Vector2.zero;
        }
    }

    public void OnRotateInspectionItem(InputValue value)
    {
        rotateDeltaInInspectionMode = rotateInspectionItem ? value.Get<Vector2>() : Vector2.zero;
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

    public void SetCursorState(bool newState)
    {
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.Confined;
    }
}