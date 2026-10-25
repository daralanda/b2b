using B2b.Plugin.Dto;
using OfficeOpenXml;
using System.Globalization;

namespace B2b.Plugin.EPPlus
{
    public class EPPLusPlugin
    {
        private const int ProductSheetColumnCount = 15;

        // ---- Yardımcılar ------------------------------------------------------

        private static string GetText(object value)
        {
            if (value == null) return null;
            var text = Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim();
            return string.IsNullOrEmpty(text) ? null : text;
        }

        private static bool TryGetDecimal(object value, out decimal result)
        {
            result = 0;
            if (value == null) return false;

            switch (value)
            {
                case double d: result = (decimal)d; return true;
                case decimal m: result = m; return true;
                case int i: result = i; return true;
                case long l: result = l; return true;
                case float f: result = (decimal)f; return true;
            }

            var text = GetText(value);
            if (text == null) return false;

            // Metin hücrelerde virgül varsa Türkçe biçim (1.234,56), yoksa nokta ondalık ayırıcıdır.
            var culture = text.Contains(',') ? CultureInfo.GetCultureInfo("tr-TR") : CultureInfo.InvariantCulture;
            return decimal.TryParse(text, NumberStyles.Number, culture, out result);
        }

        private static bool TryGetInt(object value, out int result)
        {
            result = 0;
            if (!TryGetDecimal(value, out var d)) return false;
            if (d != Math.Truncate(d) || d < int.MinValue || d > int.MaxValue) return false;
            result = (int)d;
            return true;
        }

        private static bool IsRowEmpty(ExcelWorksheet sheet, int row, int columnCount)
        {
            for (int col = 1; col <= columnCount; col++)
            {
                if (GetText(sheet.Cells[row, col].Value) != null) return false;
            }
            return true;
        }

