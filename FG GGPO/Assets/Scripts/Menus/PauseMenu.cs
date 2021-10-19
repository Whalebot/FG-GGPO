using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject versusMenu;
    public GameObject trialMenu;
    public GameObject defaultButton;
    public GameObject trialDefaultButton;
    public GameObject moveList;
    public GameObject buttonSettings;
    public GameObject soundSettings;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        if (GameHandler.Instance.gameMode == GameMode.TrialMode)
        {
            versusMenu.SetActive(false);
            trialMenu.SetActive(true);
            UIManager.Instance.SetActive(trialDefaultButton);
            
        }
        else
        {
            trialMenu.SetActive(false);
            versusMenu.SetActive(true);
            UIManager.Instance.SetActive(defaultButton);
        }
    }

    public void Continue()
    {
        GameHandler.Instance.ResumeGame();
    }
    public void NextTrial()
    {
        MissionManager.Instance.NextComboTrial();
        GameHandler.Instance.ResumeGame();
    }
    public void MoveList()
    {
        moveList.SetActive(true);
    }
    public void ButtonSettings()
    {
        buttonSettings.SetActive(true);
    }
    public void SoundSettings()
    {
        soundSettings.SetActive(true);
    }
    public void CharacterSelect()
    {
        Time.timeScale = 1;
        StageManager.Instance.CharacterSelect();
    }

    public void MainMenu()
    {
        Time.timeScale = 1;
        StageManager.Instance.MainMenu();
    }
}
