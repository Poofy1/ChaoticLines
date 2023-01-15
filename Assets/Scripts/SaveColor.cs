using Newtonsoft.Json;
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
    public Camera cam;

    public List<SaveScheme> saveList;


    private void Start()
    {
        //JsonConvert.DefaultSettings().Converters.Add(new ColorConverter());
        saveList = new List<SaveScheme>();
        currentColorSet = new List<ColorSet>();
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
        Debug.Log("Waiting for color: " + waitingColor);
    }

    public void ChangeForeground()
    {
        currentBackground = mainEvents.GetColor();
        cam.backgroundColor = currentBackground;
        foregroundImage.color = currentBackground;
    }


    //Colors
    int colorIndex = 0;
    public void NewColor()
    {
        if (waitingColor)
        {
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

        //Create json save 
        saveList.Add(new SaveScheme
        {
            colors = temp,
            foreground = new PlainColor(currentBackground)
        });

        //WriteSave and reset
        WriteSave();
        //ResetSelected();
    }



    //Write Custom Save
    public void WriteSave()
    {
        
        string json = JsonConvert.SerializeObject(saveList, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/ColorData.txt", json);

        //Re-load all data
        //LoadAll();
    }




    /*
    //Locates button refrence
    public int LocateButton(string name)
    {
        for (int i = 0; i < saveList.Count; i++)
        {
            if (saveList[i].SaveName.Equals(name)) return i;
        }
        return -1;
    }

    //Delete Color in set 
    public void DeleteColor(string name)
    {
        int i = LocateButton(name);
        if (i != -1)
        {
            //Destroy button
            Destroy(buttonList[i].button);

            //Delete preview
            string filePath = Application.dataPath + "/PreviewImages/" + saveList[i].date + ".png";
            if (File.Exists(filePath)) File.Delete(filePath);

            //Remove from list
            saveList.RemoveAt(i);
            buttonList.RemoveAt(i);

            WriteSave();
        }
    }
    */







    //Save Color Sets as List
    public class SaveScheme
    {
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