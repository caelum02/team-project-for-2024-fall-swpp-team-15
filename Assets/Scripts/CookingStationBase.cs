using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections;
using Yogaewonsil.Common;
using TMPro;

/// <summary>
/// 조리 기구(조리기능이 있는 주방기구)의 기본 기능을 제공하는 추상 클래스입니다.
/// 재료 추가/제거 뿐만 아니라 조리 로직을 추가해 관리합니다.
/// </summary>
public abstract class CookingStationBase : KitchenInteriorWithAddandRemove
{

    [Header("Station Setting")]
    public CookMethod cookingMethod; // 조리 방법

    [Header("UI References")]
    protected Button cookButton; // 요리 시작 버튼
    protected Transform gaugeBarPanel; // 게이지 바 패널
    protected GaugeBar gaugeBar; // 게이지 바

    [Header("Station State")]
    protected bool isCooking = false; // 현재 조리 중인지 여부
    protected bool isMiniGameActive = false; // 미니게임 활성화 여부

    [Header("Animator")]
    protected Animator animator; // 애니메이터 제어기

    /// <summary>
    /// 초기화 메서드로, 필요한 컴포넌트와 UI 요소를 설정합니다.
    /// </summary>
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

        // GaugeBar 설정
        gaugeBar = gaugeBarPanel.GetComponentInChildren<GaugeBar>();
        if (gaugeBar == null)
        {
            Debug.LogError($"GaugeBar component not found in GaugeBarPanel!");
            return;
        }

        // 초기 상태로 게이지 바 패널 비활성화
        gaugeBarPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 버튼의 활성화 상태를 업데이트합니다.
    /// </summary>
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
        addButton.interactable = heldFood != null && ingredients.Count < 4;   // AddButton: 플레이어가 재료를 들고 있고 재료 수가 4개 미만일 때 활성화
        cookButton.interactable = ingredients.Count > 0;       // CookButton: 재료가 있으면 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;     // RemoveButton: 재료가 있고 플레이어가 아무것도 들고 있지 않을 때 활성화
    }

    /// <summary>
    /// 요리를 시작합니다.
    /// </summary>
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

    /// <summary>
    /// 조리 로직을 처리합니다.
    /// 실제 조리 로직은 상속받은 클래스에서 구현됩니다.
    /// </summary>
    protected virtual void Cook()
    {
        // 조리기구에서 MiniGame을 진행하고 결과에 따라 CompleteCook를 호출하는 식으로 구현
    }

    /// <summary>
    /// 요리가 완료되었을 때 호출됩니다.
    /// </summary>
    /// <param name="isMiniGameSuccess">미니게임 성공 여부</param>
    protected void CompleteCook(bool isMiniGameSuccess)
    {   
        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        StartCoroutine(CompleteCookWithDelay(isMiniGameSuccess));
    }

    /// <summary>
    /// 요리 완료 후 지연을 처리하는 코루틴입니다.
    /// </summary>
    /// <param name="isMiniGameSuccess">미니게임 성공 여부</param>
    /// <returns></returns>
    private IEnumerator CompleteCookWithDelay(bool isMiniGameSuccess)
    {   
        // Player 활성화
        PlayerController.Instance.SetMovementEnabled(true);

        // 3초 대기
        yield return new WaitForSeconds(3f); // 3초 정도 대기

        // Recipe.Execute를 활용하여 요리 결과 확인
        Food resultFood = Food.실패요리;
        if (isMiniGameSuccess)
        {
            resultFood = Recipe.Execute(cookingMethod, ingredients);
        }

        // 재료 리스트 초기화
        ingredients.Clear();

        if (resultFood == Food.밥 || resultFood == Food.라멘육수)
        {   
            // 밥이나 라멘육수의 경우 한 번에 5번 사용 가능하도록 처리
            Debug.Log($"Cooking successful: {resultFood}");
            for (int i = 0; i < 5; i++)
            {
                ingredients.Add(resultFood);
            }
        }
        else if (resultFood != Food.실패요리)
        {   
            // 일반적인 요리 성공
            Debug.Log($"Cooking successful: {resultFood}");
            ingredients.Add(resultFood);
        }
        else
        {
            // 요리 실패
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
