using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// PresentationManager
// manages the character animations and other effects for the presentation scene characters
// created 16/9/23
// last modified 16/9/23

public enum PresentationAnim
{
    Attack = 0,
    Jump,
    Fall,
    Sprint,
    Idle,
    IdleArmed,
    Attack2,
    Attack3
}

public class PresentationManager : MonoBehaviour
{
    [SerializeField] private Animator[] maleAnim; // set male animators
    [SerializeField] private PresentationAnim[] maleAnimChoice; // select animation
    [SerializeField] private Animator[] femAnim; // separate handling as the animator isn't fully set up at the date of this writing
    [SerializeField] private PresentationAnim[] femAnimChoice; // select animation
    [SerializeField] private Animation dragAnim; // and the dragon
    [SerializeField] private PresentationAnim dragAnimChoice; // select animation

    private void Start()
    {
        for (int i = 0; i < maleAnim.Length; i++)
        {
            maleAnim[i].SetBool("Armed", true);
            switch (maleAnimChoice[i])
            {
                case PresentationAnim.Jump:
                    maleAnim[i].SetBool("Flying", true);
                    maleAnim[i].SetFloat("Air Speed", 1f);
                    break;
                case PresentationAnim.Fall:
                    maleAnim[i].SetBool("Flying", true);
                    maleAnim[i].SetFloat("Air Speed", -1f);
                    break;
                case PresentationAnim.Sprint:
                    maleAnim[i].SetFloat("Run Blend", 1f);
                    break;
                case PresentationAnim.Idle:
                    maleAnim[i].SetBool("Armed", false);
                    break;
                case PresentationAnim.IdleArmed:
                    //maleAnim[i].dostuff;
                    break;
            }
        }
        for (int i = 0; i < femAnim.Length; i++)
        {
            switch (femAnimChoice[i])
            {
                case PresentationAnim.Jump:
                    femAnim[i].SetBool("Flying", true);
                    femAnim[i].SetFloat("Air Speed", 1f);
                    break;
                case PresentationAnim.Fall:
                    femAnim[i].SetBool("Flying", true);
                    femAnim[i].SetFloat("Air Speed", -1f);
                    break;
                case PresentationAnim.Sprint:
                    femAnim[i].SetFloat("Run Blend", 1f);
                    break;
                case PresentationAnim.Idle:
                    femAnim[i].SetBool("Armed", false);
                    break;
                case PresentationAnim.IdleArmed:
                    //maleAnim[i].dostuff;
                    break;
            }
        }
        switch (dragAnimChoice)
        {
            case PresentationAnim.Attack:
                dragAnim.Play("sj001_run");
                break;
            case PresentationAnim.Jump:
                dragAnim.Play("sj001_run");
                break;
            case PresentationAnim.Fall:
                dragAnim.Play("sj001_run");
                break;
            case PresentationAnim.Sprint:
                dragAnim.Play("sj001_run");
                break;
            case PresentationAnim.Idle:
                dragAnim.Play("sj001_wait");
                break;
            case PresentationAnim.IdleArmed:
                dragAnim.Play("sj001_wait");
                break;
        }
    }

    private void Update()
    {
        for (int i = 0; i < maleAnim.Length; i++)
        {
            if (maleAnimChoice[i] == PresentationAnim.Attack)
            {
                maleAnim[i].SetTrigger("Attack1");
                break;
            }
            if (maleAnimChoice[i] == PresentationAnim.Attack2)
            {
                maleAnim[i].SetTrigger("Attack2");
                break;
            }
            if (maleAnimChoice[i] == PresentationAnim.Attack3)
            {
                maleAnim[i].SetTrigger("Attack3");
                break;
            }
        }
        for (int i = 0; i < femAnim.Length; i++)
        {
            if (femAnimChoice[i] == PresentationAnim.Attack)
            {
                femAnim[i].SetTrigger("Attack1");
                break;
            }
            if (femAnimChoice[i] == PresentationAnim.Attack2)
            {
                femAnim[i].SetTrigger("Attack1");
                break;
            }
            if (femAnimChoice[i] == PresentationAnim.Attack3)
            {
                femAnim[i].SetTrigger("Attack1");
                break;
            }
        }
    }
}
