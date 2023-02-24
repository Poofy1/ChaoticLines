using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimationHandler : MonoBehaviour
{
    [Header("Refrences")]
    public GameObject hideHud;
    public MainMenu mmController;
    public CanvasGroup hudFade;
    public GameObject crosshair;

    [Header("AnimatedObjects")]
    public Transform title;
    public Transform[] MMButtons;
    public Transform[] OffPositions;
    public GameObject[] ControlButton;
    public GameObject[] CustomButton;
    public GameObject[] OptionButtons;
    public GameObject[] DetailButtons;
    public GameObject[] hints;
    public GameObject TitlePos;
    public SelfAni previewImg;
    public SelfAni MMSettings;
    public SelfAni MMInfo;
    public Transform map;

    private bool hudActive;

    private void Update()
    {
        if (Input.GetKeyDown("tab") && hudActive)
        {
            hideHud.SetActive(!hideHud.activeSelf);
            crosshair.SetActive(!crosshair.activeSelf);
        }
    }

    public void LeaveMM()
    {
        //MM Leave Trasition
        title.LeanMoveX(-800, .5f).setEaseInOutCubic();

        if (MMSettings.open) MMSettings.Clicked();
        if (MMInfo.open) MMInfo.Clicked();

        hudActive = true;

        for (int i = 0; i < 4; i++) MMButtons[i].gameObject.GetComponent<Button>().interactable = false;

        MMButtons[0].transform.LeanMoveX(-800, .5f).setEaseInOutCubic();
        MMButtons[1].transform.LeanMoveX(-800, .65f).setEaseInOutCubic();
        MMButtons[2].transform.LeanMoveX(-800, .8f).setEaseInOutCubic();
        MMButtons[3].transform.LeanMoveX(-800, .95f).setEaseInOutCubic();

        //HUD Enter Transition
        StartCoroutine(GoToHUD());

    }
    IEnumerator GoToHUD()
    {
        yield return new WaitForSeconds(1);
        mmController.StartScene();
        hudFade.alpha = 0;

        ShowHud();
        

        hudFade.gameObject.SetActive(false);
    }


    public void EnterMM()
    {
        hudActive = false;
        StartCoroutine(GoToMM());
    }
    
    IEnumerator GoToMM()
    {
        hudFade.gameObject.SetActive(true);

        //remove hud
        HideHud();

        yield return StartCoroutine(mmController.FadeDark(hudFade, 1f, 0));

        

        //reset menu positions
        title.localPosition = new Vector3(TitlePos.transform.localPosition.x, title.localPosition.y, title.localPosition.z);
        for (int i = 0; i < 4; i++) MMButtons[i].localPosition = new Vector3(-50, MMButtons[i].localPosition.y, MMButtons[i].localPosition.z);

        for (int i = 0; i < 4; i++) MMButtons[i].gameObject.GetComponent<Button>().interactable = true;
        mmController.GetComponent<MainMenu>().StartMainMenu();
    }


    public void HideHud()
    {
        LeanTween.moveLocalX(ControlButton[0], OffPositions[1].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(CustomButton[0], OffPositions[1].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(OptionButtons[0], OffPositions[0].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(DetailButtons[0], OffPositions[0].transform.localPosition.x, 1).setEaseInOutCubic();

        if (ControlButton[0].GetComponent<SelfAni>().open)
        {
            previewImg.Clicked();
            ControlButton[0].GetComponent<SelfAni>().open = false;
        }
        if (CustomButton[0].GetComponent<SelfAni>().open) CustomButton[0].GetComponent<SelfAni>().open = false;
        if (OptionButtons[0].GetComponent<SelfAni>().open) OptionButtons[0].GetComponent<SelfAni>().open = false;
        if (DetailButtons[0].GetComponent<SelfAni>().open) DetailButtons[0].GetComponent<SelfAni>().open = false;
    }

    public void ShowHud()
    {
        LeanTween.moveLocalX(ControlButton[0], ControlButton[1].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(CustomButton[0], CustomButton[1].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(OptionButtons[0], OptionButtons[1].transform.localPosition.x, 1).setEaseInOutCubic();
        LeanTween.moveLocalX(DetailButtons[0], DetailButtons[1].transform.localPosition.x, 1).setEaseInOutCubic();
    }
}
