namespace ClinicProApp.Models
{
    public class InvoiceItem
    {
        public string Description { get; set; }
        public double Price { get; set; }
        public int Qty { get; set; }
        public double Total => Price * Qty;
    }
}
