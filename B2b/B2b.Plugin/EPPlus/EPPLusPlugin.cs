using B2b.Plugin.Dto;
using OfficeOpenXml;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
                                Price = Convert.ToDecimal(worksheet.Cells[row, 7].Value?.ToString() ?? "0"),
                                CurrencyName = worksheet.Cells[row, 8].Value?.ToString(),
                                Vat = int.Parse(worksheet.Cells[row, 9].Value?.ToString() ?? "0"),
                                StockQuantity = int.Parse(worksheet.Cells[row, 10].Value?.ToString() ?? "0"),
                                IsDefault = worksheet.Cells[row, 11].Value?.ToString(),
                                UnitTypeCount = int.Parse(worksheet.Cells[row, 12].Value?.ToString() ?? "0"),
                                ImageUrl = worksheet.Cells[row, 13].Value?.ToString(),

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

        public bool CreateTemplate<T>(List<T> data,string Template,string Url)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add(Template);

                    // 1. Veriyi Dinamik Olarak Yükle
                    // 'true' parametresi property isimlerini otomatik başlık yapar
                    worksheet.Cells["A1"].LoadFromCollection(data, true, OfficeOpenXml.Table.TableStyles.Medium9);

                    // 2. İlk 3 Kolonu Kilitle (Freeze Panes)
                    worksheet.View.FreezePanes(2, 4);

                    // 3. Başlıkları Özelleştirme (Opsiyonel)
                    // Eğer property isimleri yerine özel isimler isterseniz:
                    var properties = typeof(T).GetProperties();
                    for (int i = 0; i < properties.Length; i++)
                    {
                        // Örn: Property ismi 'ProductName' ise bunu 'Ürün Adı' yap gibi logicler kurulabilir
                        // worksheet.Cells[1, i + 1].Value = properties[i].Name.ToUpper();
                    }

                    // 4. Hücreleri Otomatik Genişlet
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    package.SaveAs(Url);
                }
                return true;
            }
            catch (Exception ex)
            {
                string es = ex.Message; 
                return false;
            }

            
        }
        public List<T> ReadData<T>(Stream stream) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var list = new List<T>();
            if (stream.CanSeek) stream.Position = 0;
            try
            {
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0]; // İlk sayfa
                    var rowCount = worksheet.Dimension?.Rows ?? 0;
                    var colCount = worksheet.Dimension?.Columns ?? 0;
                    if (rowCount < 2) return list; // Veri yoksa boş liste dön

                    // 1. Başlık eşleştirmesi (Property Name == Excel Header)
                    var properties = typeof(T).GetProperties();
                    var headerMap = new Dictionary<string, int>();

                    for (int col = 1; col <= colCount; col++)
                    {
                        var headerText = worksheet.Cells[1, col].Value?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(headerText))
                        {
                            headerMap[headerText] = col;
                        }
                    }

                    // 2. Verileri nesnelere map etme
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var obj = new T();
                        foreach (var prop in properties)
                        {
                            if (headerMap.TryGetValue(prop.Name, out int colIndex))
                            {
                                var cellValue = worksheet.Cells[row, colIndex].Value;
                                if (cellValue != null)
                                {
                                    try
                                    {
                                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                        var convertedValue = Convert.ChangeType(cellValue, targetType);
                                        prop.SetValue(obj, convertedValue);
                                    }
                                    catch { /* Tip dönüşüm hataları için loglama yapılabilir */ }
                                }
                            }
                        }
                        list.Add(obj);
                    }
                }
            }
            catch (Exception ex)
            {
                string e=ex.ToString();
            }
            return list;
        }
    }
}
