
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections;
using NCalc;

public class Chaos : MonoBehaviour
{
    //System vars
    public GameObject trail;
    public GameObject crosshair;
    public MapRotate mapRotator;
    public Transform mapBounds;
    public int bounds = 10000;
    public int trail_Amount = 1;
    public float t;
    public float step;
    public float scale;
    public bool safety = true;
    public float speedLimit;
    private int color;


    //Custom
    public string[] customFunc;
    public GameObject[] customInput;
    public GameObject[] errors;
    public GameObject manageSaves;
    public GameObject randomButton;
    public SaveHandler saveHandler;
    private bool customOn;
    private bool savedOn;
    Expression e;

    //private vars
    private float[] x_AR;
    private float[] y_AR;
    private float[] z_AR;
    private bool[] active;
    private GameObject[] trails;
    private int system;
    public bool on = false;
    private float trailLength;
    private float lineThickness;
    private Vector3[] pastLoc;

    //Canvas vars
    public GameObject canvas;
    public GameObject preMadeSystems;
    public GameObject savedSystems;
    public TMP_InputField Amount;
    public TMP_Text AmountText;
    public TMP_Text warningText;
    public Slider ScaleSlider;
    public Text ScaleText;
    public Slider StepSlider;
    public Text StepText;
    public Text TimeText;
    public Text SystemTitle;
    public Text SystemDisplay;
    public Button Activate;
    public Text ActivateText;
    public Button createRand;
    public Button loadButton;
    public Button[] SysBut;
    public Button[] ColBut;

    public Slider ThicknessSlider;
    public Text ThicknessText;
    public Slider LengthSlider;
    public Text LengthText;

    private bool pauseTemp = false;

    //Stats
    public Text percentActive;
    private float activeCount = 0;




    private void Update()
    {
        int boxBound = bounds * 100;
        mapBounds.localScale = new Vector3(boxBound, boxBound, boxBound);
        mapRotator.maxZoomOut = boxBound / 2;
    }






    //Canvas Updates
    public void UpdateAmount()
    {
        if (Int32.TryParse(Amount.text, out trail_Amount))
        {
            if (trail_Amount < 1)
            {
                trail_Amount = 1;
                warningText.text = "Please enter a positve whole number";
                AmountText.text = "";
            }
            else if (trail_Amount > 5000)
            {
                warningText.text = "It is recommended to not go above 5,000";
            }
            else warningText.text = "";

        }
        else
        {
            trail_Amount = 1;
            warningText.text = "Please enter a positve whole number";
            AmountText.text = "";
        }

    }

    public void UpdateScale()
    {
        scale = ScaleSlider.value;
        ScaleText.text = (scale / 20).ToString("0.0");
    }
    public void UpdateStep(bool outsideOveride = false)
    {
        if (pauseTemp == false && outsideOveride == false) step = StepSlider.value;
        else StepSlider.value = step;
        StepText.text = (StepSlider.value * 50).ToString("0.00");
    }

    public void UpdateThickness()
    {
        lineThickness = ThicknessSlider.value;
        ThicknessText.text = lineThickness.ToString("0.0");
        if (on)
        {
            for (int i = 0; i < trail_Amount; i++)
            {
                trails[i].GetComponent<TrailRenderer>().startWidth = lineThickness / 5f;
                trails[i].GetComponent<TrailRenderer>().endWidth = lineThickness / 10f;
            }
        }
    }

    public void UpdateLength()
    {
        trailLength = LengthSlider.value;
        LengthText.text = trailLength.ToString("0.0");
        if (trailLength == 20)
        {
            trailLength = 9999999999999;
            LengthText.text = "∞";
        }

        if (on)
        {
            for (int i = 0; i < trail_Amount; i++) trails[i].GetComponent<TrailRenderer>().time = trailLength * 5;
        }
    }

