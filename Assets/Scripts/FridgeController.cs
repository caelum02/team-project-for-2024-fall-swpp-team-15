using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; 

public class FridgeController : KitchenInteriorBase
{
    private Button openButton; // 냉장고 여는(재료상점 여는) 버튼
    private bool isOpen = false;

    [Header("Animator")]
    protected Animator animator;

    protected virtual void Start()
    {
        base.Start();

        openButton = interactionPanel.Find("OpenButton")?.GetComponent<Button>();
        if (openButton == null)
        {
            Debug.LogError($"openButton not found in FridgeController");
            return;
        }
        openButton.onClick.AddListener(OpenIngredientShop);

        // Animator 자동 연결
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject or its children!");
        }
    }

    private void OpenIngredientShop()
    {
        // 재료 상점 UI와 연결
        animator.SetBool("isOpen", true);
        isOpen = true;
    }

    protected override void UpdateAllButtons()
    {   
        openButton.interactable = !isOpen;
    }

    // 지금은 냉장고를 닫을 로직이 없어서 (상점과의 연결 후 만들 예정) 임시적으로 Player가 멀어질 경우 문 닫게 구현
    protected override void HandleInteractionMenu()
    {
        base.HandleInteractionMenu();

        if (activeStation != this )
        {
            animator.SetBool("isOpen", false);
            isOpen = false;
        }
    }
}