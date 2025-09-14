var DataList = [];
var Data = {
    BrandId: 0,
    BrandName: '',
    ImageUrl: '',
    Queno: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/BrandApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "BrandName" },
                {
                    "data": "ImageUrl",
                    render: function (x) {
                        return "<img src='" + x + "' class='data-tables-img' width='100px'/>"
                    }
                },

                {
                    "data": "BrandId",
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
    document.getElementById("BrandName").value = "";
    document.getElementById("ImageUrl").value = "";
    document.getElementById("Queno").value = 0;
    document.getElementById("SilinecekResimler").innerHTML = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Marka Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Marka Kaydet";
        FormClean();
        Data.BrandId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Marka Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Marka Güncelle";
        Data.BrandId = obj.id;
        var subdata = DataList.find(x => x.BrandId == obj.id);
        document.getElementById("BrandName").value = subdata.BrandName;
        document.getElementById("ImageUrl").value = subdata.ImageUrl;
        document.getElementById("Queno").value = subdata.Queno;
        document.getElementById("SilinecekResimler").innerHTML = '<img src="' + subdata.ImageUrl + '" class="' + subdata.ImageUrl + '" id="silinecekimg"  width="80" height="auto"/>';
    }
    
}
function PostData() {
    var state = false;
    Data.BrandName = document.getElementById("BrandName").value;
    Data.Queno = document.getElementById("Queno").value;
    if (document.getElementById("silinecekimg") != null) {
        Data.ImageUrl = document.getElementById("silinecekimg").className;
        state = true;
    }
    else {
        Swal.fire({
            title: "Lütfen Resim seçiniz !",
            text: "Resim yüklemeden kayıt yapılamaz!",
            icon: "error",
        });
        state = false;
    }
    if (Data.BrandId == 0 && state==true)
    {
        $.ajax({
            url: '/api/BrandApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Marka Ekleme",
                        text: "Marka başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Marka Ekleme",
                        text: "Marka eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/BrandApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Marka Güncelleme",
                        text: "Marka başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Marka Güncelleme",
                        text: "Marka güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}