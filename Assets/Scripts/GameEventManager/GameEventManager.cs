using System;
using UnityEngine;

public static class GameEventManager
{
    // 🔔 Événement de changement de score
    public static event Action<int> OnScoreChanged;

    // 🔔 Événement d’étoile récupérée
    public static event Action<int> OnStarCollected;

    // 🔔 Événement de fin de niveau
    public static event Action OnLevelCompleted;

    public static void RaiseScoreChanged(int newScore) => OnScoreChanged?.Invoke(newScore);
    public static void RaiseStarCollected(int starCount) => OnStarCollected?.Invoke(starCount);
    public static void RaiseLevelCompleted() => OnLevelCompleted?.Invoke();
}
