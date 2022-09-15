using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    public GameObject hud;
    public Animator[] hudAnim;
    public Animator[] mmAnim;
    public MainMenu mmController;
    public CanvasGroup hudFade;
    public AudioSource uiSounds;
    public AudioClip audioSelect;
    public AudioClip audioPress;

    private bool inHud;

    private void Update()
    {
        if (Input.GetKeyDown("tab") && inHud)
        {
            hud.SetActive(!hud.activeSelf);
        }
    }

    public void UiSound(int index)
    {
        if(index == 0) uiSounds.PlayOneShot(audioSelect, .15f);
        else uiSounds.PlayOneShot(audioPress, .3f);
    }


    public void GoToHUD()
    {
        hudFade.alpha = 0;
        hudAnim[0].Play("CPEnter");
        hudAnim[1].Play("CUSTEnter");
        hudAnim[2].Play("SETEnter");
        hudAnim[3].Play("MMEnter");
        hudFade.gameObject.SetActive(false);
        inHud = true;
    }

    public void GoToMM()
    {
        StartCoroutine(timer1());
    }

    IEnumerator timer1()
    {
        hudFade.gameObject.SetActive(true);
        StartCoroutine(mmController.FadeIn(hudFade, .01f, 0));
        hudAnim[0].Play("CPLeave");
        hudAnim[1].Play("CUSTLeave");
        hudAnim[2].Play("SETLeave");
        hudAnim[3].gameObject.GetComponent<selfAnimate>().block = true;
        hudAnim[3].Play("MMLeave");
        inHud = false;
        yield return new WaitForSeconds(1f);
        mmController.GetComponent<MainMenu>().StartMainMenu();
        hudAnim[3].gameObject.GetComponent<selfAnimate>().block = false;
    }
}
