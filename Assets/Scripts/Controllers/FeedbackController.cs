using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private TextMeshProUGUI textFeedback;
    [SerializeField] private RawImage rawImage;
    private bool isShowing = false;
    
    public static FeedbackController Instance;
    
    private FeedbackController(){}

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMessage(string _text, Color32 _bgColor)
    {
        if (isShowing) return;
        
        textFeedback.text = _text;
        rawImage.color = _bgColor;
        StartCoroutine(ShowFeedback());
    }

    public void ResetMessage()
    {
        StopAllCoroutines();
        isShowing = false;
    }

    IEnumerator ShowFeedback()
    {
        isShowing = true;
        
        animator.Play("FadeIn");

        yield return new WaitForSeconds(4);
        
        animator.Play("FadeOut");
        
        yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0).Length);
 
        isShowing = false;
    }
}
