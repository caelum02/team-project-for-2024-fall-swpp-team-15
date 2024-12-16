using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonClickListener : MonoBehaviour
{
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
            if (button != null && button.name != "OpenButton" && button.name != "DeleteButton")
            {
                // 버튼이 클릭되었음을 확인하고 사운드 재생
                PlayClickSound1();
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
