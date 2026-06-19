/* ==========================================================================
   PRODUCT.JS - FULL ENTEGRE (TABLOYA MİKTAR GİRİŞİ EKLEDİ)
   ========================================================================== */
let allProductsData = [];
let filteredProducts = [];
let selectedBrands = [];
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
        const categoryId = $('#category-list li.active').data('category-id') || 'all';
        const minVal = $('#minPrice').val();
        const maxVal = $('#maxPrice').val();
        const minPrice = minVal !== "" ? parseFloat(minVal) : 0;
        const maxPrice = maxVal !== "" ? parseFloat(maxVal) : Infinity;

        // Filtre temizle butonu görünürlüğü
        if (searchTerm !== "" || categoryId !== 'all' || selectedBrands.length > 0 || minVal !== "" || maxVal !== "") {
            $('#clearFiltersArea').removeClass('d-none');
        } else {
            $('#clearFiltersArea').addClass('d-none');
        }

        filteredProducts = allProductsData.filter(item => {
            const matchesSearch = searchTerm.length < 2 ||
                normalizeTR(item.ProductName).includes(searchTerm) ||
                normalizeTR(item.ProductCode).includes(searchTerm);
            const matchesCategory = (categoryId === 'all' || item.CategoryId == categoryId);
            const matchesBrand = (selectedBrands.length === 0 || selectedBrands.includes(item.BrandId.toString()));
            const currentPrice = item.DiscountedPrice > 0 ? item.DiscountedPrice : item.Price;
            const matchesPrice = (currentPrice >= minPrice && currentPrice <= maxPrice);
            return matchesSearch && matchesCategory && matchesBrand && matchesPrice;
        });

        applySorting();
    }

    function applySorting() {
        const sortType = $('#sortSelect').val();
        filteredProducts.sort((a, b) => {
            const priceA = a.DiscountedPrice > 0 ? a.DiscountedPrice : a.Price;
            const priceB = b.DiscountedPrice > 0 ? b.DiscountedPrice : b.Price;
            switch (sortType) {
                case 'priceAsc': return priceA - priceB;
                case 'priceDesc': return priceB - priceA;
                case 'nameAsc': return a.ProductName.localeCompare(b.ProductName, 'tr');
                case 'nameDesc': return b.ProductName.localeCompare(a.ProductName, 'tr');
                default: return 0;
            }
        });
        renderPagination(filteredProducts);
    }

    function resetFilters() {
        $('#searchInput').val('');
        $('#minPrice').val('');
        $('#maxPrice').val('');
        $('#sortSelect').val('');
        $('#category-list li').removeClass('active');
        $('#category-list li[data-category-id="all"]').addClass('active');
        $('.brand-checkbox').prop('checked', false);
        selectedBrands = [];
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
                        <div class="col col-lg-3 col-xsm-4 mb-4">
                            <div class="card product-card h-100 shadow-sm">
                                <div class="card-body">
                                    <div class="product-img position-relative text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                        <div class="position-absolute col-12 top-0 start-0">
                                            <span class="badge bg-logo-1 font-size-10 d-block text-black mb-1">${item.BrandName}</span>
                                            <span class="badge bg-logo-2 font-size-10 d-block">${item.CategoryName}</span>
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
                        </div>`;
                    });

                    $('#product-list').html(html || '<div class="col-12 text-center py-5"><h5>Eşleşen ürün bulunamadı.</h5></div>');

                } else {
                    // ================= LIST (TABLO) GÖRÜNÜMÜ =================
                    if (pageData.length === 0) {
                        $('#product-list').html('<div class="col-12 text-center py-5"><h5>Eşleşen ürün bulunamadı.</h5></div>');
                        return;
                    }

                    // Tablo Başlıkları
                    html += `
                    <div class="col-12">
                        <div class="table-responsive">
                            <table class="table table-bordered table-hover align-middle bg-white shadow-sm">
                                <thead class="table-light">
                                    <tr>
                                        <th style="width: 80px;" class="text-center">Görsel</th>
                                        <th>Ürün Kodu</th>
                                        <th>Ürün Adı</th>
                                        <th>Marka</th>
                                        <th>Kategori</th>
                                        <th class="text-center" style="width: 90px;">Birim</th>
                                        <th class="text-end" style="width: 140px;">Fiyat</th>
                                        <th class="text-center" style="width: 180px;">Miktar / İşlem</th>
                                    </tr>
                                </thead>
                                <tbody>`;

                    // Tablo Satırları
                    pageData.forEach(item => {
                        const hasDiscount = item.DiscountedPrice > 0 && item.DiscountedPrice !== item.Price;
                        let pricesHTML = hasDiscount
                            ? `<div class="text-end"><del class="text-muted small d-block">${formatPrice(item.Price)}</del><b class="text-danger">${formatPrice(item.DiscountedPrice)}</b></div>`
                            : `<div class="text-end"><b>${formatPrice(item.Price)}</b></div>`;

                        const productUnit = item.UnitName || item.Unit || '-';

                        html += `
                        <tr>
                            <td class="text-center" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                <img src="${item.ProductImage}" style="max-height: 40px; max-width: 40px; object-fit: contain;">
                            </td>
                            <td><small class="text-muted">${item.ProductCode || '-'}</small></td>
                            <td>
                                <span class="fw-medium text-dark mb-0 d-block" onclick="GetProduct(${item.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl" style="cursor:pointer;">
                                    ${item.ProductName}
                                </span>
                            </td>
                            <td><span class="badge bg-logo-1 text-black">${item.BrandName}</span></td>
                            <td><span class="badge bg-logo-2">${item.CategoryName}</span></td>
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

                    html += `
                                </tbody>
                            </table>
                        </div>
                    </div>`;

                    $('#product-list').html(html);
                }
            }
        });
    }

    // Event Listeners
    $('#searchInput').on('keyup', applyFilters);
    $('#category-list').on('click', 'li', function () {
        $('#category-list li').removeClass('active');
        $(this).addClass('active');
        applyFilters();
    });
    $(document).on('change', '.brand-checkbox', function () {
        selectedBrands = $('.brand-checkbox:checked').map(function () { return this.value; }).get();
        applyFilters();
    });
    $('#minPrice, #maxPrice').on('input', applyFilters);
    $('#sortSelect').on('change', applySorting);
    $('#btnClearFilters').on('click', resetFilters);

    // Görünüm Değiştirme Dinamik Dinleyicileri (Delegate Sistemi)
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
        allProductsData.forEach(p => { if (!brandMap.has(p.BrandId)) brandMap.set(p.BrandId, p.BrandName); });
        let html = '';
        Array.from(brandMap.entries()).sort((a, b) => a[1].localeCompare(b[1], 'tr')).forEach(([id, name]) => {
            html += `<div class="form-check mb-1"><input class="form-check-input brand-checkbox" type="checkbox" value="${id}" id="brand_${id}"><label class="form-check-label" for="brand_${id}" style="cursor:pointer">${name}</label></div>`;
        });
        $('#brand-filter').html(html || 'Marka bulunamadı.');
    }

    function updateCategoryCounts() {
        $('#category-list li').each(function () {
            const id = $(this).data('category-id');
            if (id !== 'all') {
                const count = allProductsData.filter(p => p.CategoryId == id).length;
                $(this).find('.badge').text(count);
            }
        });
    }
});

function GetCategories() {
    const cat = $('#category-list');
    cat.html(`<li class="list-group-item active d-flex justify-content-between align-items-center" data-category-id="all" style="cursor:pointer"><span>Tüm Kategoriler</span><span class="badge bg-dark rounded-pill">*</span></li>`);
    $.ajax({
        url: '/api/CommerceApi/GetCategories',
        type: 'GET',
        headers: { 'Authorization': localStorage.getItem('token') },
        success: function (res) {
            if (res.List) {
                res.List.forEach(item => {
                    cat.append(`<li class="list-group-item d-flex justify-content-between align-items-center" data-category-id="${item.CategoryId}" style="cursor:pointer"><span>${item.CategoryName}</span><span class="badge bg-primary rounded-pill">0</span></li>`);
                });
            }
        }
    });
}