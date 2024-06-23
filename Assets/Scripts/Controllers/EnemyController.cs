using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[SuppressMessage("ReSharper", "CheckNamespace")]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float rayDistanceHorizontal;
    [SerializeField] private float rayDistanceVertical;
    [SerializeField] private float speed;
    [SerializeField] private AudioClip deathSoundClip;

    private string[] groundLayerNames =
    {
        "Ground",
        "Box"
    };
    
    private Rigidbody2D rb;
    private Transform visuals;
    private AudioSource audioSource;
    private TextMeshProUGUI score;
    private bool canMove = false;
    private float direction = 1;
    private bool canChangeDirection = true;
    private bool hasHeadKicked = false;

    private void OnEnable()
    {
        SFXObserver.OnSetAudioSource += GetSFXAudioSource;
        ScoreObserver.OnScoreUpdate += GetScoreText;
    }
    
    private void Start()
    {
        animator.enabled = false;

        StartCoroutine(StartTimer());

        rb = GetComponent<Rigidbody2D>();
        visuals = transform.Find("Visuals");
    }

    IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(1.5f);

        canMove = true;
        animator.enabled = true;
        animator.Play("Walk");
    }

    private void OnCollisionEnter2D(Collision2D _collision)
    {
        if (_collision.collider.CompareTag("Player") && !hasHeadKicked)
        {
            PlayerDeathController.IsDead = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D _other)
    {
        if (_other.CompareTag(("Player")) && !hasHeadKicked)
        {
            hasHeadKicked = true;
            
            foreach (Collider2D col in GetComponents<Collider2D>())
            {
                col.enabled = false;
            }
            
            StartCoroutine(DeathTimer());
        }
    }

    private void Walk()
    {
        direction = spriteRenderer.flipX ? -1f : 1f;
        Vector2 movement = new Vector2(direction * speed * Time.deltaTime, 0);
        transform.Translate(movement);
    }

    private void ChangeDirection()
    {
        canChangeDirection = false;

        spriteRenderer.flipX = !spriteRenderer.flipX;

        StartCoroutine(WaitTimer());
    }

    private void EnemyCollisions()
    {
        Vector2 startPosition = transform.position;
        startPosition += new Vector2(spriteRenderer.bounds.extents.x * direction, 0);

        Vector2 directionHorizontal = spriteRenderer.flipX ? Vector2.right : Vector2.left;
        RaycastHit2D raycastHorizontal = Physics2D.Raycast(startPosition, directionHorizontal, rayDistanceHorizontal, LayerMask.GetMask(groundLayerNames));
        Vector2 directionVertical = Vector2.down;
        RaycastHit2D raycastVertical = Physics2D.Raycast(transform.position, directionVertical, rayDistanceVertical, LayerMask.GetMask(groundLayerNames));

        if (!raycastVertical.collider)
        {
            if (canChangeDirection)
                ChangeDirection();
        }

        if (raycastHorizontal.collider != null)
        {
            if (canChangeDirection)
                ChangeDirection();
        }
    }

    IEnumerator WaitTimer()
    {
        yield return new WaitForSeconds(0.2f);

        canChangeDirection = true;
    }

    IEnumerator DeathTimer()
    {
        rb.freezeRotation = false;
        rb.AddTorque(Random.RandomRange(-180f, 180f), ForceMode2D.Impulse);

        if (audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = deathSoundClip;
            audioSource.Play();
        }
        
        visuals.GetComponent<SpriteRenderer>().sortingOrder = 3;
        Vector3 target = new Vector3(transform.position.x, transform.position.y - 7f, 0);

        var text = score.text.Replace("Score: ", "");
        if (Int32.TryParse(text, out int result))
            score.text = $"Score: {(result + 50).ToString()}";
        else
            score.text += "50";
        
        yield return new WaitUntil(() => MoveToDestination(target, 1f));
        
        visuals.gameObject.SetActive(false);
        Destroy(gameObject);
    }
    
    private bool MoveToDestination(Vector3 _target, float _speed)
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);

        return Vector3.Distance(transform.position, _target) < 0.001f;
    }
    
    private void Update()
    {
        if (!canMove) return;
        if (hasHeadKicked) return;
        
        Walk();
        EnemyCollisions();

        if (rb.velocity.y <= -20) //! --- "Death zone" by free fall ---
        {
            Destroy(gameObject);
        }
    }

    #region --- Get From Oberservers ---
    private void GetSFXAudioSource(AudioSource _audioSource)
    {
        audioSource = _audioSource;

        if (audioSource != null) SFXObserver.OnSetAudioSource -= GetSFXAudioSource;
    }

    private void GetScoreText(TextMeshProUGUI _score)
    {
        score = _score;
        
        if (score != null) ScoreObserver.OnScoreUpdate -= GetScoreText;
    }
    #endregion

    #region --- Editor ---
    private void OnDrawGizmos()
    {
        DebugDrawGroundCheckGizmo();
    }

    void DebugDrawGroundCheckGizmo()
    {
        Vector2 startPosH = transform.position;
        Vector2 directionHorizontal = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        startPosH += new Vector2(spriteRenderer.bounds.extents.x * direction, 0);
        Debug.DrawRay(startPosH, directionHorizontal * rayDistanceHorizontal, Color.red);

        Vector2 startPosV = transform.position;
        Vector2 directionVertical = Vector2.down;
        Debug.DrawRay(startPosV, directionVertical * rayDistanceVertical, Color.green);
    }
    #endregion
}
