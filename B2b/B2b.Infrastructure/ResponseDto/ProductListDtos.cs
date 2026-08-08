using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B2b.Infrastructure.ResponseDto
{
    public class ProductListDtos
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int CategoryId { get; set; }
        public int CurrencyId { get; set; }
        public int Vat { get; set; }
        public int StockQuantity { get; set; }
        public string ProductCode { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public string CategoryName { get; set; }
        public string BrandName { get; set; }
        public string CurrencyName { get; set; }
        public string ImageUrl { get; set; }
        public int UnitTypeId { get; set; }
        public string UnitTypeName { get; set; }
        public bool IsDefault { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }

    }
}
