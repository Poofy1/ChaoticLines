using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    public AudioClip[] audioClips;
    private AudioSource audio;
    private int currentClipIndex;
    private int[] randomOrder;

    private void Start()
    {
        audio = GetComponent<AudioSource>();

        // Generate a random order for the audio clips
        randomOrder = GenerateRandomOrder(audioClips.Length);

        // Play the first audio clip
        StartCoroutine(PlayNextClip());
    }

    private IEnumerator PlayNextClip()
    {
        // Play the current audio clip
        audio.PlayOneShot(audioClips[randomOrder[currentClipIndex]]);

        // Wait for the current audio clip to finish playing
        yield return new WaitForSeconds(audioClips[randomOrder[currentClipIndex]].length);

        // Increment the current clip index, and wrap around to the beginning if needed
        currentClipIndex = (currentClipIndex + 1) % audioClips.Length;

        // Play the next audio clip
        StartCoroutine(PlayNextClip());
    }

    private int[] GenerateRandomOrder(int length)
    {
        int[] order = new int[length];

        // Initialize the order array
        for (int i = 0; i < length; i++)
        {
            order[i] = i;
        }

        // Shuffle the order array
        for (int i = 0; i < length; i++)
        {
            int randomIndex = Random.Range(i, length);
            int temp = order[i];
            order[i] = order[randomIndex];
            order[randomIndex] = temp;
        }

        return order;
    }
}
