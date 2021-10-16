using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionManager : MonoBehaviour
{
    public int comboStep;
    public Combo activeComboTrial;
    public Status playerStatus;
    public Status dummyStatus;    
    public AttackScript playerAttack;
    public AttackScript dummyAttack;
    // Start is called before the first frame update
    void Start()
    {
        playerAttack.attackHitEvent += ActionPerformed;
    }

    public void ActionPerformed(Move move) {
        if (activeComboTrial.actions[comboStep].move == move) {
            comboStep++;
        }
    }
}
