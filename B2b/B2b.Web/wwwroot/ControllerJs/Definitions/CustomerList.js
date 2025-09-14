var DataList = [];
var Districts = [];
var Data = {
    CustomerId: 0,
    CustomerName: '',
    TaxOffice: '',
    TaxNo: '',
    Address: '',
    DistrictId: 0,
    CityId: 0,
    Mail: '',
    Phone: '',
    IsIndividual: 0,
    IsActive: 0,
}
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    GetCities();
    GetDistricts();
    $.ajax({
        url: '/api/CustomerApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "CustomerName" },
                { "data": "TaxOffice" },
                { "data": "TaxNo" },
                { "data": "CityId" },
                {
                    "data": "IsActive",
                    render: function (data) {
                        return Chekbox(data, true);
                    }
                },
                {
                    "data": "CustomerId",
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
    document.getElementById("CustomerName").value = "";
    document.getElementById("TaxOffice").value = "";
    document.getElementById("TaxNo").value = "";
    document.getElementById("Address").value = "";
    document.getElementById("DistrictId").value = 0;
    document.getElementById("CityId").value = 0;
    document.getElementById("Mail").value = "";
    document.getElementById("Phone").value = "";
    document.getElementById("IsIndividual").value = 0;
    document.getElementById("IsActive").value = 0;

}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Müşteri Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Müşteri Kaydet";
        FormClean();
        Data.CustomerId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Müşteri Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Müşteri Güncelle";
        Data.CustomerId = obj.id;
        var subdata = DataList.find(x => x.CustomerId == obj.id);
        document.getElementById("CustomerName").value = subdata.CustomerName;
        document.getElementById("TaxOffice").value = subdata.TaxOffice;
        document.getElementById("TaxNo").value = subdata.TaxNo;
        document.getElementById("Address").value = subdata.Address;
        
        document.getElementById("CityId").value = subdata.CityId;
        CityChange();
        document.getElementById("DistrictId").value = subdata.DistrictId;
        document.getElementById("Mail").value = subdata.Mail;
        document.getElementById("Phone").value = subdata.Phone;
        if (subdata.IsIndividual) {
            document.getElementById("IsIndividual").value = 1;
        }
        else {
            document.getElementById("IsIndividual").value = 0;
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
    Data.CustomerName = document.getElementById("CustomerName").value;
    Data.TaxOffice = document.getElementById("TaxOffice").value;
    Data.TaxNo = document.getElementById("TaxNo").value;
    Data.Address = document.getElementById("Address").value;
    Data.DistrictId = document.getElementById("DistrictId").value;
    Data.CityId = document.getElementById("CityId").value;
    Data.Mail = document.getElementById("Mail").value;
    Data.Phone = document.getElementById("Phone").value;
    Data.IsActive = document.getElementById("IsActive").value == 1 ? true : false;
    Data.MüşteriType = document.getElementById("IsIndividual").value == 1 ? true : false;
    if (Data.CustomerId == 0)
    {
        $.ajax({
            url: '/api/CustomerApi/Add',
            type: 'Post',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data), 
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Müşteri Ekleme",
                        text: "Müşteri başarıyla eklenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Müşteri Ekleme",
                        text: "Müşteri eklenirken beklenmedik bir sorun ile karşılaşıldı.",
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
    else  {
        $.ajax({
            url: '/api/CustomerApi/Update',
            type: 'Put',
            dataType: 'Json',
            headers: { 'Authorization': localStorage.getItem("token") },
            data: JSON.stringify(Data),
            contentType: 'application/json',
            success: function (data) {
                if (data.State) {
                    Swal.fire({
                        title: "Müşteri Güncelleme",
                        text: "Müşteri başarıyla güncellenmiştir.",
                        icon: "success",
                    });
                    PageLoad();  
                }
                else {
                    Swal.fire({
                        title: "Müşteri Güncelleme",
                        text: "Müşteri güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                        icon: "warning",
                    });
                }
            },
            error: function (x) {
                console.clear();
                console.log(x);
                console.log(Data);
            }
        });

    }
  
}

function GetCities() {
    $.ajax({
        url: '/api/CustomerApi/GetAllCities',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            var select = document.getElementById('CityId');
            if (data.State) {
                data.List.forEach(x => {
                    var opt = document.createElement('option');
                    opt.value = x.CityId;
                    opt.innerHTML = x.CityName;
                    select.appendChild(opt);
                })
            }
        },
        error: function (x) { },
        async: false
    });
}

function GetDistricts() {
    $.ajax({
        url: '/api/CustomerApi/GetAllDistricts',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            if (data.State) {
                Districts = data.List;
            }
        },
        error: function (x) { },
        async: false
    });
}
function CityChange() {
    var id = document.getElementById("CityId").value;
    var disct = Districts.filter(x => x.CityId == id);
    var select = document.getElementById('DistrictId');
    console.clear();
    select.innerHTML = "<option value='0'>Seçiniz</option>";
    disct.forEach(x => {
        var opt = document.createElement('option');
        opt.value = x.DistrictId;
        opt.innerHTML = x.DistrictName;
        select.appendChild(opt);
    })
}