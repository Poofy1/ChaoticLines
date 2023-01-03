
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
    //public GameObject[] lines;
    public PlayerMovement playerMovement;
    public GameObject crosshair;
    public TrailTemplate mainTrail;
    public MapRotate mapRotator;
    public Transform mapBounds;
    public int bounds = 10000;
    public int trail_Amount = 1;
    public float t;
    public float step;
    public bool safety = true;
    public GameObject[] cursor3D;
    private int color;
    private int boxBound = 0;
    private bool pauseTemp = false;
    private Vector3[] prevPos;
    public double targetSpeed = 0;
    public double exponent = 0;
    public float dampen = 0;
    private float average = 0;

    //Main Render Pipe
    public List<FunctionInput> func;
    public TMP_InputField[] defaultInput;
    public SaveHandler saveHandler;
    private Expression[] exp;

    //Secondary Render Pipe
    public GameObject customInputButton;
    public Transform varParent;

    //private vars
    private bool[] active;
    private TrailTemplate[] trails;
    public bool on = false;
    public bool absolute = false;
    private float lineThickness;
    private float trailLength;

    //Canvas vars
    public GameObject canvas;
    public TMP_InputField Amount;
    public TMP_Text AmountText;
    public TMP_Text warningText;
    public Slider TargetSlider;
    public Text TargetText;
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
    public Image[] polarToggles;
    public Image[] cartToggles;

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
        for (int i = 0; i < 3; i++) SetCart(i);
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

    public void UpdateTarget(bool outsideOveride = false)
    {
        if (pauseTemp == false && outsideOveride == false) targetSpeed = TargetSlider.value;
        else TargetSlider.value = step * 1000;
        TargetText.text = TargetSlider.value.ToString("0.00");
    }

    public void UpdateThickness()
    {
        //lineThickness = ThicknessSlider.value;
        //ThicknessText.text = lineThickness.ToString("0.0");
        //lineThickness /= 5;
        if (on)
        {
            for (int i = 0; i < trail_Amount; i++) trails[i].SetWidth(lineThickness);
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

        trailLength *= 5;
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

            UpdateAmount();
            UpdateTarget();

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
            avgSpeed.text = "Average SPeed: 0";
            stepText.text = "Step: 0";

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
            int colorNum = saveHandler.currentColorSet.Count;
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
                    trails[i].SetColor(saveHandler.currentColorSet[a].col);
                    a++;
                }
            }
        }
    }


    //True = polar //False == cart
    private bool[] cordSystem = new bool[3];
    public void SetPolar(int index)
    {
        if (on) On();
        polarToggles[index].color = new Color(0, 0, 0, 1);
        cartToggles[index].color = new Color(.8f, .8f, .8f, 1);
        cordSystem[index] = true;

        //Change Cursor
        cursor3D[index].SetActive(false);
        cursor3D[index + 3].SetActive(true);
    }

    public void SetCart(int index)
    {
        if (on) On();
        cartToggles[index].color = new Color(0, 0, 0, 1);
        polarToggles[index].color = new Color(.8f, .8f, .8f, 1);
        cordSystem[index] = false;

        //Change Cursor
        cursor3D[index].SetActive(true);
        cursor3D[index + 3].SetActive(false);
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
            exp[a].Parameters[func[b].name.ToString()] = func[b].mainCords[i - 1];
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
                exp[a].Parameters[func[b].name.ToString()] = func[b].mainCords[i - 1];
                exp[a].Parameters["t"] = t;
            }
            else if(i == Mathf.Floor(trail_Amount / 2))
            {
                resetPoint = i;
                exp[a].Parameters[func[b].name.ToString()] = func[b].mainCords[0];
                exp[a].Parameters["t"] = -t;
            }
            else
            {
                exp[a].Parameters[func[b].name.ToString()] = func[b].mainCords[i - 1];
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

    //Calculate
    public void PolCalc(int a, int i)
    {

        for (int b = 0; b < func.Count; b++)
        {
            exp[a].Parameters[func[b].name.ToString()] = func[b].mainCords[i - 1];
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
        prevPos[0] = new Vector3(0, 0, 0);

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


                    //Find Distance from last pos -> average
                    Vector3 currentPos = new Vector3(func[0].mainCords[i], func[1].mainCords[i], func[2].mainCords[i]);
                    average += Vector3.Distance(prevPos[i], currentPos);
                    prevPos[i] = currentPos;
                }
                
                if (active[i] == false) activeCount++;
            }
        }

        //Calc average distance traveled
        average /= trail_Amount;

        step = dampen * (float)Math.Pow(targetSpeed / average, exponent);


        if (step > 1.5f) step = 1.5f;

    }
    
    private void UpdateLines()
    {
        for (int i = 0; i < trail_Amount; i++)
        {
            if (active[i])
            {
                trails[i].TrailPos(new Vector3(func[0].mainCords[i] * 20, func[1].mainCords[i] * 20, func[2].mainCords[2] * 20), cordSystem);
                trails[i].PolarRotate(new Vector3(func[0].mainCords[i], func[1].mainCords[i], func[1].mainCords[i]), cordSystem);
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
        TimeText.text = "Time: " + t.ToString("0.000000");

        if (on)
        {
            //Update Thickness
            lineThickness = playerMovement.dis * .01f;
            UpdateThickness();

            if (!pauseTemp)
            {
                t += step * step * step * 0.01f;

                UpdateEquations();
                UpdateLines();

                percentActive.text = "Diverged: " + 100 * (activeCount / trail_Amount) + "%";
                avgSpeed.text = "Average Speed: " + (average).ToString("0.000000");
                stepText.text = "Step: " + step.ToString("0.0000");
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

public class ColorSet
{
    public int identifier;
    public ColorButton obj;
    public Color col;

    public ColorSet(int id, ColorButton objIn, Color colIn)
    {
        identifier = id;
        obj = objIn;
        col = colIn;
    }
}