        /// <summary>
        /// Resim hücresini okur: hücre köprü (hyperlink) ise gerçek adresi, değilse metni alır.
        /// Boşluk/satır sonu temizlenir, "www." ve "//" ile başlayan adreslere https:// eklenir.
        /// Yalnızca http(s) adresleri ve "/" ile başlayan site içi yollar geçerlidir.
        /// </summary>
        private static string ReadImageUrl(ExcelRange cell, out string warning)
        {
            warning = null;
            string raw = null;

            if (cell.Hyperlink != null)
                raw = cell.Hyperlink.OriginalString;
            if (string.IsNullOrWhiteSpace(raw))
                raw = GetText(cell.Value);
            if (string.IsNullOrWhiteSpace(raw))
            {
                // =HYPERLINK("adres";"metin") formülü
                var formula = cell.Formula;
                if (!string.IsNullOrWhiteSpace(formula) && formula.StartsWith("HYPERLINK", StringComparison.OrdinalIgnoreCase))
                {
                    var start = formula.IndexOf('"');
                    var end = start >= 0 ? formula.IndexOf('"', start + 1) : -1;
                    if (start >= 0 && end > start) raw = formula.Substring(start + 1, end - start - 1);
                }
            }
            if (string.IsNullOrWhiteSpace(raw)) return null;

            var url = raw.Trim().Trim((char)34, (char)39, (char)60, (char)62).Replace("\r", "").Replace("\n", "").Replace(" ", "%20");
            if (url.StartsWith("//")) url = "https:" + url;
            else if (url.StartsWith("www.", StringComparison.OrdinalIgnoreCase)) url = "https://" + url;

            if (url.StartsWith("/")) return url;   // site içi yol (ör. /uploads/a.jpg)

            if (Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                return uri.ToString();

            warning = $"Resim adresi geçersiz, resim eklenmedi: '{raw.Trim()}'.";
            return null;
        }

        private static EPPlusDto Failed(int row, string message) => new EPPlusDto
        {
            RowNumber = row,
            Result = new ResultDto { State = false, Message = message }
        };

        // ---- Ürün yükleme (Stok.xlsx şablonu) ----------------------------------
        // Sütunlar: 1 Ürün Kodu, 2 Ürün Adı, 3 Kategori, 4 Marka, 5 Açıklama, 6 Birim, 7 Fiyat,
        // 8 Döviz, 9 Vergi Oranı, 10 Stok Adeti, 11 Varsayılan, 12 Birim Adeti, 13 Resim, 14 Aktif, 15 Barkod

        public List<EPPlusDto> ExcelUpload(Stream stream)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var result = new List<EPPlusDto>();
            try
            {
                if (stream.CanSeek) stream.Position = 0;

                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets["Ürünler"] ?? package.Workbook.Worksheets.FirstOrDefault();

                if (worksheet?.Dimension == null)
                {
                    result.Add(Failed(0, "Excel dosyasında okunacak sayfa veya veri bulunamadı."));
                    return result;
                }

                int totalRows = worksheet.Dimension.End.Row;
                for (int row = 2; row <= totalRows; row++)
                {
                    // Biçimlendirilmiş ama boş satırlar (ör. 10.000 satıra uzayan şablon) atlanır.
                    if (IsRowEmpty(worksheet, row, ProductSheetColumnCount)) continue;

                    var errors = new List<string>();

                    string code = GetText(worksheet.Cells[row, 1].Value);
                    string name = GetText(worksheet.Cells[row, 2].Value);
                    string category = GetText(worksheet.Cells[row, 3].Value);
                    string brand = GetText(worksheet.Cells[row, 4].Value);
                    string unit = GetText(worksheet.Cells[row, 6].Value);
                    string currency = GetText(worksheet.Cells[row, 8].Value);

                    if (code == null) errors.Add("Ürün Kodu boş olamaz.");
                    if (name == null) errors.Add("Ürün Adı boş olamaz.");
                    if (category == null) errors.Add("Kategori Adı boş olamaz.");
                    if (brand == null) errors.Add("Marka Adı boş olamaz.");
                    if (unit == null) errors.Add("Birim boş olamaz.");
                    if (currency == null) errors.Add("Döviz boş olamaz.");

                    decimal price = 0;
                    var priceCell = worksheet.Cells[row, 7].Value;
                    if (GetText(priceCell) == null) errors.Add("Fiyat boş olamaz.");
                    else if (!TryGetDecimal(priceCell, out price)) errors.Add("Fiyat sayı olmalı.");
                    else if (price <= 0) errors.Add("Fiyat 0'dan büyük olmalı.");

                    int vat = 0;
                    var vatCell = worksheet.Cells[row, 9].Value;
                    if (GetText(vatCell) != null && !TryGetInt(vatCell, out vat)) errors.Add("Vergi Oranı tam sayı olmalı.");

                    int stock = 0;
                    var stockCell = worksheet.Cells[row, 10].Value;
                    if (GetText(stockCell) != null && !TryGetInt(stockCell, out stock)) errors.Add("Stok Adeti tam sayı olmalı.");

                    int unitCount = 1;
                    var unitCountCell = worksheet.Cells[row, 12].Value;
                    if (GetText(unitCountCell) != null && (!TryGetInt(unitCountCell, out unitCount) || unitCount <= 0))
                        errors.Add("Birim adeti 0'dan büyük tam sayı olmalı.");

                    var imageUrl = ReadImageUrl(worksheet.Cells[row, 13], out var imageWarning);

                    result.Add(new EPPlusDto
                    {
                        RowNumber = row,
                        ProductCode = code,
                        ProductName = name,
                        CategoryName = category,
                        BrandName = brand,
                        Description = GetText(worksheet.Cells[row, 5].Value),
                        UnitTypeName = unit,
                        Price = price,
                        CurrencyName = currency,
                        Vat = vat,
                        StockQuantity = stock,
                        IsDefault = GetText(worksheet.Cells[row, 11].Value),   // boşsa serviste "Evet" kabul edilir
                        UnitTypeCount = unitCount,
                        ImageUrl = imageUrl,
                        Warning = imageWarning,
                        IsActive = GetText(worksheet.Cells[row, 14].Value),    // boşsa serviste "Evet" kabul edilir
                        Barcode = GetText(worksheet.Cells[row, 15].Value),
                        Result = new ResultDto
                        {
                            State = errors.Count == 0,
                            Message = string.Join(" ", errors)
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // Hata artık yutulmuyor: kullanıcıya tek satırlık hata olarak döner.
                result.Add(Failed(0, "Excel dosyası okunamadı: " + ex.Message));
            }
            return result;
        }

        // ---- Şablon oluşturma -------------------------------------------------

        public bool CreateTemplate<T>(List<T> data, string Template, string Url)
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add(Template);

                    // 'true' parametresi property isimlerini otomatik başlık yapar
                    worksheet.Cells["A1"].LoadFromCollection(data, true, OfficeOpenXml.Table.TableStyles.Medium9);

                    // İlk 3 kolonu kilitle (Freeze Panes)
                    worksheet.View.FreezePanes(2, 4);

                    // Hücreleri otomatik genişlet (veri yoksa Dimension null olabilir)
                    if (worksheet.Dimension != null)
                    {
                        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();
                    }
                    package.SaveAs(Url);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // ---- Genel okuma (başlık adı == property adı) ---------------------------

        /// <summary>
        /// İlk sayfayı okur. Boş satırlar da listeye eklenir; böylece liste indeksi + 2 = Excel satır numarasıdır.
        /// Dönüştürülemeyen hücreler 'errors' listesine yazılır. Dosya okunamazsa exception fırlatır.
        /// </summary>
        public List<T> ReadData<T>(Stream stream, List<string> errors = null) where T : new()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            var list = new List<T>();
            if (stream.CanSeek) stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets.FirstOrDefault();
            if (worksheet?.Dimension == null) return list;

            int rowCount = worksheet.Dimension.Rows;
            int colCount = worksheet.Dimension.Columns;
            if (rowCount < 2) return list;

            var properties = typeof(T).GetProperties().Where(p => p.CanWrite).ToList();
            var headerMap = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            for (int col = 1; col <= colCount; col++)
            {
                var headerText = GetText(worksheet.Cells[1, col].Value);
                if (headerText != null && !headerMap.ContainsKey(headerText)) headerMap[headerText] = col;
            }

            for (int row = 2; row <= rowCount; row++)
            {
                var obj = new T();
                foreach (var prop in properties)
                {
                    if (!headerMap.TryGetValue(prop.Name, out int colIndex)) continue;

                    var cellValue = worksheet.Cells[row, colIndex].Value;
                    if (GetText(cellValue) == null) continue;

                    try
                    {
                        var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                        object converted;
                        if (targetType == typeof(string)) converted = GetText(cellValue);
                        else if (targetType == typeof(decimal))
                            converted = TryGetDecimal(cellValue, out var d) ? d : throw new FormatException("sayı değil");
                        else if (targetType == typeof(int))
                            converted = TryGetInt(cellValue, out var n) ? n : throw new FormatException("tam sayı değil");
                        else converted = Convert.ChangeType(cellValue, targetType, CultureInfo.InvariantCulture);

                        prop.SetValue(obj, converted);
                    }
                    catch (Exception)
                    {
                        errors?.Add($"Satır {row}, '{prop.Name}' sütunu: '{GetText(cellValue)}' değeri okunamadı.");
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}
