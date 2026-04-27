using Azure.Core;
using B2b.Infrastructure.Service.CartService;
using B2b.Infrastructure.Service.OrderService;
using B2b.Infrastructure.Service.PaymentService;
using B2b.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace B2b.Web.Controllers
{
    public class EcommerceController(ILogger<EcommerceController> logger,
        IOrderService orderService, ICartService cartService, IPaymentManager payment) : Controller
    {
        private readonly ILogger<EcommerceController> _logger = logger;
        private readonly IOrderService _orderService=orderService;
        private readonly ICartService _cartService=cartService;
        private readonly IPaymentManager _payment = payment;
        #region b2b
        public IActionResult Products()
        {
            return View();
        }
        public IActionResult CampaignProducts()
        {
            return View();
        }
        public IActionResult OrderTrack()
        {
            return View();
        }
        public IActionResult MyCart()
        {
            return View();
        }
        public IActionResult Checkout()
        {
            return View();
        }
        public IActionResult MyProfile()
        {
            return View();
        }
        public IActionResult AccountNumbers()
        {
            return View();
        }
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult PaymentStatus()
        {
            int user = _cartService.FindPaymentIdUser(Request.Form["MERCHANTPAYMENTID"]);
            if (user != 0)
            {
                ViewBag.UserId = user;
                if (Request.Form["ResponseCode"] == "00")
                {
                    _orderService.CreateOrder(new Infrastructure.RequestDto.OrderDto { OrderNote = "", PaymentType = 1 }, user);
                }
                else
                {
                    string msg = GetErrorMessage(Request.Form["ResponseCode"]);
                    ViewBag.CustomMessage = msg;
                }
            }
            else {
                var formData = Request.Form.ToDictionary(x => x.Key, x => x.Value.ToString());
                _payment.ManuelPayment(formData);
                string msg = GetErrorMessage(Request.Form["ResponseCode"]);
                ViewBag.CustomMessage = msg;
                ViewBag.State = Request.Form["ResponseCode"] == "00" ? true : false;
                ViewBag.PostUrl = "/online-odeme";
                ViewBag.UserId = 0;
            }
            return View(Request.Form);
        }

        public IActionResult PaymentManuel()
        {
              return View();
        }

        public string GetErrorMessage(string errorCode)
        {
            var errors = new Dictionary<string, string>
            {
                {"ERR10010", "İstekte zorunlu parametrelerden biri bulunamadı"},
                {"ERR10011", "Aynı parametre bir defadan fazla olarak gönderilmiş"},
                {"ERR10012", "Bu değer için azami büyüklük değeri aşıldı."},
                {"ERR10013", "Bu değer için geçersiz veri tipi belirtilmiş"},
                {"ERR10014", "Geçersiz güvenlik algoritması belirtilmiş"},
                {"ERR10015", "Geçersiz üye iş yeri bilgisi belirtilmiş"},
                {"ERR10016", "Geçersiz tutar bilgisi belirtilmiş"},
                {"ERR10017", "Geçersiz para birimi belirtilmiş"},
                {"ERR10018", "Geçersiz dil seçimi"},
                {"ERR10019", "Genel hata"},
                {"ERR10020", "Geçersiz kullanıcı bilgileri"},
                {"ERR10021", "Boş parametre belirtilmiş, tüm parametreleri kontrol edin"},
                {"ERR10022", "Sipariş edilen ürünlerin toplam tutarı gerçek tutarla örtüşmüyor"},
                {"ERR10023", "Ödeme tutarı hesaplanan tutarla örtüşmüyor"},
                {"ERR10024", "Geçersiz vergi tutarı belirtilmiş"},
                {"ERR10025", "Belirtilen durumda vergi tutarı sıfır olmalıdır"},
                {"ERR10026", "Geçersiz entegrasyon modeli belirtilmiş"},
                {"ERR10027", "Geçersiz kart bilgisi (TOKEN) belirtilmiş"},
                {"ERR10028", "Belirtilen ödeme sistemi (sanal POS) bulunamadı"},
                {"ERR10029", "Belirtilen ödeme tipi (kampanya) bulunamadı"},
                {"ERR10030", "Belirtilen işlem bulunamadı"},
                {"ERR10031", "Bu işlem iade edilemez"},
                {"ERR10032", "Geçersiz iade tutarı belirtilmiş ya da bu işlem daha önce iade edilmiş"},
                {"ERR10033", "Bu işlem iptal edilemez"},
                {"ERR10034", "Belirtilen ödeme bulunamadı"},
                {"ERR10035", "Bu işlem için ön otorizasyon kaydı bulunmamaktadır"},
                {"ERR10036", "Geçersiz son otorizasyon (POSTAUTH) tutarı belirtilmiş"},
                {"ERR10037", "Belirtilen Kart Sahibi (Müşteri) kayıtlı değil"},
                {"ERR10038", "İlgili ödeme onay beklemektedir"},
                {"ERR10039", "Geçersiz ödeme durumu belirtilmiş"},
                {"ERR10040", "Geçersiz alt işlem (SUBACTION) belirtilmiş"},
                {"ERR10041", "Belirtilen kart daha önce eklenmiş"},
                {"ERR10042", "Kart daha önceden silinmiş"},
                {"ERR10043", "Geçersiz zaman aralığı belirtilmiş"},
                {"ERR10044", "Geçersiz tarih formatı belirtilmiş"},
                {"ERR10045", "Belirtilen kart numarası geçersizdir"},
                {"ERR10046", "Belirtilen kredi kartı geçerlilik tarihi geçersizdir"},
                {"ERR10047", "Kullanıcının API servislerini kullanma yetkisi bulunmamaktadır"},
                {"ERR10048", "Başarılı Bir İşlem zaten bu üye iş yeri sipariş numarası ile var"},
                {"ERR10049", "Geçersiz üye iş yeri grup numarası"},
                {"ERR10050", "Geçersiz HASH değeri"},
                {"ERR10051", "Herhangi bir ödeme sistemi (sanal pos) tanımı yok. Lütfen, kontrol ediniz."},
                {"ERR10052", "Desteklenmeyen para birimi:"},
                {"ERR10053", "Kullanıcının bu üye iş yeri üzerinde işlem yapma yetkisi yok"},
                {"ERR10054", "Ödeme geçerlilik süresi maksimum limitin üstündedir."},
                {"ERR10055", "Ödeme geçerlilik süresi minimum limitin altındadır."},
                {"ERR10056", "Geçersiz API isteği belirtilmiş"},
                {"ERR10057", "Kart BIN bilgisi geçersiz"},
                {"ERR10058", "Kart daha önce etkinleştirilmiş"},
                {"ERR10059", "Kart daha önce kullanım dışı bırakılmış"},
                {"ERR10060", "Geçersiz IP Adresi"},
                {"ERR10062", "Belirtilen kart henüz aktive edilmemiştir."},
                {"ERR10063", "Bu işlem sadece LetsBodrum kart ile yapılabilir."},
                {"ERR10064", "Lütfen LetsBodrum kart veya Türkiye İş Bankası kredi kartı kullanınız."},
                {"ERR10065", "Belirtilen kart numarası daha önceden tanımlanmış."},
                {"ERR10066", "Belirtilen zaman bilgisi geçersiz ya da tutarsızdır"},
                {"ERR10067", "Belirtilen period değeri çok yüksek"},
                {"ERR10068", "Geçersiz tekrar düzeni parametresi"},
                {"ERR10069", "Zamanlayıcı (Quartz) hatası oluştu"},
                {"ERR10070", "Başlangı tarihi gelecekteki bir tarih olmalıdır"},
                {"ERR10071", "Geçersiz tekrarlı ödeme durum parametresi belirtilmiş"},
                {"ERR10072", "Tekrarlı ödeme planı zaten etkin durumda"},
                {"ERR10074", "Tekrarlı ödeme planının zaten süresi geçmiş"},
                {"ERR10075", "Üye iş yeri görsel (logo) bilgisi hatalı"},
                {"ERR10076", "Geçersiz tekrarlı ödeme durum parametresi"},
                {"ERR10078", "İşlem kilitlidir"},
                {"ERR10079", "Bu kart sistemde kayıtlıdır."},
                {"ERR10080", "Lütfen, Üye İş Yeri Sipariş numarasını veya Ödeme Oturumu(Token) veriniz"},
                {"ERR10081", "Geçersiz işlem durumu"},
                {"ERR10082", "Kullanıcının bu işlem için yetkisi yoktur."},
                {"ERR10083", "Geçersiz statü"},
                {"ERR10084", "Faiz veya indirim oranı sıfır değeri olmalıdır"},
                {"ERR10085", "Geçerli bitiş tarihi ,geçerli başlangıç tarihten daha büyük olamaz"},
                {"ERR10086", "Geçerli bitiş tarihi şimdiki tarihten daha büyük olmalıdır"},
                {"ERR10087", "Taksit sayı numarası zaten bu ödeme sistemi ile bir ödeme tipi var"},
                {"ERR10088", "Taksit bilgisi 1-12 arasında bir değer olmalıdır."},
                {"ERR10089", "Tekrarlı ödemeye ait kart silinemez."},
                {"ERR10090", "İşlem başarısız"},
                {"ERR10091", "Ödeme sistemi devre dışı bırakıldığı için işlem gerçekleştiremiyor. Lütfen Üye İş Yeri Süper Yöneticisiyle iletişime geçiniz."},
                {"ERR10092", "Geçersiz offset değeri"},
                {"ERR10093", "Geçersiz limit değeri"},
                {"ERR10094", "Tanımlı bir kart bulunamadı."},
                {"ERR10095", "Kayıtlı bulunan tekrarlayan ödeme planlarından dolayı kart silinemez."},
                {"ERR10096", "Geçersiz oturum (session) bilgisi."},
                {"ERR10097", "Sonlandırılmış oturum (session) bilgisi."},
                {"ERR10098", "Bu oturum anahtarının yapılmak istenen işleme yetkisi yoktur."},
                {"ERR10099", "Bu işlem başka bir üye iş yerine ait."},
                {"ERR10100", "Bu ödeme için birden fazla başarılı işlem vardır. Lütfen PGTRANID parametresini kullanınız."},
                {"ERR10101", "Geçersiz URL parametresi belirtilmiştir."},
                {"ERR10102", "Geçersiz BIN değeri belirtilmiştir."},
                {"ERR10103", "İşlem isteği Inact RT servisi tarafından raporlanan fraud olasılığı nedeniyle reddedilmiştir."},
                {"ERR10104", "Kullanılabilir komisyon şeması bulunmamaktadır."},
                {"ERR10105", "Mevcut Ödeme Sistemi havuzda bulunmamaktadır"},
                {"ERR10106", "İşlem tutarı üye iş yeri hesabına geçmemiştir, iade yapılamaz."},
                {"ERR10107", "Bu ödeme zaten yapılmıştır, verilen Üye İş Yeri Sipariş Numarası ile yeni ödeme oturumu oluşturulamaz."},
                {"ERR10108", "Üye iş yeri onaylanmamış"},
                {"ERR10109", "Ödeme havuzu üye iş yeri için henüz onaylanmamıştır."},
                {"ERR10110", "Kullanilan ödeme sistemi kampanya kullanımını desteklememektedir."},
                {"ERR10111", "Puan sorgulama ödeme sistemi tarafından desteklenmemektedir."},
                {"ERR10112", "Hatali puan formatı lütfen API Dokümantasyonundan puan kullanım formatını kontrol ediniz."},
                {"ERR10113", "Kullanilan ödeme sistemi puan kullanımını desteklememektedir."},
                {"ERR10115", "Üye iş yeri tarafından desteklenmeyen taksit sayısı belirtilmiştir."},
                {"ERR10116", "Bu işlem kullanımda olmayan üye iş yeri bilgileriyle gerçekleştirilemez."},
                {"ERR10117", "Bu sipariş numarası sonlanan bir oturumda kullanılmıştır lütfen farklı bir sipariş numarası ile oturum anahtarı oluşturun."},
                {"ERR10118", "İstek ile mevcut sipariş numarasına ait oturumun tutar, kur, oturum tipi, url dönüş değeri ya da yapılmak istenen işlem değerlerinden biri uyuşmamaktadır."},
                {"ERR10119", "Tam ve ya noktalı kısımda limit aşımı"},
                {"ERR10120", "Bu plan koduna ait bir tekrarlı ödeme bulunuyor"},
                {"ERR10121", "Geçersiz tekrarlı ödeme kodu"},
                {"ERR10122", "Sonlanmış durumdaki tekrarlı ödeme güncellenemez."},
                {"ERR10123", "Geçersiz işlem tipi"},
                {"ERR10125", "Mutabakat sorgusu için en az bir parametre geçilmeli."},
                {"ERR10126", "Birden fazla işlem bulundu."},
                {"ERR10127", "Ödeme sistemi puan parametresi hatalı, işlemin gönderileceği ödeme sisteminde gönderilen puan parametresi tanımlı değildir."},
                {"ERR10128", "Geçersiz parametre değeri"},
                {"ERR10129", "Parçalı puan kullanımı bu ödeme sistemi tarafından desteklenmemektedir"},
                {"ERR10130", "İşlem fraud süphesiyle reddedilmiştir. Detaylı bilgi için destek ekibiyle iletişime geçebilirsiniz. (TMX rejected)"},
                {"ERR10131", "Komisyon masrafları, satıcı komisyon tutarını aşamaz."},
                {"ERR10132", "Ödeme isteğinde pazaryeri parametrelerini kullanmaya yetkiniz bulunmamaktadır."},
                {"ERR10133", "İstenen işlem güncellenemez."},
                {"ERR10134", "Ödeme sistemi tipi ya da EFT kodu bulunamadı."},
                {"ERR10135", "EXTRA parametresi decode edilemiyor."},
                {"ERR10136", "Bu üye iş yeri için ortak ödeme sayfası (HPP) kullanılamaz."},
                {"ERR10137", "Query Campaign Not Supported By PaymentSystem"},
                {"ERR10138", "3D işlem yaparken hata oluştu."},
                {"ERR10139", "Üye İşyeri Entegrasyon Modeli Hatalı"},
                {"ERR10140", "İşlem tipi bu ödeme sistemi tarafından desteklenmiyor."},
                {"ERR10141", "Beklenemedik ödeme sistemi entegrasyonu hatası"},
                {"ERR10142", "Geçersiz Yönlendirme Adresi"},
                {"ERR10143", "ÖDENMİŞ veya İPTAL EDİLMİŞ ödeme"},
                {"ERR10144", "Üye iş yerinin yabancı banka kartları ile işlem yapma yetkisi yoktur"},
                {"ERR10145", "Tekrarlı ödeme bulunamadı."},
                {"ERR10146", "Tekrarlı ödeme kartı bulunamadı."},
                {"ERR10147", "3D doğrulama olmaksızın kart ekleme yetkiniz yoktur."},
                {"ERR10148", "Tekrarlı ödeme planı zaten bu kart daha önce eklenmiş."},
                {"ERR10149", "Bu işlem için desteklenmeyen para birimi"},
                {"ERR10150", "İndirim tutarı sipariş tutarından yüksek olamaz."},
                {"ERR10151", "Satıcı bulunamadı"},
                {"ERR10152", "Bu id ile satıcı mevcuttur."},
                {"ERR10153", "İade işlemi VakıfPayS Finans ekibi tarafından red edilmiştir"},
                {"ERR10154", "İşlem 3D kısıtlamasıyla başarısız olmuştur."},
                {"ERR10155", "Satıcı deaktive durumdadir."},
                {"ERR10160", "Eksik parametre"},
                {"ERR10161", "Ödeme sistemi havuzda bulunamadı"},
                {"ERR10168", "Bu kart markası desteklenmemektedir"},
                {"ERR10169", "Taksit bu kart markası için uygun değildir"},
                {"ERR10182", "Sipariş kalemi bulunamadı"},
                {"ERR10220", "Geçersiz IBAN TRY."},
                {"ERR10232", "Yabancı kart numarası ile işlem yapılamaz"},
                {"ERR10233", "Geçersiz e-posta"},
                {"ERR10243", "Lütfen doğru bir TCKN/VKN değeri giriniz"},
                {"ERR10244", "CUSTOMERPHONE ve CUSTOMEREMAIL alanları zorunludur"},
                {"ERR20001", "Manuel onay için bankanızla iletişime geçiniz"},
                {"ERR20005", "İşleme onay verilmedi"},
                {"ERR20033", "Belirtilen kredi kartının geçerlilik süresi bitmiştir"},
                {"ERR20034", "İşlemde sahtecilik (fraud) şüphesi"},
                {"ERR20041", "Sanal POS hatası: Kayıp kart, karta el koyunuz"},
                {"ERR20043", "Sanal POS hatası: Çalıntı kart, karta el koyunuz"},
                {"ERR20051", "Belirtilen kredi kartının limiti bu işlem için yeterli değildir"},
                {"ERR20054", "Kartın kullanım süresi geçmiş"},
                {"ERR20057", "Kart sahibine bu işlem yetkisi verilmemiştir"},
                {"ERR20062", "Belirtilen kredi kartı kısıtlanmıştır"},
                {"ERR20082", "Geçersiz / Hatalı CVV değeri"},
                {"ERR20093", "Kartınız e-ticaret işlemlerine kapalıdır. Bankanızı arayınız."},
                {"ERR30002", "3D işlemi başarılı şekilde sonlanmadı."},
                {"ERR30004", "Bu istek fraud (sahtecilik) kuralları tarafından reddedilmiştir."},
                {"ERR30005", "Banka tarafından yanıt alınmadı."}
            };

            return errors.ContainsKey(errorCode) ? errors[errorCode] : "Tanımlanamayan bir hata oluştu.";
        }
        #endregion


    }
}
