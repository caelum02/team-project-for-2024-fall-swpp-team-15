/// <summary>
/// 손님의 주문 정보 클래스
/// </summary>
[System.Serializable]
public class Order
{
    /// <summary>
    /// 주문한 손님 이름
    /// </summary>
    public string customerName;

    /// <summary>
    /// 주문된 요리 데이터
    /// </summary>
    public FoodData dish;

    /// <summary>
    /// Order 클래스 생성자
    /// </summary>
    /// <param name="customerName">주문한 손님의 이름</param>
    /// <param name="dish">주문된 요리 데이터</param>
    public Order(string customerName, FoodData dish)
    {
        this.customerName = customerName;
        this.dish = dish;
    }
}