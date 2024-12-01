[System.Serializable]
public class Order
{
    public string customerName;
    public FoodData dish;

    public Order(string customerName, FoodData dish)
    {
        this.customerName = customerName;
        this.dish = dish;
    }
}