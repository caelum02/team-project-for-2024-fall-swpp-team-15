using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

/// <summary>
/// CountertopController는 조리대(Countertop)의 동작을 관리하는 클래스입니다.
/// 기본적인 재료 추가/제거, 요리 시작, 요리 방법 변경 등의 기능을 제공합니다.
/// </summary>
public class CountertopController : CookingStationBase
{   

    [Header("Additional UI References")]
    private Button changeButton; // 요리 방법을 변경하는 버튼

    [Header("Additional Station States")]
    private bool isSliceMode = true; // 현재 요리 방법이 '손질'인지 여부
    private bool isChanging = false; // 조리방법이 바뀌는 애니메이션 중인지 여부를 나타냄

    /// <summary>
    /// 초기화 메서드로, 필요한 컴포넌트와 UI 요소를 설정합니다.
    /// </summary>
    protected override void Start()
    {
        base.Start();

        // Countertop 고유 설정
        stationName = "Countertop"; // 조리대 이름 설정
        cookingMethod = CookMethod.손질; // 기본 요리 방법 설정

        // ChangeButton 찾기 및 이벤트 연결
        changeButton = interactionPanel.Find("ChangeButton")?.GetComponent<Button>();
        if (changeButton == null)
        {
            Debug.LogError($"ChangeButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        changeButton.onClick.AddListener(ChangeCookMethod);
    }

    /// <summary>
    /// 요리 방법을 변경합니다. 버튼을 눌렀을 때 호출됩니다.
    /// </summary>
    private void ChangeCookMethod()
    {   

        if (isSliceMode)
        {
            animator.SetBool("isSliceMode", false); // 비가열조리모드로 외형 변경
            isChanging = true; // 애니메이션 동안 버튼 비활성화
            cookingMethod = CookMethod.비가열조리; // 요리 방법을 '비가열조리'로 변경
            Debug.Log("Cooking method set to 비가열조리.");
        }
        else
        {
            animator.SetBool("isSliceMode", true); // 재료손질모드로 외형 변경
            isChanging = true; // 요리 방법을 '비가열조리'로 변경
            cookingMethod = CookMethod.손질; // 요리 방법을 '손질'로 변경
            Debug.Log("Cooking method set to 손질.");
        }

        // 애니메이션이 끝날 때까지 잠시 대기
        StartCoroutine(DelayForAnimation());
        
        // 상태 전환
        isSliceMode = !isSliceMode;
    }

    /// <summary>
    /// 버튼 상태를 업데이트합니다.
    /// </summary>
    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        if (isCooking || isChanging)
        {
            // 조리 중이거나 애니메이션 중에는 모든 버튼 비활성화
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            changeButton.interactable = false;
            return;
        }

        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        addButton.interactable = heldFood != null && ingredients.Count < 4;
        cookButton.interactable = ingredients.Count > 0; 
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;
        changeButton.interactable = true; // ChangeButton: 조리중이거나 애니메이션 중이 아니라면 항상 활성화
    }

    /// <summary>
    /// 애니메이션이 끝날 때까지 대기하는 코루틴입니다.
    /// </summary>
    /// <returns>애니메이션 대기 코루틴</returns>
    private IEnumerator DelayForAnimation()
    {
        // 애니메이션 지속 시간 동안 대기
        yield return new WaitForSeconds(1f); // 애니메이션 시간을 1초로 가정
        isChanging = false; // 애니메이션이 끝나면 버튼 활성화
    }

    /// <summary>
    /// 요리를 시작합니다.
    /// </summary>
    protected override void StartCook()
    {
        base.StartCook();

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);
    }

    /// <summary>
    /// 조리 로직을 처리합니다.
    /// </summary>
    protected override void Cook()
    {    
        // 미니게임 시작
        isMiniGameActive = true;

        // 요리 방법에 따라 다른 미니게임 실행
        if (cookingMethod == CookMethod.손질){
            // '손질' 모드에서는 클릭으로 게이지바 채우기 게임 진행
            gaugeBar.StartGame(GaugeBar.GameMode.FillGaugeByClicking, 5f);
        }
        else {
            // '비가열조리' 모드에서는 마커를 게이지바 가운데에 맞추기 게임 진행
            gaugeBar.StartGame(GaugeBar.GameMode.MarkerMatching, 5f);
        }
        gaugeBar.OnGameComplete += OnGameComplete; // 미니게임 완료 이벤트 연결
    }

    /// <summary>
    /// 미니게임 완료 시 호출됩니다.
    /// </summary>
    /// <param name="isSuccess">미니게임 성공 여부</param>
    private void OnGameComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        Debug.Log(isSuccess
            ? "Mini-game succeeded! Cooking successful."
            : "Mini-game failed! Cooking failed.");

        EndMiniGame(isSuccess);
    }

    /// <summary>
    /// 미니게임 종료를 처리합니다.
    /// </summary>
    /// <param name="isSuccess">미니게임 성공 여부</param>
    private void EndMiniGame(bool isSuccess)
    {
        isMiniGameActive = false;

        // 미니게임 이벤트 해제
        gaugeBar.OnGameComplete -= OnGameComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
