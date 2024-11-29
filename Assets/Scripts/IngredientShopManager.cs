using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Yogaewonsil.Common;

/// <summary>
/// 냉장고 재료 구매 UI를 관리하는 클래스입니다.
/// </summary>
public class IngredientShopManager : MonoBehaviour, IBuyable
{
    // 싱글톤 인스턴스
    public static IngredientShopManager Instance { get; private set; }
    // public Fridge fridge; // 냉장고 참조
    public GameObject fridgeScroll; // 재료 스크롤 UI
    public Image buyOrNotScreen; // 구매 확인 창
    public Image boughtScreen; // 구매 완료 창
    private Food? selectedIngredient = Food.두부; // 상점에서 선택된 재료

    /// <summary>
    /// 싱글톤 설정
    /// </summary>
    private void Awake()
    {
        // 싱글톤 설정
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 기존 인스턴스가 있다면 현재 객체 삭제
            return;
        }
        Instance = this;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 재료 가져오기 버튼 클릭 시 (냉장고 "열기"버튼과 연동)
    public void OnClickOpenFridge()
    {
        fridgeScroll.gameObject.SetActive(true);
    }

    // // 냉장고 닫기 버튼 클릭 시 -> 상점을 닫을 시 냉장고가 닫히도록 구현
    // public void OnClickCloseFridge()
    // {
    //     fridgeScroll.gameObject.SetActive(false);
    // }

    // 가격 버튼 클릭 시 
    public void OnClickPrice()
    {   
        //
        // TODO: 선택된 재료가 무엇인지 확인하고 selectedIngredient에 저장
        //

        buyOrNotScreen.gameObject.SetActive(true); // "구매하시겠습니까?" 창이 뜬다
    }

    // 닫기 버튼 클릭 시 
    public void OnClickClose()
    {   
        // 재료상점UI 닫기
        fridgeScroll.gameObject.SetActive(false);
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(false);

        selectedIngredient = null; // 선택된 재료 초기화

        // ToDO 냉장고 문 닫음
        FridgeController.Instance.CloseFridge();
    }

    // 구매하시겠습니까? 창에서 "네" 버튼 클릭 시 
    public void OnClickYes()
    {   
        // 재료상점UI 닫기
        fridgeScroll.gameObject.SetActive(false);
        buyOrNotScreen.gameObject.SetActive(false);
        boughtScreen.gameObject.SetActive(true);
        
        PlayerController.Instance.PickUpFood(selectedIngredient); // 구매가 완료된 재료를 Player에게 전달

        selectedIngredient = null;  // 선택된 재료 초기화

        FridgeController.Instance.CloseFridge(); // 냉장고 문 닫음
    }

    // 구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    public void OnClickNo()
    {   
        selectedIngredient = null;  // 선택된 재료 초기화

        buyOrNotScreen.gameObject.SetActive(false); // "구매하시겠습니까?" 창 닫기
    }

    // 재료 개수 업데이트 -> Legacy: 재료구매하면 그냥 바로 플레이어에게 추가
    // public void UpdateUI(string ingredient)
    // {
    //     GameObject ingredientObject = GameObject.FindWithTag(ingredient);
    //     TextMeshProUGUI text = ingredientObject.GetComponent<TextMeshProUGUI>();
    //     text.text = "x" + fridge.GetIngredientStock(ingredient);
    // }

}
