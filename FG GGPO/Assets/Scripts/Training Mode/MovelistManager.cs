using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class MovelistManager : MonoBehaviour
{
    public enum MovelistTab { Normals, Specials, Supers }
    public MovelistTab activeTab;
    public CustomButton[] tabButtons;
    public GameObject[] tabs;
    public GameObject[] defaultGO;
    public Moveset moveset;
    public List<Move> moves;
    public GameObject movelistPrefab;
    public Transform container;
    public Transform normalDivider;
    public Transform specialDivider;
    public Transform superDivider;
    public MovelistDescription description;


    private void OnEnable()
    {
        InputManager.Instance.p1Input.rightInput += RightTab;
        InputManager.Instance.p1Input.leftInput += LeftTab;
        InputManager.Instance.p2Input.rightInput += RightTab;
        InputManager.Instance.p2Input.leftInput += LeftTab;
        OpenTab();
    }

    private void OnDisable()
    {
        InputManager.Instance.p1Input.rightInput -= RightTab;
        InputManager.Instance.p1Input.leftInput -= LeftTab;
        InputManager.Instance.p2Input.rightInput -= RightTab;
        InputManager.Instance.p2Input.leftInput -= LeftTab;
    }


    [Button]
    public void RightTab()
    {
        int tabID = (int)activeTab;
        tabID = (tabID + 1) % 3;
        activeTab = (MovelistTab)(tabID);
        OpenTab();
    }
    [Button]
    public void LeftTab()
    {
        int tabID = (int)activeTab;
        tabID = ((tabID - 1) % 3);
        if (tabID < 0) tabID = 2;

        activeTab = (MovelistTab)(tabID);
        OpenTab();
    }


    [Button]
    public void OpenTab()
    {

        for (int i = 0; i < tabButtons.Length; i++)
        {
            tabButtons[i].selected = (i == (int)activeTab);
            tabButtons[i].ExpandTab();
        }
        for (int i = 0; i < tabs.Length; i++)
        {
            tabs[i].SetActive(i == (int)activeTab);
        }
        if (UIManager.Instance != null)
            switch (activeTab)
            {
                case MovelistTab.Normals:
                    UIManager.Instance.SetActive(normalDivider.gameObject);
                    break;
                case MovelistTab.Specials:
                    UIManager.Instance.SetActive(specialDivider.gameObject);
                    break;
                case MovelistTab.Supers:
                    UIManager.Instance.SetActive(superDivider.gameObject);
                    break;
                default:
                    break;
            }
    }

    public void DescriptionWindow(Move move)
    {
        description.DisplayMove(move);
    }

    private void OnValidate()
    {
        OpenTab();
    }

    [Button]
    public void GetMoveset()
    {
        moves.Clear();
        FieldInfo[] defInfo1 = moveset.GetType().GetFields();

        for (int i = defInfo1.Length - 1; i > -1; i--)
        {
            object obj = moveset;
            object var1 = defInfo1[i].GetValue(obj);
            //ADDING VALUES
            if (var1 is Move)
            {
                moves.Add((Move)var1);
            }
            else if (var1 is SpecialMove[])
            {
                foreach (var item in (SpecialMove[])var1)
                {
                    moves.Add(item.move);
                }
            }
        }
    }

    [Button]
    void SetupMove()
    {
        foreach (var item in moves)
        {
            if (item.type != MoveType.Movement && item.type != MoveType.UniversalMechanics)
            {
                GameObject GO;
                switch (item.type)
                {
                    case MoveType.Normal:
                        GO = Instantiate(movelistPrefab, normalDivider.parent);
                        GO.GetComponent<MovelistDisplay>().SetupMove(item);
                        GO.transform.SetSiblingIndex(normalDivider.GetSiblingIndex() + 1);
                        break;
                    case MoveType.Special:
                        GO = Instantiate(movelistPrefab, normalDivider.parent);
                        GO.GetComponent<MovelistDisplay>().SetupMove(item);
                        GO.transform.SetSiblingIndex(normalDivider.GetSiblingIndex() + 1);
                        break;

                    default:
                        break;
                }
            }
        }

        foreach (var item in moveset.specials)
        {
            if (item.move.type != MoveType.Movement && item.move.type != MoveType.UniversalMechanics)
            {
                GameObject GO;
                switch (item.move.type)
                {
                    case MoveType.Special:
                        GO = Instantiate(movelistPrefab, specialDivider.parent);
                        GO.GetComponent<MovelistDisplay>().SetupSpecialMove(item.move, item.motionInput);
                        GO.transform.SetSiblingIndex(specialDivider.GetSiblingIndex() + 1);
                        break;
                    case MoveType.EX:
                        GO = Instantiate(movelistPrefab, specialDivider.parent);
                        GO.GetComponent<MovelistDisplay>().SetupSpecialMove(item.move, item.motionInput);
                        GO.transform.SetSiblingIndex(specialDivider.GetSiblingIndex() + 1);
                        break;
                    case MoveType.Super:
                        GO = Instantiate(movelistPrefab, superDivider.parent);
                        GO.GetComponent<MovelistDisplay>().SetupSpecialMove(item.move, item.motionInput);
                        GO.transform.SetSiblingIndex(superDivider.GetSiblingIndex() + 1);
                        break;
                    default:
                        break;
                }
            }
        }
    }
    [Button]
    public void DeleteMoves()
    {
        int normalChildren = normalDivider.parent.childCount;
        for (int i = normalChildren - 1; i > 0; i--)
        {
            GameObject GO = normalDivider.parent.GetChild(i).gameObject;
            if (GO.transform != normalDivider && GO.transform != specialDivider && GO.transform != superDivider)
            {
                DestroyImmediate(GO);
            }
        }
        int specialChildren = specialDivider.parent.childCount;
        for (int i = specialChildren - 1; i > 0; i--)
        {
            GameObject GO = specialDivider.parent.GetChild(i).gameObject;
            if (GO.transform != normalDivider && GO.transform != specialDivider && GO.transform != superDivider)
            {
                DestroyImmediate(GO);
            }
        }
        int superChildren = superDivider.parent.childCount;
        for (int i = superChildren - 1; i > 0; i--)
        {
            GameObject GO = superDivider.parent.GetChild(i).gameObject;
            if (GO.transform != normalDivider && GO.transform != specialDivider && GO.transform != superDivider)
            {
                DestroyImmediate(GO);
            }
        }
    }
}
