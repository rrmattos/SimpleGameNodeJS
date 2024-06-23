using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FruitController : MonoBehaviour
{
    [SerializeField] private Transform visuals; //! --- The gameObject that contains object's visuals ---
    private CollisionBehaviour collisionBehaviour;
    private Rigidbody2D rb;
    private bool isColliding = false;
    private AudioSource audioSource;
    private TextMeshProUGUI score;
    [SerializeField] private AudioClip itemSoundClip;

    private IDebugger debugger;
    public bool IsDebugging;
    private event Action<bool> ChangeDebugging;

    private void OnEnable()
    {
        debugger = new Debugger();
        ChangeDebugging += debugger.OnDebuggingChanged;
        
        SFXObserver.OnSetAudioSource += GetSFXAudioSource;
        
        collisionBehaviour.OnCollision += VerifyCollisions;

        ScoreObserver.OnScoreUpdate += GetScoreText;
    }

    void Awake()
    {
        if (TryGetComponent(out CollisionBehaviour cBehaviour)) collisionBehaviour = cBehaviour;
        if (TryGetComponent(out Rigidbody2D rigid)) rb = rigid;
    }

    private void VerifyCollisions(CollisionStates _collisionState, Collider2D _other)
    {
        if (_other.CompareTag("Player"))
        {
            if (isColliding) return;

            if (_collisionState == CollisionStates.ENTER_COLLIDING)
            {
                isColliding = true;

                StartCoroutine(TimerDestroy());
            }
        }
    }

    IEnumerator TimerDestroy()
    {
        visuals.gameObject.SetActive(false);
        GetComponent<Collider2D>().enabled = false;

        var text = score.text.Replace("Score: ", "");
        if (Int32.TryParse(text, out int result))
            score.text = $"Score: {(result + 100).ToString()}";
        else
            score.text += "100";

        if (audioSource != null)
        {
            audioSource.clip = itemSoundClip;
            audioSource.Play();
            yield return new WaitForSeconds(audioSource.clip.length);
        }
        else
            yield return new WaitForSeconds(1);
        
        Destroy(gameObject);

        isColliding = false;
    }

    void Update()
    {
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
}
