using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections; // 코루틴에서 필요한 네임스페이스
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public abstract class CookingStationBase : KitchenInteriorWithAddandRemove
{

    [Header("Station Setting")]
    public CookMethod cookingMethod; // 조리 방법

    [Header("UI References")]
    protected Button cookButton; 
    protected Transform gaugeBarPanel;
    protected GaugeBar gaugeBar;

    [Header("Station State")]
    protected bool isCooking = false;
    protected bool isMiniGameActive = false;

    [Header("Animator")]
    protected Animator animator;

    protected override void Start()
    {   
        base.Start();
        // Animator 자동 연결
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on this GameObject or its children!");
        }

        // CookButton 찾기
        cookButton = interactionPanel.Find("CookButton")?.GetComponent<Button>();
        if (cookButton == null)
        {
            Debug.LogError($"CookButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        cookButton.onClick.AddListener(StartCook); // CookButton의 onClick 이벤트에 Cook 함수 연결

        // GaugeBarPanel 설정
        gaugeBarPanel = visualMenu.transform.Find("GaugeBarPanel");
        if (gaugeBarPanel == null)
        {
            Debug.LogError($"GaugeBarPanel is not found in {visualMenu.name}");
            return;
        }

        gaugeBar = gaugeBarPanel.GetComponentInChildren<GaugeBar>();
        if (gaugeBar == null)
        {
            Debug.LogError($"GaugeBar component not found in GaugeBarPanel!");
            return;
        }

        // 초기 상태
        gaugeBarPanel.gameObject.SetActive(false);
    }

    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        if (isCooking)
        {
            // 조리 중에는 모든 버튼 비활성화
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            return;
        }

        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        addButton.interactable = heldFood != null && ingredients.Count < 4;        // AddButton: 음식이 있으면 활성화
        cookButton.interactable = ingredients.Count > 0;       // CookButton: 재료가 있으면 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;     // RemoveButton: 재료가 있으면 활성화
    }

    protected virtual void StartCook()
    {
        if (ingredients.Count == 0)
        {
            Debug.LogWarning("No ingredients to cook!");
            return;
        }

        isCooking = true;
        animator.SetBool("isCooking", true);

        // 버튼 상태 비활성화
        UpdateAllButtons();

        // UI 상태 업데이트
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 요리 진행
        Cook();
    }

    protected virtual void Cook()
    {
        // 조리기구에서 MiniGame을 진행하고 결과에 따라 CompleteCook를 호출하는 식으로 구현
    }

    protected void CompleteCook(bool isMiniGameSuccess)
    {   
        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        StartCoroutine(CompleteCookWithDelay(isMiniGameSuccess));
    }

    private IEnumerator CompleteCookWithDelay(bool isMiniGameSuccess)
    {   
        // Player 활성화
        PlayerController.Instance.SetMovementEnabled(true);
        // 3초 대기
        yield return new WaitForSeconds(3f); // 3초 정도 대기

        // Recipe.Execute를 활용하여 결과 요리 확인
        Food resultFood = Food.실패요리;
        if (isMiniGameSuccess)
        {
            resultFood = Recipe.Execute(cookingMethod, ingredients);
        }

        ingredients.Clear();

        if (resultFood == Food.밥 || resultFood == Food.라멘육수)
        {   
            Debug.Log($"Cooking successful: {resultFood}");
            for (int i = 0; i < 5; i++)
            {
                ingredients.Add(resultFood);
            }
        }
        else if (resultFood != Food.실패요리)
        {
            Debug.Log($"Cooking successful: {resultFood}");
            ingredients.Add(resultFood);
        }
        else
        {
            Debug.LogWarning("Cooking failed! No matching recipe.");
            ingredients.Add(resultFood);
        }

        animator.SetBool("isCooking", false);
        isCooking = false;

        // 조리 후 버튼 상태 업데이트
        UpdateAllButtons();
        // 조리 후 아이콘 상태 업데이트
        UpdateIngredientIcons();
    }
}
