
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
    public float topSpeedStep;

    //System vars
    public SaveColor saveColor;
    public PlayerMovement playerMovement;
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

    //Main Render Pipe
    public List<FunctionInput> func;
    public TMP_InputField[] defaultInput;
    public SaveHandler saveHandler;
    private Expression[] exp;
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
    public Slider TargetSlider;
    public Text TargetText;
    public Slider directionSlider;
    public Text directionText;
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

    public void UpdateTarget()
    {
        targetSpeed = (decimal) ( 2 / (1 + Math.Pow( Math.E, -15 * (TargetSlider.value - 1))  ));
        TargetText.text = TargetSlider.value.ToString("0.00");
    }

    public void UpdateThickness()
    {
        lineMulti = ThicknessSlider.value;
        ThicknessText.text = lineMulti.ToString("0.0");
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
    }

    public void UpdateLength()
    {
        trailLength = LengthSlider.value;
        
        if (trailLength == 3)
        {
            trailLength = 9999999999999;
            LengthText.text = "∞";
        }
        else
        {
            LengthText.text = trailLength.ToString("0.0");
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
            pauseTemp = false;
            Activate.gameObject.SetActive(false);
            stopButton.gameObject.SetActive(true);
            Amount.interactable = false;
            createVar.interactable = false;
            createRand.interactable = false;
            cursor.SetActive(false);

            UpdateAmount();
            UpdateTarget();

            activeCount = 0;
            step = 1e-07m;

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

            //Update Prefrences
            UpdateLength();
            UpdateThickness();
            UpdateColor();


        }
        else
        {
            percentActive.text = "Diverged: 0%";
            avgSpeed.text = "Average Speed: 0";
            stepText.text = "Step: 0";

            pause.color = new Color(.8f, .8f, .8f);
            pauseTemp = false;

            t = 0;

            Activate.gameObject.SetActive(true);
            stopButton.gameObject.SetActive(false);
            Amount.interactable = true;
            createRand.interactable = true;
            createVar.interactable = true;
            cursor.SetActive(true);
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






















    //Ready Custom Equations for use
    public void SetCustomVars()
    {
        if (TestCustom())
        {
            for (int i = 0; i < func.Count; i++)
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
        for (int i = 0; i < func.Count; i++)
        {

            try
            {
                Expression test = new Expression(func[i].textInput.text);
                for (int a = 0; a < func.Count; a++)
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
    public void GenRand()
    {
        for (int i = 0; i < func.Count; i++) func[i].textInput.text = RandomFunction.Create(4, func);
    }

    //Calculate
    public void CordCalc(int a, int i)
    {

        for (int b = 0; b < func.Count; b++)
        {
            exp[a].Parameters[func[b].name.ToString()] = (double)func[b].mainCords[i - 1];
        }
            
        exp[a].Parameters["t"] = t;

        func[a].mainCords[i] = Convert.ToSingle(exp[a].Evaluate());

        try
        {
            
            func[a].mainCords[i] = SaftyCheck(func[a].mainCords[i], i);
        }
        catch
        {
            Debug.Log("input Error");
            func[a].mainCords[i] = 0;
        }
    }

    //Calculate
    public void AbsCalc(int a, int i)
    {

        for (int b = 0; b < func.Count; b++)
        {
            if (i == Mathf.Floor(trail_Amount / 2))
                exp[a].Parameters[func[b].name.ToString()] = (double)func[b].mainCords[0];
            else
                exp[a].Parameters[func[b].name.ToString()] = (double)func[b].mainCords[i - 1]; 
        }

        if (i < Mathf.Floor(trail_Amount / 2))
            exp[a].Parameters["t"] = t;
        else
            exp[a].Parameters["t"] = -t;

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


    public float maxSpeed = 0.1f;
    public double dist_multi = 500;


    public decimal topSpeed;
    public decimal preTop;
    private decimal originTopDis;
    public double originCurve;

    public decimal testVar = 0.0001m;

    private void UpdateEquations()
    {
        //Initialize equations 
        for (int a = 0; a < func.Count; a++)
        {
            func[a].mainCords[0] = (float) t;
        }

        topSpeed = 0.00000001m;
        originTopDis = 0;

        average = 0;
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
        Debug.Log("D:" + 100 * (activeCount / trail_Amount)  + "   TopS:" + topSpeed + "  step:" + step + "  T:" + t + "  ActiveC:" + activeCount);

        
        if (topSpeed < preTop) topSpeed = preTop / 2;
        preTop = topSpeed;

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
        char newName = '0';
        List<char> letters = new List<char>();

        for (int i = 0; i < func.Count; i++)
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
        }
        
    }

    public GameObject CreateVarInput(char name)
    {
        GameObject obj = Instantiate(customInputButton, new Vector3(0, 0, 0), Quaternion.identity, varParent);
        obj.gameObject.name = name.ToString();
        obj.GetComponentInChildren<TMP_InputField>().onValueChanged.AddListener(delegate { SetCustomVars(); });
        obj.GetComponentInChildren<Button>().onClick.AddListener(delegate { DestoryVarInput(name); });
        obj.gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = char.ToUpper(name) + ":";
        return obj;
    }

    public void DestoryVarInput(char name)
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
        TimeText.text = "Time: " + t.ToString("0.0000000");

        if (on)
        {
            //Update Thickness
            lineThickness = playerMovement.dis * .005f;
            for (int i = 0; i < trail_Amount; i++) trails[i].SetWidth(lineThickness * lineMulti);


            if (!pauseTemp)
            {
                if (negitive) step *= -1;
                t += step;

                UpdateEquations();
                UpdateLines();

                percentActive.text = "Diverged: " + 100 * (activeCount / trail_Amount) + "%";
                avgSpeed.text = "Average Speed: " + (average).ToString("0.0000000");
                stepText.text = "Step: " + step.ToString("0.0000000");
            }
        }
    }
}

//Input List
public class FunctionInput
{

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

