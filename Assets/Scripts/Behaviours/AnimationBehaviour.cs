using System;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationBehaviour : MonoBehaviour
{
    public Animator animator;
    private SpriteRenderer spriteRenderer;
    private string animationName;
    private Enum currentState;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (TryGetComponent(out SpriteRenderer sprite)) spriteRenderer = sprite;
    }

    public void UpdateAnimation(Enum _state, int _direction)
    {
        if (_direction < 0)
            spriteRenderer.flipX = true;
        else if (_direction > 0)
            spriteRenderer.flipX = false;

        //Debug.Log(_state);
        if (currentState == _state) return;

        currentState = _state;

        if (_state.GetType() == typeof(MoveStates))
        {
            switch (_state)
            {
                case MoveStates.NONE:
                    animationName = "idle";
                    break;

                case MoveStates.WALK:
                    animationName = "walk";
                    break;

                case MoveStates.RUN:
                    animationName = "run";
                    break;

                case MoveStates.JUMP:
                    animationName = "jump";
                    break;

                case MoveStates.CROUCH:
                    animationName = "useless";
                    break;

                case MoveStates.ROLL:
                    animationName = "roll";
                    break;

                case MoveStates.HIT:
                    animationName = "hit";
                    break;

                case MoveStates.DEATH:
                    animationName = "hit2";
                    break;
            }
        }

        animator.Play(animationName);
    }
}
