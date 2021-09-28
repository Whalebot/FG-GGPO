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
    void Start()
    {
        
    }

    // Update is called once per frame
    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
