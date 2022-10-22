using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{

    public Text handle;
    public Chaos mainEvents;
    public GameObject predefined;
    public GameObject Custom;



    public void Clicked()
    {
        if (gameObject.GetComponent<Slider>().value == 0)
        {
            handle.text = "Custom";
            gameObject.GetComponent<Slider>().value = 1;
        }
        else
        {
            handle.text = "Predefined";
            gameObject.GetComponent<Slider>().value = 0;
        }

        mainEvents.ToggleCustom();
    }



}
