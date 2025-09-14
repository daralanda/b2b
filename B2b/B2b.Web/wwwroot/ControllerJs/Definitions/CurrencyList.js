var DataList = [];
var Data = {
    CurrencyId: 0,
    CurrencyName: '',
    CurrencyCode: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/CurrencyApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "CurrencyName" },
                { "data": "CurrencyCode" },

                {
                    "data": "CurrencyId",
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
    document.getElementById("CurrencyName").value = "";
    document.getElementById("CurrencyCode").value = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Para Birimi Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Para Birimi Kaydet";
        FormClean();
        Data.CurrencyId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Para Birimi Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Para Birimi Güncelle";
        Data.CurrencyId = obj.id;
        var subdata = DataList.find(x => x.CurrencyId == obj.id);
        document.getElementById("CurrencyName").value = subdata.CurrencyName;
        document.getElementById("CurrencyCode").value = subdata.CurrencyCode;
    }
    
}
function PostData() {
    var state = true;
    Data.CurrencyName = document.getElementById("CurrencyName").value;
    Data.CurrencyCode = document.getElementById("CurrencyCode").value;

    if (Data.CurrencyId == 0 && state==true)
    {
        $.ajax({
            url: '/api/CurrencyApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Para Birimi Ekleme",
                        text: "Para Birimi başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Para Birimi Ekleme",
                        text: "Para Birimi eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/CurrencyApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Para Birimi Güncelleme",
                        text: "Para Birimi başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Para Birimi Güncelleme",
                        text: "Para Birimi güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}