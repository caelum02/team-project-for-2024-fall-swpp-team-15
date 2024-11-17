using UnityEngine;

public interface IBuyable
{
    // 가격 버튼 클릭 시 
    void OnClickPrice();

    // 구매하시겠습니까? 창에서 네 버튼 클릭 시 
    void OnClickYes();

    // 구매하시겠습니까? 창에서 아니오 버튼 클릭 시 
    void OnClickNo();
}