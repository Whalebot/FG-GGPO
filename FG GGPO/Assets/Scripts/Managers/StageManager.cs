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
       // StartCoroutine(LoadYourAsyncScene(index));
       SceneManager.LoadScene(index);
    }


    IEnumerator LoadYourAsyncScene(int index)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(index);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }

    [Button]
    public void RestartScene()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
