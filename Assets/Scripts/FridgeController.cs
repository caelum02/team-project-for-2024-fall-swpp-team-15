using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; 


/// <summary>
/// 냉장고(Fridge)를 제어하는 클래스입니다.
/// 주방 내부의 재료 상점을 여는 기능을 제공합니다.
/// </summary>
public class FridgeController : KitchenInteriorBase
{   
    // 싱글톤 인스턴스
    // public static FridgeController Instance { get; private set; }
    private Button openButton; // 냉장고 여는(재료상점 여는) 버튼
    private bool isOpen = false; // 냉장고 문 상태 (true: 열림, false: 닫힘)

    [Header("Animator")]
    protected Animator animator; // 냉장고 애니메이션 제어를 위한 Animator

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource; // 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip openSound; // 냉장고 문 개방 시 재생할 사운드
    [SerializeField] private AudioClip closeSound; // 냉장고 문 닫을 시 재생할 사운드
    
    /// <summary>
    /// 싱글톤 설정
    /// </summary>
    // private void Awake()
    // {
    //     // 싱글톤 설정
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject); // 기존 인스턴스가 있다면 현재 객체 삭제
    //         return;
    //     }
    //     Instance = this;
    // }

    /// <summary>
    /// 초기화 작업을 수행합니다. 버튼과 애니메이터를 설정하고 부모 클래스의 초기화 로직을 호출합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // InteractionPanel에서 OpenButton을 찾습니다.
        openButton = interactionPanel.Find("OpenButton")?.GetComponent<Button>();
        if (openButton == null)
        {
            Debug.LogError($"openButton not found in FridgeController");
            return;
        }
        openButton.onClick.AddListener(OpenIngredientShop); // OpenButton 클릭 시 OpenIngredientShop 호출

        // Animator 자동 연결
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject or its children!");
        }
    }

    /// <summary>
    /// 재료 상점 UI를 열고 냉장고 애니메이션을 실행합니다.
    /// </summary>
    private void OpenIngredientShop()
    {   
        // Click 트리거를 활성화
        PlayerController.Instance.playerAnimator.SetTrigger("clickTrig");

        // 문 여는 사운드 재생
        if (audioSource != null && openSound != null)
        {
            audioSource.clip = openSound;
            audioSource.loop = false; // 필요 시 루프 설정
            audioSource.Play(); // 사운드 재생
            Debug.Log("OpenSound Plays");
        }

        IngredientShopManager.Instance.OnClickOpenFridge(); // 재료상점UI를 열기(제어권이 넘어감)

        PlayerController.Instance.SetMovementEnabled(false); // 재료상점이 열려있는동안 플레이어는 움직일 수 없읍

        animator.SetBool("isOpen", true); // 냉장고 열림 애니메이션 실행
        isOpen = true; // 냉장고 상태를 열림으로 변경
    }

    /// <summary>
    /// 버튼 상태를 업데이트합니다. 냉장고가 열려 있으면 버튼을 비활성화합니다.
    /// </summary>
    protected override void UpdateAllButtons()
    {   
        Food? heldFood = PlayerController.Instance.GetHeldFood();
        openButton.interactable = heldFood == null && !isOpen; // 냉장고가 열려 있거나 플레이어가 손에 음식이 있다면 버튼 비활성화
    }


    /// <summary>
    /// 플레이어와의 거리 기반으로 상호작용 메뉴를 처리합니다.
    /// 플레이어가 냉장고에서 멀어질 경우 냉장고 문을 닫습니다.
    /// </summary>
    // protected override void HandleInteractionMenu()
    // {
    //     base.HandleInteractionMenu();

    //     // 현재 활성화된 기구가 이 냉장고가 아니라면 문을 닫습니다.
    //     if (activeStation != this )
    //     {
    //         CloseFridge();
    //     }
    // }

    /// <summary>
    /// 냉장고 문을 닫는 함수
    /// HandleInteractionMenu와 IngredientShopManager에서 사용
    /// </summary>
    public void CloseFridge()
    {   
        // Click 트리거를 활성화
        PlayerController.Instance.playerAnimator.SetTrigger("clickTrig");

        animator.SetBool("isOpen", false); // 냉장고 닫힘 애니메이션 실행
        isOpen = false; // 냉장고 상태를 닫힘으로 변경
        PlayerController.Instance.SetMovementEnabled(true);

        StartCoroutine(PlayCloseSound());
    }

    protected IEnumerator PlayCloseSound()
    {   
        // 1초 대기
        yield return new WaitForSeconds(0.9f); 

        // 문 닫는 사운드 재생
        if (audioSource != null && closeSound != null)
        {
            audioSource.clip = closeSound;
            audioSource.loop = false; // 필요 시 루프 설정
            audioSource.Play(); // 사운드 재생
        }
    }
}