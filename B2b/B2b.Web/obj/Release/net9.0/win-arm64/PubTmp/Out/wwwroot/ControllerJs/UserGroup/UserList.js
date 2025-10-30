var modalTab = document.getElementById("exampleModal");
var RoleList = [], UserList = [];
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/UserApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            RoleList = data.roleData;
            document.getElementById("role").innerHTML = "";
            RoleList.forEach(function (element) {
                document.getElementById("role").innerHTML += "<option value='" + element.RoleId + "'>" + element.RoleName + "</option>"
            });
            UserList = data.data;
            var columns = [
                { "data":"Email"},
                { "data": "FirstName" },
                { "data": "LastName" },
                {
                    "data": "RoleId",
                    render: function (data) {
                        return RoleList.find(p => p.RoleId == data).RoleName;
                    }
                },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "UserId",
                    render: function (data) {
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1text-white edit'  id='" + data + "'  data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables",data.data, columns)
        },
        error: function (x) {
            console.clear();
            console.log(x);
        }
    });
}
function btnClick(obj) {
    var User = {
        "UserId": obj.id,
        "Email":"",
        "FirstName": "",
        "LastName": "",
        "Password": "",
        "IsActive": "",
        "RoleId": "",
        "Phone":""
    };
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Kullanıcı Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Kullanıcı Kaydet";
        FormClean();
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Kullanıcı Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Kullanıcı Güncelle";
        FormFill(obj.id);
    }
}
var UserId = 0;
function FormClean() {
    document.getElementById("firstname").value = "";
    document.getElementById("email").value = "";
    document.getElementById("lastname").value = "";
    document.getElementById("password").value = "";
    document.getElementById("isactive").value = 0;
    document.getElementById("role").selected = 0;
    document.getElementById("phone").value ="";
}
function FormFill(obj) {
    UserId = obj;
    UserList.forEach(function (element) {
        if (element.UserId == obj) {
            document.getElementById("firstname").value = element.FirstName;
            document.getElementById("email").value = element.Email;
            document.getElementById("lastname").value = element.LastName;
            document.getElementById("password").value = element.Password;
            document.getElementById("phone").value = element.Phone;
            document.getElementById("role").value = element.RoleId;
            if (element.IsActive) {
                document.getElementById("isactive").value = 1;
            }
            else {
                document.getElementById("isactive").value = 0;
            }
        }
    })
}
function PostData(obj) {
    var IsActive = false;
    if (document.getElementById("isactive").value==1) {
        IsActive = true;
    }
    var User = {
        "UserId": UserId,
        "FirstName": document.getElementById("firstname").value,
        "Email": document.getElementById("email").value,
        "LastName": document.getElementById("lastname").value,
        "Password": document.getElementById("password").value,
        "IsActive": IsActive ,
        "RoleId": document.getElementById("role").value,
        "Phone": document.getElementById("phone").value,
    };
    if (obj.innerHTML =="Kullanıcı Güncelle") {
        $.ajax({
            url: '/api/UserApi/Update',
            type: 'Post',
            dataType: 'Json',
            contentType: 'application/json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(User),
            success: function (data) {
                if (data.state) {
                    modalTab.style = "display:none;";
                    document.getElementsByClassName("modal-backdrop")[0].hidden = true;
                    Swal.fire({
                        title: "Kullanıcı Güncelleme!",
                        text: "Kullanıcınız başarıyla güncellenmiştir.",
                        type: "success"
                    })
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kullanıcı Güncelleme!",
                        text: "Kullanıcını bilgilerini lütfen kontrol ediniz. İşlem tamamlanamadı",
                        type: "warning"
                    })
                }
            },
            error: function (x) {
                console.log(x);
            }
        });
    }
    else {
        $.ajax({
            url: '/api/UserApi/Add',
            type: 'Post',
            dataType: 'Json',
            contentType: 'application/json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(User),
            success: function (data) {
                if (data.state) {
                    modalTab.style = "display:none;";
                    document.getElementsByClassName("modal-backdrop")[0].hidden = true;
                    Swal.fire({
                        title: "Kullanıcı Kaydetme!",
                        text: "Kullanıcınız başarıyla kaydedildi.",
                        type: "success"
                    })
                    PageLoad();
                }
                else {
                    Swal.fire({
                        title: "Kullanıcı Kaydetme!",
                        text: "Kullanıcını bilgilerini lütfen kontrol ediniz. İşlem tamamlanamadı",
                        type: "warning"
                    })
                }
            }
        });
    }
}
