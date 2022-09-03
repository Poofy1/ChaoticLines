﻿
using UnityEngine;
using UnityEngine.UI;
using System;

public class Chaos : MonoBehaviour
{
    //System vars
    public GameObject trail;
    public int trail_Amount;
    public float t;
    public float step;
    public float scale;

    //private vars
    private float[] x_AR;
    private float[] y_AR;
    private float[] z_AR;
    private bool[] active;
    private GameObject[] trails;
    private int system = 1;
    private bool on = false;

    //Canvas vars
    public GameObject canvas;
    public Slider AmountSlider;
    public Text AmountText;
    public Slider ScaleSlider;
    public Text ScaleText;
    public Slider StepSlider;
    public Text StepText;
    public Text TimeText;
    public Text SystemTitle;
    public Text SystemDisplay;
    public Button Activate;
    public Text ActivateText;
    public Button[] SysBut;

    private bool pauseTemp = false;

    //Canvas Updates
    public void UpdateAmount()
    {
        trail_Amount = (int)AmountSlider.value;
        AmountText.text = trail_Amount.ToString("0");
    }

    public void UpdateScale()
    {
        scale = ScaleSlider.value;
        ScaleText.text = scale.ToString("0.0");
    }
    public void UpdateStep()
    {
        if(pauseTemp == false)
        {
            step = StepSlider.value;
        }
        StepText.text = (StepSlider.value * 50).ToString("0.00");
    }

    //Starting
    public void On()
    {
        on = !on;
        if(on)
        {
            pauseTemp = false;
            ActivateText.text = "Clear";
            Activate.GetComponent<Image>().color = new Color(.4f, 0, 0);
            UpdateAmount();
            UpdateScale();
            UpdateStep();
            AmountSlider.interactable = false;
            ScaleSlider.interactable = false;
            for (int i = 0; i < SysBut.Length; i++)
                SysBut[i].interactable = false;

            //Initialize x, y
            x_AR = new float[trail_Amount];
            y_AR = new float[trail_Amount];
            z_AR = new float[trail_Amount];
            active = new bool[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                active[i] = true;
            }

            //Find starting points
            UpdateEquations();

            //Initialize trails
            trails = new GameObject[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                if (!float.IsNaN(x_AR[i]) && !float.IsNaN(x_AR[i]) && !float.IsNaN(z_AR[i]) && active[i])
                {
                    Debug.Log("" + x_AR[i] + " " + x_AR[i] + " " + z_AR[i]);
                    //Create and Find
                    var name = Instantiate(trail, new Vector3(x_AR[i] * scale, y_AR[i] * scale, z_AR[i] * scale), Quaternion.identity);
                    name.gameObject.name = "Trail" + i;
                    trails[i] = GameObject.Find("Trail" + i);

                    //Colors
                    trails[i].GetComponent<TrailRenderer>().startColor = new Color(UnityEngine.Random.Range(0f, 1f),
                                                                                   UnityEngine.Random.Range(0f, 1f),
                                                                                   UnityEngine.Random.Range(0f, 1f),
                                                                                   1f);
                    trails[i].GetComponent<TrailRenderer>().endColor = new Color(.5f, .5f, .5f, .5f);
                }
            }
        }
        else
        {
            ActivateText.text = "Start";
            Activate.GetComponent<Image>().color = new Color(.6f, .6f, .6f);
            pauseTemp = false;
            t = 0;
            AmountSlider.interactable = true;
            ScaleSlider.interactable = true;
            for (int i = 0; i < SysBut.Length; i++)
                SysBut[i].interactable = true;
            for (int i = 0; i < trail_Amount; i++)
            {
                Destroy(trails[i]);
            }
        }
    }

    //Pausing
    public void Pause()
    {
        if(pauseTemp == false)
        {
            step = 0;
            pauseTemp = true;
        }
        else
        {
            step = StepSlider.value;
            pauseTemp = false;
        }

    }

    //SYSTEMS
    public void SystemConfig()
    {
        SystemTitle.text = "System "+ system;
        for (int i = 0; i < SysBut.Length; i++)
            SysBut[i].GetComponent<Image>().color = new Color(.4f, .4f, .4f);
        SysBut[system - 1].GetComponent<Image>().color = new Color(.2f, .2f, .2f);
    }
    
