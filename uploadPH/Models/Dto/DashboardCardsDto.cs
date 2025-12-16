namespace Pharmacy.Dtos
{
    public class DashboardCardsDto
    {
        public int NearExpirationStorageAmount { get; set; }
        public decimal MonthlyIncome { get; set; }
        public decimal AnnualIncome { get; set; }
        public int TotalDrugsInStorage { get; set; }
    }
}
