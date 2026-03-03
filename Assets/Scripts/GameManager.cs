using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject gameOverTextUI;  
    public GameObject restartButtonUI; 
    public GameObject finishedUI;

    [Header("Settings")]
    public float delayBeforeRestart = 2.0f; 

    private bool gameHasEnded = false;

    public void EndGame()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            
            Time.timeScale = 0f; 

            StartCoroutine(ShowRestartSequence());
        }
    }

    private IEnumerator ShowRestartSequence()
    {
        gameOverTextUI.SetActive(true);
        if (restartButtonUI != null) restartButtonUI.SetActive(false);

        yield return new WaitForSecondsRealtime(delayBeforeRestart);

        gameOverTextUI.SetActive(false);
        if (restartButtonUI != null) restartButtonUI.SetActive(true);
    }

    public void CompleteLevel()
    {
        if (gameHasEnded == false)
        {
            gameHasEnded = true;
            finishedUI.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}