    public void System1()
    {
        system = 1;
        SystemDisplay.text = "X = (-x * x) + (z * t) + y\nY = (x * x) - (y * z) - (t * t) - (y * x) + (y * t) - x + y\nZ = (z * -z) - (t * x) + (z * t) + x + y";
        SystemConfig();
    }

    public void System2()
    {
        system = 2;
        SystemDisplay.text = "X = (x * y * -t * z) + (t * -z) - x\nY = (z * y * -y) - (z * x) - t + y\nZ = (-x * t * t) * (t * x) - y * y";
        SystemConfig();
    }

    public void System3()
    {
        system = 3;
        SystemDisplay.text = "X = (-x * x) + (z * t) + y\nY = (x * t) + z - (x * x) - x\nZ = (z * -z) - (t * x) + (z * t) - x + y";
        SystemConfig();
    }

    public void System4()
    {
        system = 4;
        SystemDisplay.text = "X = (-z * z) + (z * t) + (y * y)\nY = (y * -t) - (x * x) - x + z\nZ = (x * x) - (t * -z) + (z * -t) + x + y";
        SystemConfig();
    }

    public void System5()
    {
        system = 5;
        SystemDisplay.text = "X = (x * x * -t * y) + (z * y * t) - (t * x) + y\nY = (-y * t * z) - (t * x) + x\nZ = (z * z * -y * -x * t * t) - (t * x) + (t * y) + z + y";
        SystemConfig();
    }

    public void System6()
    {
        system = 6;
        SystemDisplay.text = "X = (x * -x * -t * y) + (z * z * t) + y\nY = (-y * t * -z) - (t * x) - x\nZ = (z * z * -y * -x) + (-y * t * -z) - (t * -t) - z + y";
        SystemConfig();
    }

    public void System7()
    {
        system = 7;
        SystemDisplay.text = "X = (x * z * t) - (t * y) + (t * t) + x - y\nY = (x * x * -t) - Sqrt(Abs(t * y * x)) + (y * t) + z + x\nZ = (z * y * t) + (y * t * -z) - (x * -t) + y";
        SystemConfig();
    }

    public void System8()
    {
        system = 8;
        SystemDisplay.text = "X = (z * x * t) + (y * x * t) - (z * x * t) - (x * t) - x + y\nY = (t * -z) + (x * t) + (y * t) + y - z + t\nZ = (z * t) + (z * x * t) + (y * x * t) + y + x";
        SystemConfig();
    }

    public void System9()
    {
        system = 9;
        SystemDisplay.text = "X = (x * y) - (y * -t) + (-x * t) + t - z\nY = (x * -z) + (x * x) + (t * -t) - y - z\nZ = (y * -x) + (x * -t) + (y * y) - t";
        SystemConfig();
    }

    public void System10()
    {
        system = 10;
        SystemDisplay.text = "X = (y * y) - (y * -t) + (-x * t) + (z * t) + y - z\nY = (y * z * t) + (y * x) + (t * -t) - y + z\nZ = (y * -x * t) + (x * -t) + (y * y * -t) + (z * z * t) - (-y * -z * t) - x";
        SystemConfig();
    }

    public void System11()
    {
        system = 11;
        SystemDisplay.text = "X = (z * z) + (y * x) - (z * -x) + (-x * t) + t + z\nY = (t * -z) + (x * y) + (y * t) + y - z + t\nZ = (z * t) + (-z * t) + (y * x) + t + x";
        SystemConfig();
    }

    public void System12()
    {
        system = 12;
        SystemDisplay.text = "X = (z * z) + (y * x) - (z * -x) + (-x * t) + t + z\nY = (t * -z) + (x * y) + (y * t) + y - z + t\nZ = (z * t) + (-z * t) + (y * x) - t + x";
        SystemConfig();
    }

    public void System13()
    {
        system = 13;
        SystemDisplay.text = "X = (t * -t) + (z * t) - (y * -x) - y\nY = (x * x) - (t * z) - (t * t) + (y * x) + (y * t) - x\nZ = (z * -z) - (t * x) + (z * t) + x + y";
        SystemConfig();
    }

