using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXObserver : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    
    public static event Action<AudioSource> OnSetAudioSource;
    
    private SFXObserver(){}

    // private void OnEnable()
    // {
    //     audioSource = GetComponent<AudioSource>();
    // }

    private void Update()
    {
        OnSetAudioSource?.Invoke(audioSource);
    }
}
