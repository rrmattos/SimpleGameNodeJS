//using UnityEditor.Rendering;
//using UnityEngine;
//using UnityEngine.InputSystem;
//using UnityEngine.InputSystem.Controls;
//using UnityEngine.InputSystem.EnhancedTouch;

//public class InputMoveBehaviour_Com_StateMachine: MonoBehaviour, IMovement
//{
//    [SerializeField] private InputActionAsset inputAsset;
//    private InputMaps inputMap;
//    private StateMachine<MoveStates> stateMachine = new StateMachine<MoveStates>();
//    private enum InputMoveType
//    {
//        HORIZONTAL,
//        VERTICAL,
//        BOTH
//    }
//    [SerializeField] private InputMoveType inputMoveType = InputMoveType.HORIZONTAL;
//    private CollisionBehaviour collisionBehaviour;
//    private Rigidbody2D rb;

//    #region --- Variáveis Implementadas do IMovement (e relacioandos) ---
//    [field: SerializeField] public Vector2 Direction { get; set; } = Vector2.zero;
//    [field: SerializeField] public float Speed { get; set; } = 0;
//    private float currentSpeed;
//    [field: SerializeField] public float JumpForce { get; set; } = 10;
//    public float maxJumpForce = 10f; // Força máxima do pulo
//    public float maxChargeTime = 3f; // Tempo máximo de carga em segundos
//    private float jumpHoldTime = 0f;
//    private float maxJumpHoldTime = 3f;
//    private bool isJumping = false;
//    #endregion

//    public bool IsInputBlocked = false;

//    private void Awake()
//    {
//        #region --- Configurando InputActions ---
//        inputMap = new InputMaps(inputAsset);

//        inputMap.MoveAction.started += OnMoveStarted;
//        inputMap.MoveAction.performed += OnMovePerformed;
//        inputMap.MoveAction.canceled += OnMoveCanceled;

//        inputMap.JumpAction.started += OnJumpStarted;
//        inputMap.JumpAction.performed += OnJumpPerformed;
//        inputMap.JumpAction.canceled += OnJumpCanceled;

//        //inputMap.RollAction.started += OnRollStarted;
//        //inputMap.RollAction.performed += OnRollPerformed;
//        //inputMap.RollAction.canceled += OnRollCanceled;

//        //inputMap.CrouchAction.started += OnCrouchStarted;
//        //inputMap.CrouchAction.performed += OnCrouchPerformed;
//        //inputMap.CrouchAction.canceled += OnCrouchCanceled;
//        #endregion

//        #region --- Configurando a StateMachine ---
//        stateMachine.AddState(MoveStates.NONE);
//        stateMachine.AddState(MoveStates.WALK);
//        stateMachine.AddState(MoveStates.RUN);
//        stateMachine.AddState(MoveStates.JUMP);
//        stateMachine.AddState(MoveStates.ROLL);
//        stateMachine.AddState(MoveStates.CROUCH);

//        //! --- Define as transições entre os estados ---
//        //! --- Primeiro estado é "a partir de", seguindo a linha é "para" (que no caso pode ir para qualquer um dos demais estados) ---
//        stateMachine.AddTransitions(MoveStates.NONE, MoveStates.WALK, MoveStates.RUN, MoveStates.JUMP, MoveStates.ROLL, MoveStates.CROUCH);
//        stateMachine.AddTransitions(MoveStates.WALK, MoveStates.NONE, MoveStates.RUN, MoveStates.JUMP, MoveStates.ROLL, MoveStates.CROUCH);
//        stateMachine.AddTransitions(MoveStates.RUN, MoveStates.NONE, MoveStates.WALK, MoveStates.JUMP, MoveStates.ROLL, MoveStates.CROUCH);
//        stateMachine.AddTransitions(MoveStates.JUMP, MoveStates.NONE, MoveStates.WALK, MoveStates.RUN, MoveStates.ROLL, MoveStates.CROUCH);
//        stateMachine.AddTransitions(MoveStates.ROLL, MoveStates.NONE, MoveStates.WALK, MoveStates.RUN, MoveStates.JUMP, MoveStates.CROUCH);
//        stateMachine.AddTransitions(MoveStates.CROUCH, MoveStates.NONE, MoveStates.WALK, MoveStates.RUN, MoveStates.JUMP, MoveStates.ROLL);

//        //! --- Define quais estados podem coexistir ---
//        stateMachine.AddCoexistingStates(MoveStates.JUMP, MoveStates.WALK);
//        stateMachine.AddCoexistingStates(MoveStates.JUMP, MoveStates.RUN);
//        #endregion

//        if (TryGetComponent(out CollisionBehaviour behaviour)) collisionBehaviour = behaviour;

//        if (collisionBehaviour != null) collisionBehaviour.OnSetRigidbody += GetRigidbodyRef;

//    }

//    private void OnDisable()
//    {
//        if (collisionBehaviour != null) collisionBehaviour.OnSetRigidbody -= GetRigidbodyRef;
//    }

//    #region --- Funções dos InputActions ---
//    public void OnMoveStarted(InputAction.CallbackContext context)
//    {
//        Debug.Log("Movimento iniciado");

//    }

