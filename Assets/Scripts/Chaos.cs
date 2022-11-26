
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
    public GameObject[] lines;
    public int lineSelection;
    public GameObject crosshair;
    public MapRotate mapRotator;
    public Transform mapBounds;
    public int bounds = 10000;
    public int trail_Amount = 1;
    public float t;
    public float step;
    public bool safety = true;
    private int color;
    private int boxBound = 0;
    private bool pauseTemp = false;


    //Main Render Pipe
    public List<FunctionInput> func;
    public TMP_InputField[] defaultInput;
    public SaveHandler saveHandler;
    private Expression[] exp;

    //Secondary Render Pipe
    public GameObject customInputButton;
    public GameObject varNaming;
    public TMP_InputField nameInput;
    public Transform varParent;

    //private vars
    private bool[] active;
    private Transform[] trails;
    public bool on = false;
    public bool absolute = false;
    private float lineThickness;
    private float trailLength;

    //Canvas vars
    public GameObject canvas;
    public TMP_InputField Amount;
    public TMP_Text AmountText;
    public TMP_Text warningText;
    public Slider StepSlider;
    public Text StepText;
    public TMP_Text TimeText;
    public Text SystemTitle;
    public Button Activate;
    public Button SaveButton;
    public GameObject stopButton;
    public Image pause;
    public Button createRand;
    public Button createVar;
    public Button[] ColBut;
    public Slider ThicknessSlider;
    public Text ThicknessText;
    public Slider LengthSlider;
    public Text LengthText;

    //Stats
    public Text percentActive;
    private float activeCount = 0;

    //Customizer
    public RectTransform colorTexture;
    public Texture2D colorRef;


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
                Amount.text = "";
                Activate.interactable = false;
            }
            else if (trail_Amount > 5000)
            {
                warningText.text = "It is recommended to not go above 5,000";
            }
            else
            {
                warningText.text = "";
                Activate.interactable = true;
            }

        }
        else
        {
            trail_Amount = 1;
            warningText.text = "Please enter a positve whole number";
            Amount.text = "";
            Activate.interactable = false;
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
        crosshair.GetComponent<Billboard>().ToggleCrosshair();


        if (on)
        {
            pauseTemp = false;
            Activate.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
            Amount.interactable = false;
            createVar.interactable = false;
            createRand.interactable = false;

            UpdateAmount();
            UpdateStep();

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
                var name = Instantiate(lines[lineSelection], new Vector3(0, 0, 0), Quaternion.identity);
                name.gameObject.name = "Trail" + i;
                trails[i] = name.transform;
            }

            //Update Prefrences
            if (lineSelection == 0)
            {
                UpdateLength();
                UpdateThickness();
                Color(color);
            }
            

        }
        else
        {
            percentActive.text = "Diverged: 0%";

            pause.color = new Color(.8f, .8f, .8f);
            pauseTemp = false;

            t = 0;

            Activate.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
            Amount.interactable = true;
            createRand.interactable = true;
            createVar.interactable = true;
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

    public void GetColor()
    {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorTexture, Input.mousePosition, null, out delta);

        float width = colorTexture.rect.width;
        float height = colorTexture.rect.height;

        delta += new Vector2(width / 2, height / 2);

        float x = Mathf.Clamp(delta.x / width, 0f, 1f);
        float y = Mathf.Clamp(delta.y / height, 0f, 1f);

        Color c = colorRef.GetPixel(Mathf.RoundToInt(x * colorRef.width), Mathf.RoundToInt(y * colorRef.height));

        pause.color = c;
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
            SaveButton.interactable = true;
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
            }
            catch {
                func[i].textInput.GetComponent<Image>().color = new Color(.5f, .098f, .098f);
                Activate.interactable = false;
                SaveButton.interactable = false;

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

    //Calculate
    int resetPoint = 0;
    public void AbsCalc(int a, int i)
    {

        for (int b = 0; b < func.Count; b++)
        {
            if (i < Mathf.Floor(trail_Amount / 2))
            {
                exp[a].Parameters[func[b].name] = func[b].mainCords[i - 1];
                exp[a].Parameters["t"] = t;
            }
            else if(i == Mathf.Floor(trail_Amount / 2))
            {
                resetPoint = i;
                exp[a].Parameters[func[b].name] = func[b].mainCords[0];
                exp[a].Parameters["t"] = -t;
            }
            else
            {
                exp[a].Parameters[func[b].name] = func[b].mainCords[i - 1];
                exp[a].Parameters["t"] = -t;
            }
        }

        

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
                    if (absolute)
                        AbsCalc(a, i);
                    else
                        CordCalc(a, i);
                }
                
                if (active[i] == false) activeCount++;
            }
        }
    }

    
    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++) if (active[i]) trails[i].transform.position = new Vector3(func[0].mainCords[i] * 20, func[1].mainCords[i] * 20, func[0].mainCords[2] * 20);
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

