using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDeathController : MonoBehaviour
{
    public static bool IsDead = false;

    [SerializeField] private List<AudioClip> audioClips = new();
    private AudioSource audioSource;
    private CollisionBehaviour collisionBehaviour;
    private GravityBehaviour gravityBehaviour;
    private AnimationBehaviour animationBehaviour;
    private bool isDying = false;

    private PlayerDeathController() { }

    private void OnEnable()
    {
        SFXObserver.OnSetAudioSource += GetSFXAudioSource;
    }
    
    private void Awake()
    {
        // if (TryGetComponent(out AudioSource audio))
        //     audioSource = audio;
        // else
        //     audioSource = transform.AddComponent<AudioSource>();

        if (TryGetComponent(out CollisionBehaviour cBehaviour)) collisionBehaviour = cBehaviour;
        if (TryGetComponent(out GravityBehaviour gBehaviour)) gravityBehaviour = gBehaviour;
        if (transform.Find("Visuals").TryGetComponent(out AnimationBehaviour aBehaviour)) animationBehaviour = aBehaviour;
    }

    IEnumerator DeathByEnemy()
    {
        foreach (var enemy in FindObjectsOfType<EnemyController>())
        {
            if (enemy != this)
            {
                //var enemyAnimator = enemy.GetComponent<Animator>();
                //if (enemyAnimator != null)
                //{
                //    enemyAnimator.speed = 0;
                //}
                enemy.enabled = false;
            }
        }

        var spawner = FindObjectOfType<SpawnerController>();
        spawner.enabled = false;

        GetComponent<InputMoveBehaviour>().IsInputBlocked = true;
        animationBehaviour.UpdateAnimation(MoveStates.HIT, 1);
        audioSource.clip = audioClips[0]; //! 0 = huh_cat
        audioSource.Play();

        yield return new WaitForSeconds(1);

        animationBehaviour.UpdateAnimation(MoveStates.DEATH, 1);
        audioSource.clip = audioClips[1]; //! 1 = yoshi_awawawa_low_pitch
        audioSource.Play();

        gravityBehaviour.Rb.isKinematic = true;
        gravityBehaviour.enabled = false;
        collisionBehaviour.Collider.enabled = false;
        collisionBehaviour.enabled = false;

        Vector3 target = new Vector3(transform.position.x, transform.position.y + 0.8f, 0);
        yield return new WaitUntil(() => MoveToDestination(target, 1));

        transform.Find("Visuals").GetComponent<SpriteRenderer>().sortingOrder = 3;
        target = new Vector3(transform.position.x, transform.position.y - 7f, 0);
        yield return new WaitUntil(() => MoveToDestination(target, 4.5f));

        IsDead = false;
        isDying = false;

        ResetScene();
    }

    private bool MoveToDestination(Vector3 _target, float _speed)
    {
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _target, step);

        return Vector3.Distance(transform.position, _target) < 0.001f;
    }

    IEnumerator DeathByFall()
    {
        audioSource.clip = audioClips[1]; //! 1 = yoshi_awawawa_low_pitch
        audioSource.Play();

        yield return new WaitForSeconds(audioSource.clip.length + 1);

        ResetScene();
    }

    private void ResetScene()
    {
        gravityBehaviour.Rb.isKinematic = false;
        gravityBehaviour.enabled = true;
        collisionBehaviour.Collider.enabled = true;
        collisionBehaviour.enabled = true;
        GetComponent<InputMoveBehaviour>().IsInputBlocked = false;
        GlobalChecker.IsFirstLoad = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Update()
    {
        if (gravityBehaviour?.Rb.velocity.y <= -10)
        {
            if (!isDying)
            {
                isDying = true;
                StartCoroutine(DeathByFall());
            }
        }

        if (!IsDead) return;

        if (!isDying)
        {
            isDying = true;
            StartCoroutine(DeathByEnemy());
        }
    }
    
    #region --- Get From Oberservers ---
    private void GetSFXAudioSource(AudioSource _audioSource)
    {
        audioSource = _audioSource;

        if (audioSource != null) SFXObserver.OnSetAudioSource -= GetSFXAudioSource;
    }
    #endregion
}
