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
                    pricesHTML = `<h5 class="my-0"><span class="text-muted me-2"><del>${formatPrice(item.Price)}</del></span><b>${formatPrice(item.DiscountedPrice)}</b></h5>`;
                } else {
                    // İndirim yoksa sadece normal fiyatı göster
                    pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                }
                items += `
                                     <div class="card product-card">
                                         <div class="card-body">
                                             <div class="product-img position-relative">
                                                 <div class="badge bg-success font-size-11">${item.BrandName}</div>
                                                 <div class="badge bg-danger font-size-11">${item.CategoryName}</div>
                                                 ${discountBadge}
                                                 <img src="${item.ProductImage}" alt="${item.ProductName}" class="img-fluid mx-auto d-block">
                                                 <div class="hover-btn-wrapper">
                                                     <button type="button" onclick="GetProduct(${item.ProductId})" class="btn btn-success" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl">
                                                         Detayı Gör
                                                     </button>
                                                 </div>
                                             </div>
                                             <div class="mt-2 text-center">
                                                 <h5 class="mb-3 text-truncate"><a href="javascript: void(0);" class="text-dark">${item.ProductName} </a></h5>
                                                 ${pricesHTML}
                                                 <a class="btn btn-primary waves-effect waves-light mt-2 me-1" onclick="AddCart(${item.ProductId},1)">
                                                     <i class="bx bx-cart me-2"></i> Sepete Ekle
                                                 </a>
                                             </div>
                                         </div>
                                     </div>`;
            });

            // Oluşturulan HTML'i carousel'e ekle
            owl.html(items);
            // Owl Carousel'i başlat
            owl.owlCarousel({
                loop: false,
                nav: false,
                dots: false,
                margin: 10,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 1,
                    },
                    600: {
                        items: 3,
                    },
                    1000: {
                        items: 5,
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
                    discountBadge = `<div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-primary font-size-11">-% ${item.TotalDiscountRate}</span></div>`;
                    pricesHTML = `<h5 class="my-0"><span class="text-muted me-2"><del>${formatPrice(item.Price)}</del></span><b>${formatPrice(item.DiscountedPrice)}</b></h5>`;
                } else {
                    // İndirim yoksa sadece normal fiyatı göster
                    pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                }
                items += `
                                     <div class="card product-card">
                                         <div class="card-body">
                                             <div class="product-img position-relative">
                                                 <div class="badge bg-success font-size-11">${item.BrandName}</div>
                                                 <div class="badge bg-danger font-size-11">${item.CategoryName}</div>
                                                 ${discountBadge}
                                                 <img src="${item.ProductImage}" alt="${item.ProductName}" class="img-fluid mx-auto d-block">
                                                 <div class="hover-btn-wrapper">
                                                     <button type="button" onclick="GetProduct(${item.ProductId})" class="btn btn-success" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl">
                                                         Detayı Gör
                                                     </button>
                                                 </div>
                                             </div>
                                             <div class="mt-2 text-center">
                                                 <h5 class="mb-3 text-truncate"><a href="javascript: void(0);" class="text-dark">${item.ProductName} </a></h5>
                                                 ${pricesHTML}
                                                 <a class="btn btn-primary waves-effect waves-light mt-2 me-1" onclick="AddCart(${item.ProductId},1)>
                                                     <i class="bx bx-cart me-2"></i> Sepete Ekle
                                                 </a>
                                             </div>
                                         </div>
                                     </div>`;
            });

            // Oluşturulan HTML'i carousel'e ekle
            owl.html(items);
            // Owl Carousel'i başlat
            owl.owlCarousel({
                loop: false,
                nav: false,
                dots: false,
                margin: 10,
                responsiveClass: true,
                responsive: {
                    0: {
                        items: 1,
                    },
                    600: {
                        items: 3,
                    },
                    1000: {
                        items: 5,
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