    public void System14()
    {
        system = 14;
        SystemDisplay.text = "X = (-x * t) - (y * -t) + (-x * t) - t + z\nY = (x * -z * t) + (x * t) + (t * x) - y - z\nZ = (y * t) - (x * -t) + (z * t) - z";
        SystemConfig();
    }

    public void System15()
    {
        system = 15;
        SystemDisplay.text = "X = (y * y * t) + (-y * x * t) - (z * y * t) - (x * t)\nY = (t * -z) + (x * -t) + (y * t) + y - z + t - y\nZ = (z * -t) + (z * z * t) + (y * x * t) - y - x + t";
        SystemConfig();
    }

    public void System16()
    {
        system = 16;
        SystemDisplay.text = "X = (-x * x * y) + (z * t) + (x * t) + y\nY = (x * t) - (x * -x) - x + y\nZ = (z * x) - (-t * x) + (z * t) + y";
        SystemConfig();
    }

    public void System17()
    {
        system = 17;
        SystemDisplay.text = "X = (y * -t) + (-x * t) + (z * -t) - z\nY = (y * z * t) + (y * x) - y + z\nZ = (y * -x * t) + (y * z * -t) + (z * -z * t) - (-y * -z * -t) - x";
        SystemConfig();
    }

    public void System18()
    {
        system = 18;
        SystemDisplay.text = "X = (z * x * t) + (y * t) - (z * t) + (-x * t) + z + x\nY = (t * -z) - (x * y) + (-y * t) + y - z\nZ = (z * t) + (-z * t) - (y * z * t) - t";
        SystemConfig();
    }

    public void System19()
    {
        system = 19;
        SystemDisplay.text = "X = (z * z * -y) + (-y * t * -z) + (z * -t) + y\nY = (t * x * -z) + (-y * t) + z\nZ = (z * t) + (-z * t) - (y * x) - t + x";
        SystemConfig();
    }

    public void System20()
    {
        system = 20;
        SystemDisplay.text = "X = (t * -z) + (x * y) + y - z + t\nY = (t * -z) + (y * -t) - (y * t) + (-x * t) - y\nZ = (-x * -t) - (z * x * t) + (z * y * t)";
        SystemConfig();
    }

    public void System21()
    {
        system = 21;
        SystemDisplay.text = "X = (-x * t) - (y * -t) + (-x * t)+ z\nY = (y * -z * -t) + (x * t) + (t * x) - y\nZ = (z * t) + (x * -t) + (-z * t) - z + t";
        SystemConfig();
    }

    public void System22()
    {
        system = 22;
        SystemDisplay.text = "X = (z * x) - (y * x) - (t * -x) + t + z\nY = (t * -z) + (x * y) + y - z + t\nZ = (z * t) + (-z * t) - (y * x) - t + x";
        SystemConfig();
    }

    public void System23()
    {
        system = 23;
        SystemDisplay.text = "X = (-z * z) + (z * t) + (y * y) - (y * t) - y - z\nY = (y * -t) - (x * -x) - x + z + t\nZ = (x * z) - (t * -z) + (y * t) + x + y";
        SystemConfig();
    }

    public void System24()
    {
        system = 24;
        SystemDisplay.text = "X = (z * -x * t * z) + (y * -z * t) - z\nY = (y * t * z) - (t * x) - x + y + t\nZ = (z * z * -y * -x * t) + (-y * t * -z) - (z * -t)";
        SystemConfig();
    }




    //System 1
    private float eq_X1(float x, float y, float z, float t)
    {
        return (-x * x) + (z * t) + y;
    }

    private float eq_Y1(float x, float y, float z, float t)
    {
        return (x * x) - (y * z) - (t * t) - (y * x) + (y * t) - x + y;
    }

    private float eq_Z1(float x, float y, float z, float t)
    {
        return (z * -z) - (t * x) + (z * t) + x + y;
    }



    //System 2
    private float eq_X2(float x, float y, float z, float t)
    {
        return (x * y * -t * z) + (t * -z) - x;
    }

    private float eq_Y2(float x, float y, float z, float t)
    {
        return (z * y * -y) - (z * x) - t + y;
    }

    private float eq_Z2(float x, float y, float z, float t)
    {
        return (-x * t * t) * (t * x) - y * y;
    }



    //System 3
    private float eq_X3(float x, float y, float z, float t)
    {
        return (-x * x) + (z * t) + y;
    }

