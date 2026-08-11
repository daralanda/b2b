/* ==========================================================================
   ProductList.js - Ürün Yönetimi JS Modülü (Optimize Edilmiş Sürüm)
   ========================================================================== */

var DataList = [];
var Data = {
    ProductId: 0,
    ProductName: '',
    CategoryId: 0,
    CurrencyId: 0,
    Vat: 0,
    StockQuantity: 0,
    ProductCode: '',
    BrandId: 0,
    Description: '',
    IsActive: false,
    ProductImages: [],
    ProductPrices: []
};

var dataPriceList = [];
var dataImageList = [];
var UnitTypeList = [];
var UnitTypeMap = {}; // O(1) hızlı arama için Map
var Categories = [];
var Brands = [];
var IsCopy = false;

$(document).ready(function () {
    PageLoad();
});

/* ==========================================================================
   SAYFA YÜKLEME VE EŞZAMANLI (PARALEL) VERİ ÇEKME
   ========================================================================== */
function PageLoad() {
    // 1. Tüm lookup verilerini PARALEL olarak çek
    $.when(
        GetUnitTypes(),
        GetCategories(),
        GetBrands(),
        GetCurrency()
    ).done(function () {
        // 2. Lookuplar dolduktan sonra ürün listesini getir
        GetProductList();
    });
}

function GetProductList() {
    $.ajax({
        url: '/api/ProductApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List || [];

            var columns = [
                {
                    "data": "ImageUrl",
                    "render": function (data, type, row) {
                        var imageUrl = data ? data : '/uploads/default.jpg';
                        return `<img src="${imageUrl}" alt="${row.ProductName || ''}" class="img-thumbnail" style="max-height: 50px; max-width: 50px;" />`;
                    }
                },
                { "data": "ProductCode" },
                { "data": "ProductName" },
                { "data": "CategoryName" },
                { "data": "BrandName" },
                { "data": "StockQuantity" },
                { "data": "Barcode", "visible": false },
                { "data": "Count", "visible": false },
                { "data": "CurrencyName", "visible": false },
                { "data": "IsDefault", "visible": false },
                { "data": "Price", "visible": false },
                { "data": "UnitTypeName", "visible": false },
                { "data": "Description", "visible": false },
                { "data": "Vat", "visible": false },
                { "data": "ImageUrl", "visible": false },
                {
                    "data": "IsActive",
                    "render": function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "ProductId",
                    "render": function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1 text-white edit' id='" + data + "' data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a>";
                    }
                }
            ];

            DatatablesLoad("datatables", DataList, columns);
            $('#exampleModal').modal('hide');
        }
    });
}

/* ==========================================================================
   LOOKUP / DROPDOWN DOLDURMA FONKSİYONLARI (DOM Optimizasyonlu)
   ========================================================================== */
const selectElement = document.getElementById('unitType');

function GetUnitTypes() {
    return $.ajax({
        url: '/api/UnitTypeApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            var list = data.List || [];
            UnitTypeList = [];
            UnitTypeMap = {};
            var optionsHtml = '';

            list.forEach(function (x) {
                UnitTypeList.push({ id: x.UnitTypeId, text: x.UnitTypeName });
                UnitTypeMap[x.UnitTypeId] = x.UnitTypeName; // Hızlı erişim haritası
                optionsHtml += `<option value="${x.UnitTypeId}">${x.UnitTypeName}</option>`;
            });

            if (selectElement) {
                selectElement.innerHTML = optionsHtml;
            }
        }
    });
}

var selectCat = document.getElementById('CategoryId');
function GetCategories() {
    return $.ajax({
        url: '/api/CategoryApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Categories = data.List || [];
            if (!selectCat) return;

            var optionsHtml = '<option value="0">Seçiniz</option>';
            Categories.forEach(function (x) {
                optionsHtml += `<option value="${x.CategoryId}">${x.CategoryName}</option>`;
            });

            selectCat.innerHTML = optionsHtml;
        }
    });
}

