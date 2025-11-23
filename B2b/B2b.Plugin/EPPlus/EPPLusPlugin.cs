using B2b.Plugin.Dto;
using OfficeOpenXml;

namespace B2b.Plugin.EPPlus
{
    public  class EPPLusPlugin
    {

        public List<EPPlusDto> ExcelUpload(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var result = new List<EPPlusDto>();
            try
            {
                stream.Position = 0;

                using (var package = new ExcelPackage(stream))
                {
                    foreach (var sheet in package.Workbook.Worksheets)
                    {
                        string pageName = sheet.Name;
                    }
                    var worksheet = package.Workbook.Worksheets["Ürünler"];

                    if (worksheet.Dimension != null)
                    {
                        int totalRows = worksheet.Dimension.End.Row;
                        for (int row = 2; row <= totalRows; row++)
                        {
                            string message = string.Empty;

                            if (worksheet.Cells[row, 1].Value == null)
                            {
                                message += "Ürün Kodu boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 2].Value == null)
                            {
                                message += "ürün Adı boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 3].Value == null)
                            {
                                message += "Kategori Adı boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 4].Value == null)
                            {
                                message += "Marka Adı boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 6].Value == null)
                            {
                                message += "Birim boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 7].Value == null)
                            {
                                message += "Fiyat boş olamaz. ";
                            }
                            if (worksheet.Cells[row, 8].Value == null)
                            {
                                message += "Döviz boş olamaz. ";
                            }
                            result.Add(new EPPlusDto
                            {
                                ProductCode = worksheet.Cells[row, 1].Value?.ToString(),
                                ProductName = worksheet.Cells[row, 2].Value?.ToString(),
                                CategoryName = worksheet.Cells[row, 3].Value?.ToString(),
                                BrandName = worksheet.Cells[row, 4].Value?.ToString(),
                                Description = worksheet.Cells[row, 5].Value?.ToString(),
                                UnitTypeName = worksheet.Cells[row, 6].Value?.ToString(),
                                Price = Convert.ToDecimal(worksheet.Cells[row, 7].Value?.ToString()),
                                CurrencyName = worksheet.Cells[row, 8].Value?.ToString(),
                                Vat = int.Parse(worksheet.Cells[row, 9].Value?.ToString()),
                                StockQuantity = int.Parse(worksheet.Cells[row, 10].Value?.ToString()),
                                IsDefault = worksheet.Cells[row, 11].Value?.ToString(),
                                Result = new ResultDto
                                {
                                    State = string.IsNullOrEmpty(message),
                                    Message = message
                                }
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                string es = ex.Message;
            }
            return result;
        }
    }
}
