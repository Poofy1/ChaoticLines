using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public AnimationHandler animHandle;

    //Scene 
    public Chaos mainEvents;
    public SaveHandler saveHandler;
    public Transform player;
    public Transform camera;
    public GameObject mainOri;
    public PlayerMovement movementController;
    public MouseLook mouseLook;

    //Camera Movement
    private float randSceneTime;
    private Vector3[] randZoomRange;
    private Vector3 zoom;
    private Vector3 randRotation;

    //Menu
    public CanvasGroup[] title;
    public CanvasGroup buttons;
    public CanvasGroup fade;

    //local
    private bool MM_Active;
    private bool slowDown;

    private void Start()
    {
        mainEvents.InitializeMainCords();
        StartMainMenu();
    }

    public void StartMainMenu()
    {
        if (mainEvents.on == true) mainEvents.On();
        movementController.active = false;
        mouseLook.active = false;
        MM_Active = true;

        randZoomRange = new Vector3[2];
        StartCoroutine(ResetScene());
        StartCoroutine(MenuFade());
    }

    public void StartScene()
    {
        buttons.alpha = 0;
        title[0].GetComponent<CanvasGroup>().alpha = 0;
        title[1].GetComponent<CanvasGroup>().alpha = 0;
        title[2].GetComponent<CanvasGroup>().alpha = 0;
        movementController.active = true;
        mouseLook.active = true;
        MM_Active = false;
        slowDown = true;
        fade.alpha = 1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (MM_Active)
        {
            player.transform.Rotate(randRotation);
            mainOri.transform.localPosition += zoom;
            camera.LookAt(new Vector3(0, 0, 0));
        }
        else if (slowDown)
        {
            player.transform.Rotate(randRotation);
            mainOri.transform.localPosition += zoom;
            zoom *= 0.9f;
            randRotation *= 0.95f;
            camera.LookAt(new Vector3(0, 0, 0));
            if (Math.Abs(randRotation.x) < 0.005) slowDown = false;
        }
    }


    IEnumerator ResetScene()
    {
        //Reset
        yield return 0;

        //Load random system
        int rand = UnityEngine.Random.Range(0, saveHandler.saveList.Count);
        saveHandler.ButtonClicked(rand);
        saveHandler.LoadSystem(saveHandler.saveList[rand].SaveName);

        //random color
        mainEvents.Color(UnityEngine.Random.Range(1, 10));
        mainEvents.On();
        StartCoroutine(FadeOut(fade, .005f, 0));

        //random thickness 
        mainEvents.ThicknessSlider.value = UnityEngine.Random.Range(0.1f, 1f);
        mainEvents.UpdateThickness();

        //Random step
        float tempStep = UnityEngine.Random.Range(.25f, 1f);
        if (UnityEngine.Random.Range(0, 2) == 0) tempStep *= -1;
        mainEvents.step = tempStep;
        mainEvents.UpdateStep(true);

        //Reset Camera
        randSceneTime = UnityEngine.Random.Range(15f, 30f);

        randZoomRange[0] = new Vector3(0, 0, UnityEngine.Random.Range(-25f, -10f));
        randZoomRange[1] = new Vector3(0, 0, UnityEngine.Random.Range(-50f, -30f));
        zoom = (randZoomRange[1] - randZoomRange[0]) / (randSceneTime * 50);
        mainOri.transform.localPosition = randZoomRange[0];

        randRotation = new Vector3(UnityEngine.Random.Range(-.25f, .25f),
                                   UnityEngine.Random.Range(-.25f, .25f),
                                   UnityEngine.Random.Range(-.25f, .25f));

        //Wait
        yield return new WaitForSeconds(randSceneTime);

        //ResetAgain if in MM
        if (MM_Active)
        {
            yield return StartCoroutine(FadeIn(fade, .005f, 0));
            mainEvents.On();
            StartCoroutine(ResetScene());
        }
        
    }

    IEnumerator MenuFade()
    {
        for (int i = 0; i < title.Length; i++) title[i].alpha = 0;
        buttons.alpha = 0;

        yield return new WaitForSeconds(2);

        StartCoroutine(FadeIn(title[0], .01f, 0f));
        StartCoroutine(FadeIn(title[1], .01f, .5f));
        StartCoroutine(FadeIn(title[2], .01f, 1f));

        StartCoroutine(FadeIn(buttons, .01f, 1f));

    }

    public IEnumerator FadeIn(CanvasGroup obj, float speed, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (obj.alpha < 1)
        {
            obj.alpha += speed;
            yield return new WaitForSeconds(.0025f);
        }
    }

    public IEnumerator FadeOut(CanvasGroup obj, float speed, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (obj.alpha > 0)
        {
            obj.alpha -= speed;
            yield return new WaitForSeconds(.0025f);
        }
    }
}
