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
    public GameObject crosshair;

    //Camera Movement
    private float randSceneTime;
    private Vector3[] randZoomRange;
    private Vector3 zoom;
    private Vector3 randRotation;

    //Menu
    public CanvasGroup[] title;
    public CanvasGroup buttons;

    void Start()
    {
        randZoomRange = new Vector3[2];
        StartCoroutine(ResetScene());
        StartCoroutine(MenuFade());
    }

    public void StartScene()
    {
        hud.SetActive(true);
        crosshair.SetActive(true);
        mainMenu.SetActive(false);
        mainCam.SetActive(true);

        mainEvents.System(Random.Range(1, 25));
        mainEvents.Color(Random.Range(1, 10));
        mainEvents.On();
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
        yield return new WaitForSeconds(.1f);
        mainEvents.System(Random.Range(1, 25));
        mainEvents.Color(Random.Range(1, 10));
        mainEvents.On();
        mainEvents.step = Random.Range(-.0025f, .0025f);

        //Reset Camera
        randSceneTime = Random.Range(15f, 45f);

        randZoomRange[0] = new Vector3(0, 0, Random.Range(-10f, -2f));
        randZoomRange[1] = new Vector3(0, 0, Random.Range(-20f, -2f));
        zoom = (randZoomRange[1] - randZoomRange[0]) / (randSceneTime * 50);
        menuCam.transform.localPosition = randZoomRange[0];

        randRotation = new Vector3(Random.Range(-.2f, .2f),
                                   Random.Range(-.2f, .2f),
                                   Random.Range(-.2f, .2f));

        //Wait
        yield return new WaitForSeconds(randSceneTime);

        //ResetAgain
        mainEvents.On();
        StartCoroutine(ResetScene());
    }

    IEnumerator MenuFade()
    {
        for (int i = 0; i < title.Length; i++) title[i].alpha = 0;
        buttons.alpha = 0;

        yield return new WaitForSeconds(2);

        StartCoroutine(Fade(title[0], .0015f, 0f));
        StartCoroutine(Fade(title[1], .0025f, .5f));
        StartCoroutine(Fade(title[2], .0035f, 1f));

        StartCoroutine(Fade(buttons, .0015f, 3f));

    }

    IEnumerator Fade(CanvasGroup obj, float speed, float wait)
    {
        yield return new WaitForSeconds(wait);
        while (obj.alpha < 1)
        {
            obj.alpha += speed;
            yield return new WaitForSeconds(.0025f);
        }
    }
}