var selectBrand = document.getElementById('BrandId');
function GetBrands() {
    return $.ajax({
        url: '/api/BrandApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Brands = data.List || [];
            if (!selectBrand) return;

            var optionsHtml = '<option value="0">Seçiniz</option>';
            Brands.forEach(function (x) {
                optionsHtml += `<option value="${x.BrandId}">${x.BrandName}</option>`;
            });

            selectBrand.innerHTML = optionsHtml;
        }
    });
}

var selectCurrency = document.getElementById('CurrencyId');
function GetCurrency() {
    return $.ajax({
        url: '/api/CurrencyApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            var Currency = data.List || [];
            if (!selectCurrency) return;

            var optionsHtml = '<option value="0">Seçiniz</option>';
            Currency.forEach(function (x) {
                optionsHtml += `<option value="${x.CurrencyId}">${x.CurrencyName}</option>`;
            });

            selectCurrency.innerHTML = optionsHtml;
        }
    });
}

/* ==========================================================================
   RESİM VE FİYAT LİSTELEME DETAYLARI
   ========================================================================== */
function GetImage(id) {
    $.ajax({
        url: '/api/ProductApi/GetImage?id=' + id,
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            dataImageList = data.List || [];
            GetImageTable(dataImageList);
        }
    });
}

function GetPrice(id) {
    $.ajax({
        url: '/api/ProductApi/GetPrice?id=' + id,
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            dataPriceList = data.List || [];
            GetPriceTable(dataPriceList);
        }
    });
}

function GetPriceTable(data) {
    table2.clear();
    table2.rows.add(data);
    table2.draw();
}

function GetImageTable(data) {
    dataImageList = data || [];
    renderImageTable();
}

/* ==========================================================================
   RESİM TABLOSU MANUEL RENDER (Toplu HTML Ekleme Optimizasyonu)
   ========================================================================== */
function renderImageTable() {
    var tbody = $("#imageTable tbody");

    if (!dataImageList || dataImageList.length === 0) {
        tbody.html('<tr><td colspan="3" class="text-center text-muted">Resim eklenmedi.</td></tr>');
        return;
    }

    var rowsHtml = '';
    dataImageList.forEach(function (item, index) {
        rowsHtml += `
            <tr>
                <td>
                    <img src="${item.ImageUrl}" class="img-thumbnail" width="80" alt="Resim" />
                </td>
                <td>
                    <input type="number" class="form-control form-control-sm img-queue-input" 
                           value="${item.Queue !== undefined ? item.Queue : 1}" 
                           style="width: 80px;" 
                           onchange="updateImageQueue(${index}, this.value)" 
                           onkeyup="updateImageQueue(${index}, this.value)" />
                </td>
                <td>
                    <button type="button" class="btn btn-danger btn-sm text-white" onclick="removeImageRow(${index})">
                        Sil
                    </button>
                </td>
            </tr>
        `;
    });

    tbody.html(rowsHtml);
}

function updateImageQueue(index, val) {
    if (dataImageList[index]) {
        dataImageList[index].Queue = parseInt(val, 10) || 0;
    }
}

function removeImageRow(index) {
    dataImageList.splice(index, 1);
    renderImageTable();
}

/* ==========================================================================
   FİYAT DATATABLE TANIMLAMASI
   ========================================================================== */
const table2 = new DataTable('#priceTable', {
    searching: false,
    paging: false,
    ordering: false,
    info: false,
    destroy: true,
    columns: [
        {
            "data": "UnitTypeId",
            render: function (x) {
                return UnitTypeMap[x] || '-'; // O(1) Anlık Harita Erişimi
            },
        },
        { "data": "Barcode" },
        { "data": "Count" },
        { "data": "Price" },
        {
            "data": "IsDefault",
            render: function (data) {
                return Chekbox(data, true);
            }
        },
        {
            "data": "ProductPriceId",
            render: function (data) {
                return '<a id="deleteRowPrice" class="btn btn-danger text-white pull-right">Sil</a>';
            }
        }
    ]
});

