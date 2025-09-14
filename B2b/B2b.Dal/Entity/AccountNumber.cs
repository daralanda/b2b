using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class AccountNumber
    {
        [Key]
        public int AccountNumberId { get; set; }
        public string Logo { get; set; }
        public string Bank { get; set; }
        public string BranchCode { get; set; }
        public string AccountNo { get; set; }
        public string AccountName { get; set; }
        public string Currency { get; set; }
        public string IBAN { get; set; }

    }
}
