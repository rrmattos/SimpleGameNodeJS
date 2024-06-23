using System;
using TMPro;
using UnityEngine;

public class ScoreObserver : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public static event Action<TextMeshProUGUI> OnScoreUpdate;

    private ScoreObserver(){}

    private void Update()
    {
        OnScoreUpdate?.Invoke(scoreText);
    }
}
