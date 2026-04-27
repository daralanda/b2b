var DataList = [];
var Data = {
    PaymentSettingId: 0,
    Merchant: '',
    MerchantUser: '',
    MerchantPassword: '',
    BaseUrl: '',
    Currency: '',
    ReturnUrl: '',
    SessionType: '',
    IsDefault: true,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/PaymentApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "Merchant" },
                { "data": "BaseUrl" },
                { "data": "MerchantUser" },
                { "data": "Currency" },
                {
                    "data": "PaymentSettingId",
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
    document.getElementById("Merchant").value = "";
    document.getElementById("MerchantUser").value = "";
    document.getElementById("MerchantPassword").value = "";
    document.getElementById("BaseUrl").value = "";
    document.getElementById("Currency").value = "";
    document.getElementById("ReturnUrl").value = "";
    document.getElementById("SessionType").value = "";
    document.getElementById("IsActive").value = true;

}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Pos Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Pos Kaydet";
        FormClean();
        Data.PaymentSettingId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Pos Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Pos Güncelle";
        Data.PaymentSettingId = obj.id;
        var subdata = DataList.find(x => x.PaymentSettingId == obj.id);
        document.getElementById("Merchant").value = subdata.Merchant;
        document.getElementById("MerchantUser").value = subdata.MerchantUser;
        document.getElementById("MerchantPassword").value = subdata.MerchantPassword;
        document.getElementById("BaseUrl").value = subdata.BaseUrl;
        document.getElementById("Currency").value = subdata.Currency;
        document.getElementById("ReturnUrl").value = subdata.ReturnUrl;
        document.getElementById("SessionType").value = subdata.SessionType;
        document.getElementById("IsActive").value = subdata.IsDefault?1:0;
    }
    
}
function PostData() {
    var state = true;
    Data.Merchant = document.getElementById("Merchant").value;
    Data.MerchantUser = document.getElementById("MerchantUser").value;
    Data.MerchantPassword = document.getElementById("MerchantPassword").value;
    Data.BaseUrl = document.getElementById("BaseUrl").value ;
    Data.Currency = document.getElementById("Currency").value ;
    Data.ReturnUrl = document.getElementById("ReturnUrl").value ;
    Data.SessionType = document.getElementById("SessionType").value;
    Data.IsDefault = document.getElementById("IsActive").value == 1 ? true : false;
    if (Data.PaymentSettingId == 0 && state==true)
    {
        $.ajax({
            url: '/api/PaymentApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Pos Ekleme",
                        text: "Pos başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Pos Ekleme",
                        text: "Pos eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else if (state == true) {
        $.ajax({
            url: '/api/PaymentApi/Update',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Pos Güncelleme",
                        text: "Pos başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Pos Güncelleme",
                        text: "Pos güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}