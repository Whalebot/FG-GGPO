using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    MainMenu mainMenu;
    // Start is called before the first frame update
    void Start()
    {
        mainMenu = GetComponentInParent<MainMenu>();
    }

    public void Selected()
    {
        if (Time.timeSinceLevelLoad > 0.3F)
            mainMenu.HoverSound();
    }
    public void Clicked()
    {
        mainMenu.ClickSound();
    }
}
