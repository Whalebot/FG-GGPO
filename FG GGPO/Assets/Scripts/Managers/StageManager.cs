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



    // Start is called before the first frame update
    [Button]
    public void LoadScene(int index) {
        SceneManager.LoadScene(index);
    }

    [Button]
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
