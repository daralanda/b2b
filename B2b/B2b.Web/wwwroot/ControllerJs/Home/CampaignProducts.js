let allProductsData = [];


$(document).ready(function () {

    function GetProducts() {
        $.ajax({
            url: '/api/CommerceApi/GetCampaignProductList',
            type: 'Get',
            dataType: 'Json',
            contentType: 'application/json',
            headers: { 'Authorization': localStorage.getItem("token") },
            success: function (response) {
                console.log("API'den gelen yanıt:", response);

                // API'den gelen veriyi sakla
                allProductsData = response.List;

                if (allProductsData && allProductsData.length > 0) {
                    // Başlangıçta tüm ürünleri filtrele ve sayfalama yap
                    filterAndPaginate(allProductsData);
                } else {
                    $('#product-list').html("<p>Ürün bulunamadı.</p>");
                }
            },
            error: function (err) {
                console.error("API'den ürünler getirilirken bir hata oluştu.", err);
                $('#product-list').html("<p>Ürünler yüklenirken bir sorun oluştu.</p>");
            },
            async: false
        });
    }


    // Fiyat formatlama için yardımcı fonksiyon
    const formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };

    // Sayfalama ve filtreleme işlemini yöneten ana fonksiyon
    const filterAndPaginate = (filteredData) => {
        $('#pagination-container').pagination({
            dataSource: filteredData,
            pageSize: 12, // Sayfa başına ürün sayısını belirle
            callback: function (data, pagination) {
                let html = '';
                $.each(data, function (index, item) {
                    let discountBadge = '';
                    let pricesHTML = '';

                    // ISCampaign veya Price != DiscountedPrice ise indirim rozeti ve fiyatları göster
                    if (item.IsCampaign === 1 || item.Price !== item.DiscountedPrice) {
                        discountBadge = `<div class="avatar-sm product-ribbon"><span class="avatar-title rounded-circle bg-danger font-size-11">-% ${item.DiscountTxt}</span></div>`;
                        pricesHTML = `<h5 class="my-0"><span class="text-muted me-2"><del>${formatPrice(item.Price)}</del></span><b>${formatPrice(item.DiscountedPrice)}</b></h5>`;
                    } else {
                        // İndirim yoksa sadece normal fiyatı göster
                        pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                    }

                    html += `
                            <div class="col col-lg-3 col-xsm-6 mb-4">
                              <div class="card product-card h-100 shadow-sm">
                                  <div class="card-body">
                                      <div class="product-img position-relative text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" >
                                          <div class="position-absolute col-12 top-0 start-0">
                                              <span class="badge bg-logo-1 font-size-10 d-block text-black mb-1">${item.BrandName}</span>
                                              <span class="badge bg-logo-2 font-size-10 d-block">${item.CategoryName}</span>
                                              ${discountBadge}
                                          </div>
                                          <img src="${item.ProductImage}" class="img-fluid mx-auto d-block mt-5" style="max-height:150px; object-fit:contain;">
                                      </div>
                                      <div class="mt-3 text-center">
                                          <h6 class="product-title mb-2" title="${item.ProductName}">${item.ProductName}</h6>
                                          ${pricesHTML}
                                          <button class="btn btn-logo-2 btn-sm mt-2 w-100" onclick="AddCart(${item.ProductId},1)">
                                              <i class="bx bx-cart me-1"></i> Sepete Ekle
                                          </button>
                                      </div>
                                  </div>
                              </div>
                          </div>
                            `;
                });
                $('#product-list').html(html);
            }
        });
    };

    // Arama kutusu olay dinleyicisi
    $('#searchInput').on('keyup', function () {
        const searchTerm = $(this).val().toLowerCase().trim();
        let filteredData;

        // Kategori seçimini sıfırla
        $('#category-list li').removeClass('active');
        $('#category-list li[data-category-id="all"]').addClass('active');

        if (searchTerm.length >= 2) { // 2 karakterden fazla girilince filtrele
            filteredData = allProductsData.filter(item => {
                return item.ProductName.toLowerCase().includes(searchTerm) ||
                    item.ProductCode.toLowerCase().includes(searchTerm);
            });
        } else {
            // Arama terimi 2 karakterden azsa tüm ürünleri göster
            filteredData = allProductsData;
        }
        filterAndPaginate(filteredData);
    });

    // Kategori tıklama olay dinleyicisi
    $('#category-list').on('click', 'li', function (e) {
        e.preventDefault();
        const categoryId = $(this).data('category-id');
        let filteredData;

        // Arama kutusunu temizle
        $('#searchInput').val('');

        // Aktif kategoriyi vurgula
        $('#category-list li').removeClass('active');
        $(this).addClass('active');

        if (categoryId === 'all') {
            // "Tüm Kategoriler" seçildiyse
            filteredData = allProductsData;
        } else {
            // Belirli bir kategori seçildiyse
            filteredData = allProductsData.filter(item => item.CategoryId == categoryId);
        }
        filterAndPaginate(filteredData);
    });
    GetProducts();

    GetCaegories();
    // Sayfa yüklendiğinde API'yi çağır
});


function GetCaegories() {
    var cat = document.getElementById("category-list");
    cat.innerHTML = `<li class="list-group-item active" data-category-id="all"><a> <span class="tablist-name">Tüm Kategoriler</span></a></li>`;
    $.ajax({
        url: '/api/CommerceApi/GetCategories',
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            response.List.forEach(function (item) {
                var row = allProductsData.find(x => x.CategoryId == item.CategoryId);
                if (row != null) {
                    cat.innerHTML += `<li class="list-group-item" data-category-id="${item.CategoryId}"><a> <span class="tablist-name">${item.CategoryName}</span></a></li>`;
                }
            });
        },
        error: function (err) { },
        async: false
    });
}

