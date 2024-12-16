using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Yogaewonsil.Common;

public class KitchenTable : TableBase
{   
    protected Button removeButton; // 재료를 제거하는 버튼
    protected Transform visualMenu; // 시각적 메뉴
    protected Transform iconPanel; // 재료 아이콘 패널
    [SerializeField] private GameObject framePrefab; // 프레임 프리팹

    [Header("Additional Audio")]
    [SerializeField] private AudioClip pickSound; // 음식을 잡을 때 재생할 사운드

    /// <summary>
    /// 초기화 메서드. 버튼과 패널을 설정합니다.
    /// </summary>
    protected virtual void Start()
    {   
        base.Start();

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

        // RemoveButton 찾기
        removeButton = interactionPanel.Find("RemoveButton")?.GetComponent<Button>();
        if (removeButton == null)
        {
            Debug.LogError($"RemoveButton not found in InteractionPanel of {gameObject.name}");
            return;
        }
        removeButton.onClick.AddListener(removePlate); // RemoveButton의 onClick 이벤트에 ShowSelectionPanel 함수 연결

        removeButton.gameObject.SetActive(false);

        visualMenu.gameObject.SetActive(true); // visualMenu를 처음부터 활성화 해둬야 조리기구 위 아이콘이 보임
        iconPanel.gameObject.SetActive(true); // IconPanel도 초기에 활성화
    }

    /// <summary>
    /// 버튼 활성화 상태를 업데이트합니다.
    /// </summary>
    protected override void UpdateAllButtons()  // private일지 protected일지 고려 -> 조리대에서 버튼 하나 추가되면 바뀔 수 있을 듯
    {
        Food? heldFood = PlayerController.Instance.GetHeldFood();

        putButton.interactable = plateFood == null && heldFood != null; // 플레이어가 손에 음식이 있고 테이블에 음식이 없어야 버튼 활성화
        
        // RemoveButton은 재료가 있고 플레이어가 아무것도 들고 있지 않을 때 활성화
        removeButton.interactable = plateFood != null && heldFood == null;
    }

    protected override void PutDish()
    {
        base.PutDish();
        UpdateIngredientIcon();

        putButton.gameObject.SetActive(false);
        removeButton.gameObject.SetActive(true);
    }

    private void removePlate()
    {   
        // 테이블에서 음식을 제거하고 PlayerController에 음식 추가
        if(plateFood != null && currentPlateObject != null && PlayerController.Instance != null)
        {
            if (PlayerController.Instance.PickUpFood(plateFood)){
                Debug.Log($"Player picked up: {plateFood}"); // 플레이어가 재료를 집음
                plateFood = null;
                Destroy(currentPlateObject); // 위에 있는 객체 삭제
                currentPlateObject = null;
                Debug.Log($"Removed ingredient: {plateFood}");

                if (audioSource != null && pickSound != null)
                {
                    audioSource.clip = pickSound;
                    audioSource.loop = false; // 필요 시 루프 설정
                    audioSource.Play(); // 사운드 재생
                }
            } 
            else {
                Debug.Log($"Player already hold: {PlayerController.Instance.GetHeldFood()}"); // 플레이어가 이미 집고 있는 재료가 있음
            }
        }
        else
        {
            Debug.LogWarning("PlayerController instance not found!");
        }

        UpdateAllButtons();
        UpdateIngredientIcon();

        putButton.gameObject.SetActive(true);
        removeButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// 재료 아이콘을 업데이트합니다.
    /// </summary>
    protected void UpdateIngredientIcon()
    {
        // 기존 아이콘 제거
        foreach (Transform child in iconPanel)
        {
            Destroy(child.gameObject);
        }

        if (plateFood != null)
        {
          CreateIngredientIcon((Food)plateFood);
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
}