    private float eq_Y3(float x, float y, float z, float t)
    {
        return (x * t) + z - (x * x) - x;
    }

    private float eq_Z3(float x, float y, float z, float t)
    {
        return (z * -z) - (t * x) + (z * t) - x + y;
    }

    //System 4
    private float eq_X4(float x, float y, float z, float t)
    {
        return (-z * z) + (z * t) + (y * y);
    }

    private float eq_Y4(float x, float y, float z, float t)
    {
        return (y * -t) - (x * x) - x + z;
    }

    private float eq_Z4(float x, float y, float z, float t)
    {
        return (x * x) - (t * -z) + (z * -t) + x + y;
    }

    //System 5
    private float eq_X5(float x, float y, float z, float t)
    {
        return (x * x * -t * y) + (z * y * t) - (t * x) + y;
    }

    private float eq_Y5(float x, float y, float z, float t)
    {
        return (-y * t * z) - (t * x) + x;
    }

    private float eq_Z5(float x, float y, float z, float t)
    {
        return (z * z * -y * -x * t * t) - (t * x) + (t * y) + z + y;
    }

    //System 6
    private float eq_X6(float x, float y, float z, float t)
    {
        return (x * -x * -t * y) + (z * z * t) + y;
    }

    private float eq_Y6(float x, float y, float z, float t)
    {
        return (-y * t * -z) - (t * x) - x;
    }

    private float eq_Z6(float x, float y, float z, float t)
    {
        return (z * z * -y * -x) + (-y * t * -z) - (t * -t) - z + y;
    }

    //System 7
    private float eq_X7(float x, float y, float z, float t)
    {
        return (x * z * t) - (t * y) + (t * t) + x - y;
    }

    private float eq_Y7(float x, float y, float z, float t)
    {
        return (x * x * -t) - (float)Math.Sqrt(Math.Abs(t * y * x)) + (y * t) + z + x;
    }

    private float eq_Z7(float x, float y, float z, float t)
    {
        return (z * y * t) + (y * t * -z) - (x * -t) + y ;
    }

    //System 8
    private float eq_X8(float x, float y, float z, float t)
    {
        return (z * x * t) + (y * x * t) - (z * x * t) - (x * t) - x + y;
    }

    private float eq_Y8(float x, float y, float z, float t)
    {
        return (t * -z) + (x * t) + (y * t) + y - z + t;
    }

    private float eq_Z8(float x, float y, float z, float t)
    {
        return (z * t) + (z * x * t) + (y * x * t) + y + x;
    }

    //System 9
    private float eq_X9(float x, float y, float z, float t)
    {
        return (x * y) - (y * -t) + (-x * t) + t - z;
    }

    private float eq_Y9(float x, float y, float z, float t)
    {
        return (x * -z) + (x * x) + (t * -t) - y - z;
    }

    private float eq_Z9(float x, float y, float z, float t)
    {
        return (y * -x) + (x * -t) + (y * y) - t;
    }

    //System 10
    private float eq_X10(float x, float y, float z, float t)
    {
        return (y * y) - (y * -t) + (-x * t) + (z * t) + y - z;
    }

    private float eq_Y10(float x, float y, float z, float t)
    {
        return (y * z * t) + (y * x) + (t * -t) - y + z;
    }

    private float eq_Z10(float x, float y, float z, float t)
    {
        return (y * -x * t) + (x * -t) + (y * y * -t) + (z * z * t) - (-y * -z * t) - x;
    }

    //System 11
    private float eq_X11(float x, float y, float z, float t)
    {
        return (z * z) + (y * x) - (z * -x) + (-x * t) + t + z;
    }

    private float eq_Y11(float x, float y, float z, float t)
    {
        return (t * -z) + (x * y) + (y * t) + y - z + t;
    }

    private float eq_Z11(float x, float y, float z, float t)
    {
        return (z * t) + (-z * t) + (y * x) + t + x;
    }

    //System 12
    private float eq_X12(float x, float y, float z, float t)
    {
        return (z * z) + (y * x) - (z * -x) + (-x * t) + t + z;
    }

    private float eq_Y12(float x, float y, float z, float t)
    {
        return (t * -z) + (x * y) + (y * t) + y - z + t;
    }

