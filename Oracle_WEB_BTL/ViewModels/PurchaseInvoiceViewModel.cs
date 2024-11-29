using Oracle_WEB_BTL.Models;

namespace Oracle_WEB_BTL.ViewModels
{
    public class PurchaseInvoiceViewModel
    {
        public Hoadonnhap Invoice { get; set; } 
        public List<Chitiethoadonnhap> InvoiceDetails { get; set; } 
    }
}
