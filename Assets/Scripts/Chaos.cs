
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using NCalc;
using System.Collections.Generic;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

public class Chaos : MonoBehaviour
{
    //System vars
    public GameObject trail;
    public GameObject crosshair;
    public MapRotate mapRotator;
    public Transform mapBounds;
    public PlayerMovement player;
    public int bounds = 10000;
    public int trail_Amount = 1;
    public float t;
    public float step;
    public bool safety = true;
    public float speedLimit;
    private int color;
    private int boxBound = 0;


    //Main Render Pipe
    public List<FunctionInput> func;
    public TMP_InputField[] defaultInput;
    public GameObject[] errors;
    public GameObject randomButton;
    public SaveHandler saveHandler;
    private bool customOn;
    private Expression[] exp;

    //Secondary Render Pipe
    public GameObject customInputButton;
    public GameObject varNaming;
    public TMP_InputField nameInput;
    public Transform varParent;
    private GameObject[] varGameObjects;

    //private vars
    private bool[] active;
    private Transform[] trails;
    private int system;
    public bool on = false;
    private float trailLength;
    private float lineThickness;
    private Vector3[] pastLoc;

    //Canvas vars
    public GameObject canvas;
    public GameObject savedSystems;
    public TMP_InputField Amount;
    public TMP_Text AmountText;
    public TMP_Text warningText;
    public Slider StepSlider;
    public Text StepText;
    public Text TimeText;
    public Text SystemTitle;
    public Text SystemDisplay;
    public Button Activate;
    public GameObject stopButton;
    public RawImage pause;
    public Button createRand;
    public Button loadButton;
    public Button[] ColBut;

    public Slider ThicknessSlider;
    public Text ThicknessText;
    public Slider LengthSlider;
    public Text LengthText;

    private bool pauseTemp = false;

    //Stats
    public Text percentActive;
    private float activeCount = 0;

    public void Start()
    {
        func = new List<FunctionInput>();
        
    }