    private float eq_Z12(float x, float y, float z, float t)
    {
        return (z * t) + (-z * t) + (y * x) - t + x;
    }

    //System 13
    private float eq_X13(float x, float y, float z, float t)
    {
        return (t * -t) + (z * t) - (y * -x) - y;
    }

    private float eq_Y13(float x, float y, float z, float t)
    {
        return (x * x) - (t * z) - (t * t) + (y * x) + (y * t) - x;
    }

    private float eq_Z13(float x, float y, float z, float t)
    {
        return (z * -z) - (t * x) + (z * t) + x + y;
    }

    //System 14
    private float eq_X14(float x, float y, float z, float t)
    {
        return (-x * t) - (y * -t) + (-x * t) - t + z;
    }

    private float eq_Y14(float x, float y, float z, float t)
    {
        return (x * -z * t) + (x * t) + (t * x) - y - z;
    }

    private float eq_Z14(float x, float y, float z, float t)
    {
        return (y * t) - (x * -t) + (z * t) - z;
    }

    //System 15
    private float eq_X15(float x, float y, float z, float t)
    {
        return (y * y * t) + (-y * x * t) - (z * y * t) - (x * t);
    }

    private float eq_Y15(float x, float y, float z, float t)
    {
        return (t * -z) + (x * -t) + (y * t) + y - z + t - y;
    }

    private float eq_Z15(float x, float y, float z, float t)
    {
        return (z * -t) + (z * z * t) + (y * x * t) - y - x + t;
    }

    //System 16
    private float eq_X16(float x, float y, float z, float t)
    {
        return (-x * x * y) + (z * t) + (x * t) + y;
    }

    private float eq_Y16(float x, float y, float z, float t)
    {
        return (x * t) - (x * -x) - x + y;
    }

    private float eq_Z16(float x, float y, float z, float t)
    {
        return (z * x) - (-t * x) + (z * t) + y;
    }

    //System 17
    private float eq_X17(float x, float y, float z, float t)
    {
        return (y * -t) + (-x * t) + (z * -t) - z;
    }

    private float eq_Y17(float x, float y, float z, float t)
    {
        return (y * z * t) + (y * x) - y + z;
    }

    private float eq_Z17(float x, float y, float z, float t)
    {
        return (y * -x * t) + (y * z * -t) + (z * -z * t) - (-y * -z * -t) - x;
    }

    //System 18
    private float eq_X18(float x, float y, float z, float t)
    {
        return (z * x * t) + (y * t) - (z * t) + (-x * t) + z + x;
    }

    private float eq_Y18(float x, float y, float z, float t)
    {
        return (t * -z) - (x * y) + (-y * t) + y - z;
    }

    private float eq_Z18(float x, float y, float z, float t)
    {
        return (z * t) + (-z * t) - (y * z * t) - t;
    }

    //System 19
    private float eq_X19(float x, float y, float z, float t)
    {
        return (z * z * -y) + (-y * t * -z) + (z * -t) + y;
    }

    private float eq_Y19(float x, float y, float z, float t)
    {
        return (t * x * -z) + (-y * t) + z;
    }

    private float eq_Z19(float x, float y, float z, float t)
    {
        return (z * t) + (-z * t) - (y * x) - t + x;
    }

    //System 20
    private float eq_X20(float x, float y, float z, float t)
    {
        return (t * -z) + (x * y) + y - z + t;
    }

    private float eq_Y20(float x, float y, float z, float t)
    {
        return (t * -z) + (y * -t) - (y * t) + (-x * t) - y;
    }

    private float eq_Z20(float x, float y, float z, float t)
    {
        return (-x * -t) - (z * x * t) + (z * y * t);
    }

    //System 21
    private float eq_X21(float x, float y, float z, float t)
    {
        return (-x * t) - (y * -t) + (-x * t)+ z;
    }

    private float eq_Y21(float x, float y, float z, float t)
    {
        return (y * -z * -t) + (x * t) + (t * x) - y;
    }

    private float eq_Z21(float x, float y, float z, float t)
    {
        return (z * t) + (x * -t) + (-z * t) - z + t;
    }

    //System 22
    private float eq_X22(float x, float y, float z, float t)
    {
        return (z * x) - (y * x) - (t * -x) + t + z;
    }

