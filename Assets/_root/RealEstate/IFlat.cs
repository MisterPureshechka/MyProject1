namespace _root.RealEstate
{
    public interface IFlat
    {
        string ID { get; set; }
        string Description { get; set; }
        int MonthPayment { get; set; }
        int DayToPay { get; set; }
    }
}