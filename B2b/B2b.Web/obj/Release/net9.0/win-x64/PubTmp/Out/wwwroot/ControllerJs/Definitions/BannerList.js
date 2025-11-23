var DataList = [];
var Data = {
    BannerId: 0,
    BannerName: '',
    ImageUrl: '',
    BannerUrl: '',
    IsActive: false,
    BannerType: 0,
    Queno: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/BannerApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "BannerName" },
                {
                    "data": "ImageUrl",
                    render: function (x) {
                        return "<img src='" + x + "' class='data-tables-img' width='100px'/>"
                    }
                },
                {
                    "data": "BannerUrl",
                    render: function (x) {
                        return "<a href='" + x + "' class='btn btn-success' target='_balnk'>Linki Ac</a>"
                    }                },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "BannerId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables", DataList, columns);
            $('#exampleModal').modal('hide');
        }
    });
}

function FormClean() {
    document.getElementById("BannerName").value = "";
    document.getElementById("ImageUrl").value = "";
    document.getElementById("BannerUrl").value = "";
    document.getElementById("IsActive").value = 0;
    document.getElementById("BannerType").value = 0;
    document.getElementById("Queno").value = 0;
    document.getElementById("SilinecekResimler").innerHTML = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Banner Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Banner Kaydet";
        FormClean();
        Data.BannerId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Banner Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Banner Güncelle";
        Data.BannerId = obj.id;
        var subdata = DataList.find(x => x.BannerId == obj.id);
        document.getElementById("BannerName").value = subdata.BannerName;
        document.getElementById("ImageUrl").value = subdata.ImageUrl;
        document.getElementById("BannerUrl").value = subdata.BannerUrl;
        document.getElementById("BannerType").value = subdata.BannerType;
        document.getElementById("Queno").value = subdata.Queno;
        document.getElementById("SilinecekResimler").innerHTML = '<img src="' + subdata.ImageUrl + '" class="' + subdata.ImageUrl + '" id="silinecekimg"  width="80" height="auto"/>';

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
    Data.BannerName = document.getElementById("BannerName").value;
    Data.BannerUrl = document.getElementById("BannerUrl").value;
    Data.IsActive = document.getElementById("IsActive").value == 1 ? true : false;
    Data.BannerType = document.getElementById("BannerType").value;
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
    if (Data.BannerId == 0 && state==true)
    {
        $.ajax({
            url: '/api/BannerApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Banner Ekleme",
                        text: "Banner başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Banner Ekleme",
                        text: "Banner eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/BannerApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Banner Güncelleme",
                        text: "Banner başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Banner Güncelleme",
                        text: "Banner güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}