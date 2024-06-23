using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class InputMoveBehaviour : MonoBehaviour, IMovement
{
    [SerializeField] private InputActionAsset inputAsset;
    private InputMaps inputMap;
    private StateManager<MoveStates> stateManager;

    private enum InputMoveType
    {
        HORIZONTAL,
        VERTICAL,
        BOTH
    }
    [SerializeField] private InputMoveType inputMoveType = InputMoveType.HORIZONTAL;

    private CollisionBehaviour collisionBehaviour;
    private GravityBehaviour gravityBehaviour;
    private AnimationBehaviour animationBehaviour;
    public bool IsInputBlocked = false;

    #region --- Variáveis Implementadas do IMovement (e relacioandos) ---
    [field: SerializeField] public Vector2 Direction { get; set; } = Vector2.zero;
    [field: SerializeField] public float Speed { get; set; }
    private float currentSpeed;
    [field: SerializeField] public float JumpForce { get; set; }

    [SerializeField] private float maxJumpForce;
    [SerializeField] private float jumpAddAmount;
    private float jumpHoldTime;
    [SerializeField] private float maxJumpHoldTime;
    private bool hasJumped = false;
    //private bool isJumping = false;
    #endregion

    private IDebugger debugger;
    public bool IsDebugging;
    private event Action<bool> ChangeDebugging;
    private bool isGamepad;
    private bool isKeyboardAndMouse;


    private void OnEnable()
    {
        debugger = new Debugger();
        ChangeDebugging += debugger.OnDebuggingChanged;
        collisionBehaviour.OnCollision += VerifyCollisions;
    }

    private void Awake()
    {
        #region --- Configurando StateManager ---
        stateManager = new StateManager<MoveStates>();
        #endregion

        #region --- Configurando InputActions ---
        inputMap = new InputMaps(inputAsset);

        inputMap.MoveAction.started += OnMoveStarted;
        inputMap.MoveAction.performed += OnMovePerformed;
        inputMap.MoveAction.canceled += OnMoveCanceled;

        inputMap.RunKeyboardAction.started += OnRunKeyboardStarted;
        inputMap.RunKeyboardAction.canceled += OnRunKeyboardCanceled;

        inputMap.JumpAction.started += OnJumpStarted;
        inputMap.JumpAction.performed += OnJumpPerformed;
        inputMap.JumpAction.canceled += OnJumpCanceled;

        inputMap.UselessAction.performed += OnUselessPerformed;
        inputMap.UselessAction.canceled += OnUselessCanceled;

        VerifyControlScheme();
        #endregion

        //! --- Defines default state by NONE ---
        stateManager.ChangeState(MoveStates.NONE);

        #region --- Pega as referências dos Behaviour se existirem ---
        if (TryGetComponent(out CollisionBehaviour cBehaviour)) collisionBehaviour = cBehaviour;
        if (TryGetComponent(out GravityBehaviour gBehaviour)) gravityBehaviour = gBehaviour;
        if (transform.Find("Visuals").TryGetComponent(out AnimationBehaviour aBehaviour)) animationBehaviour = aBehaviour;
        #endregion
    }

    private void OnDisable()
    {
        if (debugger != null) ChangeDebugging -= debugger.OnDebuggingChanged;
        if (collisionBehaviour != null) collisionBehaviour.OnCollision -= VerifyCollisions;
        
        #region --- Configurando InputActions ---
        inputMap.MoveAction.started -= OnMoveStarted;
        inputMap.MoveAction.performed -= OnMovePerformed;
        inputMap.MoveAction.canceled -= OnMoveCanceled;

        inputMap.RunKeyboardAction.started -= OnRunKeyboardStarted;
        inputMap.RunKeyboardAction.canceled -= OnRunKeyboardCanceled;

        inputMap.JumpAction.started -= OnJumpStarted;
        inputMap.JumpAction.performed -= OnJumpPerformed;
        inputMap.JumpAction.canceled -= OnJumpCanceled;

        inputMap.UselessAction.performed -= OnUselessPerformed;
        inputMap.UselessAction.canceled -= OnUselessCanceled;
        #endregion
    }

    private void VerifyControlScheme()
    {
        isKeyboardAndMouse = Keyboard.current != null;
        isGamepad = Gamepad.current != null;
    }

    #region --- Funções dos InputActions ---
    public void OnMoveStarted(InputAction.CallbackContext _context)
    {
        debugger.Log("Movimento iniciado");

    }

    public void OnMovePerformed(InputAction.CallbackContext _context)
    {
        debugger.Log("Movimento executado");
        Direction = _context.ReadValue<Vector2>();
        Move();
    }

    public void OnMoveCanceled(InputAction.CallbackContext _context)
    {
        debugger.Log("Movimento cancelado");

        stateManager.RemoveState(MoveStates.WALK);
        stateManager.RemoveState(MoveStates.RUN);

        VerifyExecutingActions();
        currentSpeed = 0;
        //Direction = Vector2.zero;
    }

    public void OnRunKeyboardStarted(InputAction.CallbackContext _context)
    {
        debugger.Log("Correr teclado iniciado");
    }

    public void OnRunKeyboardCanceled(InputAction.CallbackContext _context)
    {
        debugger.Log("Correr teclado iniciado");

    }

    public void OnJumpStarted(InputAction.CallbackContext _context)
    {
        debugger.Log("Pulo iniciado");

        if (gravityBehaviour.Rb == null) return;

        if (!gravityBehaviour.IsGrounded) return;

        jumpHoldTime = 0f;

        if (stateManager.Contains(MoveStates.WALK) || stateManager.Contains(MoveStates.RUN))
            stateManager.AddJointState(MoveStates.JUMP);
        else
            stateManager.ChangeState(MoveStates.JUMP);
    }

    public void OnJumpPerformed(InputAction.CallbackContext _context)
    {
        debugger.Log("Pulo executado");
    }

    public void OnJumpCanceled(InputAction.CallbackContext _context)
    {
        debugger.Log("Pulo cancelado");
        //jumpHoldTime = 0;

        VerifyExecutingActions();

        if (Mathf.Sign(gravityBehaviour.Rb.velocity.y) > 0) //! --- Verifica se o número é negativo ---
            gravityBehaviour.Rb.velocity = new Vector2(gravityBehaviour.Rb.velocity.x, 0);
    }

    public void OnUselessPerformed(InputAction.CallbackContext _context)
    {
        debugger.Log("Useless executado");
        stateManager.ChangeState(MoveStates.CROUCH);
    }

    public void OnUselessCanceled(InputAction.CallbackContext _context)
    {
        debugger.Log("Useless executado");
        stateManager.RemoveState(MoveStates.CROUCH);
        VerifyExecutingActions();
    }
    #endregion

    public void Move()
    {
        if (isKeyboardAndMouse)
        {
            //if (inputMap.RunKeyboardAction.IsPressed())
            //{
            if (!gravityBehaviour.IsGrounded)
            {
                if (stateManager.Contains(MoveStates.JUMP))
                {
                    stateManager.AddJointState(MoveStates.WALK);
                    currentSpeed = Speed;
                }
                else
                {
                    stateManager.ChangeState(MoveStates.WALK);
                    currentSpeed = Speed;
                }
                //else
                //    goto direction;
            }

            //if (!stateManager.Contains(MoveStates.RUN))
            //{
            //    stateManager.ChangeState(MoveStates.RUN);
            //    currentSpeed = Speed * 2;
            //}
            //}
            else
            {
                if (!stateManager.Contains(MoveStates.WALK))
                {
                    stateManager.ChangeState(MoveStates.WALK);
                    currentSpeed = Speed;
                }
            }
        }
        else
        {
            if ((Direction.x > 0) && !stateManager.Contains(MoveStates.WALK))
            {
                stateManager.ChangeState(MoveStates.WALK);
                currentSpeed = Speed;
            }
            //else if ((Direction.x > 0.2f && Direction.x <= 1f) ||
            //         (Direction.x < -0.2f && Direction.x >= -1f) &&
            //         !stateManager.Contains(MoveStates.RUN))
            //{
            //    stateManager.ChangeState(MoveStates.RUN);
            //    currentSpeed = Speed * 2;
            //}
        }

        //direction:

        Vector2 direction = Direction;

        if (inputMoveType == InputMoveType.HORIZONTAL)
        {
            direction *= new Vector2(1, 0);
        }
        else if (inputMoveType == InputMoveType.VERTICAL)
        {
            direction *= new Vector2(0, 1);
        }

        transform.Translate(direction * currentSpeed * Time.deltaTime);
    }

    public void Jump()
    {
        jumpHoldTime += Time.deltaTime;
        if (jumpHoldTime > maxJumpHoldTime)
        {
            jumpHoldTime = maxJumpHoldTime;
        }
        else
        {
            if (gravityBehaviour.Rb.velocity.y < maxJumpForce)
            {
                if (gravityBehaviour.Rb.velocity.y < 1)
                    gravityBehaviour.Rb.velocity += new Vector2(0, JumpForce);
                else
                    gravityBehaviour.Rb.velocity += new Vector2(0, jumpAddAmount);
            }
        }
    }

    private void VerifyExecutingActions()
    {
        if (stateManager.CurrentState.Count == 0)
        {
            stateManager.ChangeState(MoveStates.NONE);
        }
    }

    private void VerifyCollisions(CollisionStates _collisionState, Collider2D _other)
    {
        //TODO: Implementara lógica...
    }

    #region --- Funções para Get de Referências ---

    //public void GetRigidbodyRef(Rigidbody2D _rb)
    //{
    //    gravityBehaviour.Rb = _rb;
    //    if (gravityBehaviour.Rb != null) collisionBehaviour.OnSetRigidbody -= GetRigidbodyRef;
    //}

    #endregion

    private void Update()
    {
        foreach (MoveStates a in stateManager.CurrentState)
        {
            Debug.Log(a);
        }

        //! --- Update debugger "permission" ---
        ChangeDebugging?.Invoke(IsDebugging);

        //foreach (MoveStates state in stateManager.CurrentState)
        //{
        //    Debug.Log(state);
        //}

        if (IsInputBlocked) return;

        #region --- Chama as ações dependendo do que há na lista de estados ---

        if (stateManager.Contains(MoveStates.WALK) ||
          stateManager.Contains(MoveStates.RUN))
        {
            Move();
        }

        if (stateManager.Contains(MoveStates.JUMP))
        {
            if (inputMap.JumpAction.WasPerformedThisFrame() && !hasJumped)
                Jump();

            if (!gravityBehaviour.IsGrounded && gravityBehaviour.Rb.velocity.y < 0)
            {
                hasJumped = true;
            }

            if ((gravityBehaviour.IsGrounded && hasJumped) || gravityBehaviour.Rb.velocity.y == 0)
            {
                stateManager.RemoveState(MoveStates.JUMP);
                VerifyExecutingActions();
                hasJumped = false;
            }
            
            // if (gravityBehaviour.Rb.velocity.y == 0)
            // {
            //     stateManager.RemoveState(MoveStates.JUMP);
            //     VerifyExecutingActions();
            //     hasJumped = false;
            // }
        }

        #endregion

        if (stateManager.CurrentState.Count > 0 && animationBehaviour != null)
        {
            if (stateManager.CurrentState[^1] == MoveStates.WALK)
            {
                if (Direction.x == 0) return;
            }

            animationBehaviour.UpdateAnimation(stateManager.CurrentState[^1], (int)Mathf.Sign(Direction.x));
        }
    }

}