namespace Pharmacy.Models.Dto
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Invoice { get; set; }
    }
}
