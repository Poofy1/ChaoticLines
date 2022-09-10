using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    //Scene 
    public GameObject mainMenu;
    public Chaos mainEvents;
    public GameObject hud;
    public GameObject mainCam;
    public GameObject menuCam;
    public GameObject mainOri;
    public GameObject crosshair;

    //Camera Movement
    private float randSceneTime;
    private Vector3[] randZoomRange;
    private Vector3 zoom;
    private Vector3 randRotation;

    //Menu
    public CanvasGroup[] title;
    public CanvasGroup buttons;
    public CanvasGroup fade;

    void Start()
    {
        StartMainMenu();
    }

    public void StartMainMenu()
    {
        if (mainEvents.on == true) mainEvents.On();
        hud.SetActive(false);
        crosshair.SetActive(false);
        mainCam.SetActive(false);
        mainMenu.SetActive(true);
        randZoomRange = new Vector3[2];
        StartCoroutine(ResetScene());
        StartCoroutine(MenuFade());
    }

    public void StartScene()
    {
        hud.SetActive(true);
        crosshair.SetActive(true);
        mainMenu.SetActive(false);
        mainCam.transform.rotation = menuCam.transform.rotation;
        mainOri.transform.position = menuCam.transform.position;
        mainCam.SetActive(true);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.transform.Rotate(randRotation);
        menuCam.transform.localPosition += zoom;
    }

    IEnumerator ResetScene()
    {
        //Reset
        yield return 0;
        mainEvents.System(UnityEngine.Random.Range(1, 25));
        mainEvents.Color(UnityEngine.Random.Range(1, 10));
        mainEvents.On();
        StartCoroutine(FadeOut(fade, .001f, 0));

        float tempStep = UnityEngine.Random.Range(.004f, .008f);
        if (UnityEngine.Random.Range(0, 2) == 0) tempStep *= -1;
        mainEvents.step = tempStep;
        mainEvents.UpdateStep(true);

        //Reset Camera
        randSceneTime = UnityEngine.Random.Range(15f, 45f);

        randZoomRange[0] = new Vector3(0, 0, UnityEngine.Random.Range(-5f, -2f));
        randZoomRange[1] = new Vector3(0, 0, UnityEngine.Random.Range(-20f, -10f));
        zoom = (randZoomRange[1] - randZoomRange[0]) / (randSceneTime * 50);
        menuCam.transform.localPosition = randZoomRange[0];

        randRotation = new Vector3(UnityEngine.Random.Range(-.2f, .2f),
                                   UnityEngine.Random.Range(-.2f, .2f),
                                   UnityEngine.Random.Range(-.2f, .2f));

        //Wait
        yield return new WaitForSeconds(randSceneTime);

        //ResetAgain
        yield return StartCoroutine(FadeIn(fade, .001f, 0));
        mainEvents.On();
        StartCoroutine(ResetScene());
    }

    IEnumerator MenuFade()
    {
        for (int i = 0; i < title.Length; i++) title[i].alpha = 0;
        buttons.alpha = 0;

        yield return new WaitForSeconds(2);

        StartCoroutine(FadeIn(title[0], .0015f, 0f));
        StartCoroutine(FadeIn(title[1], .0025f, .5f));
        StartCoroutine(FadeIn(title[2], .0035f, 1f));

        StartCoroutine(FadeIn(buttons, .0015f, 2f));

    }

    IEnumerator FadeIn(CanvasGroup obj, float speed, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (obj.alpha < 1)
        {
            obj.alpha += speed;
            yield return new WaitForSeconds(.0025f);
        }
    }

    IEnumerator FadeOut(CanvasGroup obj, float speed, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (obj.alpha > 0)
        {
            obj.alpha -= speed;
            yield return new WaitForSeconds(.0025f);
        }
    }
}
