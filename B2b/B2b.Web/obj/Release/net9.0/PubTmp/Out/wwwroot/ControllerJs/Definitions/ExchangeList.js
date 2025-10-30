var DataList = [];
var Data = {
    ExchangeId: 0,
    Buying: 0,
    Selling: 0,
    CurrencyId: 0
}
var Currency = [];
$(document).ready(function () {
    GetCurrency();
    PageLoad();
    
});
function PageLoad() {
    $.ajax({
        url: '/api/ExchangeApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                {
                    "data": "CurrencyId",
                    render: function (data) {
                        return Currency.find(x => x.CurrencyId == data).CurrencyName;
                    }
                },
                { "data": "Buying" },
                { "data": "Selling" },
                { "data": "TransactionDate" },
                {
                    "data": "ExchangeId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables", DataList, columns)
            $('#exampleModal').modal('hide');


        }
    });
}

function FormClean() {
    document.getElementById("Buying").value = "";
    document.getElementById("Selling").value = "";
    document.getElementById("CurrencyId").value = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Döviz Kuru Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Döviz Kuru Kaydet";
        FormClean();
        Data.ExchangeId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Döviz Kuru Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Döviz Kuru Güncelle";
        Data.ExchangeId = obj.id;
        var subdata = DataList.find(x => x.ExchangeId == obj.id);
        document.getElementById("Buying").value = subdata.Buying;
        document.getElementById("Selling").value = subdata.Selling;
        document.getElementById("CurrencyId").value = subdata.CurrencyId;
    }
    
}
function PostData() {
    var state = true;
    Data.Buying = document.getElementById("Buying").value;
    Data.Selling = document.getElementById("Selling").value;
    Data.CurrencyId = document.getElementById("CurrencyId").value;
  

    if (Data.ExchangeId == 0 && state == true)
    {
        $.ajax({
            url: '/api/ExchangeApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Döviz Kuru Ekleme",
                        text: "Döviz Kuru başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Döviz Kuru Ekleme",
                        text: "Döviz Kuru eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/ExchangeApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Döviz Kuru Güncelleme",
                        text: "Döviz Kuru başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Döviz Kuru Güncelleme",
                        text: "Döviz Kuru güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}
function GetCurrency() {
    $.ajax({
        url: '/api/CurrencyApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Currency = data.List;
            var options = "<option value=''>Lütfen Seçiniz</option>";
            Currency.forEach(function (x) {
                options += "<option value='" + x.CurrencyId + "'>" + x.CurrencyName + "</option>";
            });
            document.getElementById("CurrencyId").innerHTML = options;
        },
        async: false
    });
}
