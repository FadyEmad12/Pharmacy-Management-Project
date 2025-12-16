namespace Pharmacy.Models.Dto
{
    public class DrugCreated
    {
        public bool Success { get; set; }
        public string? Msg { get; set; }
        public int DrugId { get; set; }
        public string? ImagePath { get; set; }
        public string? ErrorMessage { get; set; }



    }
}