using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayUIManager : MonoBehaviour
{
    #region EDITOR_VALS
    /// <summary>The complete UI</summary>
    [SerializeField] private GameObject overlayUI;

    [SerializeField] private Button startButton;
    #endregion

    void Start()
    {
        ShowUI();
        MovementController.OnPlayerStuck += RestartGame;
        PointManager.OnMaxPointReached += RestartGame;
        startButton.onClick.AddListener(StartGame);
    }

    void OnDestroy()
    {
        MovementController.OnPlayerStuck -= RestartGame;
        PointManager.OnMaxPointReached -= RestartGame;
    }

    private void StartGame()
    {       
        Time.timeScale = 1;
        overlayUI.SetActive(false);
        Cursor.visible = false;
    }

    public void ShowUI()
    {
        Time.timeScale = 0;
        overlayUI.SetActive(true);
        Cursor.visible = true;
    }

    public void RestartGame()
    {      
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    } 
}
