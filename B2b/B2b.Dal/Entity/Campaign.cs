using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Campaign
    {
        [Key]
        public int CampaignId { get; set; }
        public string CampaignName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
        public decimal DiscountValue { get; set; }
        public bool IsPercentage { get; set; }
        public int CategoryId { get; set; }
    }
}
