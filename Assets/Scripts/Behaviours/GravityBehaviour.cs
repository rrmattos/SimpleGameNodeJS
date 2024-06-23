using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GravityBehaviour : MonoBehaviour
{
    public LayerMask GroundLayer; // Layer que representa o chão
    public bool IsGrounded; // Flag para verificar se está no chão
    [HideInInspector] public Rigidbody2D Rb;

    [SerializeField] private float rayDistance;
    [SerializeField] private float capsuleCastWidth;
    [SerializeField] private float capsuleCastHeight;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, rayDistance, GroundLayer);
        IsGrounded = Physics2D.CapsuleCast(transform.position, new Vector2(capsuleCastWidth, capsuleCastHeight), CapsuleDirection2D.Vertical, 0f, Vector2.down, rayDistance, GroundLayer);

    }

    void OnDrawGizmos()
    {
        DebugDrawGroundCheckGizmo(IsGrounded);
    }

    void DebugDrawGroundCheckGizmo(bool isGrounded)
    {
        Color color = isGrounded ? Color.green : Color.red;
        Vector2 startPos = transform.position;
        Debug.DrawRay(startPos, Vector2.down * rayDistance, color);
    }
}