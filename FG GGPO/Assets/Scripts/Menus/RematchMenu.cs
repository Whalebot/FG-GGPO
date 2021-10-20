using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RematchMenu : MonoBehaviour
{
    public GameObject defaultButton;
    public int characterSelectIndex = 1;
    public int mainMenuIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        UIManager.Instance.SetActive(defaultButton);
    }

    public void Rematch() {
        GameHandler.isPaused = false;
        StageManager.Instance.RestartScene();
    }
    public void CharacterSelect()
    {
        StageManager.Instance.LoadScene(characterSelectIndex);
    }

    public void MainMenu()
    {
        StageManager.Instance.LoadScene(mainMenuIndex);
    }

}