    //Starting
    public void On()
    {
        on = !on;
        UpdateAmount();
        crosshair.GetComponent<Billboard>().ToggleCrosshair();


        if (on)
        {
            pauseTemp = false;
            ActivateText.text = "Clear";
            Activate.GetComponent<Image>().color = new Color(.4f, 0, 0);
            UpdateAmount();
            UpdateScale();
            UpdateStep();
            Amount.interactable = false;
            ScaleSlider.interactable = false;
            createRand.interactable = false;
            loadButton.interactable = false;
            activeCount = 0;
            for (int i = 0; i < SysBut.Length; i++) SysBut[i].interactable = false;

            for (int i = 0; i < 3; i++) customInput[i].GetComponent<TMP_InputField>().interactable = false;

            //Initialize x, y
            x_AR = new float[trail_Amount];
            y_AR = new float[trail_Amount];
            z_AR = new float[trail_Amount];
            pastLoc = new Vector3[trail_Amount];
            active = new bool[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                active[i] = true;
            }

            //Initialize trails
            trails = new GameObject[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                if (!float.IsNaN(x_AR[i]) && !float.IsNaN(x_AR[i]) && !float.IsNaN(z_AR[i]) && active[i])
                {
                    //Create and Find
                    var name = Instantiate(trail, new Vector3(0,0,0), Quaternion.identity);
                    name.gameObject.name = "Trail" + i;
                    trails[i] = name;//GameObject.Find("Trail" + i);
                }
            }

            UpdateLength();
            UpdateThickness();

            //COLORS
            Color(color);

        }
        else
        {
            ActivateText.text = "Start";
            Activate.GetComponent<Image>().color = new Color(.6f, .6f, .6f);
            if (!customOn)
            {
                for (int i = 0; i < SysBut.Length; i++) SysBut[i].interactable = true;
            }
            pauseTemp = false;
            t = 0;
            Amount.interactable = true;
            ScaleSlider.interactable = true;
            createRand.interactable = true;
            loadButton.interactable = true;
            for (int i = 0; i < 3; i++) customInput[i].GetComponent<TMP_InputField>().interactable = true;
            for (int i = 0; i < trail_Amount; i++) Destroy(trails[i]);
        }
    }

    //Pausing
    public void Pause()
    {
        if (pauseTemp == false)
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
        SystemTitle.text = "System " + system;
        for (int i = 0; i < SysBut.Length; i++)
            SysBut[i].GetComponent<Image>().color = new Color(.4f, .4f, .4f);
        SysBut[system - 1].GetComponent<Image>().color = new Color(.2f, .2f, .2f);
    }


