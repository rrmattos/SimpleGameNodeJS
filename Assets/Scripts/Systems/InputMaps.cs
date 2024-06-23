using UnityEngine;
using UnityEngine.InputSystem;

public class InputMaps
{
    [HideInInspector] public InputActionAsset InputAsset;
    [HideInInspector] public InputAction MoveAction;
    [HideInInspector] public InputAction RunKeyboardAction;
    [HideInInspector] public InputAction JumpAction;
    [HideInInspector] public InputAction RollAction;
    [HideInInspector] public InputAction CrouchAction;
    [HideInInspector] public InputAction LeftMouseAction;
    [HideInInspector] public InputAction UselessAction;

    public InputMaps(InputActionAsset _InputAsset)
    {
        InputAsset = _InputAsset;
        InputAsset.Enable(); //!--- Enable all actions; If you want to enble one by one, do ...Action.Enable() ---
        MoveAction = InputAsset.FindAction("Move");
        RunKeyboardAction = InputAsset.FindAction("RunKeyboard");
        JumpAction = InputAsset.FindAction("Jump");
        RollAction = InputAsset.FindAction("Roll");
        LeftMouseAction = InputAsset.FindAction("LeftMouse");
        UselessAction = InputAsset.FindAction("Useless");
    }
}
