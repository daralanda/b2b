var DataList = [];
var Data = {
    ExchangeId: 0,
    Buying: 0,
    Selling: 0,
    CurrencyId: 0
};
var Currency = [];

$(document).ready(function () {
    // Önce döviz tiplerini çekip, işlem bittiğinde sayfa verilerini yüklüyoruz
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
                    "render": function (data) {
                        var item = Currency.find(x => x.CurrencyId == data);
                        return item ? item.CurrencyName : "-";
                    }
                },
                { "data": "Buying" },
                { "data": "Selling" },
                {
                    "data": "TransactionDate",
                    "render": function (data, type, row) {
                        // DataTables sıralama (sort) yaparken sayısal zaman damgası (timestamp) kullanır
                        if (type === 'sort' || type === 'type') {
                            return data ? new Date(data).getTime() : 0;
                        }
                        // Ekranda gösterilecek görsel format
                        return TarihFormat(data);
                    }
                },
                {
                    "data": "ExchangeId",
                    "render": function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1 text-white edit' id='" + data + "' data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a>";
                    }
                }
            ];

            // 3. indeks (TransactionDate) sütununa göre azalan (desc) sıralama
            DatatablesLoadOrder("datatables", DataList, columns, [[3, "desc"]]);
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
        if (subdata) {
            document.getElementById("Buying").value = subdata.Buying;
            document.getElementById("Selling").value = subdata.Selling;
            document.getElementById("CurrencyId").value = subdata.CurrencyId;
        }
    }
}

function PostData() {
    var state = true;
    Data.Buying = document.getElementById("Buying").value;
    Data.Selling = document.getElementById("Selling").value;
    Data.CurrencyId = document.getElementById("CurrencyId").value;

    if (Data.ExchangeId == 0 && state == true) {
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
        async: false,
        success: function (data) {
            Currency = data.List;
            var options = "<option value=''>Lütfen Seçiniz</option>";
            Currency.forEach(function (x) {
                options += "<option value='" + x.CurrencyId + "'>" + x.CurrencyName + "</option>";
            });
            document.getElementById("CurrencyId").innerHTML = options;
        }
    });
}

function AutoUpdate() {
    $.ajax({
        url: '/api/ExchangeApi/AutoUpdate',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        async: false,
        success: function (data) {
            if (data.State) {
                Swal.fire({
                    title: "Döviz Kuru",
                    text: "Döviz Kuru başarıyla çekilmiştir.",
                    icon: "success",
                });
                PageLoad();
            }
            else {
                Swal.fire({
                    title: "Döviz Kuru",
                    text: "Döviz Kuru çekilirken hata oluşmuştur.",
                    icon: "warning",
                });
            }
        },
        error: function (data) { }
    });
}