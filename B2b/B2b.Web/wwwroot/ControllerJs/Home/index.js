var last = [];
var campain = [];

$(document).ready(function () {
    formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };

    LastProducts();
    CampaignProducts();
});
function LastProducts() {
    var owl = $('#newProducts');

    $.ajax({
        url: '/api/CommerceApi/GetLatestProducts',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            last = data.List;
            // Başarılı olursa
            var items = '';
            $.each(data.List, function (index, item) {
                // Her bir ürün için HTML yapısını oluştur
                let discountBadge = '';
                let pricesHTML = '';

                // ISCampaign veya Price != DiscountedPrice ise indirim rozeti ve fiyatları göster
                if (item.IsCampaign === 1 || item.Price !== item.DiscountedPrice) {
                    discountBadge = `<div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-primary font-size-11">-% ${item.TotalDiscountRate}</span></div>`;
                    pricesHTML = `<h5 class="my-0 d-flex flex-column flex-sm-row align-items-sm-center">
    <span class="text-muted me-2 mb-1 mb-sm-0 ">
        <del>${formatPrice(item.Price)}</del>
    </span>
    <b>${formatPrice(item.DiscountedPrice)}</b>
</h5>`;
                } else {
                    // İndirim yoksa sadece normal fiyatı göster
                    pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                }
                items += `
                                     <div class="card product-card">
                                         <div class="card-body">
                    <div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-danger font-size-11">% ${sonundakiArtiyiSil(item.DiscountTxt)}</span></div>

                                             <div class="product-img position-relative text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" >
                                                
                                                <img src="${item.ProductImage}" class="img-fluid mx-auto d-block mt-2" style="max-height:150px; object-fit:contain;">
                                            </div>
                                            <div class="mt-3 text-center">
                                                <h6 class="product-title mb-2" title="${item.ProductName}">${item.ProductName}</h6>
                                                ${pricesHTML}
                                                <button class="btn btn-logo-2 btn-sm mt-2 w-100" onclick="AddCart(${item.ProductId},1)">
                                                    <i class="bx bx-cart me-1"></i>Ekle
                                                </button>
                                            </div>
                                         </div>
                                     </div>`;
            });

            // Oluşturulan HTML'i carousel'e ekle
            owl.html(items);
            // Owl Carousel'i başlat
            owl.owlCarousel({
                loop: false,
                nav: true,
                dots: false,
                margin: 10,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 2,
                    },
                    600: {
                        items: 2,
                    },
                    1000: {
                        items: 6,
                    }
                }
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // Hata olursa
            console.log('JSON verisi yüklenirken bir hata oluştu: ' + textStatus + ', ' + errorThrown);
        }
    });
}
function CampaignProducts() {
    var owl = $('#saleProducts');
    $.ajax({
        url: '/api/CommerceApi/GetCampaignProducts',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            campain = data.List;
            // Başarılı olursa
            var items = '';
            $.each(data.List, function (index, item) {
                // Her bir ürün için HTML yapısını oluştur
                let discountBadge = '';
                let pricesHTML = '';

                // ISCampaign veya Price != DiscountedPrice ise indirim rozeti ve fiyatları göster
                if (item.IsCampaign === 1 || item.Price !== item.DiscountedPrice) {
                    discountBadge = `<div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-danger font-size-11">% ${item.DiscountTxt}</span></div>`;
                    pricesHTML = `<h5 class="my-0 d-flex flex-column flex-sm-row align-items-sm-center">
    <span class="text-muted me-2 mb-1 mb-sm-0">
        <del>${formatPrice(item.Price)}</del>
    </span>
    <b>${formatPrice(item.DiscountedPrice)}</b>
</h5>`;
                } else {
                    // İndirim yoksa sadece normal fiyatı göster
                    pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                }
                items += `
                                     <div class="card product-card">
                                         <div class="card-body">
                                                                          <div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-danger font-size-11">% ${sonundakiArtiyiSil(item.DiscountTxt)}</span></div>

                                              <div class="product-img position-relative text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" >
                 
                                                <img src="${item.ProductImage}" class="img-fluid mx-auto d-block mt-2" style="max-height:150px; object-fit:contain;">
                                            </div>
                                            <div class="mt-3 text-center">
                                                <h6 class="product-title mb-2" title="${item.ProductName}">${item.ProductName}</h6>
                                                ${pricesHTML}
                                                <button class="btn btn-logo-2 btn-sm mt-2 w-100" onclick="AddCart(${item.ProductId},1)">
                                                    <i class="bx bx-cart me-1"></i>Ekle
                                                </button>
                                            </div>
                                         </div>
                                     </div>`;
            });

            // Oluşturulan HTML'i carousel'e ekle
            owl.html(items);
            // Owl Carousel'i başlat
            owl.owlCarousel({
                loop: false,
                nav: true,
                dots: false,
                margin: 10,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 2,
                    },
                    600: {
                        items: 2,
                    },
                    1000: {
                        items: 6,
                    }
                }
            });
        },
        error: function (jqXHR, textStatus, errorThrown) {
            // Hata olursa
            console.log('JSON verisi yüklenirken bir hata oluştu: ' + textStatus + ', ' + errorThrown);
        }
    });
}
function sonundakiArtiyiSil(text) {
    return text.replace(/\+ $/, "");
}
const mergeUniqueData = (arr1, arr2) => {
    // İki diziyi birleştir
    const mergedData = arr1.concat(arr2);

    // Map ile benzersiz elemanları bul
    const uniqueMap = new Map();

    mergedData.forEach(item => {
        // id'yi anahtar olarak kullanarak her elemanı Map'e ekle
        uniqueMap.set(item.id, item);
    });

    // Map'in değerlerini diziye dönüştürerek sonucu döndür
    return Array.from(uniqueMap.values());
};

