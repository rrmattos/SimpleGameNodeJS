using System.Collections;
using UnityEngine;

public class CanvasInstructionsController : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float time;

    void Start()
    {
        animator = GetComponent<Animator>();

        StartCoroutine(Timer(time));
    }

    IEnumerator Timer(float time)
    {
        yield return new WaitForSeconds(time);

        animator.Play("FadeGroup");
    }
}