    //Display Premade Systems
    public void System(int input)
    {
        system=input;
        if(input==1)SystemDisplay.text="X=(-x*x)+(z*t)+y\nY=(x*x)-(y*z)-(t*t)-(y*x)+(y*t)-x+y\nZ=(z*-z)-(t*x)+(z*t)+x+y";
        else if(input==2)SystemDisplay.text="X=(x*y*-t*z)+(t*-z)-x\nY=(z*y*-y)-(z*x)-t+y\nZ=(-x*t*t)*(t*x)-y*y";
        else if(input==3)SystemDisplay.text="X=(-x*x)+(z*t)+y\nY=(x*t)+z-(x*x)-x\nZ=(z*-z)-(t*x)+(z*t)-x+y";
        else if(input==4)SystemDisplay.text="X=(-z*z)+(z*t)+(y*y)\nY=(y*-t)-(x*x)-x+z\nZ=(x*x)-(t*-z)+(z*-t)+x+y";
        else if(input==5)SystemDisplay.text="X=(x*x*-t*y)+(z*y*t)-(t*x)+y\nY=(-y*t*z)-(t*x)+x\nZ=(z*z*-y*-x*t*t)-(t*x)+(t*y)+z+y";
        else if(input==6)SystemDisplay.text="X=(x*-x*-t*y)+(z*z*t)+y\nY=(-y*t*-z)-(t*x)-x\nZ=(z*z*-y*-x)+(-y*t*-z)-(t*-t)-z+y";
        else if(input==7)SystemDisplay.text="X=(x*z*t)-(t*y)+(t*t)+x-y\nY=(x*x*-t)-Sqrt(Abs(t*y*x))+(y*t)+z+x\nZ=(z*y*t)+(y*t*-z)-(x*-t)+y";
        else if(input==8)SystemDisplay.text="X=(z*x*t)+(y*x*t)-(z*x*t)-(x*t)-x+y\nY=(t*-z)+(x*t)+(y*t)+y-z+t\nZ=(z*t)+(z*x*t)+(y*x*t)+y+x";
        else if(input==9)SystemDisplay.text="X=(x*y)-(y*-t)+(-x*t)+t-z\nY=(x*-z)+(x*x)+(t*-t)-y-z\nZ=(y*-x)+(x*-t)+(y*y)-t";
        else if(input==10)SystemDisplay.text="X=(y*y)-(y*-t)+(-x*t)+(z*t)+y-z\nY=(y*z*t)+(y*x)+(t*-t)-y+z\nZ=(y*-x*t)+(x*-t)+(y*y*-t)+(z*z*t)-(-y*-z*t)-x";
        else if(input==11)SystemDisplay.text="X=(z*z)+(y*x)-(z*-x)+(-x*t)+t+z\nY=(t*-z)+(x*y)+(y*t)+y-z+t\nZ=(z*t)+(-z*t)+(y*x)+t+x";
        else if(input==12)SystemDisplay.text="X=(z*z)+(y*x)-(z*-x)+(-x*t)+t+z\nY=(t*-z)+(x*y)+(y*t)+y-z+t\nZ=(z*t)+(-z*t)+(y*x)-t+x";
        else if(input==13)SystemDisplay.text="X=(t*-t)+(z*t)-(y*-x)-y\nY=(x*x)-(t*z)-(t*t)+(y*x)+(y*t)-x\nZ=(z*-z)-(t*x)+(z*t)+x+y";
        else if(input==14)SystemDisplay.text="X=(-x*t)-(y*-t)+(-x*t)-t+z\nY=(x*-z*t)+(x*t)+(t*x)-y-z\nZ=(y*t)-(x*-t)+(z*t)-z";
        else if(input==15)SystemDisplay.text="X=(y*y*t)+(-y*x*t)-(z*y*t)-(x*t)\nY=(t*-z)+(x*-t)+(y*t)+y-z+t-y\nZ=(z*-t)+(z*z*t)+(y*x*t)-y-x+t";
        else if(input==16)SystemDisplay.text="X=(-x*x*y)+(z*t)+(x*t)+y\nY=(x*t)-(x*-x)-x+y\nZ=(z*x)-(-t*x)+(z*t)+y";
        else if(input==17)SystemDisplay.text="X=(y*-t)+(-x*t)+(z*-t)-z\nY=(y*z*t)+(y*x)-y+z\nZ=(y*-x*t)+(y*z*-t)+(z*-z*t)-(-y*-z*-t)-x";
        else if(input==18)SystemDisplay.text="X=(z*x*t)+(y*t)-(z*t)+(-x*t)+z+x\nY=(t*-z)-(x*y)+(-y*t)+y-z\nZ=(z*t)+(-z*t)-(y*z*t)-t";
        else if(input==19)SystemDisplay.text="X=(z*z*-y)+(-y*t*-z)+(z*-t)+y\nY=(t*x*-z)+(-y*t)+z\nZ=(z*t)+(-z*t)-(y*x)-t+x";
        else if(input==20)SystemDisplay.text="X=(t*-z)+(x*y)+y-z+t\nY=(t*-z)+(y*-t)-(y*t)+(-x*t)-y\nZ=(-x*-t)-(z*x*t)+(z*y*t)";
        else if(input==21)SystemDisplay.text="X=(-x*t)-(y*-t)+(-x*t)+z\nY=(y*-z*-t)+(x*t)+(t*x)-y\nZ=(z*t)+(x*-t)+(-z*t)-z+t";
        else if(input==22)SystemDisplay.text="X=(z*x)-(y*x)-(t*-x)+t+z\nY=(t*-z)+(x*y)+y-z+t\nZ=(z*t)+(-z*t)-(y*x)-t+x";
        else if(input==23)SystemDisplay.text="X=(-z*z)+(z*t)+(y*y)-(y*t)-y-z\nY=(y*-t)-(x*-x)-x+z+t\nZ=(x*z)-(t*-z)+(y*t)+x+y";
        else if(input==24)SystemDisplay.text="X=(z*-x*t*z)+(y*-z*t)-z\nY=(y*t*z)-(t*x)-x+y+t\nZ=(z*z*-y*-x*t)+(-y*t*-z)-(z*-t)";
        SystemConfig();
    }

