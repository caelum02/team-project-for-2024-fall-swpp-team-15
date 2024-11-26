using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using Yogaewonsil.Common;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스

public class CounterTopController : MonoBehaviour
{
    // 조리기구 정보
    public string stationName; // 조리기구 이름
    public CookMethod cookingMethod; // 조리 방법

    // 현재 조리기구 상태
    public List<Food> ingredients = new List<Food>(); // 추가된 재료 목록
    public Transform dishSpawnPoint; // 결과물 생성 위치

    // World Space Canvas 관련
    private Canvas interactionCanvas; // 상호작용 Canvas (자식 객체로 설정)
    private Transform interactionPanel;
    private Button addButton; // AddButton (InteractionCanvas의 자식)
    private Button cookButton; // RemoveButton (InteractionCanvas의 자식)
    private Button removeButton; // RemoveButton (InteractionCanvas의 자식)
    private Transform selectionPanel;
    private Button backButton;

    [SerializeField] private GameObject buttonPrefab;
    

    // 현재 활성화된 조리기구
    public static CounterTopController activeStation;

    // 플레이어 관련
    // public PlayerController player;

private void Start()
{
    // 자식 객체에서 InteractionCanvas를 찾음
    interactionCanvas = GetComponentInChildren<Canvas>(true);
    if (interactionCanvas == null)
    {
        Debug.LogError($"InteractionCanvas not found in {gameObject.name}");
        return;
    }

    // InteractionPanel 찾기
    interactionPanel = interactionCanvas.transform.Find("InteractionPanel");
    if (interactionPanel == null)
    {
        Debug.LogError($"InteractionPanel not found in {interactionCanvas.name}");
        return;
    }

    // SelectionPanel 찾기
    selectionPanel = interactionCanvas.transform.Find("SelectionPanel");
    if (selectionPanel == null)
    {
        Debug.LogError($"SelectionPanel not found in {interactionCanvas.name}");
        return;
    }

    // AddButton 찾기
    addButton = interactionPanel.Find("AddButton")?.GetComponent<Button>();
    if (addButton == null)
    {
        Debug.LogError($"AddButton not found in InteractionPanel of {gameObject.name}");
        return;
    }
    addButton.onClick.AddListener(AddIngredient); // AddButton의 onClick 이벤트에 AddIngredient 함수 연결

    // CookButton 찾기
    cookButton = interactionPanel.Find("CookButton")?.GetComponent<Button>();
    if (cookButton == null)
    {
        Debug.LogError($"CookButton not found in InteractionPanel of {gameObject.name}");
        return;
    }
    cookButton.onClick.AddListener(Cook); // CookButton의 onClick 이벤트에 Cook 함수 연결

    // RemoveButton 찾기
    removeButton = interactionPanel.Find("RemoveButton")?.GetComponent<Button>();
    if (removeButton == null)
    {
        Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
        return;
    }
    removeButton.onClick.AddListener(ShowSelectionPanel); // RemoveButton의 onClick 이벤트에 RemoveIngredient 함수 연결

    // BackButton 찾기
    backButton = selectionPanel.Find("BackButton")?.GetComponent<Button>();
    if (backButton == null)
    {
        Debug.LogError($"BackButton not found in InteractionPanel of {gameObject.name}");
        return;
    }
    backButton.onClick.AddListener(HideSelectionPanel); // BackButton의 onClick 이벤트에 HideSelectionPanel함수 연결

    // 초기에 selectionPanel 비활성화
    interactionCanvas.gameObject.SetActive(false);
    interactionPanel.gameObject.SetActive(true);
    selectionPanel.gameObject.SetActive(false);
}


    private void Update()
    { 
        UpdateAddButtonState(); // AddButton 상태 업데이트
        UpdateCookAndRemoveButtonState(); // RemoveButton 상태 업데이트
        HandleInteractionCanvas();
    }

    private void UpdateAddButtonState()
    {
        // Player가 음식을 들고 있는지 확인
        if (PlayerController.Instance == null)
        {
            addButton.interactable = false;
            return;
        }

        Food heldFood = PlayerController.Instance.GetHeldFood();

        // Food.None일 경우 버튼 비활성화
        addButton.interactable = heldFood != Food.None;
    }

