using System.ComponentModel.DataAnnotations;

namespace B2b.Dal.Entity
{
    public class Order
    {
        [Key]
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public DateTime OrderDate { get; set; }
        public int OrderStatus { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public string OrderNote { get; set; }
        public int PaymentType { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }

    }
}
