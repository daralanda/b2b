var DataList = [];
var Data = {
    AccountNumberId: 0,
    Logo: '',
    Bank: '',
    BranchCode: '',
    AccountNo: '',
    AccountName: '',
    Currency: '',
    IBAN: ''
}

$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/AccountNumberApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "Logo" },
                { "data": "Bank" },
                { "data": "BranchCode" },
                { "data": "AccountNo" },
                { "data": "AccountName" },
                { "data": "Currency" },
                { "data": "IBAN" },
                {
                    "data": "AccountNumberId",
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
    document.getElementById("Logo").value = "";
    document.getElementById("Bank").value = "";
    document.getElementById("BranchCode").value = "";
    document.getElementById("AccountNo").value = "";
    document.getElementById("Currency").value = "";
    document.getElementById("Queno").value = "";
    document.getElementById("IBAN").value = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Banka Hesabı Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Banka Hesabı Kaydet";
        FormClean();
        Data.AccountNumberId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Banka Hesabı Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Banka Hesabı Güncelle";
        Data.AccountNumberId = obj.id;
        var subdata = DataList.find(x => x.AccountNumberId == obj.id);
        document.getElementById("Logo").value = subdata.Logo;
        document.getElementById("Bank").value = subdata.Bank;
        document.getElementById("BranchCode").value = subdata.BranchCode;
        document.getElementById("AccountNo").value = subdata.AccountNo;
        document.getElementById("AccountName").value = subdata.AccountName;
        document.getElementById("Currency").value = subdata.Currency;
        document.getElementById("IBAN").value = subdata.IBAN;
    }
    
}
function PostData() {
    Data.Logo = document.getElementById("Logo").value;
    Data.Bank = document.getElementById("Bank").value;
    Data.BranchCode = document.getElementById("BranchCode").value;
    Data.AccountNo = document.getElementById("AccountNo").value;
    Data.AccountName = document.getElementById("AccountName").value;
    Data.Currency = document.getElementById("Currency").value;
    Data.IBAN = document.getElementById("IBAN").value;
    if (Data.AccountNumberId == 0)
    {
        $.ajax({
            url: '/api/AccountNumberApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Banka Hesabı Ekleme",
                        text: "Banka Hesabı başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Banka Hesabı Ekleme",
                        text: "Banka Hesabı eklenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });
    }
    else {
        $.ajax({
            url: '/api/AccountNumberApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Banka Hesabı Güncelleme",
                        text: "Banka Hesabı başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Banka Hesabı Güncelleme",
                        text: "Banka Hesabı güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            }
        });

    }
  
}