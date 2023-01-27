using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SoundFX : MonoBehaviour, IPointerClickHandler
{
    public AudioClip sound;
    public AudioSource player;

    public void OnPointerClick(PointerEventData eventData)
    {
        player.PlayOneShot(sound);
    }


    public void Start()
    {
        //Adds a listener to the main slider and invokes a method when the value changes.
        GetComponent<Slider>().onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    // Invoked when the value of the slider changes.
    public void ValueChangeCheck()
    {
        player.PlayOneShot(sound);
    }
}
