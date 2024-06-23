using UnityEngine;

public interface IMovement
{
    Vector2 Direction { get; set; }
    float Speed { get; set; }
    float JumpForce { get; set; }

    //void SetMoveState(MoveStates state);
    //MoveStates GetMoveState();

    void Move();
    void Jump();
}