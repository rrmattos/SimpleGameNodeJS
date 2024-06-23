using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))] //! --- Antes de adicionar esse Behaviour ao objeto, adicionar um Collider ---
public class CollisionBehaviour : MonoBehaviour
{
    public event Action<CollisionStates, Collider2D> OnCollision;
    private CollisionStates collisionState;
    public Collider2D Collider;
    private Collider2D other;

    private void Awake()
    {
        Collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        other = _other;
        collisionState = CollisionStates.ENTER_COLLIDING;
    }

    private void OnTriggerStay2D(Collider2D _other)
    {
        other = _other;
        collisionState = CollisionStates.STAY_COLLIDING;
    }

    private void OnTriggerExit2D(Collider2D _other)
    {
        other = _other;
        collisionState = CollisionStates.EXIT_COLLIDING;
    }

    private void OnCollisionEnter2D(Collision2D _other)
    {
        other = _other.collider;
        collisionState = CollisionStates.ENTER_COLLIDING;
    }

    private void OnCollisionStay2D(Collision2D _other)
    {
        other = _other.collider;
        collisionState = CollisionStates.STAY_COLLIDING;
    }

    private void OnCollisionExit2D(Collision2D _other)
    {
        other = _other.collider;
        collisionState = CollisionStates.EXIT_COLLIDING;
    }

    private void Update()
    {
        if (other != null)
            OnCollision?.Invoke(collisionState, other);

        if (collisionState == CollisionStates.EXIT_COLLIDING)
            other = null;
    }
}