/* ==========================================================================
   TABLO SATIR EKLEME VE DOSYA YÜKLEME İŞLEMLERİ
   ========================================================================== */
function NewRowPrice() {
    var newData = {
        UnitTypeId: parseInt(document.getElementById("unitType").value) || 0,
        Count: parseInt(document.getElementById("Count").value) || 1,
        Price: parseFloat(document.getElementById("Price").value) || 0,
        IsDefault: parseInt(document.getElementById("IsDefault").value) || 0,
        Barcode: document.getElementById("Barcode").value || '',
        ProductPriceId: 0
    };
    table2.row.add(newData).draw(false);
}

function FileUpload(elem) {
    var fileInput = elem.files[0];
    if (!fileInput) return;

    var formData = new FormData();
    formData.append("file", fileInput);

    $.ajax({
        url: "/api/ProductApi/UploadImage",
        type: "POST",
        data: formData,
        contentType: false,
        processData: false,
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (res) {
            if (res.State || res.success) {
                var path = res.FilePath || res.filePath || res.Url || res.data;
                if (document.getElementById("ImageUrl")) {
                    document.getElementById("ImageUrl").innerText = path;
                }
                if (document.getElementById("ImgUrl")) {
                    document.getElementById("ImgUrl").value = path;
                }
            } else {
                Swal.fire("Hata", res.Message || "Resim yüklenemedi", "error");
            }
        },
        error: function (xhr) {
            console.error("Resim Yükleme Hatası:", xhr.responseText);
        }
    });
}

function NewRowImage() {
    var imgPath = "";

    if (document.getElementById("ImageUrl") && document.getElementById("ImageUrl").innerText) {
        imgPath = document.getElementById("ImageUrl").innerText;
    } else if (document.getElementById("ImgUrlText") && document.getElementById("ImgUrlText").value) {
        imgPath = document.getElementById("ImgUrlText").value;
    } else if (document.getElementById("ImgUrl") && document.getElementById("ImgUrl").value) {
        imgPath = document.getElementById("ImgUrl").value;
    } else if (document.getElementById("silinecekimg")) {
        imgPath = document.getElementById("silinecekimg").className;
    }

    if (!imgPath) {
        Swal.fire("Uyarı", "Lütfen bir resim seçin veya URL girin!", "warning");
        return;
    }

    var queueVal = 1;
    var queueInput = document.getElementById("Queue");
    if (queueInput && queueInput.value !== "") {
        queueVal = parseInt(queueInput.value, 10) || 1;
    }

    dataImageList.push({
        ImageUrl: imgPath,
        Queue: queueVal,
        ProductImageId: 0
    });

    renderImageTable();

    if (document.getElementById("ImageUrl")) document.getElementById("ImageUrl").innerText = "";
    if (document.getElementById("ImgUrlText")) document.getElementById("ImgUrlText").value = "";
    if (document.getElementById("ImgUrl")) document.getElementById("ImgUrl").value = "";
    if (document.getElementById("Queue")) document.getElementById("Queue").value = "";
}

if (document.querySelector('#addRowPrice')) {
    document.querySelector('#addRowPrice').addEventListener('click', NewRowPrice);
}
if (document.querySelector('#addRowImage')) {
    document.querySelector('#addRowImage').addEventListener('click', NewRowImage);
}

$("#priceTable tbody").on("click", "#deleteRowPrice", function () {
    let row = $(this).closest("tr");
    table2.row(row).remove().draw();
});

/* ==========================================================================
   FORM TEMİZLEME VE MODAL AÇILIŞ
   ========================================================================== */
