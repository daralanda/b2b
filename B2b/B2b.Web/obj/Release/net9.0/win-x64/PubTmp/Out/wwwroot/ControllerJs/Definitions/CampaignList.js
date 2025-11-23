var MainCategory = [];
var DataList = [];
var Data = {
CampaignId:0,
CampaignName:"",
StartDate:"",
EndDate:"",
IsActive:0,
DiscountValue:0,
IsPercentage:0,
CategoryId:0,
}
$(document).ready(function () {
    GetCategories();
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/CampaignApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            data.List.forEach(function (y) {
                if (y.MainCategoryId == 0) {
                    MainCategory.push(y);
                }
            });
            console.clear();
            console.log(data);
            document.getElementById("CategoryId").innerHTML = "<option value='0'>Seçiniz</option>";
            MainCategory.forEach(function (x) {
                document.getElementById("CategoryId").innerHTML += "<option value='" + x.CategoryId + "'>" + x.CategoryName + "</option>";
            });

            
            
            
            
     
            var columns = [
                { "data": "CampaignName" },
                {
                    "data": "StartDate",
                   render: function (x) {
                        const tarih = new Date(x);

                        return tarih.toLocaleString('tr-TR', {
                            year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour12: false // 24 saat formatı için
                        });
                    }
                },
                {
                    "data": "EndDate",
                    render: function (x) {
                        const tarih = new Date(x);

                        return tarih.toLocaleString('tr-TR', {
                         year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour12: false // 24 saat formatı için
                        });
                    }
                },
                {
                    "data": "IsPercentage",
                    render: function (x) {
                        return x?"Oran olarak":"Tutar olarak "
                    }
                },
                { "data": "DiscountValue" },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "CampaignId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables", DataList, columns);
            $('#exampleModal').modal('hide');
        },
        error: function (x) { },
        async: false
    });
}

function FormClean() {
    document.getElementById("CampaignName").value = "";
    document.getElementById("IsActive").value = 0;
    document.getElementById("DiscountValue").value = 0;
    document.getElementById("EndDate").value = Date.now();
    document.getElementById("IsPercentage").value = 0;
    document.getElementById("StartDate").value = Date.now();
    document.getElementById("CategoryId").value = 0;
}

function GetCategories() {
    $.ajax({
        url: '/api/CategoryApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (x) {
            MainCategory = x.List;
        },
        error: function (x) { },
        async: false
    });
}
function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Kampanya Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Kampanya Kaydet";
        FormClean();
        Data.CampaignId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Kampanya Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Kampanya Güncelle";
        Data.CampaignId = obj.id;
        var subdata = DataList.find(x => x.CampaignId == obj.id);

        document.getElementById("CampaignName").value = subdata.CampaignName;
        document.getElementById("CategoryId").value = subdata.CategoryId;
        document.getElementById("DiscountValue").value = subdata.DiscountValue;
        document.getElementById("EndDate").value = subdata.EndDate.split('T')[0];
        document.getElementById("StartDate").value = subdata.StartDate.split('T')[0];
        if (subdata.IsPercentage) {
            document.getElementById("IsPercentage").value = 1;
        }
        else {
            document.getElementById("IsPercentage").value = 0;
        }
        if (subdata.IsActive) {
            document.getElementById("IsActive").value = 1;
        }
        else {
            document.getElementById("IsActive").value = 0;
        }
    }

}
function PostData() {
    var state = false;
    Data.CampaignName = document.getElementById("CampaignName").value;
    Data.IsActive = document.getElementById("IsActive").value == 1 ? true : false;
    Data.IsPercentage = document.getElementById("IsPercentage").value == 1 ? true : false;
    Data.StartDate = document.getElementById("StartDate").value;
    Data.EndDate = document.getElementById("EndDate").value;
    Data.DiscountValue = document.getElementById("DiscountValue").value;
    Data.CategoryId = document.getElementById("CategoryId").value;

    if (Data.CampaignId == 0) {
        $.ajax({
            url: '/api/CampaignApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Kampanya Ekleme",
                        text: "Kampanya başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kampanya Ekleme",
                        text: "Kampanya eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else {
        $.ajax({
            url: '/api/CampaignApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Kampanya Güncelleme",
                        text: "Kampanya başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kampanya Güncelleme",
                        text: "Kampanya güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }

}