    public void InitializeMainCords()
    {
        func.Add(new FunctionInput(defaultInput[0], "x", ""));
        func.Add(new FunctionInput(defaultInput[1], "y", ""));
        func.Add(new FunctionInput(defaultInput[2], "z", ""));
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

    public void UpdateStep(bool outsideOveride = false)
    {
        if (pauseTemp == false && outsideOveride == false) step = StepSlider.value;
        else StepSlider.value = step;
        StepText.text = StepSlider.value.ToString("0.00");
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
            Activate.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
            UpdateAmount();
            UpdateStep();

            createRand.interactable = false;
            loadButton.interactable = false;
            activeCount = 0;

            //Initialize inputs and vars
            exp = new Expression[func.Count];
            boxBound = bounds * 2000;
            mapBounds.localScale = new Vector3(boxBound, boxBound, boxBound);
            mapRotator.maxZoomOut = boxBound / 20;


            for (int i = 0; i < func.Count; i++)
            {
                func[i].textInput.interactable = false;
                func[i].SetSize(trail_Amount);
                exp[i] = new Expression(func[i].function);
            }
            

            pastLoc = new Vector3[trail_Amount];
            active = new bool[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                active[i] = true;
            }

            //Initialize trails
            trails = new Transform[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                //Create and Find
                var name = Instantiate(trail, new Vector3(0, 0, 0), Quaternion.identity);
                name.gameObject.name = "Trail" + i;
                trails[i] = name.transform;
            }

            //Update Prefrences
            UpdateLength();
            UpdateThickness();
            Color(color);

        }
        else
        {
            percentActive.text = "Diverged: 0%";

            pause.color = new Color(.8f, .8f, .8f);
            pauseTemp = false;

            t = 0;

            Activate.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);

            createRand.interactable = true;
            loadButton.interactable = true;
            for (int i = 0; i < func.Count; i++) func[i].textInput.interactable = true;
            for (int i = 0; i < trail_Amount; i++) Destroy(trails[i].gameObject);
        }
    }

    //Pausing
    public void Pause()
    {
        if (!on) return;

        if (pauseTemp == false)
        {
            pause.color = new Color(.0862745f, .0862745f, .0862745f);
            pauseTemp = true;
        }
        else
        {
            pause.color = new Color(.8f, .8f, .8f);
            pauseTemp = false;
        }
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

    //Ready Custom Equations for use
    public void SetCustomVars()
    {
        if (TestCustom())
        {
            for (int i = 0; i < func.Count; i++)
                func[i].function = func[i].textInput.text;

            Activate.interactable = true;
            SystemDisplay.text = "X="+ func[0].textInput.text+ "\nY=" + func[1].textInput.text + "\nZ=" + func[2].textInput.text;
        }
    }

    //Tester
    public bool TestCustom()
    {
        bool faultSearch = true;
        for (int i = 0; i < func.Count; i++)
        {

            try
            {
                Expression test = new Expression(func[i].textInput.text);
                for (int a = 0; a < func.Count; a++)
                    test.Parameters[func[a].name] = 0;

                test.Parameters["t"] = 0;
                test.Evaluate();

                func[i].textInput.GetComponent<Image>().color = new Color(.098f, .098f, .098f);
                errors[0].SetActive(false);
            }
            catch {
                func[i].textInput.GetComponent<Image>().color = new Color(.5f, .098f, .098f);
                errors[0].SetActive(true);
                Activate.interactable = false;


                faultSearch = false;
            }
        }
        return faultSearch;
    }

    //Create Random System
    public void GenRand()
    {
        for (int i = 0; i < func.Count; i++) func[i].textInput.text = RandomFunction.Create(4, func);
    }

    //Calculate
    public void CordCalc(int a, int i)
    {

        for (int b = 0; b < func.Count; b++)
        {
            exp[a].Parameters[func[b].name] = func[b].mainCords[i - 1];
        }
            
        exp[a].Parameters["t"] = t;

        try
        {
            func[a].mainCords[i] = Convert.ToSingle(exp[a].Evaluate());
            func[a].mainCords[i] = SaftyCheck(func[a].mainCords[i], i);
        }
        catch
        {
            Debug.Log("input Error");
            func[a].mainCords[i] = 0;
        }
    }


    private void UpdateEquations()
    {
        //Initialize equations 
        for (int a = 0; a < func.Count; a++)
        {
            func[a].mainCords[0] = t;

            
        }
        for (int i = 1; i < trail_Amount; i++)
        {
            if (active[i])
            {

                for (int a = 0; a < func.Count; a++)
                {

                    //Calculate
                    CordCalc(a, i);
                }

                /*
                if (safety && false) //keep off for now
                {
                    //THERE IS VECTOR3.DISTANCE DUMBY
                    float distance = 0;//Distance(pastLoc[i][0], pastLoc[i][1], pastLoc[i][2], mainCords[0][i], mainCords[1][i], mainCords[2][i]);
                    if (distance > speedLimit)
                    {
                        trails[i].GetComponent<TrailRenderer>().emitting = false;
                        active[i] = false;
                    }
                    //else trails[i].GetComponent<TrailRenderer>().emitting = true;
                }

                

                //pastLoc[i] = new Vector3(func[0].mainCords[i], func[1].mainCords[i], func[2].mainCords[i]);
                */
                
                if (active[i] == false) activeCount++;
            }
        }
    }

    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++) if(active[i]) trails[i].position = new Vector3(func[0].mainCords[i] * 20, func[1].mainCords[i] * 20, func[0].mainCords[2] * 20);
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
        else return input;
    }


    //Secondary List Stuff
    private bool appliedRename;
    public void NewSecondary()
    {
        StartCoroutine(CreateVar());
    }

    IEnumerator CreateVar()
    {
        //Wait for name
        varNaming.SetActive(true);
        yield return new WaitUntil(() => appliedRename);

        //Spawn + Add to list
        GameObject cus = CreateVarInput(nameInput.text);
        func.Add(new FunctionInput(cus.gameObject.GetComponentInChildren<TMP_InputField>(), nameInput.text, ""));

        //Reset
        appliedRename = false;
        varNaming.SetActive(false);
        nameInput.text = "";
}

    //Apply Rename
    public void ApplyRename()
    {
        appliedRename = true;
    }

    public GameObject CreateVarInput(string name)
    {
        var obj = Instantiate(customInputButton, new Vector3(0, 0, 0), Quaternion.identity, varParent);
        obj.gameObject.name = name;
        obj.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener(delegate { SetCustomVars(); });
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate { DestoryVarInput(name); });
        obj.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = char.ToUpper(name[0]) + ":";
        return obj;
    }

    public void DestoryVarInput(string name)
    {
        for (int i = 0; i < func.Count; i++)
        {
            if (func[i].name.Equals(name))
            {
                Destroy(func[i].textInput.transform.parent.gameObject);
                func.RemoveAt(i);
                if (on) On();
                TestCustom();
                return;
            }
        }
        
    }

    private void FixedUpdate()
    {
        TimeText.text = "Time: " + t.ToString("0.000000");

        if (on && !pauseTemp)
        {
            t += step * step * step * 0.01f;

            UpdateEquations();
            UpdateLines();

            percentActive.text = "Diverged: " + 100*(activeCount/trail_Amount) + "%";

        }

        
    }

}

//Input List
public class FunctionInput
{

    public TMP_InputField textInput;
    public string name;
    public string function;
    public float[] mainCords;

    //Constructor
    public FunctionInput(TMP_InputField obj, string nameIn, string funcIn)
    {
        textInput = obj;
        name = nameIn;
        function = funcIn;
    }

    public void SetSize(int len)
    {
        mainCords = new float[len];
    }

}

