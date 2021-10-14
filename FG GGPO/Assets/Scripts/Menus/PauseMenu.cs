using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject defaultButton;
    public GameObject moveList;
    public GameObject buttonSettings;
    public GameObject soundSettings;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        UIManager.Instance.SetActive(defaultButton);
    }

    public void Continue()
    {
        GameHandler.Instance.ResumeGame();
    }
    public void MoveList() {
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
        StageManager.Instance.CharacterSelect();
    }

    public void MainMenu()
    {
        StageManager.Instance.MainMenu();
    }
}
