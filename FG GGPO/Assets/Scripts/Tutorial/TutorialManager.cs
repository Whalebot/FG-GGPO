using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public TutorialSO tutorial;
    public int page;
    public TextMeshProUGUI title;
    public TextMeshProUGUI description;
    public TextMeshProUGUI pageNumbers;
    public GameObject forwardArrow;
    public GameObject backArrow;

    // Start is called before the first frame update
    void Start()
    {

    }

    [Button]
    void SetupTutorial()
    {
        if (tutorial.tutorialDescription.Length < page) print("Tutorial error, lacks description pages");
        title.text = tutorial.tutorialName;
        description.text = tutorial.tutorialDescription[page];
        pageNumbers.text = (page + 1) + "/" + tutorial.tutorialDescription.Length;

        forwardArrow.SetActive(tutorial.tutorialDescription.Length > page + 1);
        backArrow.SetActive(page > 0);
    }
    [Button]
    void NextPage()
    {
        if (tutorial.tutorialDescription.Length <= page + 1) { return; }
        else
        {
            page++;
            SetupTutorial();
        }
    }
    [Button]
    void PreviousPage()
    {
        if (page <= 0) { return; }
        else
        {
            page--;
            SetupTutorial();
        }
    }
}
