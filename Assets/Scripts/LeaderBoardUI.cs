using TMPro;
using UnityEngine;

public class LeaderboardUI : MonoBehaviour
{
    [SerializeField] private GridManager gridManager; // Asignar desde el inspector
    public TextMeshProUGUI[] leaderboardTexts;       // Asigna 3 TextMeshPro desde el inspector

    private void Start()
    {
        if (gridManager == null)
        {
            Debug.LogError("GridManager no está asignado en LeaderboardUI.");
            return;
        }
    }

    /// <summary>
    /// Muestra el leaderboard en los TextMeshPro.
    /// </summary>
    public void DisplayLeaderboard()
    {
        var leaderboard = gridManager.GetLeaderboard();

        // Limpiar los textos
        for (int i = 0; i < leaderboardTexts.Length; i++)
        {
            leaderboardTexts[i].text = "";
        }

        // Mostrar los puntajes existentes
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboardTexts[i].text = $"{i + 1}. {leaderboard[i].playerName}: {leaderboard[i].score}";
        }
    }
}
