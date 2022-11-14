using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveHandler : MonoBehaviour
{
    public Chaos mainEvents;
    public Settings settings;
    public List<SaveList> saveList;
    public List<SaveButton> buttonList;
    public SettingItem savedSet;
    public MouseLook mouse;
    public GameObject button;
    public GameObject buttonParent;
    public GameObject[] functionButtons;
    public GameObject renamePanel;
    public TMP_InputField renameInput;
    public GameObject hud;
    public GameObject photoHud;
    public RawImage preview;


    private GameObject[] spawnedButtons;
    public int currentSelected;
    private bool appliedRename;
    private string newName;
    private string newDate;

    private void Start()
    {
        mainEvents = GameObject.Find("Chaos_Event").GetComponent<Chaos>();
        saveList = new List<SaveList>();
        buttonList = new List<SaveButton>();
        savedSet = new SettingItem();
        spawnedButtons = new GameObject[0];
        LoadAll();



        //LoadAll Settings
        if (File.Exists(Application.dataPath + "/UserSettings.txt"))
        {
            //Deserialize
            string saveString = File.ReadAllText(Application.dataPath + "/UserSettings.txt");
            savedSet = JsonConvert.DeserializeObject<SettingItem>(saveString);
            LoadSettings();
        }
        else
        {
            Screen.fullScreen = true;
        }
    }

    //SETTINGS:

    //Save Settings
    public void SaveSettings()
    {
        savedSet.fullscreen = settings.fullscreen.isOn;
        savedSet.AA = settings.AAMenu.GetComponent<TMP_Dropdown>().value;
        savedSet.vSync = !settings.vSyncOn;
        savedSet.mouseSen = mouse.setting.value;
        savedSet.sound = settings.musicSlider.value;
        savedSet.fov = settings.fovSlider.value;
        savedSet.hudScale = settings.hudSlider.value;
        savedSet.safety = mainEvents.safety;

        //Write
        string json = JsonConvert.SerializeObject(savedSet, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/UserSettings.txt", json);
    }

    //LoadSettings
    public void LoadSettings()
    {
        settings.fullscreen.isOn = savedSet.fullscreen;
        settings.AAMenu.GetComponent<TMP_Dropdown>().value = savedSet.AA;
        settings.vSyncOn = savedSet.vSync;
        mouse.setting.value = savedSet.mouseSen;
        settings.musicSlider.value = savedSet.sound;
        settings.fovSlider.value = savedSet.fov;
        settings.hudSlider.value = savedSet.hudScale;
        mainEvents.safety = !savedSet.safety;

        mouse.SensitivityChanged(0);
        settings.UpdateAll();
    }



    //SYSTEMS:

    //Save Custom
    public void SaveCurrent()
    {
        StartCoroutine(SaveCurrentCall());
    }

    IEnumerator SaveCurrentCall()
    {
        if (mainEvents.TestCustom())
        {


            //Wait For Screenshot
            photoHud.SetActive(true);
            yield return StartCoroutine(ScreenShot());

            //Wait for rename
            yield return StartCoroutine(RenameSystemCall());

            photoHud.SetActive(false);




            //Create json save 
            List<FunctionInput> temp = new List<FunctionInput>();
            for (int i = 0; i < mainEvents.func.Count; i++)
            {
                temp.Add(new FunctionInput(null, mainEvents.func[i].name, mainEvents.func[i].function));
            }
            
            saveList.Add(new SaveList
            {
                CustomFunctions = temp,
                date = newDate,
                SaveName = newName
            });

            //WriteSave and reset
            WriteSave();
            ResetSelected();
            
        }
        else
        {
            mainEvents.errors[3].SetActive(true);
        }
        
    }


    //ScreenShot
    IEnumerator ScreenShot()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            yield return null;
        }

        yield return null;
        hud.SetActive(false);
        yield return new WaitForEndOfFrame();

        int res = 0;
        if (Screen.height < Screen.width) res = Screen.height;
        else res = Screen.width;

        newDate = System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-ff");

        Texture2D currentCapture = new Texture2D(res, res, TextureFormat.RGB24, false);
        currentCapture.ReadPixels(new Rect(res / 2, 0, res, res), 0, 0, false);

        currentCapture.Apply();
        hud.SetActive(true);

        byte[] bytes = currentCapture.EncodeToPNG();
        System.IO.File.WriteAllBytes(Application.dataPath + "/PreviewImages/" + newDate + ".png", bytes);
    }



    //Write Custom Save
    public void WriteSave()
    {
        string json = JsonConvert.SerializeObject(saveList, Formatting.Indented);
        File.WriteAllText(Application.dataPath + "/UserData.txt", json);

        //Re-load all data
        LoadAll();
    }

    //Load All Custom Data
    public void LoadAll()
    {
        if (File.Exists(Application.dataPath + "/UserData.txt"))
        {
            //Deserialize
            string saveString = File.ReadAllText(Application.dataPath + "/UserData.txt");
            saveList = JsonConvert.DeserializeObject<List<SaveList>>(saveString);

            //Delete Buttons
            for (int i = 0; i < spawnedButtons.Length; i++)
            {
                Destroy(spawnedButtons[i]);
            }

            //Spawn Buttons
            buttonList = new List<SaveButton>();
            spawnedButtons = new GameObject[saveList.Count];

            for (int i = 0; i < saveList.Count; i++)
            {
                //Spawn
                var name = Instantiate(button, new Vector3(0, 0, 0), Quaternion.identity, buttonParent.transform);
                name.gameObject.name = "Button" + i;
                spawnedButtons[i] = name.gameObject;

                //Create List
                buttonList.Add(new SaveButton(saveList[i].SaveName, saveList[i].CustomFunctions, spawnedButtons[i]));

                //Add details
                spawnedButtons[i].transform.GetChild(1).GetComponent<Text>().text = "Layers: " + saveList[i].CustomFunctions.Count + "\nDate Created: " + saveList[i].date.Substring(0, 10);

                //Set Listener
                setButtonListener(i);
            }
        }
        else
        {
            Debug.Log("No Save!"); //UI This in the future
        }
    }

    //Set Custom Button Listeners
    private void setButtonListener(int i)
    {
        buttonList[i].button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicked(i); });
    }


    //When Save is clicked
    public void ButtonClicked(int a)
    {
        
        for (int i = 1; i < functionButtons.Length; i++) functionButtons[i].GetComponent<Button>().interactable = true;

        currentSelected = a;
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.235294f, .235294f, .235294f, 1);

        buttonList[a].button.GetComponent<Image>().color = new Color(0, 0, 0, .6749f);

        //show preview
        Texture2D tex = null;
        string filePath = Application.dataPath + "/PreviewImages/" + saveList[a].date + ".png";

        if (File.Exists(filePath))
        {
            tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(filePath));
            preview.texture = tex;
        }
    }

    //Reset currently Selected
    public void ResetSelected()
    {
        for (int i = 1; i < functionButtons.Length; i++) functionButtons[i].GetComponent<Button>().interactable = false;
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.235294f, .235294f, .235294f, 1);
    }


    //Load System
    public void LoadSystem()
    {
        mainEvents.SystemTitle.text = saveList[currentSelected].SaveName;

        while(mainEvents.func.Count > 3)
        {
            Destroy(mainEvents.func[mainEvents.func.Count - 1].textInput.transform.parent.gameObject);
            mainEvents.func.RemoveAt(mainEvents.func.Count - 1);
        }

        for (int i = 0; i < saveList[currentSelected].CustomFunctions.Count; i++)
        {

            string funcText = saveList[currentSelected].CustomFunctions[i].function;
            string funcName = saveList[currentSelected].CustomFunctions[i].name;

            if (i > 2)
            {
                GameObject cus = mainEvents.CreateVarInput(funcName);

                mainEvents.func.Add(new FunctionInput(cus.gameObject.GetComponentInChildren<TMP_InputField>(), funcName, funcText));
                mainEvents.func[i].textInput.text = funcText;
            }
            else
            {
                mainEvents.func[i].function = funcText;
                mainEvents.func[i].textInput.text = funcText;
                
            }
                

            

        }
            
    }

    //Rename System
    public void RenameSystem()
    {
        StartCoroutine(RenameSystem1());
    }
    IEnumerator RenameSystem1()
    {
        yield return StartCoroutine(RenameSystemCall());
        saveList[currentSelected].SaveName = newName;
        ResetSelected();
        WriteSave();
    }

    //Set Name
    IEnumerator RenameSystemCall()
    {
        renamePanel.SetActive(true);
        yield return new WaitUntil(() => appliedRename);
        newName = renameInput.text;
        appliedRename = false;
        renamePanel.SetActive(false);
        renameInput.text = "";
    }

    //Apply Name
    public void ApplyRename()
    {
        appliedRename = true;
    }

    //Delete System
    public void DeleteSystem()
    {
        //Delete preview
        string filePath = Application.dataPath + "/PreviewImages/" + saveList[currentSelected].date + ".png";
        if (File.Exists(filePath)) File.Delete(filePath);

        //Remove from list
        saveList.RemoveAt(currentSelected);
        ResetSelected();
        WriteSave();
    }


    



    //SaveSystem
    public class SaveList
    {
        public List<FunctionInput> CustomFunctions { get; set; }
        public string date { get; set; }
        public string SaveName { get; set; }
    }

    //SaveSettings
    public class SettingItem
    {
        public bool fullscreen { get; set; }
        public int AA { get; set; }
        public bool vSync { get; set; }
        public float mouseSen { get; set; }
        public float sound { get; set; }
        public float fov { get; set; }
        public float hudScale { get; set; }
        public bool safety { get; set; }
    }

    //Save Item List
    public class SaveButton
    {
        public List<FunctionInput> customInputs;
        public string name;
        public GameObject button;

        //Constructor
        public SaveButton(string nameInput, List<FunctionInput> custIn, GameObject obj)
        {
            customInputs = custIn;
            name = nameInput;
            button = obj;

            button.transform.GetChild(0).GetComponent<Text>().text = name;
        }
    }

}