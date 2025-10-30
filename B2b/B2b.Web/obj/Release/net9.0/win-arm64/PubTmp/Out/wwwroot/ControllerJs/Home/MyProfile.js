var city = [];
var district = [];
$(document).ready(function () {
    formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };
    GetCarts();

});
function GetCarts() {
    $.ajax({
        url: '/api/UserInfoApi/GetById',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            document.getElementById("CustomerName").value = data.data.CustomerName;
            document.getElementById("TaxNo").value = data.data.TaxNo;
            document.getElementById("TaxOffice").value = data.data.TaxOffice;
            document.getElementById("Email").value = data.data.Email;
            document.getElementById("Phone").value = data.data.Phone;
            document.getElementById("Address").value = data.data.Address;
            document.getElementById("FirstName").value = data.data.FirstName;
            document.getElementById("LastName").value = data.data.LastName;
            document.getElementById("Password").value = data.data.Password;
            document.getElementById("IsIndividual").checked = data.data.IsIndividual;
            district = data.Districts;
            city = data.Cities;
            document.getElementById("CityId").innerHTML = '<option value="0">Seçiniz</option>';
            data.Cities.forEach(function (x) {
                document.getElementById("CityId").innerHTML += '<option value="' + x.CityId + '">' + x.CityName + '</option>';
            })
            document.getElementById("CityId").value = data.data.CityId;
            CityChange();
            document.getElementById("DistrictId").value = data.data.DistrictId;

        },
        error: function (x) { },
        async: false
    });
}

function CityChange() {
    document.getElementById("DistrictId").innerHTML = '<option value="0">Seçiniz</option>';
 
    var newCityId = document.getElementById("CityId").value;
    var newDis = district.filter(x => x.CityId == newCityId);
    newDis.forEach(function (x) {
        document.getElementById("DistrictId").innerHTML += '<option value="' + x.DistrictId + '">' + x.DistrictName + '</option>';
    })
}

function SaveInfo() {
    var data = {
        CustomerName: document.getElementById("CustomerName").value,
        TaxNo: document.getElementById("TaxNo").value,
        TaxOffice: document.getElementById("TaxOffice").value,
        Email: document.getElementById("Email").value,
        Phone: document.getElementById("Phone").value,
        Address: document.getElementById("Address").value,
        FirstName: document.getElementById("FirstName").value,
        LastName: document.getElementById("LastName").value,
        Password: document.getElementById("Password").value,
        IsIndividual: (document.getElementById("IsIndividual").checked)?1:0,
        DistrictId: document.getElementById("DistrictId").value,
        CityId: document.getElementById("CityId").value
    }
    $.ajax({
        url: '/api/UserInfoApi/Update',
        type: 'Post',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(data),
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            Swal.fire({
                title: "Bilgi Güncelleme",
                text: "Kullanıcı Bilgileriniz Güncellenmiştir.",
                icon: "success"
            });
        },
        error: function (data) { },
        async: false
    });
}