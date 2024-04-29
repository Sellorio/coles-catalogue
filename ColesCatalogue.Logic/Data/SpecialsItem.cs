namespace ColesCatalogue.Logic.Data
{
    public class SpecialsItem
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemImageUrl { get; set; }

        public string SpecialDescription { get; set; }

        public decimal NormalPrice { get; set; }
        public decimal CurrentPrice { get; set; }
        public decimal SavingsPercent { get; set; }

        public string NormalPricePerAmountDescription { get; set; }
        public string CurrentPricePerAmountDescription { get; set; }
    }
}