    private void UpdateCookAndRemoveButtonState()
    { 
        cookButton.interactable = ingredients.Count > 0; // ingredients가 비어 있으면 버튼 비활성화
        removeButton.interactable = ingredients.Count > 0; // ingredients가 비어 있으면 버튼 비활성화
    }

    private void HandleInteractionCanvas()
    {

        if (PlayerController.Instance == null|| interactionCanvas == null) return;

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer <= 3f) // 상호작용 범위 내
        {
            if (activeStation == null || activeStation == this)
            {
                ShowCanvas();
                activeStation = this;
            }
        }
        else
        {
            if (activeStation == this)
            {
                HideCanvas();
                activeStation = null;
            }
        }
    }

    public void ShowCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(true);
        }
    }

    public void HideCanvas()
    {
        if (interactionCanvas != null)
        {
            interactionCanvas.gameObject.SetActive(false);
        }
    }

    public void AddIngredient()
    { 
        Debug.Log("AddIngredient!");
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != Food.None)
        {
            ingredients.Add(PlayerController.Instance.heldFood); // 재료 추가
            PlayerController.Instance.DropFood(); // 플레이어가 들고 있는 재료 내려놓기
            Debug.Log("Ingredients in the cooking station:");
            foreach (var ingredient in ingredients)
            {
                Debug.Log($"- {ingredient}");
            }
        }
    }

    private void RemoveIngredient(Food ingredient)
    {
        // 조리도구에서 재료 제거
        if (ingredients.Contains(ingredient))
        {
            // PlayerController에 재료 추가
            if(PlayerController.Instance != null)
            {
                if (PlayerController.Instance.PickUpFood(ingredient)){
                    Debug.Log($"Player picked up: {ingredient}"); // 플레이어가 재료를 집음
                    ingredients.Remove(ingredient);
                    Debug.Log($"Removed ingredient: {ingredient}");
                } 
                else {
                    Debug.Log($"Player already hold: {PlayerController.Instance.GetHeldFood()}"); // 플레이어가 이미 집고 있는 재료가 있음
                }
            }
            else
            {
                Debug.LogWarning("PlayerController instance not found!");
            }
        }

        // SelectionPanel 숨김
        HideSelectionPanel();
    }

    public void Cook()
    {
        // Recipe.Execute를 활용하여 결과 요리 확인
        Food resultFood = Recipe.Execute(cookingMethod, ingredients);

        Debug.Log($"Cooking successful: {resultFood}");

        // 재료 초기화
        ingredients.Clear();
        ingredients.Add(resultFood);
    }

    public void ShowSelectionPanel()
    {
        // InteractionPanel 비활성화
        interactionPanel.gameObject.SetActive(false);

        // SelectionPanel 활성화
        selectionPanel.gameObject.SetActive(true);

        // 기존 버튼 삭제 (BackButton 제외)
        foreach (Transform child in selectionPanel)
        {
            if (child.name != "BackButton")
            {
                Destroy(child.gameObject);
            }
        }

        // 재료 버튼 생성
        foreach (var ingredient in ingredients)
        {
            CreateIngredientButton(ingredient);
        }
    }

    private void HideSelectionPanel()
    {
        // SelectionPanel 비활성화, InteractionPanel 활성화
        interactionPanel.gameObject.SetActive(true);
        selectionPanel.gameObject.SetActive(false);
    }

  private void CreateIngredientButton(Food ingredient)
  {
      // Button Prefab 복제
      GameObject buttonObject = Instantiate(buttonPrefab, selectionPanel);

      buttonObject.transform.SetSiblingIndex(0);

      // TextMeshProUGUI 컴포넌트 설정
      TMP_Text buttonText = buttonObject.GetComponentInChildren<TMP_Text>(); // TextMeshProUGUI를 찾음
      if (buttonText != null)
      {
          buttonText.text = ingredient.ToString(); // 재료 이름 설정
      }
      else
      {
          Debug.LogWarning($"TextMeshProUGUI component not found in Button Prefab for {ingredient}.");
      }

      // 버튼 클릭 이벤트 추가
      Button button = buttonObject.GetComponent<Button>();
      if (button != null)
      {
          button.onClick.AddListener(() => RemoveIngredient(ingredient));
      }
  }
}
