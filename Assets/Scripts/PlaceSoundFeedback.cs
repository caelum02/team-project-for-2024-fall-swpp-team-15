using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceSoundFeedback : MonoBehaviour
{
    [SerializeField]
    private AudioClip placeSound, removeSound, errorSound;

    [SerializeField]
    private AudioSource audioSource;

    public void PlaySound(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.Place:
                audioSource.PlayOneShot(placeSound);
                break;
            case SoundType.Remove:
                audioSource.PlayOneShot(removeSound);
                break;
            case SoundType.Error:
                audioSource.PlayOneShot(errorSound);
                break;
            default:
                break;
        }
    }
}

public enum SoundType
{
    Place,
    Remove,
    Error
}