function FormClean() {
    if (document.getElementById("ProductName")) document.getElementById("ProductName").value = "";
    if (document.getElementById("ProductCode")) document.getElementById("ProductCode").value = "";
    if (document.getElementById("CategoryId")) document.getElementById("CategoryId").value = 0;
    if (document.getElementById("CurrencyId")) document.getElementById("CurrencyId").value = 0;
    if (document.getElementById("Vat")) document.getElementById("Vat").value = 0;
    if (document.getElementById("StockQuantity")) document.getElementById("StockQuantity").value = 0;
    if (document.getElementById("BrandId")) document.getElementById("BrandId").value = 0;
    if (document.getElementById("Description")) document.getElementById("Description").value = "";
    if (document.getElementById("SilinecekResimler")) document.getElementById("SilinecekResimler").innerHTML = "";
    if (document.getElementById("ImgUrl")) document.getElementById("ImgUrl").value = "";
    if (document.getElementById("ImageUrl")) document.getElementById("ImageUrl").innerText = "";
    if (document.getElementById("Queue")) document.getElementById("Queue").value = "";

    dataImageList = [];
    renderImageTable();

    table2.clear().draw();
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Ürün Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Ürün Kaydet";
        FormClean();

        Data.ProductId = 0;
        document.getElementById("btnCopy").style.display = "none";
        IsCopy = false;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Ürün Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Ürün Güncelle";
        FormClean();

        Data.ProductId = obj.id;
        GetImage(obj.id);
        GetPrice(obj.id);

        var subdata = DataList.find(x => x.ProductId == obj.id);
        if (subdata) {
            document.getElementById("ProductName").value = subdata.ProductName || "";
            document.getElementById("ProductCode").value = subdata.ProductCode || "";
            document.getElementById("CategoryId").value = subdata.CategoryId || 0;
            document.getElementById("CurrencyId").value = subdata.CurrencyId || 0;
            document.getElementById("Vat").value = subdata.Vat || 0;
            document.getElementById("StockQuantity").value = subdata.StockQuantity || 0;
            document.getElementById("BrandId").value = subdata.BrandId || 0;
            document.getElementById("Description").value = subdata.Description || "";
            document.getElementById("IsActive").value = subdata.IsActive ? 1 : 0;
        }

        document.getElementById("btnCopy").style.display = "inline-block";
        IsCopy = false;
    }
}

/* ==========================================================================
   POST / PUT / COPY İŞLEMLERİ
   ========================================================================== */
function PostData() {
    var state = true;
    Data.ProductName = document.getElementById("ProductName").value;
    Data.IsActive = document.getElementById("IsActive").value == 1 ? true : false;
    Data.CategoryId = document.getElementById("CategoryId").value;
    Data.CurrencyId = document.getElementById("CurrencyId").value;
    Data.Vat = document.getElementById("Vat").value;
    Data.StockQuantity = document.getElementById("StockQuantity").value;
    Data.ProductCode = document.getElementById("ProductCode").value;
    Data.BrandId = document.getElementById("BrandId").value;
    Data.Description = document.getElementById("Description").value;

    var imgs = dataImageList || [];
    var pris = Array.from(table2.rows().data());

    Data.ProductImages = [];
    Data.ProductPrices = [];

    imgs.forEach(function (x) {
        Data.ProductImages.push({
            ProductImageId: IsCopy ? 0 : (x.ProductImageId || 0),
            ImageUrl: x.ImageUrl,
            ProductId: Data.ProductId,
            Queue: parseInt(x.Queue, 10) || 0
        });
    });

    pris.forEach(function (x) {
        Data.ProductPrices.push({
            ProductPriceId: IsCopy ? 0 : (x.ProductPriceId || 0),
            ProductId: Data.ProductId,
            Price: x.Price,
            Count: x.Count,
            UnitTypeId: x.UnitTypeId,
            IsDefault: x.IsDefault,
            Barcode: x.Barcode
        });
    });

    var targetUrl = (Data.ProductId == 0) ? '/api/ProductApi/Add' : '/api/ProductApi/Update';
    var targetType = (Data.ProductId == 0) ? 'Post' : 'Put';

    if (state) {
        $.ajax({
            url: targetUrl,
            type: targetType,
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                var actionName = Data.ProductId == 0 ? "Ekleme" : "Güncelleme";
                if (data.State) {
                    Swal.fire({
                        title: "Ürün " + actionName,
                        text: "İşlem başarıyla tamamlanmıştır.",
                        icon: "success",
                    }).then(() => {
                        PageLoad();
                    });
                } else {
                    Swal.fire({
                        title: "Ürün " + actionName,
                        text: "İşlem sırasında beklenmedik bir sorun oluştu.",
                        icon: "warning",
                    });
                }
            }
        });
    }
}

