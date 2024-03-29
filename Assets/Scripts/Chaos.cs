﻿
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using NCalc;
using System.Collections.Generic;

public class Chaos : MonoBehaviour
{
    public float topSpeedStep;

    //System vars
    public SaveColor saveColor;
    public PlayerMovement playerMovement;
    public MouseLook mouseLook;
    public GameObject crosshair;
    public TrailTemplate mainTrail;
    public MapRotate mapRotator;
    public Transform mapBounds;
    public int bounds = 10000;
    public int trail_Amount = 1;
    public decimal t;
    public decimal step;
    public bool safety = true;
    public GameObject[] cursor3D;
    private int color;
    private int boxBound = 0;
    private bool pauseTemp = false;
    public decimal targetSpeed = 0;
    public double exponent = 0;
    public float dampen = 0;
    private float average = 0;
    private decimal pastTopSpeed;

    //Main Render Pipe
    public List<FunctionInput> func;
    public TMP_InputField[] defaultInput;
    public SaveHandler saveHandler;
    private Func<double[], double>[] expression;
    double[] varinputs;
    private Vector3[] prevPos;

    //Secondary Render Pipe
    public GameObject customInputButton;
    public Transform varParent;

    //private vars
    private bool[] active;
    private TrailTemplate[] trails;
    public bool on = false;
    public bool absolute = false;
    private bool negitive = false;
    private float lineThickness;
    private float lineMulti;
    private float trailLength;

    //Canvas vars
    public GameObject canvas;
    public TMP_InputField Amount;
    public TMP_Text AmountText;
    public TMP_Text warningText;
    public TMP_Text warningText_Speed;
    public Slider TargetSlider;
    public Text TargetText;
    public Slider directionSlider;
    public TMP_Text directionText;
    public TMP_Text TimeText;
    public TMP_Text SystemTitle;
    public Button Activate;
    public Button SaveButton;
    public GameObject stopButton;
    public Image pause;
    public Button createRand;
    public Button createVar;
    public Slider ThicknessSlider;
    public Text ThicknessText;
    public Slider LengthSlider;
    public Text LengthText;
    public GameObject cursor;

    //Stats
    public TMP_Text percentActive;
    public TMP_Text avgSpeed;
    public TMP_Text stepText;
    private float activeCount = 0;

    //Customizer
    public RectTransform colorTexture;
    public Texture2D colorRef;

    public AudioLowPassFilter lowPass;
    private bool lowPassSwitch;

    //Optimized vars
    public int funcCount;


    public void Start()
    {
        func = new List<FunctionInput>();
    }

    public void InitializeMainCords()
    {
        func.Add(new FunctionInput(defaultInput[0], 'x', ""));
        func.Add(new FunctionInput(defaultInput[1], 'y', ""));
        func.Add(new FunctionInput(defaultInput[2], 'z', ""));
    }

    //Canvas Updates
    public void UpdateAmount()
    {
        if (!(Int32.TryParse(Amount.text, out trail_Amount)) || trail_Amount < 1)
        {
            trail_Amount = 1;
            warningText.text = "Please enter a positive whole number";
            Amount.text = "";
            Activate.interactable = false;

        }
        else if (trail_Amount > 1000)
        {
            warningText.text = "Performance Warning";
            Activate.interactable = true;
        }
        else
        {
            warningText.text = "";
            Activate.interactable = true;
        }

    }

    public void UpdateTarget()
    {
        targetSpeed = (decimal) ( 2 / (1 + Math.Pow( Math.E, -15 * (TargetSlider.value - 1))  ));
        TargetText.text = TargetSlider.value.ToString("0.00");
        if(TargetSlider.value > 0.8f)
        {
            warningText_Speed.text = "Increased velocity will reduce accuracy";
        }
        else
        {
            warningText_Speed.text = "";
        }
    }

    public void UpdateThickness()
    {
        lineMulti = ThicknessSlider.value;
        ThicknessText.text = lineMulti.ToString("0.00");
        lineMulti *= 2;
    }

