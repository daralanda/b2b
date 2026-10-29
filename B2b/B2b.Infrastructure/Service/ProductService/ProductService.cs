using B2b.Dal.Context;
using B2b.Dal.Entity;
using B2b.Infrastructure.ResponseDto;
using B2b.Plugin.EPPlus;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;

namespace B2b.Infrastructure.Service.ProductService
{
    public class ProductService(B2bDbContext context):IProductService
    {
        private readonly B2bDbContext _context = context;
        EPPLusPlugin ePPLusPlugin = new EPPLusPlugin();

        bool state = false;
        string message = string.Empty;
        List<Product> products = new();
        Product product = new();

        private static readonly CultureInfo TrCulture = new("tr-TR");

        // Ürün adı ve kodu her zaman Türkçe kurallarla büyük harf tutulur (i → İ, ı → I);
        // baştaki/sondaki boşluklar silinir, aradaki çoklu boşluk teke indirilir
        private static string ToUpperTr(string value) =>
            value == null ? null : Regex.Replace(value.Trim(), @"\s+", " ").ToUpper(TrCulture);

        // Kayıt eşleştirme anahtarı: tüm boşluklar yok sayılır ("ABC 01" == "abc01")
        private static string MatchKey(string value) =>
            value == null ? string.Empty : Regex.Replace(value, @"\s+", "").ToUpper(TrCulture);

        // Boşluk ve harf farkı SQL'de ayıklanamadığı için kodlar bellekte karşılaştırılır
        private Product FindByCode(string code, int excludeProductId)
        {
            var key = MatchKey(code);
            return _context.Products
                .AsNoTracking()
                .Where(p => p.ProductId != excludeProductId)
                .Select(p => new Product { ProductId = p.ProductId, ProductCode = p.ProductCode, ProductName = p.ProductName })
                .AsEnumerable()
                .FirstOrDefault(p => MatchKey(p.ProductCode) == key);
        }

        private static void NormalizeProduct(Product data)
        {
            data.ProductName = ToUpperTr(data.ProductName);
            data.ProductCode = ToUpperTr(data.ProductCode);
        }

        // Ekleme ve güncellemede ortak zorunlu alan kontrolü; hata yoksa null döner
        private string ValidateProduct(Product data)
        {
            if (string.IsNullOrWhiteSpace(data.ProductName)) return "Ürün adı boş olamaz.";
            if (string.IsNullOrWhiteSpace(data.ProductCode)) return "Ürün kodu boş olamaz.";
            if (data.CategoryId <= 0) return "Kategori seçiniz.";
            if (data.BrandId <= 0) return "Marka seçiniz.";
            if (data.CurrencyId <= 0) return "Para birimi seçiniz.";
            if (data.Vat < 0) return "KDV negatif olamaz.";
            if (data.StockQuantity < 0) return "Stok negatif olamaz.";
            return null;
        }

