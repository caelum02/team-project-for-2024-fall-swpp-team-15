using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Yogaewonsil.Common;

public class ButtonClickListener : MonoBehaviour
{   
    public RecipeHelpUIController recipeHelpUIController;
    public AudioClip buttonClickSound; // 버튼 클릭 사운드
    private AudioSource audioSource;

    private void Start()
    {
        // AudioSource 설정
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.loop = false;
        }
    }

    private void Update()
    {
        // 마우스 클릭 이벤트 감지
        if (Input.GetMouseButtonDown(0)) // 좌클릭
        {
            HandleButtonClick();
        }
    }

    private void HandleButtonClick()
    {
        // 현재 선택된 GameObject를 가져옵니다.
        GameObject selectedObject = EventSystem.current.currentSelectedGameObject;

        if (selectedObject != null)
        {
            // 선택된 객체가 Button 컴포넌트를 가지고 있는지 확인
            Button button = selectedObject.GetComponent<Button>();
            if (button != null) 
            {
                if (button.name != "OpenButton" && button.name != "DeleteButton")
                {
                    // 버튼이 클릭되었음을 확인하고 사운드 재생
                    PlayClickSound1();
                }

                // 버튼의 Tag 확인
                if (selectedObject.CompareTag("FoodButton"))
                {                       // RecipeHelpUIController의 OnClickOpen 호출
                    if (recipeHelpUIController != null)
                    {   
                        TMP_Text buttonText = selectedObject.transform.GetComponentInChildren<TMP_Text>();

                        if (buttonText == null) return; 
                        string foodString = buttonText.text;
                        
                        if (Enum.TryParse(foodString, out Food foodEnum))
                        {   
                            gameObject.SetActive(true);
                            recipeHelpUIController.OnClickOpen(foodEnum);
                        }
                        else
                        {
                            Debug.Log($"'{foodString}'은(는) 유효한 Food enum 값이 아닙니다.");
                        }
                    }
                    else
                    {
                        Debug.LogError("RecipeHelpUIController를 찾을 수 없습니다.");
                    }
                }
            }
        }
    }

    private void PlayClickSound1()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }
}
