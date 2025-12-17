let allProductsData = [];

$(document).ready(function () {
    GetCaegories();

    // TÜRKÇE normalize fonksiyonu
    function normalizeTR(text) {
        return text
            .toString()
            .toLocaleLowerCase('tr-TR')
            .normalize('NFD')
            .replace(/[\u0300-\u036f]/g, '')
            .replace(/ı/g, 'i')
            .replace(/ş/g, 's')
            .replace(/ğ/g, 'g')
            .replace(/ü/g, 'u')
            .replace(/ö/g, 'o')
            .replace(/ç/g, 'c');
    }

    // Fiyat formatlama
    const formatPrice = (price) => {
        return price.toFixed(2)
            .replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };

    // Ürünleri API'den çek
    function GetProducts() {
        $.ajax({
            url: '/api/CommerceApi/GetProducts',
            type: 'GET',
            dataType: 'json',
            contentType: 'application/json',
            headers: { 'Authorization': localStorage.getItem('token') },
            success: function (response) {
                allProductsData = response.List || [];

                if (allProductsData.length > 0) {
                    updateCategoryCounts();
                    filterAndPaginate(allProductsData);
                } else {
                    $('#product-list').html('<p>Ürün bulunamadı.</p>');
                }
            },
            error: function () {
                $('#product-list').html('<p>Ürünler yüklenirken hata oluştu.</p>');
            }
        });
    }

    // Sayfalama + listeleme
    const filterAndPaginate = (filteredData) => {
        $('#pagination-container').pagination({
            dataSource: filteredData,
            pageSize: 36,
            callback: function (data) {
                let html = '';

                $.each(data, function (index, item) {
                    let discountBadge = '';
                    let pricesHTML = '';

                    if (item.IsCampaign === 1 || item.Price !== item.DiscountedPrice) {
                        discountBadge = `
                        <div class="avatar-sm product-ribbon">
                            <span class="avatar-title rounded-circle bg-primary font-size-11">-% ${item.TotalDiscountRate}</span>
                        </div>`;

                        pricesHTML = `
                        <h5 class="my-0">
                            <span class="text-muted me-2"><del>${formatPrice(item.Price)}</del></span>
                            <b>${formatPrice(item.DiscountedPrice)}</b>
                        </h5>`;
                    } else {
                        pricesHTML = `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;
                    }

                    html += `
                    <div class="col-12 col-sm-6 col-md-4 col-lg-3 mb-4">
                        <div class="card product-card">
                            <div class="card-body">
                                <div class="product-img position-relative">
                                    <div class="badge bg-success font-size-11">${item.BrandName}</div>
                                    <div class="badge bg-danger font-size-11">${item.CategoryName}</div>
                                    ${discountBadge}
                                    <img src="${item.ProductImage}" class="img-fluid mx-auto d-block" alt="${item.ProductName}">
                                    <div class="hover-btn-wrapper">
                                        <button type="button" onclick="GetProduct(${item.ProductId})" 
                                            class="btn btn-success" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl">
                                            Detayı Gör
                                        </button>
                                    </div>
                                </div>
                                <div class="mt-2 text-center">
                                    <h5 class="mb-3 text-truncate">${item.ProductName}</h5>
                                    ${pricesHTML}
                                    <a class="btn btn-primary mt-2" onclick="AddCart(${item.ProductId},1)">
                                        <i class="bx bx-cart me-2"></i> Sepete Ekle
                                    </a>
                                </div>
                            </div>
                        </div>
                    </div>`;
                });

                $('#product-list').html(html);
            }
        });
    };

    // Arama
    $('#searchInput').on('keyup', function () {
        const searchTerm = normalizeTR($(this).val().trim());
        let filteredData;

        $('#category-list li').removeClass('active');
        $('#category-list li[data-category-id="all"]').addClass('active');

        if (searchTerm.length >= 2) {
            filteredData = allProductsData.filter(item =>
                normalizeTR(item.ProductName).includes(searchTerm) ||
                normalizeTR(item.ProductCode).includes(searchTerm)
            );
        } else {
            filteredData = allProductsData;
        }

        filterAndPaginate(filteredData);
    });

    // Kategori tıklama
    $('#category-list').on('click', 'li', function () {
        const categoryId = $(this).data('category-id');

        $('#searchInput').val('');
        $('#category-list li').removeClass('active');
        $(this).addClass('active');

        if (categoryId === 'all') {
            filterAndPaginate(allProductsData);
        } else {
            const filtered = allProductsData.filter(x => x.CategoryId == categoryId);
            filterAndPaginate(filtered);
        }
    });

    // Kategori badge güncelle
    function updateCategoryCounts() {
        $('#category-list li').each(function () {
            const catId = $(this).data('category-id');
            if (catId !== 'all') {
                const count = allProductsData.filter(p => p.CategoryId == catId).length;
                $(this).find('.badge').text(count);
            }
        });
    }

    GetProducts();
});

// Kategorileri çek
function GetCaegories() {
    const cat = document.getElementById('category-list');
    cat.innerHTML = `
    <li class="list-group-item d-flex justify-content-between align-items-center active" data-category-id="all">
        <span>Tüm Kategoriler</span>
        <span class="badge bg-dark rounded-pill">*</span>
    </li>`;

    $.ajax({
        url: '/api/CommerceApi/GetCategories',
        type: 'GET',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem('token') },
        success: function (response) {
            response.List.forEach(function (item) {
                cat.innerHTML += `
                <li class="list-group-item d-flex justify-content-between align-items-center" data-category-id="${item.CategoryId}">
                    <span>${item.CategoryName}</span>
                    <span class="badge bg-primary rounded-pill">0</span>
                </li>`;
            });
        }
    });
}
