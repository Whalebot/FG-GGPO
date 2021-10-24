using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
public class StageManager : MonoBehaviour
{
    public static StageManager Instance;


    private void Awake()
    {
        Instance = this;
    }

    public void CharacterSelect()
    {
        LoadScene(1);
    }

    public void MainMenu()
    {
        LoadScene(0);
    }

    // Start is called before the first frame update
    [Button]
    public void LoadScene(int index) {
        Time.timeScale = 1;
        SceneManager.LoadScene(index);
    }

    [Button]
    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
