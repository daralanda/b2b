var DataList = [];
var Data = {
    UnitTypeId: 0,
    UnitTypeName: '',
    UnitTypeCode: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/UnitTypeApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "UnitTypeName" },
                { "data": "UnitTypeCode" },

                {
                    "data": "UnitTypeId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> " +
                            "<a onclick='DeleteUnitType(this)' class='btn btn-xs btn-danger mr-1text-white edit'  id='" + data + "' ><i class='fas fa-trash-alt'></i></a> ";
s
                    }
                }
            ];
            DatatablesLoad("datatables", DataList, columns)
            $('#exampleModal').modal('hide');


        }
    });
}

function FormClean() {
    document.getElementById("UnitTypeName").value = "";
    document.getElementById("UnitTypeCode").value = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Birim Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Birim Kaydet";
        FormClean();
        Data.UnitTypeId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Birim Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Birim Güncelle";
        Data.UnitTypeId = obj.id;
        var subdata = DataList.find(x => x.UnitTypeId == obj.id);
        document.getElementById("UnitTypeName").value = subdata.UnitTypeName;
        document.getElementById("UnitTypeCode").value = subdata.UnitTypeCode;
    }
    
}
function PostData() {
    var state = true;
    Data.UnitTypeName = document.getElementById("UnitTypeName").value;
    Data.UnitTypeCode = document.getElementById("UnitTypeCode").value;

    if (Data.UnitTypeId == 0 && state==true)
    {
        $.ajax({
            url: '/api/UnitTypeApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Birim Ekleme",
                        text: "Birim başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Birim Ekleme",
                        text: "Birim eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/UnitTypeApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Birim Güncelleme",
                        text: "Birim başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Birim Güncelleme",
                        text: "Birim güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}

function DeleteUnitType(obj) {
    console.clear();
    $.ajax({
        url: '/api/UnitTypeApi/Remove?id=' +  obj.id,
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(Data),
        contentType: 'application/json',
        success: function (data) {
            if (data.State) {
                Swal.fire({
                    title: "Birim Silme",
                    text: "Birim silme başarılıdır.",
                    icon: "success",
                });
                PageLoad();
            }
            else {
                Swal.fire({
                    title: "Birim Silme",
                    text: "Birim silinirken beklenmedik bir sorun ile karşılaşıldı.",
                    icon: "warning",
                });
            }
        },
        error: function (x) {
            console.clear();
            console.log(x);
        }
    });
}