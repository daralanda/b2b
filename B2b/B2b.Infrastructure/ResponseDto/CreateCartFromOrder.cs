namespace B2b.Infrastructure.ResponseDto
{
    public class CreateCartFromOrder
    {
        public string ResultMessage { get; set; }
        public int OrderId { get; set; }
        public string OrderNo { get; set; }
        public decimal FinalTotal { get; set; }
    }
}
