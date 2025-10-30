var MainCategory = [];
var DataList = [];
var Data = {
    CategoryId: 0,
    CategoryName: '',
    IsActive: false,
    MainCategoryId: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/CategoryApi/GetAll',
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
            document.getElementById("MainCategoryId").innerHTML = "<option value='0'>Seçiniz</option>";
            MainCategory.forEach(function (x) {
                document.getElementById("MainCategoryId").innerHTML += "<option value='" + x.CategoryId + "'>" + x.CategoryName + "</option>";
            });
            var columns = [
                {
                    "data": "MainCategoryId",
                    render: function (t) {
                        return (t==0)?"":DataList.find(x => x.CategoryId == t).CategoryName;
                    }
                },
                { "data": "CategoryName" },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "CategoryId",
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
    document.getElementById("CategoryName").value = "";
    document.getElementById("IsActive").value = 0;
    document.getElementById("MainCategoryId").value = 0;
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Kategori Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Kategori Kaydet";
        FormClean();
        Data.CategoryId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Kategori Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Kategori Güncelle";
        Data.CategoryId = obj.id;
        var subdata = DataList.find(x => x.CategoryId == obj.id);

        document.getElementById("CategoryName").value = subdata.CategoryName;
        document.getElementById("MainCategoryId").value = subdata.MainCategoryId;
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
    Data.CategoryName = document.getElementById("CategoryName").value;
    Data.IsActive = document.getElementById("IsActive").value == 1 ? true : false;
    Data.MainCategoryId = document.getElementById("MainCategoryId").value;
    if (Data.CategoryId == 0 ) {
        $.ajax({
            url: '/api/CategoryApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Kategori Ekleme",
                        text: "Kategori başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kategori Ekleme",
                        text: "Kategori eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else {
        $.ajax({
            url: '/api/CategoryApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Kategori Güncelleme",
                        text: "Kategori başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kategori Güncelleme",
                        text: "Kategori güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }

}