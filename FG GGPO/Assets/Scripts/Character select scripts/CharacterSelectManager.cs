using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;

public class CharacterSelectManager : MonoBehaviour
{

    public InputManager inputManager;

    public int p1ID;
    public int p2ID;

    public CharacterSelectButton p1Hover;
    public CharacterSelectButton p2Hover;

    public MMFeedbacks p1UpdateFeedback;

    [Header("Display")]

    public Transform p1Cursor;
    public Transform p2Cursor;

    public Image p1Portrait;
    public Image p2Portrait;

    public TextMeshProUGUI p1Name;
    public TextMeshProUGUI p2Name;

    public TextMeshProUGUI p1Playstyle;
    public TextMeshProUGUI p2Playstyle;

    public TextMeshProUGUI p1Description;
    public TextMeshProUGUI p2Description;




    // Start is called before the first frame update

    private void Awake()
    {
        inputManager = InputManager.Instance.GetComponent<InputManager>();

        inputManager.p1Input.upInput += p1Up;
        inputManager.p1Input.downInput += p1Down;
        inputManager.p1Input.leftInput += p1Left;
        inputManager.p1Input.rightInput += p1Right;


    }

    void Start()
    {
        UpdateP1();
    }

    // Update is called once per frame
    void Update()
    {
        p1Cursor.position = p1Hover.transform.position;
    }

    public void p1Up()
    {
        p1Hover = p1Hover.up.GetComponent<CharacterSelectButton>();
        UpdateP1();
    }

    public void p1Down()
    {
        p1Hover = p1Hover.down.GetComponent<CharacterSelectButton>();
        UpdateP1();
    }

    public void p1Left()
    {
        p1Hover = p1Hover.left.GetComponent<CharacterSelectButton>();
        UpdateP1();
    }

    public void p1Right()
    {
        p1Hover = p1Hover.right.GetComponent<CharacterSelectButton>();
        UpdateP1();
    }

    public void UpdateP1()
    {
        p1Name.text = p1Hover.profile.name;
        p1Description.text = p1Hover.profile.description;
        p1Portrait.sprite = p1Hover.profile.portrait;
        p1UpdateFeedback?.PlayFeedbacks();
    }

    public void UpdateP2()
    {

    }

}
