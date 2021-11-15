using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSFX : MonoBehaviour
{
    Status status;
    Movement mov;

    [HeaderAttribute("Hurt Sounds")]
    public GameObject hurtSFX;

    [HeaderAttribute("Death Sounds")]
    public GameObject deathSFX;
    [HeaderAttribute("Jump")]
    public GameObject runSFX;
    public VFX runVFX;
    public GameObject jumpSFX;
    public VFX jumpVFX;
    public GameObject landSFX;
    public VFX landVFX;
    [HeaderAttribute("Footsteps")]
    public GameObject footstepSFX;
    public VFX footstepVFX;

    // Start is called before the first frame update
    void Start()
    {
        status = GetComponentInParent<Status>();
        mov = GetComponentInParent<Movement>();
        status.hitstunEvent += HurtSFX;
        status.deathEvent += Death;
        if (mov != null)
            mov.landEvent += Land;
        if (mov != null)
            mov.jumpEvent += Jump;
        if (mov != null)
            mov.runEvent += Run;
    }

    void Run()
    {
        if (runSFX != null) Instantiate(runSFX, transform.position, Quaternion.identity);
        if (runVFX.prefab != null)
        {
            GameObject fx = Instantiate(runVFX.prefab, transform.position, transform.rotation, transform);
            fx.transform.localPosition = runVFX.position;
            fx.transform.localRotation = Quaternion.Euler(runVFX.rotation);
            fx.transform.localScale = runVFX.scale;
            if (GameHandler.Instance.IsPlayer1(transform))
                fx.GetComponent<VFXScript>().ID = 2;
            else fx.GetComponent<VFXScript>().ID = 1;
            fx.transform.SetParent(null);
        }
    }


    void Land()
    {
        if (landSFX != null) Instantiate(landSFX, transform.position, Quaternion.identity);
        if (landVFX.prefab != null)
        {
            GameObject fx = Instantiate(landVFX.prefab, transform.position, transform.rotation, transform);
            fx.transform.localPosition = landVFX.position;
            fx.transform.localRotation = Quaternion.Euler(landVFX.rotation);
            fx.transform.localScale = landVFX.scale;
            if (GameHandler.Instance.IsPlayer1(transform))
                fx.GetComponent<VFXScript>().ID = 2;
            else fx.GetComponent<VFXScript>().ID = 1;
            fx.transform.SetParent(null);
        }
    }

    void Jump()
    {
        if (jumpSFX != null) Instantiate(jumpSFX, transform.position, Quaternion.identity);
        if (jumpVFX.prefab != null)
        {
            GameObject fx = Instantiate(jumpVFX.prefab, transform.position, transform.rotation, transform);
            fx.transform.localPosition = jumpVFX.position;
            fx.transform.localRotation = Quaternion.Euler(jumpVFX.rotation);
            fx.transform.localScale = jumpVFX.scale;
            if (GameHandler.Instance.IsPlayer1(transform))
                fx.GetComponent<VFXScript>().ID = 2;
            else fx.GetComponent<VFXScript>().ID = 1;
            fx.transform.SetParent(null);
        }
    }

    void Death()
    {
        if (deathSFX != null) Instantiate(deathSFX, transform.position, Quaternion.identity);
    }


    public void HurtSFX()
    {
        if (hurtSFX != null)
            Instantiate(hurtSFX, transform.position, Quaternion.identity);
    }



    void Foot(AnimationEvent evt)
    {
        if (evt.animatorClipInfo.weight > 0.5F)
        {
            if (footstepSFX != null) Instantiate(footstepSFX, transform.position, Quaternion.identity);
            if (footstepVFX.prefab != null)
            {
                GameObject fx = Instantiate(footstepVFX.prefab, transform.position, transform.rotation, transform);
                fx.transform.localPosition = footstepVFX.position;
                fx.transform.localRotation = Quaternion.Euler(footstepVFX.rotation);
                fx.transform.localScale = footstepVFX.scale;
                if (GameHandler.Instance.IsPlayer1(transform))
                    fx.GetComponent<VFXScript>().ID = 1;
                else fx.GetComponent<VFXScript>().ID = 2;
                fx.transform.SetParent(null);
            }
        }
    }
}
