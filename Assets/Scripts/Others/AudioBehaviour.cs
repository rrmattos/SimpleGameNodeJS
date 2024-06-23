using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioBehaviour : MonoBehaviour
{
    [HideInInspector] public AudioSource AudioController;
    [SerializeField] private List<AudioClip> clipList = new List<AudioClip>();

    private void Awake()
    {
        AudioController = GetComponent<AudioSource>();
    }

    public void ChangeClipSound(AudioStates _audioType)
    {
        AudioController.clip = clipList[_audioType.GetHashCode()];
    }
}
