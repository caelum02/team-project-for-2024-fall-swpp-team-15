using UnityEngine;
using UnityEngine.UI; 
using System.Collections.Generic;
using System.Collections;
using Yogaewonsil.Common;
using TMPro;

/// <summary>
/// 조리 기구(조리기능이 있는 주방기구)의 기본 기능을 제공하는 추상 클래스입니다.
/// 재료 추가/제거 및 조리 로직을 추가해 관리합니다.
/// </summary>
public abstract class CookingStationBase : KitchenInteriorBase
{

    [Header("Station Setting")]
    public CookMethod cookingMethod; // 조리 방법

    [Header("UI References")]
    protected Button addButton; // 재료를 추가하는 버튼
    protected Button cookButton; // 요리 시작 버튼
    protected Button removeButton; // 재료를 제거하는 버튼
    protected Transform selectionPanel; // 재료 선택 패널
    protected Button backButton; // 선택 패널을 닫는 버튼
    protected Transform visualMenu; // 시각적 메뉴
    protected Transform iconPanel; // 재료 아이콘 패널
    protected Transform gaugeBarPanel; // 게이지 바 패널
    protected GaugeBar gaugeBar; // 게이지 바

    [Header("Station State")]
    public List<Food> ingredients = new List<Food>(); // 현재 재료 목록
    protected bool isCooking = false; // 현재 조리 중인지 여부
    protected bool isMiniGameActive = false; // 미니게임 활성화 여부

    [Header("Prefabs")]
    [SerializeField] private GameObject buttonPrefab; // 버튼 프리팹
    [SerializeField] private GameObject framePrefab; // 프레임 프리팹

    [Header("Animator")]
    protected Animator animator; // 애니메이터 제어기

    [Header("Audio Settings")]
    [SerializeField] protected AudioSource audioSource; // 요리 사운드를 재생할 AudioSource
    [SerializeField] private AudioClip cookSound; // 요리 시작 시 재생할 사운드

    [Header("Effects")]
    [SerializeField] protected GameObject cookParticlePrefab; // cookParticle 프리팹
    [SerializeField] private GameObject failParticlePrefab; // failParticle 프리팹
    private GameObject particleInstance = null; // 파티클 오브젝트

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
        cookButton.onClick.AddListener(StartCook); // CookButton의 onClick 이벤트에 Cook 함수 연결