    //Color Helpers
    private void SetColorRange(float r1, float r2, float g1, float g2, float b1, float b2)
    {
        if (on)
        {
            for (int i = 0; i < trail_Amount; i++)
            {
                float c1 = UnityEngine.Random.Range(r1, r2);
                float c2 = UnityEngine.Random.Range(g1, g2);
                float c3 = UnityEngine.Random.Range(b1, b2);
                trails[i].GetComponent<TrailRenderer>().startColor = new Color(c1, c2, c3, 1);
                trails[i].GetComponent<TrailRenderer>().endColor = new Color(c1, c2, c3, .5f);
            }
        }
    }

    private void SetColorChoice(float r1, float r2, float g1, float g2, float b1, float b2)
    {
        if (on)
        {
            for (int i = 0; i < trail_Amount; i++)
            {
                float rand = UnityEngine.Random.Range(0, 2);
                if (rand == 0)
                {
                    trails[i].GetComponent<TrailRenderer>().startColor = new Color(r1, g1, b1, 1);
                    trails[i].GetComponent<TrailRenderer>().endColor = new Color(r1, g1, b1, .5f);
                }
                else
                {
                    trails[i].GetComponent<TrailRenderer>().startColor = new Color(r2, g2, b2, 1);
                    trails[i].GetComponent<TrailRenderer>().endColor = new Color(r2, g2, b2, .5f);
                }

            }
        }
    }

    public void ColorConfig()
    {
        for (int i = 0; i < ColBut.Length; i++)
            ColBut[i].GetComponent<Image>().color = new Color(.4f, .4f, .4f);
        ColBut[color - 1].GetComponent<Image>().color = new Color(.2f, .2f, .2f);
    }

