using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager _instance;
    public TextMeshProUGUI text;
    public bool active = true;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }




    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void InitializeTooltip(string message)
    {
        if (active)
        {
            gameObject.SetActive(true);
            text.text = message;
        }
        
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
        text.text = string.Empty;
    }

}