    private float eq_Y22(float x, float y, float z, float t)
    {
        return (t * -z) + (x * y) + y - z + t;
    }

    private float eq_Z22(float x, float y, float z, float t)
    {
        return (z * t) + (-z * t) - (y * x) - t + x;
    }

    //System 23
    private float eq_X23(float x, float y, float z, float t)
    {
        return (-z * z) + (z * t) + (y * y) - (y * t) - y - z;
    }

    private float eq_Y23(float x, float y, float z, float t)
    {
        return (y * -t) - (x * -x) - x + z + t;
    }

    private float eq_Z23(float x, float y, float z, float t)
    {
        return (x * z) - (t * -z) + (y * t) + x + y;
    }

    //System 24
    private float eq_X24(float x, float y, float z, float t)
    {
        return (z * -x * t * z) + (y * -z * t) - z;
    }

    private float eq_Y24(float x, float y, float z, float t)
    {
        return (y * t * z) - (t * x) - x + y + t;
    }

    private float eq_Z24(float x, float y, float z, float t)
    {
        return (z * z * -y * -x * t) + (-y * t * -z) - (z * -t);
    }


    void Start()
    {
        System1();
        On();
    }

    private void UpdateEquations()
    {
        //Initialize equations 
        x_AR[0] = t;
        y_AR[0] = t;
        z_AR[0] = t;

        for (int i = 1; i < trail_Amount; i++)
        {
            if (active[i])
            {
                if (system == 1)
                {
                    x_AR[i] = eq_X1(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y1(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z1(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 2)
                {
                    x_AR[i] = eq_X2(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y2(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z2(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 3)
                {
                    x_AR[i] = eq_X3(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y3(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z3(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 4)
                {
                    x_AR[i] = eq_X4(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y4(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z4(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 5)
                {
                    x_AR[i] = eq_X5(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y5(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z5(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 6)
                {
                    x_AR[i] = eq_X6(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y6(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z6(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 7)
                {
                    x_AR[i] = eq_X7(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y7(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z7(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 8)
                {
                    x_AR[i] = eq_X8(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y8(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z8(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 9)
                {
                    x_AR[i] = eq_X9(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y9(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z9(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 10)
                {
                    x_AR[i] = eq_X10(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y10(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z10(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 11)
                {
                    x_AR[i] = eq_X11(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y11(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z11(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 12)
                {
                    x_AR[i] = eq_X12(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y12(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z12(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 13)
                {
                    x_AR[i] = eq_X13(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y13(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z13(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 14)
                {
                    x_AR[i] = eq_X14(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y14(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z14(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 15)
                {
                    x_AR[i] = eq_X15(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y15(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z15(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 16)
                {
                    x_AR[i] = eq_X16(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y16(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z16(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 17)
                {
                    x_AR[i] = eq_X17(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y17(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z17(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 18)
                {
                    x_AR[i] = eq_X18(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y18(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z18(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 19)
                {
                    x_AR[i] = eq_X19(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y19(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z19(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 20)
                {
                    x_AR[i] = eq_X20(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y20(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z20(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 21)
                {
                    x_AR[i] = eq_X21(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y21(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z21(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 22)
                {
                    x_AR[i] = eq_X22(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y22(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z22(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 23)
                {
                    x_AR[i] = eq_X23(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y23(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z23(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 24)
                {
                    x_AR[i] = eq_X24(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = eq_Y24(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = eq_Z24(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }

                x_AR[i] = SaftyCheck(x_AR[i], i);
                y_AR[i] = SaftyCheck(y_AR[i], i);
                z_AR[i] = SaftyCheck(z_AR[i], i);
            }
        }
    }

    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++)
        {
            trails[i].transform.position = new Vector3(x_AR[i] * scale, y_AR[i] * scale, z_AR[i] * scale);
        }
    }

    private float SaftyCheck(float input, int i)
    {
        if (input > 1000000)
        {
            active[i] = false;
            return 1000000;
        }
        else if (input < -1000000)
        {
            active[i] = false;
            return -1000000;
        }
        else
        {
            return input;
        }
    }

    private void FixedUpdate()
    {
        TimeText.text = "Time: " + t.ToString("0.00000");
        if (on == true)
        {
            t += step * Time.deltaTime;

            UpdateEquations();
            UpdateLines();
        }
    }

}


