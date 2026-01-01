/* ==========================================================================
   PRODUCT.JS - FULL ENTEGRE (CSS VE 401 DÜZELTMELİ)
   ========================================================================== */

let allProductsData = [];
let filteredProducts = [];
let selectedBrands = [];

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
                pageData.forEach(item => {
                    const hasDiscount = item.DiscountedPrice > 0 && item.DiscountedPrice !== item.Price;
                    let pricesHTML = hasDiscount
                        ? `<h5 class="my-0"><del class="text-muted me-2 small">${formatPrice(item.Price)}</del><b class="text-danger">${formatPrice(item.DiscountedPrice)}</b></h5>`
                        : `<h5 class="my-0"><b>${formatPrice(item.Price)}</b></h5>`;

                    html += `
                    <div class="col col-lg-3 col-xxsm-6 mb-4">
                        <div class="card product-card h-100 shadow-sm">
                            <div class="card-body">
                                <div class="product-img position-relative text-center">
                                    <div class="position-absolute top-0 start-0">
                                        <span class="badge bg-success font-size-10 d-block mb-1">${item.BrandName}</span>
                                        <span class="badge bg-info font-size-10 d-block">${item.CategoryName}</span>
                                    </div>
                                    <img src="${item.ProductImage}" class="img-fluid mx-auto d-block mt-5" style="max-height:150px; object-fit:contain;">
                                 
                                </div>
                                <div class="mt-3 text-center">
                                    <h6 class="product-title mb-2" title="${item.ProductName}">${item.ProductName}</h6>
                                    ${pricesHTML}
                                    <button class="btn btn-primary btn-sm mt-2 w-100" onclick="AddCart(${item.ProductId},1)">
                                        <i class="bx bx-cart me-1"></i> Sepete Ekle
                                    </button>
                                     <button type="button" onclick="GetProduct(${item.ProductId})" 
                                            class="btn btn-success btn-sm mt-2 w-100" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl">
                                            Detayı Gör
                                        </button>
                                </div>
                            </div>
                        </div>
                    </div>`;
                });
                $('#product-list').html(html || '<div class="col-12 text-center py-5"><h5>Eşleşen ürün bulunamadı.</h5></div>');
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