function CopyProduct() {
    Data.ProductId = 0;
    IsCopy = true;
    PostData();
}

/* ==========================================================================
   EXCEL AKTARIM VE TOPLU GÜNCELLEME İŞLEMLERİ
   ========================================================================== */
function uploadExcel() {
    var fileInput = $("#fileInput")[0];
    var resultDiv = $("#uploadResult");

    if (!fileInput || fileInput.files.length === 0) {
        resultDiv.html("<div class='text-danger'>⚠️ Lütfen bir dosya seçin!</div>");
        return;
    }

    var formData = new FormData();
    formData.append("file", fileInput.files[0]);

    $.ajax({
        url: "/api/ProductApi/ProductAllSet",
        type: "POST",
        data: formData,
        processData: false,
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: false,
        beforeSend: function () {
            resultDiv.html("<div class='text-info'>⏳ Yükleniyor...</div>");
        },
        success: function (response) {
            if (response.State) {
                resultDiv.html("<div class='text-success'>✅ Dosya başarıyla yüklendi!</div>");
            } else {
                var myModal = new bootstrap.Modal(document.getElementById('logModal'));
                var columns = [
                    { title: "Mesaj", data: null, render: x => x }
                ];
                DatatablesLoad("logtables", response.List, columns);
                myModal.show();
                resultDiv.html("<div class='text-warning'>Hatalı kayıtlar var!</div>");
                PageLoad();
            }
        },
        error: function (xhr, status, error) {
            console.error("Hata:", error);
            resultDiv.html("<div class='text-danger'>❌ Hata oluştu: " + xhr.responseText + "</div>");
        }
    });
}

function uploadExcel2() {
    var fileInput = $("#fileInput2")[0];
    var resultDiv = $("#uploadResult");

    if (!fileInput || fileInput.files.length === 0) {
        resultDiv.html("<div class='text-danger'>⚠️ Lütfen bir dosya seçin!</div>");
        return;
    }

    var formData = new FormData();
    formData.append("file", fileInput.files[0]);

    $.ajax({
        url: "/api/ProductApi/ProductPriceUpdateAll",
        type: "POST",
        data: formData,
        processData: false,
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: false,
        beforeSend: function () {
            resultDiv.html("<div class='text-info'>⏳ Yükleniyor...</div>");
        },
        success: function (response) {
            if (response.State) {
                resultDiv.html("<div class='text-success'>✅ Dosya başarıyla yüklendi!</div>");
            } else {
                var myModal = new bootstrap.Modal(document.getElementById('logModal'));
                var columns = [
                    { title: "Mesaj", data: null, render: x => x }
                ];
                DatatablesLoad("logtables", response.List, columns);
                myModal.show();
                resultDiv.html("<div class='text-warning'>Hatalı kayıtlar var!</div>");
                PageLoad();
            }
        },
        error: function (xhr, status, error) {
            console.error("Hata:", error);
            resultDiv.html("<div class='text-danger'>❌ Hata oluştu: " + xhr.responseText + "</div>");
        }
    });
}

function GetProductPriceTemplate() {
    $.ajax({
        url: "/api/ProductApi/GetTemplate",
        type: "Get",
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            if (response.State) {
                var link = document.createElement('a');
                link.href = "/uploads/UrunFiyatGuncelleme.xlsx";
                link.download = "UrunFiyatGuncelleme";
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }
        }
    });
}