using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorButton : MonoBehaviour
{
    public Image color;
    public Button delButton;



    public void SetColor(Color col)
    {
        color.color = col;
    }

    // Start is called before the first frame update
    void Awake()
    {
        
    }

}
