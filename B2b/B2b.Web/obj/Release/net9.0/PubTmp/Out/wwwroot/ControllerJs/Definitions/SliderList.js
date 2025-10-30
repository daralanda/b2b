var DataList = [];
var Data = {
    SliderId: 0,
    SliderName: '',
    SliderUrl: '',
    Queno: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/SliderApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "SliderName" },
                {
                    "data": "SliderUrl",
                    render: function (x) {
                        return "<img src='" + x + "' class='data-tables-img' width='100px'/>"
                    }
                },

                {
                    "data": "SliderId",
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
    document.getElementById("SliderName").value = "";
    document.getElementById("SliderUrl").value = "";
    document.getElementById("Queno").value = 0;
    document.getElementById("SilinecekResimler").innerHTML = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Slider Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Slider Kaydet";
        FormClean();
        Data.SliderId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Slider Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Slider Güncelle";
        Data.SliderId = obj.id;
        var subdata = DataList.find(x => x.SliderId == obj.id);
        document.getElementById("SliderName").value = subdata.SliderName;
        document.getElementById("ImageUrl").value = subdata.SliderUrl;
        document.getElementById("Queno").value = subdata.Queno;
        document.getElementById("SilinecekResimler").innerHTML = '<img src="' + subdata.SliderUrl + '" class="' + subdata.SliderUrl + '" id="silinecekimg"  width="80" height="auto"/>';
    }
    
}
function PostData() {
    var state = false;
    Data.SliderName = document.getElementById("SliderName").value;
    Data.Queno = document.getElementById("Queno").value;
    if (document.getElementById("silinecekimg") != null) {
        Data.SliderUrl = document.getElementById("silinecekimg").className;
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
    if (Data.SliderId == 0 && state==true)
    {
        $.ajax({
            url: '/api/SliderApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Slider Ekleme",
                        text: "Slider başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Slider Ekleme",
                        text: "Slider eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/SliderApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Slider Güncelleme",
                        text: "Slider başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Slider Güncelleme",
                        text: "Slider güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}