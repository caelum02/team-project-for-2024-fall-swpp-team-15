[System.Serializable]
public class Order
{
    public string customerName;
    public string dishName;

    public Order(string customerName, string dishName)
    {
        this.customerName = customerName;
        this.dishName = dishName;
    }
}