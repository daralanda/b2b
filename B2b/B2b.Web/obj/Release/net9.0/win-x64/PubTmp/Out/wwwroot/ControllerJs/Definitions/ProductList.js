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
    ProductPrices:[]
}
var dataPriceList = [];
var dataImageList = [];
var UnitTypeList = [];
var Categories = [];
var Brands = [];

$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    GetUnitTypes();
    GetCategories();
    GetBrands();
    $.ajax({
        url: '/api/ProductApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "ProductCode" },
                { "data": "ProductName" },
                {
                    "data": "CategoryId",
                    render: function (data) {
                        var result= Categories.find(x => x.CategoryId == data);
                        return (result != null) ? result.CategoryName : "";
                    }
                },
                {
                    "data": "BrandId",
                    render: function (data) {
                        var result = Brands.find(x => x.BrandId == data);
                        return (result != null) ? result.BrandName : "";
                    }
                },
                { "data": "StockQuantity" },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "ProductId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables", DataList, columns);
            $('#exampleModal').modal('hide');
        },
        async: false
    });

    GetUnitTypes();

    GetCurrency();
}
function GetPriceTable(data) {
    table2.rows.add(data);
    table2.draw()
}
function GetImageTable(data) {
    table.rows.add(data);
    table.draw()
}
function NewRowPrice() {
    var newData = {
        UnitTypeId: parseInt(document.getElementById("unitType").value),
        Price:parseInt(document.getElementById("Price").value),
        IsDefault:parseInt(document.getElementById("IsDefault").value),
        ProductPriceId:0
    }
   // var sub = table2.rows().data();
    table2.row.add(newData).draw(false);
}
function NewRowImage() {
    var newData = {
        ImageUrl: document.getElementById("silinecekimg").className,
        Queue: parseInt(document.getElementById("Queue").value),
        ProductImageId: 0
    }
    table.row.add(newData).draw(false);
}
function FormClean() {
    document.getElementById("ProductName").value = "";
    document.getElementById("ProductCode").value = "";
    document.getElementById("CategoryId").value = 0;
    document.getElementById("CurrencyId").value = 0;
    document.getElementById("Vat").value = 0;
    document.getElementById("StockQuantity").value = s0;
    document.getElementById("BrandId").value = 0;
    document.getElementById("Description").value = "";
    $('#priceTable').DataTable().clear().draw();
    $('#imageTable').DataTable().clear().draw();
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Ürün Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Ürün Kaydet";
        FormClean();
        Data.ProductId = 0;

    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Ürün Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Ürün Güncelle";
        Data.ProductId = obj.id;
        GetImage(obj.id);
        GetPrice(obj.id);
        var subdata = DataList.find(x => x.ProductId == obj.id);
        document.getElementById("ProductName").value = subdata.ProductName;
        document.getElementById("ProductCode").value = subdata.ProductCode;
        document.getElementById("CategoryId").value = subdata.CategoryId;
        document.getElementById("CurrencyId").value = subdata.CurrencyId;
        document.getElementById("Vat").value = subdata.Vat;
        document.getElementById("StockQuantity").value = subdata.StockQuantity;
        document.getElementById("BrandId").value = subdata.BrandId;
        document.getElementById("Description").value = subdata.Description;

        if (subdata.IsActive) {
            document.getElementById("IsActive").value = 1;
        }
        else {
            document.getElementById("IsActive").value = 0;
        }
    }
    
}
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
    var imgs = Array.from(table.rows().data());
    var pris = Array.from(table2.rows().data());
    Data.ProductImages = [];
    Data.ProductPrices = [];
    imgs.forEach(function (x) {
        Data.ProductImages.push({
            ProductImageId: x.ProductImageId,
            ImageUrl: x.ImageUrl,
            ProductId: Data.ProductId,
            Queue: x.Queue
        })
    })
    pris.forEach(function (x) {
        Data.ProductPrices.push({
            ProductPriceId: x.ProductPriceId,
            ProductId: Data.ProductId,
            Price: x.Price,
            UnitTypeId: x.UnitTypeId,
            IsDefault: x.IsDefault,
        })
    })
    console.clear();
    console.log(Data);
    if (Data.ProductId == 0 && state==true)
    {
        $.ajax({
            url: '/api/ProductApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Ürün Ekleme",
                        text: "Ürün başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Ürün Ekleme",
                        text: "Ürün eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/ProductApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Ürün Güncelleme",
                        text: "Ürün başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Ürün Güncelleme",
                        text: "Ürün güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}

function GetImage(id) {
  
    
    $.ajax({
        url: '/api/ProductApi/GetImage?id='+id,
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            dataImageList = data.List;
            GetImageTable(dataImageList);
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
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
            dataPriceList = data.List;
            GetPriceTable(dataPriceList);
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
    });

}
const selectElement = document.getElementById('unitType');

function GetUnitTypes() {
    $.ajax({
        url: '/api/UnitTypeApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            selectElement.innerHTML = '';
            data.List.forEach(function (x) {
                UnitTypeList.push({ id: x.UnitTypeId, text: x.UnitTypeName });

                var opt = document.createElement('option');
                opt.value = x.UnitTypeId;
                opt.innerHTML = x.UnitTypeName;
                selectElement.appendChild(opt);
            })
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
    });
}
var selectCat = document.getElementById('CategoryId');
function GetCategories() {
    $.ajax({
        url: '/api/CategoryApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Categories = data.List;
            Categories.forEach(function (x) {
                var opt = document.createElement('option');
                opt.value = x.CategoryId;
                opt.innerHTML = x.CategoryName;
                selectCat.appendChild(opt);
            })
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
    });
}
var selectBrand = document.getElementById('BrandId');

function GetBrands() {
    $.ajax({
        url: '/api/BrandApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Brands = data.List;
            Brands.forEach(function (x) {
                var opt = document.createElement('option');
                opt.value = x.BrandId;
                opt.innerHTML = x.BrandName;
                selectBrand.appendChild(opt);
            })
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
    });
}
var selectCurrency = document.getElementById('CurrencyId');

function GetCurrency() {
    $.ajax({
        url: '/api/CurrencyApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Brands = data.List;
            Brands.forEach(function (x) {
                var opt = document.createElement('option');
                opt.value = x.CurrencyId;
                opt.innerHTML = x.CurrencyName;
                selectCurrency.appendChild(opt);
            })
        },
        error: function (xhr, status, error) {
            console.clear();
            console.log(xhr.responseText);
        },
        async: false
    });
}


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
                return UnitTypeList.find(y => y.id == x).text;
            },
        },
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
                return '<a id="deleteRowPrice"  class="btn btn-danger text-white pull-right">Sil</a>';
            }
        },
    ]
});
document.querySelector('#addRowPrice').addEventListener('click', NewRowPrice);