    //Color Options
    public void Color(int input)
    {
        color = input;
        ColorConfig();
        if (input == 1) SetColorRange(0f, 1f, 0f, 1f, 0f, 1f);
        else if (input == 2) SetColorChoice(0.25f, 1f, .65f, .37f, .95f, .86f);
        else if (input == 3) SetColorChoice(0.86f, .5f, .08f, .08f, .82f, .84f);
        else if (input == 4) SetColorRange(.5f, .9f, .1f, .4f, .1f, .5f);
        else if (input == 5) SetColorChoice(1f, .89f, 1f, .03f, 1f, .16f);
        else if (input == 6) SetColorRange(.89f, .9f, .1f, .68f, .1f, .11f);
        else if (input == 7) SetColorChoice(1f, .1f, 1f, .56f, 1f, .89f);
        else if (input == 8) SetColorRange(.5f, .1f, .1f, .4f, .3f, .5f);
        else if (input == 9) SetColorChoice(1f, 1f, 1f, 1f, 1f, 1f);
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
        return (z * y * t) + (y * t * -z) - (x * -t) + y;
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
        return (-x * t) - (y * -t) + (-x * t) + z;
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


    //CUSTOM HANDLER

    //Input Custom Equations
    public void SetCustomVars()
    {
        bool[] working = TestCustom();
        bool allTrue = true;

        for (int i = 0; i < 3; i++)
        {
            customFunc[i] = customInput[i].GetComponent<TMP_InputField>().text;


            if (working[i] == true)
            {
                customInput[i].GetComponent<Image>().color = new Color(.098f, .098f, .098f);
                errors[0].SetActive(false);
            }
            else
            {
                customInput[i].GetComponent<Image>().color = new Color(.5f, .098f, .098f);
                errors[0].SetActive(true);
                Activate.interactable = false;
                allTrue = false;
            }
        }

        if (allTrue)
        {
            Activate.interactable = true;
            SystemDisplay.text = "X="+customInput[0].GetComponent<TMP_InputField>().text+ "\nY=" + customInput[1].GetComponent<TMP_InputField>().text + "\nZ=" + customInput[2].GetComponent<TMP_InputField>().text;
        }
    }

    //Tester
    public bool[] TestCustom()
    {
        bool[] working = new bool[3];

        for (int i = 0; i < 3; i++)
        {
            try
            {
                Expression test = new Expression(customInput[i].GetComponent<TMP_InputField>().text);
                test.Parameters["x"] = 0;
                test.Parameters["y"] = 0;
                test.Parameters["z"] = 0;
                test.Parameters["t"] = 0;
                test.Evaluate();

                working[i] = true;
            }
            catch { }
        }
        return working;
    }

    public float CustomX(float x, float y, float z, float t)
    {
        Expression e = new Expression(customFunc[0]);
        e.Parameters["x"] = x;
        e.Parameters["y"] = y;
        e.Parameters["z"] = z;
        e.Parameters["t"] = t;
        return Convert.ToSingle(e.Evaluate());
    }

    public float CustomY(float x, float y, float z, float t)
    {

        Expression e = new Expression(customFunc[1]);
        e.Parameters["x"] = x;
        e.Parameters["y"] = y;
        e.Parameters["z"] = z;
        e.Parameters["t"] = t;

        return Convert.ToSingle(e.Evaluate());
    }

    public float CustomZ(float x, float y, float z, float t)
    {

        Expression e = new Expression(customFunc[2]);
        e.Parameters["x"] = x;
        e.Parameters["y"] = y;
        e.Parameters["z"] = z;
        e.Parameters["t"] = t;

        return Convert.ToSingle(e.Evaluate());
    }

    //Keep track of save window being open
    public void openedSaved()
    {
        savedOn = !savedOn;
        if (savedOn) saveHandler.ResetSelected();
        else errors[3].SetActive(false);
    }

    //Toggle Custom Inputs
    public void ToggleCustom()
    {
        customOn = !customOn;
        SetCustomVars();
        manageSaves.SetActive(!manageSaves.activeSelf);
        randomButton.SetActive(!randomButton.activeSelf);

        if (customOn)
        {
            SystemTitle.text = "Custom System";
            SystemDisplay.text = "X = \nY = \nZ =";



            saveHandler.ResetSelected();
            preMadeSystems.gameObject.SetActive(false);
        }
        else
        {
            SystemTitle.text = "System " + system;
            System(system);
            preMadeSystems.gameObject.SetActive(true);

            if (savedOn)
            {
                openedSaved();
                //savedSystems.GetComponent<selfAnimate>().Pressed();/////////////////////////
            }

        }

        if (on) On();

        for (int i = 0; i < 3; i++) customInput[i].SetActive(!customInput[i].activeSelf);

    }


    //Create Random System
    public void GenRand()
    {
        for (int i = 0; i < 3; i++)
        {
            customInput[i].GetComponent<TMP_InputField>().text = RandomFunction.Create(6);
        }
    }


    private float Distance(float x1, float y1, float z1, float x2, float y2, float z2)
    {
        return (float)Math.Abs(Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2) + Math.Pow(z2 - z1, 2)));
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
                if (customOn)
                {
                    x_AR[i] = CustomX(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    y_AR[i] = CustomY(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                    z_AR[i] = CustomZ(x_AR[i - 1], y_AR[i - 1], z_AR[i - 1], t);
                }
                else if (system == 1)
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

                if (safety && false) //keep off for now
                {

                    float distance = Distance(pastLoc[i][0], pastLoc[i][1], pastLoc[i][2], x_AR[i], y_AR[i], z_AR[i]);
                    if (distance > speedLimit)
                    {
                        trails[i].GetComponent<TrailRenderer>().emitting = false;
                        active[i] = false;
                    }
                    //else trails[i].GetComponent<TrailRenderer>().emitting = true;
                }



                pastLoc[i] = new Vector3(x_AR[i], y_AR[i], z_AR[i]);

                
                if (active[i] == false) activeCount++;

            }
        }
    }

    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++)
        {
            if(active[i]) trails[i].transform.position = new Vector3(x_AR[i] * scale, y_AR[i] * scale, z_AR[i] * scale);
        }
    }

    private float SaftyCheck(float input, int i)
    {
        if (input > bounds)
        {
            if (safety) active[i] = false;
            return bounds;
        }
        else if (input < -bounds)
        {
            if (safety) active[i] = false;
            return -bounds;
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
            t += step * (2 + step) * Time.deltaTime;



            UpdateEquations();
            UpdateLines();


            percentActive.text = "Diverged: " + 100*(activeCount/trail_Amount) + "%";
        }

        
    }

}