//    public void OnMovePerformed(InputAction.CallbackContext context)
//    {
//        Debug.Log("Movimento executado");
//        Direction = context.ReadValue<Vector2>();

//        if (Direction.x > -0.2f && Direction.x < 0.2f)
//        {
//            stateMachine.ChangeState(MoveStates.WALK);
//            currentSpeed = Speed;
//        }
//        else if (Direction.x > 0.2f || Direction.x < -0.2f)
//        {
//            stateMachine.ChangeState(MoveStates.RUN);
//            currentSpeed = Speed * 2;
//        }
//    }

//    public void OnMoveCanceled(InputAction.CallbackContext context)
//    {
//        Debug.Log("Movimento cancelado");
//        VerifyIfAnyButtonIsPressed();
//        currentSpeed = 0;
//        Direction = Vector2.zero;
//    }

//    public void OnJumpStarted(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo iniciado");

//        if (!isJumping)
//        {
//            isJumping = true;
//            JumpForce = 0f;
//            jumpHoldTime = 0f;
//        }
//    }

//    public void OnJumpPerformed(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo executado");
//        JumpForce = context.ReadValue<float>();

//        if (stateMachine.CurrentState == MoveStates.WALK || stateMachine.CurrentState == MoveStates.RUN)
//        {
//            // Adiciona o estado de pulo aos estados ativos, sem remover os estados existentes
//            stateMachine.AddMultipleStates(MoveStates.JUMP);
//        }
//        else
//        {
//            // Se não estiver andando ou correndo, muda imediatamente para o estado de pulo
//            stateMachine.ChangeState(MoveStates.JUMP);
//        }
//        //else if (isJumping)
//        //{
//        //    // Ajusta a força do pulo com base no tempo segurando o botão
//        //    jumpForce = Mathf.Lerp(0f, maxJumpForce, context.timeHeld / maxChargeTime);
//        //}

//        if (JumpForce > 0)
//        {
//            Jump();
//        }
//    }

//    public void OnJumpCanceled(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo cancelado");
//        VerifyIfAnyButtonIsPressed();
//        isJumping = false;
//    }

//    public void OnRollStarted(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo iniciado");
//    }

//    public void OnRollPerformed(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo executado");
//    }

//    public void OnRollCanceled(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo cancelado");
//    }

//    public void OnCrouchStarted(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo iniciado");
//    }

//    public void OnCrouchPerformed(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo executado");
//    }

//    public void OnCrouchCanceled(InputAction.CallbackContext context)
//    {
//        Debug.Log("Pulo cancelado");
//    }
//    #endregion

//    public void Move()
//    {
//        Vector2 direction = Direction;

//        if (inputMoveType == InputMoveType.HORIZONTAL)
//        {
//            direction *= new Vector2(1, 0);
//        }
//        else if (inputMoveType == InputMoveType.VERTICAL)
//        {
//            direction *= new Vector2(0, 1);
//        }

//        transform.Translate(direction * currentSpeed * Time.deltaTime);
//    }

//    public void Jump()
//    {
//        if (rb == null) return;

//        rb.velocity = new Vector2(0, JumpForce);
//        //rb.velocity = new Vector2(rb.velocity.x, _jumpForce * JumpSpeed);
//    }

//    public void Roll()
//    {

//    }

//    public void Crouch()
//    {
//        //if (isCrouching)
//        //{
//        //    SetMoveState(MoveStates.CROUCH);
//        //}
//    }

//    private void VerifyIfAnyButtonIsPressed()
//    {
//        bool isAnyMovementButtonPressed = false;

//        foreach (var actionMap in inputAsset.actionMaps)
//        {
//            foreach (var action in actionMap.actions)
//            {
//                if (action.expectedControlType == "Button")
//                {
//                    if (action.ReadValue<float>() > 0)
//                    {
//                        isAnyMovementButtonPressed = true;
//                        break;
//                    }
//                }
//            }
//        }

//        // Define o estado como NONE somente se nenhum botão de movimento estiver sendo pressionado
//        if (!isAnyMovementButtonPressed)
//        {
//            stateMachine.ChangeState(MoveStates.NONE);
//        }
//    }


//    #region --- Funções para Get de Referências ---

//    public void GetRigidbodyRef(Rigidbody2D _rb)
//    {
//        rb = _rb;
//        if (rb != null) collisionBehaviour.OnSetRigidbody -= GetRigidbodyRef;
//    }

//    #endregion

//    private void Update()
//    {
//        if (IsInputBlocked) return;

//        if(stateMachine.CurrentState == MoveStates.WALK || 
//          stateMachine.CurrentState == MoveStates.RUN)
//        {
//            Move();
//        }  
        
//        if(stateMachine.CurrentState == MoveStates.JUMP)
//        {
//            if (isJumping)
//            {
//                // Incrementa o tempo que o botão de pulo foi pressionado
//                jumpHoldTime += Time.deltaTime;

//                // Limita o tempo de pressionamento do botão de pulo ao máximo
//                jumpHoldTime = Mathf.Min(jumpHoldTime, maxJumpHoldTime);

//                // Calcula a força do pulo com base no tempo de pressionamento
//                JumpForce = Mathf.Lerp(0f, maxJumpForce, jumpHoldTime / maxJumpHoldTime);
//            }
//        }
//    }

//}