    public void UpdateDirection()
    {
        if (directionSlider.value == 0)
        {
            directionText.text = "-";
            absolute = false;
            negitive = true;
        }
        else if (directionSlider.value == 1)
        {
            directionText.text = "-/+";
            absolute = true;
            negitive = false;
        }
        else if(directionSlider.value == 2)
        {
            directionText.text = "+";
            absolute = false;
            negitive = false;
        }

        //Resart if on
        if (on)
        {
            On();
            On();
        }
    }

    public void UpdateLength()
    {
        trailLength = LengthSlider.value;
        
        if (trailLength == 3)
        {
            trailLength = 9999999999999;
            LengthText.text = "inf";
        }
        else
        {
            LengthText.text = trailLength.ToString("0.00");
            trailLength *= 3;
        }

        if (on)
        {
            for (int i = 0; i < trail_Amount; i++) trails[i].SetLength(trailLength);
        }
    }

    //Starting
    public void On()
    {
        on = !on;

        if (on)
        {
            t = 0.00000001m;

            pauseTemp = false;
            Activate.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
            Amount.interactable = false;
            cursor.SetActive(false);
            lowPassSwitch = true;


            UpdateAmount();
            UpdateTarget();



            activeCount = 0;
            step = 1e-06m;
            pastTopSpeed = 0.000000001m;
            easeInFrames = 5;
            targetSpeed = targetSpeed / 100000;
            funcCount = func.Count;

            //Initialize inputs and vars
            varinputs = new double[funcCount + 1];
            expression = new Func<double[], double>[funcCount];
            boxBound = bounds * 1000;
            mapBounds.localScale = new Vector3(boxBound, boxBound, boxBound);
            mapRotator.maxZoomOut = boxBound / 20;


            for (int i = 0; i < funcCount; i++)
            {
                func[i].textInput.interactable = false;
                func[i].SetSize(trail_Amount);
                expression[i] = Evaluator.CreateMethod(func[i].function);
            }
            
            active = new bool[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                active[i] = true;
            }

            //Initialize trails
            trails = new TrailTemplate[trail_Amount];
            prevPos = new Vector3[trail_Amount];
            for (int i = 0; i < trail_Amount; i++)
            {
                //Create and Find
                var name = Instantiate(mainTrail, new Vector3(0, 0, 0), Quaternion.identity);
                name.gameObject.SetActive(true);
                name.gameObject.name = "Trail" + i;
                trails[i] = name;
            }

            trails[0].gameObject.SetActive(false);

            //Update Prefrences
            UpdateLength();
            UpdateThickness();
            UpdateColor();
        }
        else
        {
            percentActive.text = "Diverged: 0%";
            avgSpeed.text = "Average Speed: 0.000000000";
            stepText.text = "Step: 0.000000000";

            pause.color = new Color(.8f, .8f, .8f);
            pauseTemp = false;

            t = 0;

            lowPass.enabled = true;

            Activate.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
            Amount.interactable = true;
            cursor.SetActive(true);
            for (int i = 0; i < funcCount; i++) func[i].textInput.interactable = true;
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

    //Find selected color on color panel
    public Color GetColor()
    {
        Vector2 delta;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(colorTexture, Input.mousePosition, null, out delta);

        float width = colorTexture.rect.width;
        float height = colorTexture.rect.height;

        delta += new Vector2(width / 2, height / 2);

        float x = Mathf.Clamp(delta.x / width, 0f, 1f);
        float y = Mathf.Clamp(delta.y / height, 0f, 1f);

        return colorRef.GetPixel(Mathf.RoundToInt(x * colorRef.width), Mathf.RoundToInt(y * colorRef.height));
    }


    //Color Options
    public void UpdateColor()
    {
        if (on)
        {
            int colorNum = saveColor.currentColorSet.Count;
            if(colorNum == 0)
            {
                for (int i = 0; i < trails.Length; i++) trails[i].SetColor(new Color(1, 1, 1, 1));
            }
            else
            {
                int a = 0;
                for (int i = 0; i < trails.Length; i++)
                {
                    if (a >= colorNum) a = 0;
                    trails[i].SetColor(saveColor.currentColorSet[a].col);
                    a++;
                }
            }
        }
    }






    public void T_Transform(int id)
    {
        //Retreive original epxression
        string originalExp = "";
        char oldName = func[id].name;
        originalExp = func[id].textInput.text;
        func[id].textInput.text = "t*10";


        //Create new layer with info
        CreateCustomVar();
        func[func.Count - 1].textInput.text = originalExp;


        //Replace all id vars with new var
        char newName = func[func.Count - 1].name;
        Debug.Log(oldName);
        Debug.Log(newName);
        for (int i = 0; i < funcCount; i++)
        {
            func[i].textInput.text = func[i].textInput.text.Replace(oldName, newName);
        }


        SetCustomVars();
    }


    //Ready Custom Equations for use
    public void SetCustomVars()
    {
        funcCount = func.Count;
        if (TestCustom())
        {
            for (int i = 0; i < funcCount; i++)
            {
                func[i].function = func[i].textInput.text;
            }
                
            

            Activate.interactable = true;
            SaveButton.interactable = true;
        }
    }

    //Tester
    public bool TestCustom()
    {
        bool faultSearch = true;
        for (int i = 0; i < funcCount; i++)
        {

            try
            {
                Expression test = new Expression(func[i].textInput.text);
                for (int a = 0; a < funcCount; a++)
                    test.Parameters[func[a].name.ToString()] = 0;

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
    public void GenRand(int reset = 1)
    {
        if (on) On();
        for (int i = 0; i < 3; i++) func[i].textInput.text = RandomFunction.Create(5, func);
        for (int i = 3; i < funcCount; i++) func[i].textInput.text = RandomFunction.Create(2, func);
        if (reset == 1) On();
    }

    //Calculate
    public void CordCalc(int a, int i)
    {
        varinputs[0] = (double)t;
        for (int b = 0; b < funcCount; b++)
        {
            varinputs[b + 1] = func[b].mainCords[i - 1];
        }

        func[a].mainCords[i] = SaftyCheck((float)expression[a](varinputs), i);
    }

    //Calculate
    public void AbsCalc(int a, int i)
    {
        for (int b = 0; b < funcCount; b++)
        {
            if (i == Mathf.Floor(trail_Amount / 2))
                varinputs[b + 1] = func[b].mainCords[0];
            else
                varinputs[b + 1] = func[b].mainCords[i - 1];
        }

        if (i < Mathf.Floor(trail_Amount / 2))
        {
            varinputs[0] = (double)t;
            func[a].mainCords[i] = SaftyCheck((float)expression[a](varinputs), i);
        }
        else
        {
            varinputs[0] = -(double)t;
            func[a].mainCords[i] = SaftyCheck((float)expression[a](varinputs), i);
        }
            

        
    }


    public float maxSpeed = 0.1f;
    public double dist_multi = 500;


    public decimal topSpeed;
    public decimal preTop;
    private decimal originTopDis;
    public double originCurve;

    private int easeInFrames;

    public decimal testVar = 0.01m;

    private void UpdateEquations()
    {
        //Initialize equations 
        for (int a = 0; a < funcCount; a++)
        {
            func[a].mainCords[0] = (float) t;
        }

        topSpeed = 0.000000001m;
        originTopDis = 0;

        average = 0;
        for (int i = 1; i < trail_Amount; i++)
        {
            if (active[i])
            {

                for (int a = 0; a < funcCount; a++)
                {
                    //Calculate
                    if (absolute)
                        AbsCalc(a, i);
                    else
                        CordCalc(a, i);
                }
                //Find Distance from last pos
                Vector3 currentPos = new Vector3(func[0].mainCords[i], func[1].mainCords[i], func[2].mainCords[i]);

                decimal dist = (decimal)Vector3.Distance(prevPos[i], currentPos);
                float originDist = Vector3.Distance(new Vector3(0, 0, 0), currentPos);
                average += (float)dist;

                prevPos[i] = currentPos;

                if (active[i] == false) activeCount++;
                else
                {
                    if (topSpeed < dist)
                    {
                        topSpeed = dist;
                        originTopDis =  (decimal)originDist;
                    }
                    
                }

            }
        }
        

        //Speed up step slowly
        if (topSpeed < preTop) topSpeed = preTop / 2;
        preTop = topSpeed;

        

        //Prevent Overshooting initial stepper
        if (easeInFrames > 0)
        {
            targetSpeed = targetSpeed * 10;
            easeInFrames--;
        }

        //Debug.Log("2: TargetSp: " + targetSpeed + "   TopS:" + topSpeed + "  step:" + step + "  T:" + t + "  Origin: " + originTopDis + "  ActiveC:" + activeCount);
        step = (targetSpeed / ((topSpeed * Math.Max(topSpeed, 500)) + testVar)) * (1 + (originTopDis/ (decimal)originCurve));
        

        //Calc average distance traveled
        average /= trail_Amount;
    }
    
    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++)
        {
            if (active[i])
            {
                trails[i].TrailPos(new Vector3(func[0].mainCords[i] * 10, func[1].mainCords[i] * 10, func[2].mainCords[i] * 10));
            }
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
        else return input;
    }


    //Secondary List Stuff
    public void CreateCustomVar()
    {
        if (on) On();

        char newName = '0';
        List<char> letters = new List<char>();

        for (int i = 0; i < funcCount; i++)
        {
            letters.Add(func[i].name);
        }


        for (int i = 'a'; i <= 'w'; i++)
        {
            char letter = (char)i;
            if (!letters.Contains(letter))
            {
                newName = letter;
                break;
            }
        }

        if (!newName.Equals('0'))
        {
            //Spawn + Add to list
            GameObject cus = CreateVarInput(newName);
            func.Add(new FunctionInput(cus.gameObject.GetComponentInChildren<TMP_InputField>(), newName, ""));
            SetCustomVars();
        }
        
    }

    public GameObject CreateVarInput(char name)
    {
        GameObject obj = Instantiate(customInputButton, new Vector3(0, 0, 0), Quaternion.identity, varParent);
        obj.gameObject.name = name.ToString();
        obj.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener(delegate { SetCustomVars(); });
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate { DestoryVarInput(name); });
        obj.gameObject.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = char.ToUpper(name) + ":";
        return obj;
    }

    public void DestoryVarInput(char name)
    {
        for (int i = 0; i < funcCount; i++)
        {
            if (func[i].name.Equals(name))
            {
                if (on) On();
                Destroy(func[i].textInput.transform.parent.gameObject);
                func.RemoveAt(i);
                SetCustomVars();
                return;
            }
        }
    }

    private float timeSinceLastUpdate;
    private void FixedUpdate()
    {
        if (absolute) TimeText.text = "Time: -/+" + t.ToString("0.000000000");
        else TimeText.text = "Time: " + t.ToString("0.000000000");

        if (on)
        {
            //Update Thickness
            lineThickness = (playerMovement.dis * .002f) + 0.001f;
            for (int i = 0; i < trail_Amount; i++) trails[i].SetWidth(lineThickness * lineMulti);


            if (!pauseTemp)
            {
                if (negitive) t -= step;
                else t += step;

                UpdateEquations();
                UpdateLines();

                


                timeSinceLastUpdate += Time.fixedDeltaTime;

                if (timeSinceLastUpdate > 0.5f)
                {
                    timeSinceLastUpdate = 0;

                    // Update details
                    percentActive.text = "Diverged: " + 100 * (activeCount / trail_Amount) + "%";
                    avgSpeed.text = "Average Speed: " + (average).ToString("0.0000000");
                    stepText.text = "Step: " + step.ToString("0.000000000");
                }

                
            }
        }


        if (lowPassSwitch)
        {
            lowPass.cutoffFrequency += 100;
            if (lowPass.cutoffFrequency > 10000)
            {
                lowPassSwitch = false;
                lowPass.enabled = false;
                lowPass.cutoffFrequency = 1000;
            }
        }
    }



    private void Update()
    {
        if (mouseLook.active && mouseLook.locked)
        {
            if (Input.GetKeyDown(KeyCode.F)) On();
            if (Input.GetKeyDown(KeyCode.G)) Pause();

            if (Input.GetKeyDown(KeyCode.C))
            {
                TargetSlider.value -= 0.1f;
                UpdateTarget();
            }
            else if (Input.GetKeyDown(KeyCode.V))
            {
                TargetSlider.value += 0.1f;
                UpdateTarget();
            }
        }
    }
}

//Input List
public class FunctionInput
{
    [Newtonsoft.Json.JsonIgnore]
    public TMP_InputField textInput;
    public char name;
    public string function;
    public float[] mainCords;

    //Constructor
    public FunctionInput(TMP_InputField obj, char nameIn, string funcIn)
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

