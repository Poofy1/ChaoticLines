using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorScheme : MonoBehaviour
{
    public Button loadButton;
    public Button delButton;
    public Image[] colors;
    public Image background;

    public void UpdateAll(Color[] mainInput, Color back)
    {
        for (int i = 0; i < 10; i++)
        {
            colors[i].color = mainInput[i];
        }

        background.color = back;
    }
}
