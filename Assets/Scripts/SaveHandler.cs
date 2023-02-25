using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SaveHandler : MonoBehaviour
{
    public Chaos mainEvents;
    public AnimationHandler animHandler;
    public Settings settings;
    public List<SaveList> saveList;
    public List<SaveButton> buttonList;
    public SettingItem savedSet;
    public MouseLook mouse;
    public SelfAni hotkeyAnim;
    public GameObject button;
    public GameObject buttonParent;
    public GameObject renamePanel;
    public TMP_InputField renameInput;
    public GameObject hud;
    public GameObject photoHud;
    public RawImage preview;
    public SelfAni imagePreview;
    public TMP_Text details;
    public TMP_Text nameWarning;
    public KeyCode enterKey = KeyCode.Return;


    private GameObject[] spawnedButtons;
    public int currentSelected;
    private bool appliedRename;
    private string newName;
    private string newDate;
    private bool awaitingEnter;

    private void Start()
    {
        mainEvents = GameObject.Find("Chaos_Event").GetComponent<Chaos>();
        saveList = new List<SaveList>();
        buttonList = new List<SaveButton>();
        savedSet = new SettingItem();

        spawnedButtons = new GameObject[0];
        LoadAll();



        //LoadAll Settings
        if (File.Exists(Application.streamingAssetsPath + "/UserSettings.txt"))
        {
            //Deserialize
            string saveString = File.ReadAllText(Application.streamingAssetsPath + "/UserSettings.txt");
            savedSet = JsonConvert.DeserializeObject<SettingItem>(saveString);
            LoadSettings();
        }
        else
        {
            Screen.fullScreen = true;
            settings.hudScaleApply();
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

        savedSet.hotkeys = hotkeyAnim.open;

        //Write
        string json = JsonConvert.SerializeObject(savedSet, Formatting.Indented);
        File.WriteAllText(Application.streamingAssetsPath + "/UserSettings.txt", json);



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

        mouse.SensitivityChanged(0);
        settings.UpdateAll();

        if (savedSet.hotkeys && !hotkeyAnim.open)
        {
            hotkeyAnim.VerticalClicked();
        }
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
            cancelSave = false;

            //Wait For Screenshot
            photoHud.SetActive(true);
            animHandler.HideHud();
            yield return StartCoroutine(ScreenShot());
            photoHud.SetActive(false);

            if (cancelSave) yield break;

            //Wait for rename
            yield return StartCoroutine(RenameSystemCall());

            animHandler.ShowHud();
            




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

        
    }


    //ScreenShot
    private bool cancelSave;
    IEnumerator ScreenShot()
    {
        while (!Input.GetKeyDown(KeyCode.Space))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                cancelSave = true;
                animHandler.ShowHud();
                break;
            }
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

        String path = Application.streamingAssetsPath + "/PreviewImages/";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        System.IO.File.WriteAllBytes(path + newDate + ".png", bytes);
    }



    //Write Custom Save
    public void WriteSave()
    {
        string json = JsonConvert.SerializeObject(saveList, Formatting.Indented);
        File.WriteAllText(Application.streamingAssetsPath + "/UserData.txt", json);

        //Re-load all data
        LoadAll();
    }

    //Load All Custom Data
    public void LoadAll()
    {
        if (File.Exists(Application.streamingAssetsPath + "/UserData.txt"))
        {
            //Deserialize
            string saveString = File.ReadAllText(Application.streamingAssetsPath + "/UserData.txt");
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
        buttonList[i].button.transform.GetChild(1).GetComponentInChildren<Button>().onClick.AddListener(delegate { DeleteSystem(saveList[i].SaveName); });
        buttonList[i].button.transform.GetChild(2).GetComponentInChildren<Button>().onClick.AddListener(delegate { RenameSystem(saveList[i].SaveName); });
        buttonList[i].button.transform.GetChild(3).GetComponentInChildren<Button>().onClick.AddListener(delegate { LoadSystem(saveList[i].SaveName); });
    }


    //When Save is clicked
    public void ButtonClicked(int a)
    {

        currentSelected = a;
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.235294f, .235294f, .235294f, 1);

        buttonList[a].button.GetComponent<Image>().color = new Color(0, 0, 0, .6749f);

        //show preview
        Texture2D tex = null;
        string filePath = Application.streamingAssetsPath + "/PreviewImages/" + saveList[a].date + ".png";

        if (File.Exists(filePath))
        {
            tex = new Texture2D(2, 2);
            tex.LoadImage(File.ReadAllBytes(filePath));
            preview.texture = tex;
        }

        //Show details
        details.text = "Layers: " + saveList[a].CustomFunctions.Count + "\n" + saveList[a].date.Substring(0, 10);
    }

    //Reset currently Selected
    public void ResetSelected()
    {
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.235294f, .235294f, .235294f, 1);
    }

    //Reset currently Selected Load
    public void ResetLoadColor()
    {
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.transform.GetChild(3).GetChild(0).GetComponent<Image>().color = new Color(.8f, .8f, .8f, 1);
    }

    //Load System
    public void LoadSystem(string name)
    {
        if (mainEvents.on == true) mainEvents.On();

        int buttonIndex = LocateButton(name);
        mainEvents.SystemTitle.text = saveList[buttonIndex].SaveName;

        //reset selected
        ButtonClicked(buttonIndex);

        //Reset Load Color
        ResetLoadColor();
        buttonList[buttonIndex].button.transform.GetChild(3).GetChild(0).GetComponent<Image>().color = new Color(.4823529f, 0, 0, 1);

        while (mainEvents.func.Count > 3)
        {
            Destroy(mainEvents.func[mainEvents.func.Count - 1].textInput.transform.parent.gameObject);
            mainEvents.func.RemoveAt(mainEvents.func.Count - 1);
        }
         
        for (int i = 0; i < saveList[buttonIndex].CustomFunctions.Count; i++)
        {

            string funcText = saveList[buttonIndex].CustomFunctions[i].function;
            char funcName = saveList[buttonIndex].CustomFunctions[i].name;

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
    public void RenameSystem(string name)
    {
        StartCoroutine(RenameSystem1(LocateButton(name)));
    }
    IEnumerator RenameSystem1(int i)
    {
        yield return StartCoroutine(RenameSystemCall());
        saveList[i].SaveName = newName;
        WriteSave();
    }

    //Set Name
    IEnumerator RenameSystemCall()
    {
        awaitingEnter = true;
        renamePanel.SetActive(true);
        renameInput.ActivateInputField();
        yield return new WaitUntil(() => appliedRename);
        newName = renameInput.text;
        appliedRename = false;
        renamePanel.SetActive(false);
        renameInput.text = "";
    }

    private void Update()
    {
        //checking if rename is correct
        if (awaitingEnter)
        {
            if (Input.GetKey(enterKey))
            {
                if (renameInput.text == "")
                {
                    renameInput.ActivateInputField();
                    nameWarning.text = "Enter a valid name";
                }
                else
                {
                    if (LocateButton(renameInput.text) == -1)
                    {
                        nameWarning.text = "";
                        appliedRename = true;
                        awaitingEnter = false;
                    }
                    else
                    {
                        renameInput.ActivateInputField();
                        nameWarning.text = "Choose a new name";
                    }
                }
            }
        }
    }


    //Delete System 
    public void DeleteSystem(string name)
    {
        int i = LocateButton(name);
        if(i != -1)
        {
            //Destroy button
            Destroy(buttonList[i].button);

            //Delete preview
            string filePath = Application.streamingAssetsPath + "/PreviewImages/" + saveList[i].date + ".png";
            if (File.Exists(filePath)) File.Delete(filePath);

            //Remove from list
            saveList.RemoveAt(i);
            buttonList.RemoveAt(i);

            WriteSave();
        }
    }





    //Locates button refrence
    public int LocateButton(string name)
    {
        for (int i = 0; i < saveList.Count; i++)
        {
            if (saveList[i].SaveName.Equals(name)) return i;
        }
        return -1;
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

        //Hidden
        public bool hotkeys { get; set; }
    }

    //SaveSystem
    public class SaveList
    {
        public List<FunctionInput> CustomFunctions { get; set; }
        public string date { get; set; }
        public string SaveName { get; set; }
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

            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = name;
        }
    }


    

}