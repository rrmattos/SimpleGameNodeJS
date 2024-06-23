using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonsController : MonoBehaviour
{
    [SerializeField] private Button SFXButton;
    [SerializeField] private Button BGMButton;
    [SerializeField] private List<Sprite> buttonSprites = new ();

    private void Awake()
    {
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        AudioSource bgm = audioSources.FirstOrDefault(_a => _a.name == "BGMAudioSource");
        AudioSource sfx = audioSources.FirstOrDefault(_a => _a.name == "SFXAudioSource");

        if (!GlobalChecker.isBGMOn) BGMButtonClick(bgm);
        if (!GlobalChecker.isSFXOn) SFXButtonClick(sfx);
    }

    public void SFXButtonClick(AudioSource _audioSource)
    {
        _audioSource.enabled = !_audioSource.enabled;

        if (_audioSource.enabled)
        {
            SFXButton.image.sprite = buttonSprites[2];
            GlobalChecker.isSFXOn = true;
        }
        else
        {
            SFXButton.image.sprite = buttonSprites[3];
            GlobalChecker.isSFXOn = false;
        }
    }

    public void BGMButtonClick(AudioSource _audioSource)
    {
        _audioSource.enabled = !_audioSource.enabled;

        if (_audioSource.enabled)
        {
            BGMButton.image.sprite = buttonSprites[0];
            GlobalChecker.isBGMOn = true;
        }
        else
        {
            BGMButton.image.sprite = buttonSprites[1];
            GlobalChecker.isBGMOn = false;
        }
    }

    public void Quit()
    {
        //TODO: Fazer verificação se o score é maior que o atual salvo no banco ele atualiza.
        Application.Quit();
    }
}