        // RemoveButton 찾기
        removeButton = interactionPanel.Find("RemoveButton")?.GetComponent<Button>();
        if (removeButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        removeButton.onClick.AddListener(ShowSelectionPanel); // RemoveButton의 onClick 이벤트에 ShowSelectionPanel 함수 연결

        // SelectionPanel 찾기
        selectionPanel = interactionMenu.transform.Find("SelectionPanel");
        if (selectionPanel == null)
        {
            Debug.LogError($"SelectionPanel not found in {cookingStationCanvas.name}");
            return;
        }

        // BackButton 찾기
        backButton = selectionPanel.Find("BackButton")?.GetComponent<Button>();
        if (backButton == null)
        {
            Debug.LogError($"BackButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        backButton.onClick.AddListener(HideSelectionPanel); // BackButton의 onClick 이벤트에 HideSelectionPanel 함수 연결


        // VisualMenu 찾기
        visualMenu = cookingStationCanvas.transform.Find("VisualMenu");
        if (visualMenu == null)
        {
            Debug.LogError($"VisualMenu not found in {cookingStationCanvas.name}");
            return;
        }

        // IconPanel 찾기
        iconPanel = visualMenu.transform.Find("IconPanel");
        if (iconPanel == null)
        {
            Debug.LogError($"iconPanel not found in {visualMenu.name}");
            return;
        }

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

        // 초기 상태 설정
        selectionPanel.gameObject.SetActive(false);  // selectionPanel은 어떤 재료를 빼낼지 선택할 때 활성화

        visualMenu.gameObject.SetActive(true); // visualMenu를 처음부터 활성화 해둬야 조리기구 위 아이콘이 보임
        iconPanel.gameObject.SetActive(true); // IconPanel도 초기에 활성화
        gaugeBarPanel.gameObject.SetActive(false); // gaugeBarPanel 초기에 비활성화
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
    /// 메뉴를 숨길 때 선택 패널을 닫습니다.
    /// </summary>
    protected override void HideMenu()
    {
        if (selectionPanel == null) return;

        base.HideMenu();

        selectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 플레이어가 들고 있는 재료를 추가합니다.
    /// </summary>
    public void AddIngredient()
    { 
        Debug.Log("AddIngredient!");
        if (PlayerController.Instance != null && PlayerController.Instance.heldFood != null)
        {
            ingredients.Add((Food)PlayerController.Instance.heldFood); // 재료 추가
            PlayerController.Instance.DropFood(); // 플레이어가 들고 있는 재료 제거
            Debug.Log("Ingredients in the cooking station:");
            foreach (var ingredient in ingredients)
            {
                Debug.Log($"- {ingredient}");
            }
        }

        UpdateAllButtons();
        UpdateIngredientIcons();
    }

    /// <summary>
    /// 지정된 재료를 제거합니다.
    /// </summary>
    /// <param name="ingredient">제거할 재료</param>
    public virtual void RemoveIngredient(Food ingredient)
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

        UpdateAllButtons();
        UpdateIngredientIcons();
    }

    /// <summary>
    /// 재료 선택 패널을 표시합니다. (어떤걸 뺄지 선택)
    /// </summary>
    private void ShowSelectionPanel()
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

    /// <summary>
    /// 재료 선택 패널을 숨깁니다.
    /// </summary>
    private void HideSelectionPanel()
    {
        // SelectionPanel 비활성화, InteractionPanel 활성화
        interactionPanel.gameObject.SetActive(true);
        selectionPanel.gameObject.SetActive(false);
    }

    /// <summary>
    /// 재료를 선택할 수 있는 버튼을 생성합니다.
    /// </summary>
    /// <param name="ingredient">버튼에 표시할 재료</param>
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

    /// <summary>
    /// 재료 아이콘을 업데이트합니다.
    /// </summary>
    protected void UpdateIngredientIcons()
    {
        // 기존 아이콘 제거
        foreach (Transform child in iconPanel)
        {
            Destroy(child.gameObject);
        }

        // 재료별로 Frame과 Icon 생성
        foreach (Food ingredient in ingredients)
        {
            CreateIngredientIcon(ingredient); // Database에 icon 채워지면 적용
            //CreateIngredientId(ingredient);
        }
    }

    /// <summary>
    /// 재료 아이콘을 생성합니다.
    /// </summary>
    /// <param name="ingredient">아이콘을 생성할 재료</param>
    private void CreateIngredientIcon(Food ingredient)
    {
        // Frame Prefab 복제
        GameObject frameObject = Instantiate(framePrefab, iconPanel);

        // Frame 내부에 Icon 생성
        Transform iconTransform = frameObject.transform.Find("Icon");
        if (iconTransform != null)
        {
            Image iconImage = iconTransform.GetComponent<Image>();
            if (iconImage != null)
            {
                // FoodDatabase에서 해당 재료의 아이콘 정보 가져오기
                FoodData foodData = foodDatabase.foodData.Find(data => data.food == ingredient);
                if (foodData != null && foodData.icon != null)
                {
                    // Texture를 Sprite로 변환
                    Sprite iconSprite = ConvertTextureToSprite(foodData.icon);
                    if (iconSprite != null)
                    {
                        iconImage.sprite = iconSprite;
                    }
                    else
                    {
                        Debug.LogWarning($"Failed to convert texture to sprite for ingredient {ingredient}");
                    }
                }
                else
                {
                    Debug.LogWarning($"Icon not found for ingredient {ingredient}");
                }
            }
            else
            {
                Debug.LogWarning("Icon Image component not found in Frame Prefab.");
            }
        }
        else
        {
            Debug.LogWarning("Icon Transform not found in Frame Prefab.");
        }

        // Frame 이름 설정 (디버그용)
        frameObject.name = $"Frame_{ingredient}";
    }

    /// <summary>
    /// Texture를 Sprite로 변환합니다.
    /// </summary>
    /// <param name="texture">변환할 Texture</param>
    /// <returns>변환된 Sprite</returns>
    private Sprite ConvertTextureToSprite(Texture texture)
    {
        if (texture is Texture2D texture2D)
        {
            // Texture2D를 Sprite로 변환
            return Sprite.Create(
                texture2D,
                new Rect(0, 0, texture2D.width, texture2D.height),
                new Vector2(0.5f, 0.5f)
            );
        }
        return null;
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

        // 요리 사운드 재생
        if (audioSource != null && cookSound != null)
        {
            audioSource.clip = cookSound;
            audioSource.loop = true; // 필요 시 루프 설정
            audioSource.Play(); // 사운드 재생
        }

        if (cookParticlePrefab != null)
        {
            particleInstance = Instantiate(cookParticlePrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("cookParticlePrefab is not assigned!");
        }

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
    protected virtual void CompleteCook(bool isMiniGameSuccess)
    {   
        // UI 상태 복원
        gaugeBarPanel.gameObject.SetActive(false);
        iconPanel.gameObject.SetActive(true);

        if (isMiniGameSuccess)
        {
            if (cookParticlePrefab != null)
            {   
                Destroy(particleInstance);
                particleInstance = Instantiate(cookParticlePrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("CookParticlePrefab is not assigned!");
            }
        }
        else 
        {
            // 요리 실패시 겅은 연기 파티클 생성
            if (failParticlePrefab != null)
            {   
                Destroy(particleInstance);
                particleInstance = Instantiate(failParticlePrefab, transform.position, Quaternion.identity);
            }
            else
            {   
                Debug.LogWarning("FailParticlePrefab is not assigned!");
            }
        }

        StartCoroutine(CompleteCookWithDelay(isMiniGameSuccess));
    }

    /// <summary>
    /// 요리 완료 후 지연을 처리하는 코루틴입니다.
    /// </summary>
    /// <param name="isMiniGameSuccess">미니게임 성공 여부</param>
    /// <returns></returns>
    protected IEnumerator CompleteCookWithDelay(bool isMiniGameSuccess)
    {   
        // Player 활성화
        PlayerController.Instance.SetMovementEnabled(true);

        // 3초 대기
        yield return new WaitForSeconds(3f); // 3초 정도 대기

        Food resultFood = Food.실패요리;
        // Recipe.Execute를 활용하여 요리 결과 확인
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
            for (int i = 0; i < 3; i++)
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

        // 오디오 종료;
        audioSource.Stop();

        // 파티클 제거
        Destroy(particleInstance);

        // 조리 후 버튼 상태 업데이트
        UpdateAllButtons();
        // 조리 후 아이콘 상태 업데이트
        UpdateIngredientIcons();
    }
}
