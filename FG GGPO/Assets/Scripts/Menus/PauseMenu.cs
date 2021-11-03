using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public enum PauseMenuTabs { Default, Movelist, Buttons, Sounds }
    public PauseMenuTabs currentTab;
    public Characters currentCharacter;
    public GameObject versusMenu;
    public GameObject trialMenu;
    public GameObject defaultButton;
    public GameObject trialDefaultButton;
    public GameObject buttoDefault;
    public GameObject[] moveLists;
    public GameObject buttonSettings;
    public GameObject soundSettings;
    bool applicationIsClosing;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnEnable()
    {
        InputManager.Instance.p1Input.eastInput += CancelButton;
        InputManager.Instance.p1Input.R1input += NextCharacterTab;
        InputManager.Instance.p1Input.L1input += PreviousCharacterTab;
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

    private void OnApplicationQuit()
    {
        applicationIsClosing = true;
    }


    private void OnDestroy()
    {
        applicationIsClosing = true;
    }

    private void OnDisable()
    {
        InputManager.Instance.p1Input.eastInput -= CancelButton;
        InputManager.Instance.p1Input.R1input -= NextCharacterTab;
        InputManager.Instance.p1Input.L1input -= PreviousCharacterTab;
        if (!applicationIsClosing && !GameHandler.isPaused)
        {
            ResetPauseMenu();
        }
    }

    public void Continue()
    {
        // ResetPauseMenu();
        GameHandler.Instance.ResumeGame();
    }

    public void NextCharacterTab()
    {
        if (currentTab == PauseMenuTabs.Movelist)
        {
            foreach (var item in moveLists)
            {
                item.SetActive(false);
            }
            int tabID = (int)currentCharacter;
            tabID = (tabID + 1) % 3;
            currentCharacter = (Characters)(tabID);
            moveLists[(int)currentCharacter].SetActive(true);
        }
    }
    public void PreviousCharacterTab()
    {
        if (currentTab == PauseMenuTabs.Movelist)
        {
            foreach (var item in moveLists)
            {
                item.SetActive(false);
            }
            int tabID = (int)currentCharacter;
            tabID = (tabID - 1) % 3;
            if (tabID < 0) tabID = 2;

            currentCharacter = (Characters)(tabID);
            moveLists[(int)currentCharacter].SetActive(true);
        }
    }

    public void CancelButton()
    {
        if (currentTab == PauseMenuTabs.Movelist)
        {
            ResetPauseMenu();
        }
        else
            GameHandler.Instance.ResumeGame();
    }
    public void NextTrial()
    {
        MissionManager.Instance.NextComboTrial();
        GameHandler.Instance.ResumeGame();
    }

    public void ResetPauseMenu()
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
        currentTab = PauseMenuTabs.Default;
        foreach (var item in moveLists)
        {
            item.SetActive(false);
        }
    }
    public void MoveList()
    {
        currentTab = PauseMenuTabs.Movelist;
        versusMenu.SetActive(false);
        trialMenu.SetActive(false);

        currentCharacter = (Characters)GameHandler.p1CharacterID;
        moveLists[GameHandler.p1CharacterID].SetActive(true);

    }
    public void ButtonSettings()
    {
        currentTab = PauseMenuTabs.Buttons;
        versusMenu.SetActive(false);
        trialMenu.SetActive(false);


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
