/* ==========================================================================
   PRODUCT.JS - YENİ YATAY FİLTRE VE SELECT YAPISINA UYARLANMIŞ VERSİYON
   ========================================================================== */
let allProductsData = [];
let filteredProducts = [];
let selectedBrandId = ''; // Tekli select seçimi için tutulan değişken
let currentView = 'grid'; // Varsayılan görünüm: 'grid' veya 'list'

$(document).ready(function () {
    GetCategories();
    GetProducts();

    function normalizeTR(text) {
        if (!text) return "";
        return text.toString()
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

    const formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };

    function GetProducts() {
        $.ajax({
            url: '/api/CommerceApi/GetProducts',
            type: 'GET',
            dataType: 'json',
            headers: { 'Authorization': localStorage.getItem('token') },
            success: function (response) {
                allProductsData = response.List || [];
                if (allProductsData.length > 0) {
                    updateCategoryCounts();
                    buildBrandFilter();
                    applyFilters();
                } else {
                    $('#product-list').html('<div class="col-12 text-center"><p>Ürün bulunamadı.</p></div>');
                }
            }
        });
    }

    function applyFilters() {
        const searchTerm = normalizeTR($('#searchInput').val().trim());
        const categoryId = $('#category-list').val() || ''; // select elementinden değer alıyoruz
        const brandId = $('#brand-filter').val() || ''; // select elementinden değer alıyoruz

        // Filtre temizle butonu görünürlüğü (Arama veya filtrelerden biri aktifse görünür)
        if (searchTerm !== "" || categoryId !== "" || brandId !== "") {
            $('#clearFiltersArea').removeClass('d-none');
        } else {
            $('#clearFiltersArea').addClass('d-none');
        }

        filteredProducts = allProductsData.filter(item => {
            const matchesSearch = searchTerm.length < 2 ||
                normalizeTR(item.ProductName).includes(searchTerm) ||
                normalizeTR(item.ProductCode).includes(searchTerm);

            const matchesCategory = (categoryId === "" || item.CategoryId == categoryId);
            const matchesBrand = (brandId === "" || item.BrandId == brandId);

            return matchesSearch && matchesCategory && matchesBrand;
        });

        applySorting();
    }

    function applySorting() {
        const sortType = $('#sortSelect').val();
        filteredProducts.sort((a, b) => {
            // İndirimli fiyat varsa onu, yoksa normal fiyatı baz alarak sıralar
            const priceA = a.DiscountedPrice > 0 ? a.DiscountedPrice : a.Price;
            const priceB = b.DiscountedPrice > 0 ? b.DiscountedPrice : b.Price;

            switch (sortType) {
                case 'priceAsc': return priceA - priceB;   // Fiyat: Düşükten Yükseğe
                case 'priceDesc': return priceB - priceA;  // Fiyat: Yüksekten Düşüğe
                case 'nameAsc': return a.ProductName.localeCompare(b.ProductName, 'tr');
                case 'nameDesc': return b.ProductName.localeCompare(a.ProductName, 'tr');
                default: return 0;
            }
        });
        renderPagination(filteredProducts);
    }

    function resetFilters() {
        $('#searchInput').val('');
        $('#sortSelect').val('');
        $('#category-list').val(''); // Select elementini sıfırla (Tüm Kategoriler)
        $('#brand-filter').val('');  // Select elementini sıfırla (Tüm Markalar)
        applyFilters();
    }

    function renderPagination(data) {
        $('#pagination-container').pagination({
            dataSource: data,
            pageSize: 36,
            callback: function (pageData) {
                let html = '';

                if (currentView === 'grid') {
                    // ================= GRID (IZGARA) GÖRÜNÜMÜ =================
                    pageData.forEach(item => {
                        const hasDiscount = item.DiscountedPrice > 0 && item.DiscountedPrice !== item.Price;
                        let pricesHTML = hasDiscount
                            ? `<h5 class="my-0 d-flex flex-column flex-sm-row align-items-sm-center"><del class="text-muted me-2 small">${formatPrice(item.Price)}</del><b class="text-danger">${formatPrice(item.DiscountedPrice)}</b></h5>`
                            : `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;

                        html += `
                        <div class="col-xsm-6 col-md-3 col-lg-2 mb-4">
                            <div class="card product-card h-100 shadow-sm">
                                <div class="card-body">
                                    <div class="product-img position-relative text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                        <img src="${item.ProductImage}" class="img-fluid mx-auto d-block mt-2" style="max-height:150px; object-fit:contain;">
                                    </div>
                                    <div class="mt-3 text-center">
                                        <h6 class="product-title mb-2" title="${item.ProductName}">${item.ProductName}</h6>
                                        ${pricesHTML}
                                        <button class="btn btn-logo-2 btn-sm mt-2 w-100" onclick="AddCart(${item.ProductId},1)">
                                            <i class="bx bx-cart me-1"></i> Ekle
                                        </button>
                                    </div>
                                </div>
                            </div>
                        </div>`;
                    });

                    $('#product-list').html(html || '<div class="col-12 text-center py-5"><h5>Eşleşen ürün bulunamadı.</h5></div>');

                } else {
                    // ================= LIST (TABLO) GÖRÜNÜMÜ =================
                    if (pageData.length === 0) {
                        $('#product-list').html('<div class="col-12 text-center py-5"><h5>Eşleşen ürün bulunamadı.</h5></div>');
                        return;
                    }

                    html += `
                    <div class="col-12">
                        <div class="table-responsive">
                            <table class="table table-bordered table-hover align-middle bg-white shadow-sm">
                                <thead class="table-light">
                                    <tr>
                                        <th style="width: 80px;" class="text-center">Görsel</th>
                                        <th class="mobil-gizle">Ürün Kodu</th>
                                        <th>Ürün Adı</th>
                                        <th class="text-center" style="width: 90px;">Birim</th>
                                        <th class="text-end" style="width: 140px;">Fiyat</th>
                                        <th class="text-center" style="width: 180px;">Miktar / İşlem</th>
                                    </tr>
                                </thead>
                                <tbody>`;

                    pageData.forEach(item => {
                        const hasDiscount = item.DiscountedPrice > 0 && item.DiscountedPrice !== item.Price;
                        let pricesHTML = hasDiscount
                            ? `<div class="text-end"><del class="text-muted small d-block">${formatPrice(item.Price)}</del><b class="text-danger">${formatPrice(item.DiscountedPrice)}</b></div>`
                            : `<div class="text-end"><b>${formatPrice(item.Price)}</b></div>`;

                        const productUnit = item.UnitTypeName || item.Unit || '-';

                        html += `
                        <tr>
                            <td class="text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                <img src="${item.ProductImage}" style="max-height: 40px; max-width: 40px; object-fit: contain;">
                            </td>
                            <td class="mobil-gizle"><small class="text-muted">${item.ProductCode || '-'}</small></td>
                            <td>
                                <span class="fw-medium text-dark mb-0 d-block" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                    ${item.ProductName}
                                </span>
                            </td>
                            <td class="text-center"><span class="text-secondary small fw-bold">${productUnit}</span></td>
                            <td>${pricesHTML}</td>
                            <td>
                                <div class="input-group input-group-sm">
                                    <input type="number" id="qty_${item.ProductId}" class="form-control text-center" value="1" min="1" style="max-width: 70px;">
                                    <button class="btn btn-logo-2" type="button" onclick="const qty = parseInt($('#qty_${item.ProductId}').val()) || 1; AddCart(${item.ProductId}, qty);">
                                        <i class="bx bx-cart"></i> Ekle
                                    </button>
                                </div>
                            </td>
                        </tr>`;
                    });

                    html += `</tbody></table></div></div>`;
                    $('#product-list').html(html);
                }
            }
        });
    }

    // Event Listeners (Select Elemanlarının Değişimini Dinler)
    $('#searchInput').on('keyup', applyFilters);
    $('#category-list').on('change', applyFilters);
    $('#brand-filter').on('change', applyFilters);
    $('#sortSelect').on('change', applySorting);
    $('#btnClearFilters').on('click', resetFilters);

    // Görünüm Değiştirme Dinamik Dinleyicileri (Grid / List)
    $(document).on('click', '#btn-grid', function (e) {
        e.preventDefault();
        currentView = 'grid';
        $('#btn-list').removeClass('active');
        $(this).addClass('active');
        applyFilters();
    });

    $(document).on('click', '#btn-list', function (e) {
        e.preventDefault();
        currentView = 'list';
        $('#btn-grid').removeClass('active');
        $(this).addClass('active');
        applyFilters();
    });

    function buildBrandFilter() {
        const brandMap = new Map();
        allProductsData.forEach(p => {
            if (p.BrandId && !brandMap.has(p.BrandId)) brandMap.set(p.BrandId, p.BrandName);
        });

        let html = '<option value="">Tüm Markalar</option>';
        Array.from(brandMap.entries())
            .sort((a, b) => a[1].localeCompare(b[1], 'tr'))
            .forEach(([id, name]) => {
                html += `<option value="${id}">${name}</option>`;
            });
        $('#brand-filter').html(html);
    }

    function updateCategoryCounts() {
        // Seçenek metinlerinin yanına adet bilgisini (12) formatında yazar
        $('#category-list option').each(function () {
            const id = $(this).val();
            if (id !== "") {
                const count = allProductsData.filter(p => p.CategoryId == id).length;
                const originalText = $(this).text().replace(/\s\(\d+\)/g, ''); // Varsa eski sayıyı siler
                $(this).text(`${originalText} (${count})`);
            }
        });
    }
});

function GetCategories() {
    const cat = $('#category-list');
    cat.html(`<option value="">Tüm Kategoriler</option>`);
    $.ajax({
        url: '/api/CommerceApi/GetCategories',
        type: 'GET',
        headers: { 'Authorization': localStorage.getItem('token') },
        success: function (res) {
            if (res.List) {
                res.List.forEach(item => {
                    cat.append(`<option value="${item.CategoryId}">${item.CategoryName}</option>`);
                });

                // Kategoriler API'den geç gelirse adetleri senkronize etmek için ek kontrol
                if (allProductsData.length > 0) {
                    $('#category-list option').each(function () {
                        const id = $(this).val();
                        if (id !== "") {
                            const count = allProductsData.filter(p => p.CategoryId == id).length;
                            $(this).text(`${$(this).text()} (${count})`);
                        }
                    });
                }
            }
        }
    });
}