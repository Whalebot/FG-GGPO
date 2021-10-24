using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class MainMenu : MonoBehaviour
{
    public StageManager stageManager;
    public GameObject defaultButton;
    public EventSystem eventSystem;
    public GameObject hoverSound;
    public GameObject clickSound;
    // Start is called before the first frame update
    void Start()
    {
        GameHandler.p1Wins = 0;
        GameHandler.p2Wins = 0;
        GameHandler.p1WinStreak = 0;
        GameHandler.p2WinStreak = 0;

        SetActive(defaultButton);
    }

    public void HoverSound()
    {
        Instantiate(hoverSound);
    }

    public void ClickSound()
    {
        Instantiate(clickSound);

    }
    public void VersusMode()
    {
        GameHandler.gameModeID = 0;
        stageManager.LoadScene(1);
    }
    public void TrainingMode()
    {
        GameHandler.gameModeID = 1;
        stageManager.LoadScene(1);
    }
    public void TrialMode()
    {
        GameHandler.gameModeID = 2;
        stageManager.LoadScene(1);
    }
    public void SetActive(GameObject GO)
    {
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(GO);
    }
}
