namespace _root.RealEstate
{
    public class Flat : IFlat
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public int MonthPayment { get; set; }
        public int DayToPay { get; set; }
    }
}