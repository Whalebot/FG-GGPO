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

    public enum Phase {charSelect, stageBGMSelect };

    public Phase phase = Phase.charSelect;

    public InputManager inputManager;

    public GameObject characterSelect;
    public GameObject stageBGMSelect;

    public int p1ID;
    public int p2ID;
    public int stageID;
    public int bgmID;

    bool p1Selected;
    bool p2Selected;

    public CharacterSelectButton p1Hover;
    public CharacterSelectButton p2Hover;

    public int stageHoverID;
    public int bgmHoverID;

    public BGMProfile[] bgmProfiles;

    public MMFeedbacks p1UpdateFeedback;
    public MMFeedbacks p2UpdateFeedback;
    public MMFeedbacks p1SelectFeedback;
    public MMFeedbacks p2SelectFeedback;
    public MMFeedbacks EndCharacterSelectFeedback;

    public float cursorSpeed;
    Vector3 velocity1;
    Vector3 velocity2;
    Vector3 velocity3;

    [Header("Character select display")]

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

    [Header("Stage & BGM select display")]

    public Transform stageSelectCursor;

    public Transform[] stages;

    public Image stagePreview;
    public Image stageLayout;
    public TextMeshProUGUI stageName;

    public Image bgmPortrait;
    public TextMeshProUGUI bgmName;


    // Start is called before the first frame update

    private void Awake()
    {
        inputManager = InputManager.Instance.GetComponent<InputManager>();


    }



    void Start()
    {
        inputManager.p1Input.upInput += p1Up;
        inputManager.p1Input.downInput += p1Down;
        inputManager.p1Input.leftInput += p1Left;
        inputManager.p1Input.rightInput += p1Right;
        inputManager.p1Input.southInput += P1Select;
        inputManager.p1Input.eastInput += P1Deselect;

        if (GameHandler.gameModeID == 0 || GameHandler.gameModeID == -1)
        {
            inputManager.p2Input.upInput += p2Up;
            inputManager.p2Input.downInput += p2Down;
            inputManager.p2Input.leftInput += p2Left;
            inputManager.p2Input.rightInput += p2Right;
            inputManager.p2Input.southInput += P2Select;
            inputManager.p2Input.eastInput += P2Deselect;
        }
    }

    private void OnDisable()
    {
        inputManager.p1Input.upInput -= p1Up;
        inputManager.p1Input.downInput -= p1Down;
        inputManager.p1Input.leftInput -= p1Left;
        inputManager.p1Input.rightInput -= p1Right;
        inputManager.p1Input.southInput -= P1Select;
        inputManager.p1Input.eastInput -= P1Deselect;

        if (GameHandler.gameModeID == 0 || GameHandler.gameModeID == -1)
        {
            inputManager.p2Input.upInput -= p2Up;
            inputManager.p2Input.downInput -= p2Down;
            inputManager.p2Input.leftInput -= p2Left;
            inputManager.p2Input.rightInput -= p2Right;
            inputManager.p2Input.southInput -= P2Select;
            inputManager.p2Input.eastInput -= P2Deselect;
        }

        else if (GameHandler.gameModeID == 1)
        {
            inputManager.p1Input.upInput -= p2Up;
            inputManager.p1Input.downInput -= p2Down;
            inputManager.p1Input.leftInput -= p2Left;
            inputManager.p1Input.rightInput -= p2Right;
            inputManager.p1Input.southInput -= P2Select;
            inputManager.p1Input.eastInput -= P2Deselect;
        }

    }

    // Update is called once per frame
    void Update()
    {
        p1Cursor.position = Vector3.SmoothDamp(p1Cursor.position, p1Hover.transform.position, ref velocity1, cursorSpeed);
        p2Cursor.position = Vector3.SmoothDamp(p2Cursor.position, p2Hover.transform.position, ref velocity2, cursorSpeed);

        stageSelectCursor.position = Vector3.SmoothDamp(stageSelectCursor.position, stages[stageHoverID].position, ref velocity3, cursorSpeed);
    }

    public void p1Up()
    {
        switch (phase)
        {
            case Phase.charSelect:
                if (p1Selected) return;
                p1Hover = p1Hover.up.GetComponent<CharacterSelectButton>();
                UpdateP1();
                break;

            case Phase.stageBGMSelect:
                if (bgmHoverID == 0)
                {
                    bgmHoverID = bgmProfiles.Length - 1;
                }
                else
                {
                    bgmHoverID -= 1;
                }
                UpdateBGM();
                break;
        }


    }

    public void p1Down()
    {
        switch (phase)
        {
            case Phase.charSelect:
                if (p1Selected) return;
                p1Hover = p1Hover.down.GetComponent<CharacterSelectButton>();
                UpdateP1();
                break;

            case Phase.stageBGMSelect:
                if(bgmHoverID == bgmProfiles.Length - 1)
                {
                    bgmHoverID = 0;
                }
                else
                {
                    bgmHoverID += 1;
                }
                UpdateBGM();
                break;
        }

    }

    public void p1Left()
    {
        switch (phase)
        {
            case Phase.charSelect:
                if (p1Selected) return;
                p1Hover = p1Hover.left.GetComponent<CharacterSelectButton>();
                UpdateP1();
                break;

            case Phase.stageBGMSelect:
                if (stageHoverID == 0)
                {
                    stageHoverID = stages.Length - 1;
                }
                else
                {
                    stageHoverID -= 1;
                }
                UpdateStage();
                break;
        }

    }

    public void p1Right()
    {
        switch (phase)
        {
            case Phase.charSelect:
                if (p1Selected) return;
                p1Hover = p1Hover.right.GetComponent<CharacterSelectButton>();
                UpdateP1();
                break;

            case Phase.stageBGMSelect:
                if (stageHoverID == stages.Length - 1)
                {
                    stageHoverID = 0;
                }
                else
                {
                    stageHoverID += 1;
                }
                UpdateStage();
                break;
        }

    }

    public void p2Up()
    {
        if (p2Selected) return;
        p2Hover = p2Hover.up.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Down()
    {
        if (p2Selected) return;
        p2Hover = p2Hover.down.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Left()
    {
        if (p2Selected) return;
        p2Hover = p2Hover.left.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void p2Right()
    {
        if (p2Selected) return;
        p2Hover = p2Hover.right.GetComponent<CharacterSelectButton>();
        UpdateP2();
    }

    public void UpdateP1()
    {
        p1UpdateFeedback?.PlayFeedbacks();
        p1Name.text = p1Hover.profile.name;
        p1Description.text = p1Hover.profile.description;
        p1Playstyle.text = p1Hover.profile.playstyle;
        p1Portrait.sprite = p1Hover.profile.splashArt;
        p1ID = p1Hover.profile.iD;
    }

    public void UpdateP2()
    {
        p2UpdateFeedback?.PlayFeedbacks();
        p2Name.text = p2Hover.profile.name;
        p2Description.text = p2Hover.profile.description;
        p2Playstyle.text = p2Hover.profile.playstyle;
        p2Portrait.sprite = p2Hover.profile.splashArt;
        p2ID = p2Hover.profile.iD;
    }

    public void UpdateStage()
    {
        stagePreview.sprite = stages[stageHoverID].GetComponent<StageSelectButton>().stageProfile.preview;
        stageLayout.sprite = stages[stageHoverID].GetComponent<StageSelectButton>().stageProfile.overview;
        stageName.text = stages[stageHoverID].GetComponent<StageSelectButton>().stageProfile.name;
        stageID = stages[stageHoverID].GetComponent<StageSelectButton>().stageProfile.ID;
    }

    public void UpdateBGM()
    {
        bgmPortrait.sprite = bgmProfiles[bgmHoverID].characterPortrait;
        bgmName.text = bgmProfiles[bgmHoverID].name;
        bgmID = bgmProfiles[bgmHoverID].ID;
    }

    [Button]
    public void P1Select()
    {

        switch (phase)
        {
            case Phase.charSelect:
                p1SelectFeedback?.PlayFeedbacks();
                p1Selected = true;
                if (GameHandler.gameModeID == 1)
                {
                    P1ControlP2();
                }
                else if (GameHandler.gameModeID == 2) EndCharacterSelect();

                if (p2Selected) StageBGMSelect();
                break;

            case Phase.stageBGMSelect:

                EndCharacterSelect();
                break;
        }


    }

    public void P1ControlP2()
    {
        inputManager.p1Input.upInput += p2Up;
        inputManager.p1Input.downInput += p2Down;
        inputManager.p1Input.leftInput += p2Left;
        inputManager.p1Input.rightInput += p2Right;
        inputManager.p1Input.southInput += P2Select;
        inputManager.p1Input.eastInput += P2Deselect;

        inputManager.p1Input.upInput -= p1Up;
        inputManager.p1Input.downInput -= p1Down;
        inputManager.p1Input.leftInput -= p1Left;
        inputManager.p1Input.rightInput -= p1Right;
        inputManager.p1Input.southInput -= P1Select;
        inputManager.p1Input.eastInput -= P1Deselect;
       
    }

    [Button]
    public void P2Select()
    {
        switch (phase)
        {
            case Phase.charSelect:
                p2SelectFeedback?.PlayFeedbacks();
                p2Selected = true;
                if (p1Selected) StageBGMSelect();
                break;

            case Phase.stageBGMSelect:

                break;
        }
    }

    public void P1Deselect()
    {
        switch (phase)
        {
            case Phase.charSelect:
                p1Selected = false;
                break;

            case Phase.stageBGMSelect:

                CharacterSelect();
                break;
        }

    }

    [Button]
    public void P2Deselect()
    {
        switch (phase)
        {
            case Phase.charSelect:
                p2Selected = false;
                break;

            case Phase.stageBGMSelect:

                CharacterSelect();
                break;
        }

    }

    [Button]
    public void StageBGMSelect()
    {
        phase = Phase.stageBGMSelect;
        UpdateBGM();
        UpdateStage();

        characterSelect.SetActive(false);
        stageBGMSelect.SetActive(true);

    }

    [Button]
    public void CharacterSelect()
    {
        phase = Phase.charSelect;

        p1Selected = false;
        p2Selected = false;

        characterSelect.SetActive(true);
        stageBGMSelect.SetActive(false);

    }

    [Button]
    public void EndCharacterSelect()
    {
        StartCoroutine(DelayTransition());
    }
    IEnumerator DelayTransition()
    {
        GameHandler.p1CharacterID = p1ID;
        GameHandler.p2CharacterID = p2ID;
        yield return new WaitForFixedUpdate();

        SceneManager.LoadScene(stageID);
    }
}
