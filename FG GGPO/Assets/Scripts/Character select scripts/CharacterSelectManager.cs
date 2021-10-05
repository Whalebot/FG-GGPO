using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using MoreMountains.Feedbacks;
using UnityEngine.SceneManagement;

public class CharacterSelectManager : MonoBehaviour
{

    public InputManager inputManager;

    public int p1ID;
    public int p2ID;
    public int stageID;

    bool p1Selected;
    bool p2Selected;

    public CharacterSelectButton p1Hover;
    public CharacterSelectButton p2Hover;

    public MMFeedbacks p1UpdateFeedback;
    public MMFeedbacks p2UpdateFeedback;
    public MMFeedbacks p1SelectFeedback;
    public MMFeedbacks p2SelectFeedback;

    public float cursorSpeed;
    Vector3 velocity1;
    Vector3 velocity2;

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
        inputManager.p1Input.southInput += P1Select;


        inputManager.p2Input.upInput += p2Up;
        inputManager.p2Input.downInput += p2Down;
        inputManager.p2Input.leftInput += p2Left;
        inputManager.p2Input.rightInput += p2Right;
        inputManager.p2Input.southInput += P2Select;

    }

    void Start()
    {
        //UpdateP1();
        //UpdateP2();
    }

    // Update is called once per frame
    void Update()
    {
        p1Cursor.position = Vector3.SmoothDamp(p1Cursor.position, p1Hover.transform.position, ref velocity1, cursorSpeed);
        p2Cursor.position = Vector3.SmoothDamp(p2Cursor.position, p2Hover.transform.position, ref velocity2, cursorSpeed);
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

    public void p2Up()
    {
        p2Hover = p2Hover.up.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Down()
    {
        p2Hover = p2Hover.down.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Left()
    {
        p2Hover = p2Hover.left.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Right()
    {
        p2Hover = p2Hover.right.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void UpdateP1()
    {
        p1UpdateFeedback?.PlayFeedbacks();
        p1Name.text = p1Hover.profile.name;
        p1Description.text = p1Hover.profile.description;
        p1Playstyle.text = p1Hover.profile.playstyle;
        p1Portrait.sprite = p1Hover.profile.portrait;
        p1ID = p1Hover.profile.iD;
    }

    public void UpdateP2()
    {
        p2UpdateFeedback?.PlayFeedbacks();
        p2Name.text = p2Hover.profile.name;
        p2Description.text = p2Hover.profile.description;
        p2Playstyle.text = p2Hover.profile.playstyle;
        p2Portrait.sprite = p2Hover.profile.portrait;
        p2ID = p2Hover.profile.iD;
    }

    [Button]
    public void P1Select()
    {
        p1SelectFeedback?.PlayFeedbacks();
        p1Selected = true;
    }

    [Button]
    public void P2Select()
    {
        p2SelectFeedback?.PlayFeedbacks();
        p2Selected = true;
    }

    [Button]
    public void EndCharacterSelect()
    {
        SceneManager.LoadScene(stageID);
        GameHandler.p1CharacterID = p1ID;
        GameHandler.p2CharacterID = p2ID;
    }

}
