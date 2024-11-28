using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Yogaewonsil.Common;

public class NewCountertopController : CookingStationBase
{   

    [Header("Additional UI References")]
    private Canvas extraCanvas;
    private Transform gaugeBarPanel;
    private GaugeBar gaugeBar;
    private Button changeButton;

    private bool isMiniGameActive = false;
    private bool isSliceMode = true;
    private bool isChanging = false;

    protected override void Start()
    {
        base.Start();

        // Countertop 고유 설정
        stationName = "Countertop";
        cookingMethod = CookMethod.손질;

        gaugeBarPanel = visualMenu.transform.Find("GaugeBarPanel");
        if (gaugeBarPanel == null)
        {
            Debug.LogError($"GaugeBarPanel is not found in {visualMenu.name}");
            return;
        }

        gaugeBar = gaugeBarPanel.GetComponentInChildren<GaugeBar>();
        if (gaugeBar == null)
        {
            Debug.LogError("GaugeBar component not found in GaugeBarPanel!");
            return;
        }

        changeButton = interactionPanel.Find("ChangeButton")?.GetComponent<Button>();
        if (changeButton == null)
        {
            Debug.LogError($"ChangeButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        changeButton.onClick.AddListener(ChangeCookMethod);

        gaugeBarPanel.gameObject.SetActive(false);
    }

    private void ChangeCookMethod()
    {   

        if (isSliceMode)
        {
            animator.SetBool("isSliceMode", false);
            isChanging = true; // 다른 버튼들을 비활성화시키기 위해
            cookingMethod = CookMethod.비가열조리; // CookMethod.비가열조리
            Debug.Log("Cooking method set to 비가열조리.");
        }
        else
        {
            animator.SetBool("isSliceMode", true);
            isChanging = true; // 다른 버튼들을 비활성화시키기 위해
            cookingMethod = CookMethod.손질; // CookMethod.비가열조리
            Debug.Log("Cooking method set to 손질.");
        }

        StartCoroutine(DelayForAnimation());

        isSliceMode = !isSliceMode;
    }

    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        if (isCooking || isChanging)
        {
            // 조리 중에는 모든 버튼 비활성화
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            changeButton.interactable = false;
            return;
        }

        if (PlayerController.Instance == null)
        {
            addButton.interactable = false;
            cookButton.interactable = false;
            removeButton.interactable = false;
            changeButton.interactable = false;
            return;
        }

        Food? heldFood = PlayerController.Instance.GetHeldFood();

        // 버튼 활성화/비활성화 상태 업데이트
        addButton.interactable = heldFood != null && ingredients.Count < 4;        // AddButton: 음식이 있으면 활성화
        cookButton.interactable = ingredients.Count > 0;       // CookButton: 재료가 있으면 활성화
        removeButton.interactable = ingredients.Count > 0 && heldFood == null;
        changeButton.interactable = true;     // RemoveButton: 재료가 있으면 활성화
    }

    private IEnumerator DelayForAnimation()
    {
        // 애니메이션 재생 시간을 고려하여 20프레임(약 0.333초) 대기
        yield return new WaitForSeconds(1f); // assuming 60 FPS

        isChanging = false;
    }

    protected override void StartCook()
    {
        base.StartCook();

        // Player 비활성화
        PlayerController.Instance.SetMovementEnabled(false);
    }

    protected override void Cook()
    {    
        // UI 상태 업데이트
        gaugeBarPanel.gameObject.SetActive(true);
        iconPanel.gameObject.SetActive(false);

        // 미니게임 초기화
        isMiniGameActive = true;

        // 게이지 바 채우기 모드 시작
        if (cookingMethod == CookMethod.손질){
          gaugeBar.StartGame(GaugeBar.GameMode.FillGaugeByClicking, 5f);
        }
        else {
          gaugeBar.StartGame(GaugeBar.GameMode.MarkerMatching, 5f);
        }
        gaugeBar.OnGameComplete += OnGameComplete;
    }

    private void OnGameComplete(bool isSuccess)
    {
        if (!isMiniGameActive) return;

        Debug.Log(isSuccess
            ? "Mini-game succeeded! Cooking successful."
            : "Mini-game failed! Cooking failed.");

        EndMiniGame(isSuccess);
    }

    private void EndMiniGame(bool isSuccess)
    {
        isMiniGameActive = false;

        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        // 이벤트 해제
        gaugeBar.OnGameComplete -= OnGameComplete;

        // CompleteCook() 호출하여 요리 결과 처리
        CompleteCook(isSuccess);
    }
}
