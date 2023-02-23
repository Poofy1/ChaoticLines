using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SaveColor : MonoBehaviour
{
    public Chaos mainEvents;
    public Image foregroundImage;
    public ColorButton ColorObj;
    public Transform ColorObjParent;
    public List<ColorSet> currentColorSet;
    public ColorScheme SchemeObj;
    public Transform ColorSchemeParent;
    public Color currentBackground;
    public Image bgButton;
    public Camera cam;

    public List<SaveScheme> saveList;
    private List<GameObject> buttonList;

    private void Start()
    {
        //JsonConvert.DefaultSettings().Converters.Add(new ColorConverter());
        saveList = new List<SaveScheme>();
        currentColorSet = new List<ColorSet>();

        buttonList = new List<GameObject>();
        LoadAll();
    }



    //Locates color var button refrence
    public void DeleteColorVar(int name)
    {
        for (int i = 0; i < currentColorSet.Count; i++)
        {
            if (currentColorSet[i].identifier == name)
            {
                Destroy(currentColorSet[i].obj.gameObject);
                currentColorSet.RemoveAt(i);

                //Update colors real time
                mainEvents.UpdateColor();
                return;
            }
        }
    }


    //Reseting ForeGround??
    private bool waitingColor = false;
    public void RequestForeground()
    {
        waitingColor = !waitingColor;
        bgButton.color = new Color(.5f, .5f, .5f);
    }

    public void ChangeForeground()
    {
        cam.backgroundColor = currentBackground;
        foregroundImage.color = currentBackground;
        bgButton.color = new Color(1, 1, 1);
    }


    //Colors
    int colorIndex = 0;
    public void NewColor()
    {
        if (waitingColor)
        {
            currentBackground = mainEvents.GetColor();
            ChangeForeground();
            waitingColor = false;
            return;
        }


        //Spawn New
        var obj = Instantiate(ColorObj, new Vector3(0, 0, 0), Quaternion.identity, ColorObjParent);
        obj.gameObject.name = "Color";

        //Set Color
        Color c = mainEvents.GetColor();
        obj.SetColor(c);

        //Set Delete Button
        int copy = colorIndex;
        obj.delButton.onClick.AddListener(delegate { DeleteColorVar(copy); });

        //Add to list
        currentColorSet.Add(new ColorSet(colorIndex, obj, c));

        colorIndex++;

        //Update colors real time
        mainEvents.UpdateColor();
    }

    //If No Saves Exist
    public void NewRandomColor()
    {
        //Spawn New
        var obj = Instantiate(ColorObj, new Vector3(0, 0, 0), Quaternion.identity, ColorObjParent);
        obj.gameObject.name = "Color";

        //Set Color
        Color c = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
        obj.SetColor(c);

        //Set Delete Button
        int copy = colorIndex;
        obj.delButton.onClick.AddListener(delegate { DeleteColorVar(copy); });

        //Add to list
        currentColorSet.Add(new ColorSet(colorIndex, obj, c));

        colorIndex++;

        //Update colors real time
        mainEvents.UpdateColor();
    }







    //Schemes + Saving






    public void SaveColorScheme()
    {
        var obj = Instantiate(SchemeObj, new Vector3(0, 0, 0), Quaternion.identity, ColorSchemeParent);

        Color[] current = new Color[10];
        for (int i = 0; i < 10; i++)
        {
            if (currentColorSet.Count > i)
            {
                current[i] = currentColorSet[i].col;
            }
        }

        //Update Save pallette 
        obj.UpdateAll(current, currentBackground);


        //Create "empty" tempColor List
        List<PlainColor> temp = new List<PlainColor>();
        for (int i = 0; i < currentColorSet.Count; i++)
        {
            temp.Add(new PlainColor(currentColorSet[i].col));
        }

        //Find IdentifierSpot
        int listSpace = 0;
        while (true)
        {
            bool found = false;
            for (int i = 0; i < saveList.Count; i++)
            {
                if (saveList[i].identifier == listSpace)
                {
                    found = true;
                    break;
                }
            }
            if (!found) break;

            listSpace++;
        }


        //Create json save 
        saveList.Add(new SaveScheme
        {
            identifier = listSpace,
            colors = temp,
            foreground = new PlainColor(currentBackground)
        });

        //Add to button list
        buttonList.Add(obj.gameObject);
        buttonList[buttonList.Count - 1].GetComponentInChildren<ColorScheme>().delButton.onClick.AddListener(delegate { DeleteColor(listSpace); });
        buttonList[buttonList.Count - 1].GetComponentInChildren<ColorScheme>().loadButton.onClick.AddListener(delegate { loadScheme(listSpace); });

        //WriteSave and reset
        WriteSave();

        
    }



    //Write Custom Save
    public void WriteSave()
    {
        
        string json = JsonConvert.SerializeObject(saveList, Formatting.Indented);
        File.WriteAllText(Application.streamingAssetsPath + "/ColorData.txt", json);

        //Re-load all data
        //LoadAll();
    }



    //Load All Custom Data
    public void LoadAll()
    {
        if (File.Exists(Application.streamingAssetsPath + "/ColorData.txt"))
        {
            //Deserialize
            string saveString = File.ReadAllText(Application.streamingAssetsPath + "/ColorData.txt");
            saveList = JsonConvert.DeserializeObject<List<SaveScheme>>(saveString);

            //Delete Buttons
            for (int i = 0; i < buttonList.Count; i++)
            {
                Destroy(buttonList[i]);
            }
            buttonList = new List<GameObject>();

            //Spawn Buttons
            for (int i = 0; i < saveList.Count; i++)
            {
                //Spawn
                var name = Instantiate(SchemeObj, new Vector3(0, 0, 0), Quaternion.identity, ColorSchemeParent);
                name.gameObject.name = "Button" + i;

                //Add Details
                Color[] current = new Color[10];
                for (int a = 0; a < 10; a++)
                {
                    if (saveList[i].colors.Count > a)
                    {
                        current[a] = saveList[i].colors[a].ToUnityColor();
                    }
                }
                
                //Update Save pallette 
                name.UpdateAll(current, saveList[i].foreground.ToUnityColor());


                //Create List
                buttonList.Add(name.gameObject);



                //Set Listener
                int tempVar = saveList[i].identifier;
                buttonList[i].GetComponentInChildren<ColorScheme>().delButton.onClick.AddListener(delegate { DeleteColor(tempVar); });
                buttonList[i].GetComponentInChildren<ColorScheme>().loadButton.onClick.AddListener(delegate { loadScheme(tempVar); });
            }
        }
        else
        {
            Debug.Log("No Save!"); //UI This in the future
        }
    }


    //Locates button refrence
    public int LocateButton(int name)
    {
        for (int i = 0; i < saveList.Count; i++)
        {
            if (saveList[i].identifier == name) return i;
        }
        return -1;
    }

    //Delete Color in set 
    public void DeleteColor(int name)
    {
        int i = LocateButton(name);
        if (i != -1)
        {
            //Destroy button
            Destroy(buttonList[i]);


            //Remove from list
            saveList.RemoveAt(i);
            buttonList.RemoveAt(i);

            WriteSave();
        }
    }

    //Load Color Scheme
    public void loadScheme(int name)
    {
        colorIndex = 0;
        int i = LocateButton(name);

        //Clear current color set
        while (currentColorSet.Count > 0)
        {
            Destroy(currentColorSet[0].obj.gameObject);
            currentColorSet.RemoveAt(0);
        }

        if (i != -1)
        {
            //Foreground
            currentBackground = saveList[i].foreground.ToUnityColor();
            ChangeForeground();
            waitingColor = false;

            //Spawn Colors
            for (int a = 0; a < saveList[i].colors.Count; a++)
            {
                //Spawn New
                var obj = Instantiate(ColorObj, new Vector3(0, 0, 0), Quaternion.identity, ColorObjParent);
                obj.gameObject.name = "Color";

                Color c = saveList[i].colors[a].ToUnityColor();
                obj.SetColor(c);

                //Set Delete Button
                int copy = colorIndex;
                obj.delButton.onClick.AddListener(delegate { DeleteColorVar(copy); });

                //Add to list
                currentColorSet.Add(new ColorSet(colorIndex, obj, c));

                colorIndex++;
            }

            //Update colors real time
            mainEvents.UpdateColor();
        }
    }






    //Save Color Sets as List
    public class SaveScheme
    {
        public int identifier;
        public List<PlainColor> colors { get; set; }
        public PlainColor foreground { get; set; }
    }

    [System.Serializable]
    public class PlainColor
    {
        public float r;
        public float g;
        public float b;

        public Color ToUnityColor()
        {
            return new Color(r, g, b);
        }

        public PlainColor(Color color)
        {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
        }
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