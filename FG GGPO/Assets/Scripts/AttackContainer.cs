using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackContainer : MonoBehaviour
{
    public GameObject[] hitboxes;
    [HideInInspector] public Status status;
    [HideInInspector] public AttackScript attack;
    [HideInInspector] public Move move;
 public Transform target;
    public GameObject[] visuals;
    int attackNumber = 0;
    bool interrupted;

    private void Start()
    {
   }



    public void ActivateHitbox()
    {
        if (hitboxes.Length > attackNumber && !interrupted)
        {

            ActivateGameobjects(attackNumber);
            attackNumber++;
        }
    }

    void ActivateGameobjects(int number)
    {
        foreach (GameObject hitbox in hitboxes)
        {
            hitbox.SetActive(false);
        }


        foreach (GameObject visual in visuals)
        {
            visual.SetActive(false);
        }


        hitboxes[number].SetActive(true);
        visuals[number].SetActive(true);
    }

    public void InterruptAttack()
    {
        attackNumber = hitboxes.Length;
        interrupted = true;
        StopAllCoroutines();
        DeactivateHitbox();
    }

    public void DeactivateHitbox()
    {
        foreach (GameObject hitbox in hitboxes)
        {

            hitbox.SetActive(false);
        }
    }
}