        public ResultDto<Product> Add(Product data)
        {
            NormalizeProduct(data);
            var validationError = ValidateProduct(data);
            if (validationError != null)
                return new ResultDto<Product> { State = false, Message = validationError };
            // Kod ve ad (boşluklar yok sayılarak) kayıtlı bir ürünle aynıysa yeni kayıt açılmaz, o ürün güncellenir
            var existing = FindByCode(data.ProductCode, 0);
            if (existing != null)
            {
                if (MatchKey(existing.ProductName) != MatchKey(data.ProductName))
                    return new ResultDto<Product> { State = false, Message = $"Bu ürün kodu '{existing.ProductName}' adlı üründe kullanılıyor." };

                data.ProductId = existing.ProductId;
                var updateResult = Update(data);
                if (updateResult.State) updateResult.Message = "Ürün zaten kayıtlı olduğu için güncellendi.";
                return updateResult;
            }
            try
            {
                // Adresi boş resim satırları kaydedilmez
                data.ProductImages = data.ProductImages?.Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl)).ToList();
                _context.Products.Add(data);
                _context.SaveChanges();
                state = true;
                message = "Ürün eklendi.";
              
                
            }
            catch (Exception ex)
            {
                message = ex.Message;
                state = false;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<ProductListDtos> GetAll()
        {
            var data = new List<ProductListDtos>();

            try
            {
                // 1. AsNoTracking() ile takip kapatıldı.
                // 2. ToList() en sona alındı; böylece tüm JOIN ve Select işlemleri SQL tarafında tek sorguda biter.
                data = _context.Products
                    .AsNoTracking()
                    .Select(p => new ProductListDtos
                    {
                        // Ana Ürün Bilgileri
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        ProductCode = p.ProductCode,
                        CategoryId = p.CategoryId,
                        BrandId = p.BrandId,
                        CurrencyId = p.CurrencyId,
                        Vat = p.Vat,
                        StockQuantity = p.StockQuantity,
                        Description = p.Description,
                        IsActive = p.IsActive,

                        // İlişkili Tablo İsim Alanları (SQL LEFT JOIN olarak çevrilir)
                        CategoryName = _context.Categories
                            .Where(c => c.CategoryId == p.CategoryId)
                            .Select(c => c.CategoryName)
                            .FirstOrDefault(),

                        BrandName = _context.Brands
                            .Where(b => b.BrandId == p.BrandId)
                            .Select(b => b.BrandName)
                            .FirstOrDefault(),

                        CurrencyName = _context.Currencies
                            .Where(c => c.CurrencyId == p.CurrencyId)
                            .Select(c => c.CurrencyName)
                            .FirstOrDefault(),

                        // İlk Resim Bilgisi (Queue sırasına göre)
                        ImageUrl = _context.ProductImages
                            .Where(img => img.ProductId == p.ProductId && img.ImageUrl != null && img.ImageUrl.Trim() != "")
                            .OrderBy(img => img.Queue)
                            .Select(img => img.ImageUrl)
                            .FirstOrDefault() ?? "/uploads/default.jpg",

                        // Varsayılan Fiyat (IsDefault = true)
                        Barcode = _context.ProductPrices
                            .Where(pr => pr.ProductId == p.ProductId && pr.IsDefault)
                            .Select(pr => pr.Barcode)
                            .FirstOrDefault(),

                        Price = _context.ProductPrices
                            .Where(pr => pr.ProductId == p.ProductId && pr.IsDefault)
                            .Select(pr => (decimal?)pr.Price)
                            .FirstOrDefault() ?? 0,

                        IsDefault = _context.ProductPrices
                            .Where(pr => pr.ProductId == p.ProductId && pr.IsDefault)
                            .Select(pr => (bool?)pr.IsDefault)
                            .FirstOrDefault() ?? false,

                        Count = _context.ProductPrices
                            .Where(pr => pr.ProductId == p.ProductId && pr.IsDefault)
                            .Select(pr => (int?)pr.Count)
                            .FirstOrDefault() ?? 0,

                        // Birim Tipi Bilgileri (Price tablosu üzerinden Join)
                        UnitTypeId = (from pr in _context.ProductPrices
                                      where pr.ProductId == p.ProductId && pr.IsDefault
                                      join u in _context.UnitTypes on pr.UnitTypeId equals u.UnitTypeId
                                      select (int?)u.UnitTypeId).FirstOrDefault() ?? 0,

                        UnitTypeName = (from pr in _context.ProductPrices
                                        where pr.ProductId == p.ProductId && pr.IsDefault
                                        join u in _context.UnitTypes on pr.UnitTypeId equals u.UnitTypeId
                                        select u.UnitTypeName).FirstOrDefault()
                    })
                    .ToList();

                state = true;
                message = "Products retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }

            return new ResultDto<ProductListDtos>
            {
                State = state,
                Message = message,
                List = data
            };
        }
        public ResultDto<Product> GetById(int id)
        {
            try
            {
                product = _context.Products.Where(p => p.ProductId == id).FirstOrDefault();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
                Data = product
            };
        }
        public ResultDto<Product> Remove(int id)
        {

            try
            {
                var del = _context.Products.Where(p => p.ProductId == id).ToList();
                _context.Products.RemoveRange(del);
                _context.SaveChanges();
                state = true;
                message = "Products removed successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message,
            };
        }
        public ResultDto<Product> Update(Product data)
        {
            NormalizeProduct(data);
            var validationError = ValidateProduct(data);
            if (validationError != null)
                return new ResultDto<Product> { State = false, Message = validationError };
            // Kod başka bir üründe kullanılıyorsa güncelleme yapılmaz
            if (FindByCode(data.ProductCode, data.ProductId) != null)
                return new ResultDto<Product> { State = false, Message = "Bu ürün kodu kullanılıyor." };
            try
            {
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
                var upd = _context.Products.Where(p => p.ProductId == data.ProductId).First();
                upd.ProductName = data.ProductName;
                upd.CategoryId = data.CategoryId;
                upd.CurrencyId = data.CurrencyId;
                upd.Vat = data.Vat;
                upd.StockQuantity = data.StockQuantity;
                upd.ProductCode = data.ProductCode;
                upd.BrandId = data.BrandId;
                upd.Description = data.Description;
                upd.IsActive = data.IsActive;
                _context.SaveChanges();
                _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                var img=_context.ProductImages.Where(p => p.ProductId == data.ProductId).ToList();
                var price=_context.ProductPrices.Where(p => p.ProductId == data.ProductId).ToList();
                 _context.ProductImages.RemoveRange(img);
                _context.ProductPrices.RemoveRange(price);
                _context.SaveChanges();

                // Adresi boş resim satırları yeniden kaydedilmez
                foreach (var item in data.ProductImages.Where(i => !string.IsNullOrWhiteSpace(i.ImageUrl)))
                {
                    _context.ProductImages.Add(new ProductImage
                    {
                         ImageUrl=item.ImageUrl,
                         ProductId=upd.ProductId,
                         Queue=item.Queue
                    });
                    _context.SaveChanges();
                }
                foreach (var item in data.ProductPrices)
                {
                    _context.ProductPrices.Add(new ProductPrice
                    {
                        Price = item.Price,
                        UnitTypeId = item.UnitTypeId,
                        ProductId = upd.ProductId,
                        IsDefault = item.IsDefault,
                        Count = item.Count,
                        Barcode=item.Barcode
                        
                    });
                    _context.SaveChanges();
                }
                state = true;
                message = "Ürün güncellendi.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return new ResultDto<Product>
            {
                State = state,
                Message = message
            };
        }

        public ResultDto<ProductPrice> GetPrice(int id)
        {
            var result = new ResultDto<ProductPrice>();
            try
            {
                result.List = _context.ProductPrices.Where(p => p.ProductId == id).ToList();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return result;
        }
        public ResultDto<ProductImage> GetImage(int id)
        {
            var result = new ResultDto<ProductImage>();
            try
            {
                result.List = _context.ProductImages.Where(p => p.ProductId == id && p.ImageUrl != null && p.ImageUrl.Trim() != "").ToList();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return result;
        }

        // ---- Excel yardımcıları -------------------------------------------------

        private static Dictionary<string, T> BuildLookup<T>(IEnumerable<T> items, Func<T, string> key)
        {
            return items
                .Where(i => !string.IsNullOrWhiteSpace(key(i)))
                .GroupBy(i => key(i).Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.First(), StringComparer.OrdinalIgnoreCase);
        }

        private static bool IsYes(string value, bool defaultValue)
        {
            if (string.IsNullOrWhiteSpace(value)) return defaultValue;
            var v = value.Trim();
            return v.Equals("evet", StringComparison.OrdinalIgnoreCase)
                || v.Equals("e", StringComparison.OrdinalIgnoreCase)
                || v.Equals("true", StringComparison.OrdinalIgnoreCase)
                || v == "1";
        }

        private const int MaxReportedMessages = 500;

        private static void AddMessage(List<string> list, string text)
        {
            if (list.Count < MaxReportedMessages) list.Add(text);
            else if (list.Count == MaxReportedMessages) list.Add("... (çok fazla uyarı var, kalanlar gösterilmiyor)");
        }

        public ResultDto<string> ProductAllSet(Stream stream)
        {
            // Güncellenen ürün/fiyatların kaydedilmesi için izleme açık olmalı; işlem sonunda eski değer geri yüklenir.
            var previousTracking = _context.ChangeTracker.QueryTrackingBehavior;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.TrackAll;
            try
            {
                return ImportProducts(stream);
            }
            finally
            {
                _context.ChangeTracker.QueryTrackingBehavior = previousTracking;
            }
        }

        private ResultDto<string> ImportProducts(Stream stream)
        {
            var result = new ResultDto<string> { List = new List<string>() };
            var data = ePPLusPlugin.ExcelUpload(stream);
            if (data.Count == 0)
            {
                result.State = false;
                result.Message = "Excel dosyasında işlenecek satır bulunamadı.";
                return result;
            }

            // Referans tablolar bir kez okunur (eskiden her satırda tüm ürün tablosu yeniden çekiliyordu).
            var brands = BuildLookup(_context.Brands.ToList(), b => b.BrandName);
            var categories = BuildLookup(_context.Categories.ToList(), c => c.CategoryName);
            var unitTypes = BuildLookup(_context.UnitTypes.ToList(), u => u.UnitTypeName);
            var currencies = BuildLookup(_context.Currencies.ToList(), c => c.CurrencyName);
            // Anahtarda boşluk ve harf farkı yok sayılır: "abc 01" kayıtlıysa Excel'deki "ABC01" ile eşleşir
            var products = BuildLookup(_context.Products.ToList(), p => MatchKey(p.ProductCode));
            var existingPrices = _context.ProductPrices.ToList()
                .GroupBy(p => p.ProductId)
                .ToDictionary(g => g.Key, g => g.GroupBy(p => p.UnitTypeId).ToDictionary(u => u.Key, u => u.First()));
            var existingImages = _context.ProductImages.Select(i => new { i.ProductId, i.ImageUrl }).ToList()
                .GroupBy(i => i.ProductId)
                .ToDictionary(g => g.Key, g => new HashSet<string>(g.Select(i => i.ImageUrl)));

            // Fiyat ve resimler ürün nesnesine göre tutulur: yeni ürünlerin ProductId'si kayıttan önce 0 olduğu için
            // her yeni üründe SaveChanges çağırmak gerekmez (binlerce satırda isteği dakikalarca uzatan buydu).
            var priceMaps = new Dictionary<Product, Dictionary<int, ProductPrice>>();
            var imageSets = new Dictionary<Product, HashSet<string>>();
            Dictionary<int, ProductPrice> PricesOf(Product p)
            {
                if (!priceMaps.TryGetValue(p, out var map))
                    priceMaps[p] = map = p.ProductId != 0 && existingPrices.TryGetValue(p.ProductId, out var e) ? e : new Dictionary<int, ProductPrice>();
                return map;
            }
            HashSet<string> ImagesOf(Product p)
            {
                if (!imageSets.TryGetValue(p, out var set))
                    imageSets[p] = set = p.ProductId != 0 && existingImages.TryGetValue(p.ProductId, out var e) ? e : new HashSet<string>();
                return set;
            }

            Brand GetBrand(string name)
            {
                if (!brands.TryGetValue(name, out var brand))
                {
                    brand = new Brand { BrandName = name, Queno = 1, ImageUrl = "" };
                    _context.Brands.Add(brand);
                    _context.SaveChanges();
                    brands[name] = brand;
                }
                return brand;
            }
            Category GetCategory(string name)
            {
                if (!categories.TryGetValue(name, out var category))
                {
                    category = new Category { CategoryName = name, IsActive = true, MainCategoryId = 0 };
                    _context.Categories.Add(category);
                    _context.SaveChanges();
                    categories[name] = category;
                }
                return category;
            }
            UnitType GetUnitType(string name)
            {
                if (!unitTypes.TryGetValue(name, out var unitType))
                {
                    unitType = new UnitType { UnitTypeName = name };
                    _context.UnitTypes.Add(unitType);
                    _context.SaveChanges();
                    unitTypes[name] = unitType;
                }
                return unitType;
            }
            Currency GetCurrency(string name)
            {
                if (!currencies.TryGetValue(name, out var currency))
                {
                    currency = new Currency { CurrencyCode = name, CurrencyName = name };
                    _context.Currencies.Add(currency);
                    _context.SaveChanges();
                    currencies[name] = currency;
                }
                return currency;
            }

            int created = 0, updated = 0, failed = 0, pending = 0;
            bool fatal = false;

            foreach (var item in data)
            {
                if (item.Result == null || !item.Result.State)
                {
                    failed++;
                    AddMessage(result.List, $"Satır {item.RowNumber}: {item.Result?.Message?.Trim()}");
                    continue;
                }

                if (!string.IsNullOrEmpty(item.Warning))
                    AddMessage(result.List, $"Satır {item.RowNumber}: {item.Warning}");

                try
                {
                    var code = ToUpperTr(item.ProductCode);
                    var name = ToUpperTr(item.ProductName);
                    var codeKey = MatchKey(code);

                    // Kod ve ad birlikte tutarsa güncellenir; kod başka adla kayıtlıysa satır atlanır
                    if (products.TryGetValue(codeKey, out var product) && MatchKey(product.ProductName) != MatchKey(name))
                    {
                        failed++;
                        AddMessage(result.List, $"Satır {item.RowNumber}: '{code}' kodu '{product.ProductName}' adlı üründe kullanılıyor, atlandı.");
                        continue;
                    }

                    var brand = GetBrand(item.BrandName.Trim());
                    var category = GetCategory(item.CategoryName.Trim());
                    var unitType = GetUnitType(item.UnitTypeName.Trim());
                    var currency = GetCurrency(item.CurrencyName.Trim());
                    bool isActive = IsYes(item.IsActive, true);
                    bool isDefault = IsYes(item.IsDefault, true);

                    if (product == null)
                    {
                        product = new Product
                        {
                            ProductCode = code,
                            ProductName = name,
                            BrandId = brand.BrandId,
                            CategoryId = category.CategoryId,
                            CurrencyId = currency.CurrencyId,
                            Description = item.Description,
                            Vat = item.Vat,
                            IsActive = isActive,
                            StockQuantity = item.StockQuantity
                        };
                        _context.Products.Add(product);
                        products[codeKey] = product;
                        created++;
                    }
                    else
                    {
                        product.ProductCode = code;
                        product.ProductName = name;
                        product.BrandId = brand.BrandId;
                        product.CategoryId = category.CategoryId;
                        product.CurrencyId = currency.CurrencyId;
                        product.Description = item.Description;
                        product.Vat = item.Vat;
                        product.StockQuantity = item.StockQuantity;
                        product.IsActive = isActive;
                        updated++;
                    }

                    // Yeni (henüz kaydedilmemiş) ürüne navigasyonla, mevcut ürüne ProductId ile bağlanır
                    var newProduct = product.ProductId == 0 ? product : null;
                    var productPrices = PricesOf(product);
                    if (!productPrices.TryGetValue(unitType.UnitTypeId, out var price))
                    {
                        price = new ProductPrice
                        {
                            ProductId = product.ProductId,
                            Product = newProduct,
                            UnitTypeId = unitType.UnitTypeId,
                            Price = item.Price,
                            IsDefault = isDefault,
                            Count = item.UnitTypeCount,
                            Barcode = item.Barcode
                        };
                        _context.ProductPrices.Add(price);
                        productPrices[unitType.UnitTypeId] = price;
                    }
                    else
                    {
                        price.Price = item.Price;
                        price.IsDefault = isDefault;
                        price.Count = item.UnitTypeCount;
                        if (!string.IsNullOrEmpty(item.Barcode)) price.Barcode = item.Barcode;
                    }

                    // Boş resim kaydı eklenmez (eskiden boş ImageUrl'li satır açılıyordu).
                    if (!string.IsNullOrWhiteSpace(item.ImageUrl))
                    {
                        if (ImagesOf(product).Add(item.ImageUrl))
                        {
                            _context.ProductImages.Add(new ProductImage
                            {
                                ProductId = product.ProductId,
                                Product = newProduct,
                                ImageUrl = item.ImageUrl,
                                Queue = 1
                            });
                        }
                    }

                    if (++pending >= 200)
                    {
                        _context.SaveChanges();
                        pending = 0;
                    }
                }
                catch (DbUpdateException ex)
                {
                    // Kayıt hatasında izleyici bozulacağı için işlem durdurulur; önceki kayıtlar yazılmıştır.
                    fatal = true;
                    failed++;
                    AddMessage(result.List, $"Satır {item.RowNumber}: Veritabanına yazılamadı, işlem durduruldu. {ex.InnerException?.Message ?? ex.Message}");
                    break;
                }
                catch (Exception ex)
                {
                    failed++;
                    AddMessage(result.List, $"Satır {item.RowNumber}: {ex.Message}");
                }
            }

            if (!fatal)
            {
                try
                {
                    _context.SaveChanges();
                }
                catch (Exception ex)
                {
                    fatal = true;
                    AddMessage(result.List, "Kayıt sırasında hata: " + (ex.InnerException?.Message ?? ex.Message));
                }
            }

            result.State = !fatal && (created + updated) > 0;
            result.Message = $"{created} ürün eklendi, {updated} ürün güncellendi, {failed} satır hatalı.";
            return result;
        }

        public ResultDto<ProductImage> GetImages()
        {
            var result = new ResultDto<ProductImage>();
            try
            {
                result.List = _context.ProductImages.Distinct().ToList();
                state = true;
                message = "product retrieved successfully.";
            }
            catch (Exception ex)
            {
                state = false;
                message = ex.Message;
            }
            return result;
        }

        public ResultDto<string> GetTemplate(string Url) {
            var result = new ResultDto<string>();
            try
            {
                FormattableString data = $"Exec ProductPriceUpdateTemplate";
                var source = _context.Database.SqlQuery<ProductPriceTemplate>(data);
                bool IsState = ePPLusPlugin.CreateTemplate(source.ToList(), "UrunFiyatGuncelleme", Url);

                result.State = IsState;
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public ResultDto<string> ProductPriceUpdateAll(Stream stream)
        {
            var result = new ResultDto<string> { List = new List<string>() };

            var parseErrors = new List<string>();
            List<ProductPriceTemplate> data;
            try
            {
                data = ePPLusPlugin.ReadData<ProductPriceTemplate>(stream, parseErrors);
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = "Excel dosyası okunamadı: " + ex.Message;
                return result;
            }
            foreach (var e in parseErrors) AddMessage(result.List, e);

            var products = BuildLookup(_context.Products.ToList(), p => MatchKey(p.ProductCode));
            var brands = BuildLookup(_context.Brands.ToList(), b => b.BrandName);
            var categories = BuildLookup(_context.Categories.ToList(), c => c.CategoryName);
            var unitTypes = BuildLookup(_context.UnitTypes.ToList(), u => u.UnitTypeName);
            var prices = _context.ProductPrices.ToList()
                .GroupBy(p => p.ProductId + "|" + p.UnitTypeId)
                .ToDictionary(g => g.Key, g => g.First());

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            int updated = 0, skipped = 0, row = 1;

            foreach (var item in data)
            {
                row++;
                var code = ToUpperTr(item.ProductCode);

                // Tamamen boş satır: sessizce atla
                if (string.IsNullOrEmpty(code) && string.IsNullOrWhiteSpace(item.ProductName) && item.Price == 0) continue;

                if (string.IsNullOrEmpty(code))
                {
                    skipped++;
                    AddMessage(result.List, $"Satır {row}: ProductCode boş, atlandı.");
                    continue;
                }
                if (!products.TryGetValue(MatchKey(code), out var product))
                {
                    skipped++;
                    AddMessage(result.List, $"Satır {row}: '{code}' kodlu ürün bulunamadı, atlandı.");
                    continue;
                }
                if (item.Price <= 0)
                {
                    skipped++;
                    AddMessage(result.List, $"Satır {row}: '{code}' için fiyat 0 veya boş, atlandı.");
                    continue;
                }

                var unitName = item.UnitTypeName?.Trim();
                if (string.IsNullOrEmpty(unitName) || !unitTypes.TryGetValue(unitName, out var unitType))
                {
                    skipped++;
                    AddMessage(result.List, $"Satır {row}: '{code}' için birim '{unitName}' bulunamadı, atlandı.");
                    continue;
                }
                if (!prices.TryGetValue(product.ProductId + "|" + unitType.UnitTypeId, out var price))
                {
                    skipped++;
                    AddMessage(result.List, $"Satır {row}: '{code}' için '{unitName}' biriminde fiyat kaydı yok, atlandı.");
                    continue;
                }

                // Kategori / marka bulunamazsa ürünün mevcut değeri korunur, fiyat yine güncellenir.
                var categoryName = item.CategoryName?.Trim();
                if (!string.IsNullOrEmpty(categoryName))
                {
                    if (categories.TryGetValue(categoryName, out var category)) product.CategoryId = category.CategoryId;
                    else AddMessage(result.List, $"Satır {row}: '{categoryName}' kategorisi bulunamadı, kategori değişmedi.");
                }
                var brandName = item.BrandName?.Trim();
                if (!string.IsNullOrEmpty(brandName))
                {
                    if (brands.TryGetValue(brandName, out var brand)) product.BrandId = brand.BrandId;
                    else AddMessage(result.List, $"Satır {row}: '{brandName}' markası bulunamadı, marka değişmedi.");
                }

                if (!seen.Add(code + "|" + unitName))
                    AddMessage(result.List, $"Satır {row}: '{code}' / '{unitName}' dosyada birden fazla geçiyor, son satırdaki değer geçerli.");

                price.Price = item.Price;
                if (item.Count > 0) price.Count = item.Count;
                if (!string.IsNullOrEmpty(item.Barcode)) price.Barcode = item.Barcode;
                updated++;
            }

            try
            {
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result.State = false;
                result.Message = "Kayıt sırasında hata: " + (ex.InnerException?.Message ?? ex.Message);
                return result;
            }

            result.State = updated > 0;
            result.Message = $"{updated} ürün fiyatı güncellendi, {skipped} satır atlandı.";
            return result;
        }
    }
}
