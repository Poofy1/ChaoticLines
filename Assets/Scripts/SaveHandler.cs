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


    private GameObject[] spawnedButtons;
    private int currentSelected;
    private bool appliedRename;
    private string newName;

    private void Start()
    {
        mainEvents = GameObject.Find("Chaos_Event").GetComponent<Chaos>();
        saveList = new List<SaveList>();
        buttonList = new List<SaveButton>();
        savedSet = new SettingItem();
        spawnedButtons = new GameObject[0];
        LoadAll(true);



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


    //Save Custom
    public void SaveCurrent()
    {
        StartCoroutine(SaveCurrentCall());
    }

    IEnumerator SaveCurrentCall()
    {
        bool[] vaild = mainEvents.TestCustom();
        if (vaild[0] && vaild[1] && vaild[2])
        {
            yield return StartCoroutine(RenameSystemCall());

            saveList.Add(new SaveList
            {
                CustomInput = new string[3] { mainEvents.customInput[0].GetComponent<TMP_InputField>().text,
                                                                  mainEvents.customInput[1].GetComponent<TMP_InputField>().text,
                                                                  mainEvents.customInput[2].GetComponent<TMP_InputField>().text},
                SaveName = newName
            });

            WriteSave();
            ResetSelected();
        }
        else
        {
            mainEvents.errors[3].SetActive(true);
        }
        
    }

    //Save Settings
    public void SaveSettings()
    {
        savedSet.fullscreen = settings.fullscreen[0].isOn;
        savedSet.AA = settings.AAMenu[0].GetComponent<TMP_Dropdown>().value;
        savedSet.vSync = !settings.vSyncOn;
        savedSet.mouseSen = mouse.setting[0].value;
        savedSet.sound = settings.musicSlider[0].value;
        savedSet.fov = settings.fovSlider[0].value;
        savedSet.hudScale = settings.hudSlider[0].value;
        savedSet.safety = mainEvents.safety;

        //Write
        string json = JsonConvert.SerializeObject(savedSet);
        File.WriteAllText(Application.dataPath + "/UserSettings.txt", json);
    }

    //LoadSettings
    public void LoadSettings()
    {
        settings.fullscreen[0].isOn = savedSet.fullscreen;
        settings.AAMenu[0].GetComponent<TMP_Dropdown>().value = savedSet.AA;
        settings.vSyncOn = savedSet.vSync;
        mouse.setting[0].value = savedSet.mouseSen;
        settings.musicSlider[0].value = savedSet.sound;
        settings.fovSlider[0].value = savedSet.fov;
        settings.hudSlider[0].value = savedSet.hudScale;
        mainEvents.safety = !savedSet.safety;

        mouse.SensitivityChanged(0);
        settings.UpdateAll();
    }

    //Write Save
    public void WriteSave()
    {
        string json = JsonConvert.SerializeObject(saveList);
        File.WriteAllText(Application.dataPath + "/UserData.txt", json);

        //Re-load all data
        LoadAll();
    }

    //Load All Data
    public void LoadAll(bool bugWorkAround = false)
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
            int offset = -100;

            for (int i = 0; i < saveList.Count; i++)
            {
                //Spawn
                var name = Instantiate(button, new Vector3(0, 0, 0), Quaternion.identity, buttonParent.transform);
                name.gameObject.name = "Button" + i;
                spawnedButtons[i] = name.gameObject;

                //Create List
                buttonList.Add(new SaveButton(saveList[i].SaveName, saveList[i].CustomInput, spawnedButtons[i]));
                if (bugWorkAround) buttonList[i].SetPos(new Vector3(0, offset, 0), new Vector3(0,0,0) );
                else buttonList[i].SetPos(new Vector3(400, offset, 0), new Vector3(0, 0, 0));
                offset -= 150;

                //Set Listener
                setButtonListener(i);

                //ScrollSize
                buttonParent.GetComponent<RectTransform>().sizeDelta = new Vector2(0, -offset);
            }
        }
        else
        {
            Debug.Log("No Save!"); //UI This in the future
        }
    }

    //Set Listeners
    private void setButtonListener(int i)
    {
        buttonList[i].button.GetComponent<Button>().onClick.AddListener(delegate { ButtonClicked(i); });
    }

    //When Save is clicked
    public void ButtonClicked(int a)
    {
        for (int i = 1; i < functionButtons.Length; i++) functionButtons[i].GetComponent<Button>().interactable = true;

        currentSelected = a;
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.098f, .098f, .098f, .6749f);

        buttonList[a].button.GetComponent<Image>().color = new Color(0, 0, 0, .6749f);
    }

    //Reset currently Selected
    public void ResetSelected()
    {
        for (int i = 1; i < functionButtons.Length; i++) functionButtons[i].GetComponent<Button>().interactable = false;
        for (int i = 0; i < spawnedButtons.Length; i++) buttonList[i].button.GetComponent<Image>().color = new Color(.098f, .098f, .098f, .6749f);
    }


    //Load System
    public void LoadSystem()
    {
        for (int i = 0; i < 3; i++)
        {
            mainEvents.customInput[i].GetComponent<TMP_InputField>().text = saveList[currentSelected].CustomInput[i];
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
    IEnumerator RenameSystemCall()
    {
        renamePanel.SetActive(true);
        yield return new WaitUntil(() => appliedRename);
        newName = renameInput.text;
        appliedRename = false;
        renamePanel.SetActive(false);
        renameInput.text = "";
    }

    //Apply Rename
    public void ApplyRename()
    {
        appliedRename = true;
    }

    //Delete System
    public void DeleteSystem()
    {
        saveList.RemoveAt(currentSelected);
        ResetSelected();
        WriteSave();
    }









    //SaveSystem
    public class SaveList
    {
        public string[] CustomInput { get; set; }
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

    //Item List
    public class SaveButton
    {
        public string[] customInputs;
        public string name;
        public GameObject button;

        //Constructor
        public SaveButton(string nameInput, string[] input, GameObject obj)
        {
            customInputs = input;
            name = nameInput;
            button = obj;

            button.transform.GetChild(0).GetComponent<Text>().text = name;
            button.transform.GetChild(1).GetComponent<Text>().text = customInputs[0] + "\n" + customInputs[1] + "\n" + customInputs[2];
        }

        //Move
        public void SetPos(Vector3 pos, Vector3 rot)
        {
            button.transform.localPosition = pos;
            button.transform.localEulerAngles = rot;
        }
    }

}