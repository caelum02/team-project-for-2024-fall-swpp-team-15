using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using TMPro;

public class BGMController : MonoBehaviour
{
    public GameManager gameManager;
    [SerializeField] protected AudioSource audioSource; // 요리 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip backgroundMusic; // 정비 시간에 재생되는 사운드
    [SerializeField] private AudioClip cookingTimeMusic; // 정비 시간에 재생되는 사운드
    void Start()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.Play(); 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameManager.openOrCloseText.text == "영업 시간")
        {
            if (audioSource.clip != cookingTimeMusic && audioSource != null && cookingTimeMusic != null)
            {
                audioSource.clip = cookingTimeMusic;
                audioSource.loop = true;
                audioSource.Play(); 
            }
        }
        else
        {
            if (audioSource.clip != backgroundMusic && audioSource != null && backgroundMusic != null)
            {
                audioSource.clip = backgroundMusic;
                audioSource.loop = true;
                audioSource.Play(); 
            }
        }
    }
}