const table = new DataTable('#imageTable', {
    searching: false,
    paging: false,
    ordering: false,
    info: false,
    destroy: true,
    columns: [
        {
            "data": "ImageUrl",
            render: function (x) {
                return '<img src="'+x+'" class=""  width="80" height="auto">';
            }
        },
        { "data": "Queue" },
        {
            "data": "ProductImageId",
            render: function (data) {
                return '<a id="deleteRowImage"  class="btn btn-danger text-white pull-right">Sil</a>';
            }
        },
    ]
});
document.querySelector('#addRowImage').addEventListener('click', NewRowImage);


$("#priceTable tbody").on("click", "#deleteRowPrice", function () {
    let row = $(this).closest("tr");
    table2.row(row).remove().draw();
});
$("#imageTable tbody").on("click", "#deleteRowImage", function () {
    let row = $(this).closest("tr");
    table.row(row).remove().draw();
});

function uploadExcel() {
    var fileInput = $("#fileInput")[0];
    var resultDiv = $("#uploadResult");

    if (fileInput.files.length === 0) {
        resultDiv.html("<div class='text-danger'>⚠️ Lütfen bir dosya seçin!</div>");
        return;
    }

    var formData = new FormData();
    formData.append("file", fileInput.files[0]);

    // jQuery AJAX
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
                return;
            }
            else {
                var myModal = new bootstrap.Modal(document.getElementById('logModal'));
                console.log(response.List);
                var columns = [
                    { title: "Mesaj", data: null, render: x => x }
                ];
                DatatablesLoad("logtables", response.List, columns);
                myModal.show();
                resultDiv.html("<div class='text-warning'>Hatalı kayıtlar var!</div>");
                PageLoad();
                return;
            }

        },
        error: function (xhr, status, error) {
            console.error("Hata:", error);
            resultDiv.html("<div class='text-danger'>❌ Hata oluştu: " + xhr.responseText + "</div>");
        }
    });
}