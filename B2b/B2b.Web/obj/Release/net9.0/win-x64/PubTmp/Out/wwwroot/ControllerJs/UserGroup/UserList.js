var modalTab = document.getElementById("exampleModal");
var RoleList = [], UserList = [], Cities = [], Districts = [];
var BrandList = [];

$(document).ready(function () {
    PageLoad();
    GetCities();
    loadBrandsDropdown(); // Sayfa yüklendiğinde markaları çekmesi için aktif edildi
});

function PageLoad() {
    $.ajax({
        url: '/api/UserApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            RoleList = data.roleData || [];
            document.getElementById("role").innerHTML = "";
            RoleList.forEach(function (element) {
                document.getElementById("role").innerHTML += "<option value='" + element.RoleId + "'>" + element.RoleName + "</option>";
            });
            UserList = data.data || [];
            var columns = [
                { "data": "Email" },
                { "data": "FirstName" },
                { "data": "LastName" },
                {
                    "data": "RoleId",
                    render: function (data) {
                        var role = RoleList.find(p => p.RoleId == data);
                        return role ? role.RoleName : '';
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
                        return "<a onclick='btnClick(this)' class='btn btn-xs btn-info mr-1 text-white edit' id='" + data + "' data-bs-toggle='modal' data-bs-target='#exampleModal'><i class='fas fa-pencil-alt'></i></a> ";
                    }
                }
            ];
            DatatablesLoad("datatables", UserList, columns);
        },
        error: function (x) {
            console.clear();
            console.log(x);
        }
    });
}

function btnClick(obj) {
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
    UserId = 0;
    document.getElementById("firstname").value = "";
    document.getElementById("email").value = "";
    document.getElementById("lastname").value = "";
    document.getElementById("password").value = "";
    document.getElementById("isactive").value = 0;
    document.getElementById("role").selectedIndex = 0;
    document.getElementById("phone").value = "";
    document.getElementById("discountRate").value = "";
    document.getElementById("IsIndividual").value = "";
    document.getElementById("CustomerName").value = "";
    document.getElementById("TaxNo").value = "";
    document.getElementById("TaxOffice").value = "";
    document.getElementById("Address").value = "";
    document.getElementById("DistrictId").selectedIndex = 0;
    document.getElementById("CityId").selectedIndex = 0;

    // Marka indirim tablosunu temizle
    $('#brandDiscountTableBody').html('');
}

function FormFill(obj) {

    FormClean(); // Önce formu temizle
    UserId = obj;
    var currentUser = UserList.find(element => element.UserId == obj);
    if (!currentUser) return;

    document.getElementById("firstname").value = currentUser.FirstName || "";
    document.getElementById("email").value = currentUser.Email || "";
    document.getElementById("lastname").value = currentUser.LastName || "";
    document.getElementById("password").value = currentUser.Password || "";
    document.getElementById("phone").value = currentUser.Phone || "";
    document.getElementById("role").value = currentUser.RoleId || "";
    document.getElementById("discountRate").value = currentUser.DiscountRate || "";
    document.getElementById("IsIndividual").value = currentUser.IsIndividual ? 1 : 0;
    document.getElementById("CustomerName").value = currentUser.CustomerName || "";
    document.getElementById("TaxNo").value = currentUser.TaxNo || "";
    document.getElementById("TaxOffice").value = currentUser.TaxOffice || "";
    document.getElementById("Address").value = currentUser.Address || "";
    document.getElementById("CityId").value = currentUser.CityId || "0";
    CityChange();
    document.getElementById("DistrictId").value = currentUser.DistrictId || "0";

    document.getElementById("isactive").value = currentUser.IsActive ? 1 : 0;

    // Kullanıcıya ait marka indirimlerini doldur (UserList içindeki veriden güvenli şekilde)
    if (currentUser.UserBrandDiscounts && currentUser.UserBrandDiscounts.length > 0) {
        currentUser.UserBrandDiscounts.forEach(function (discountItem) {
            var brandObj = BrandList.find(x => x.BrandId == discountItem.BrandId);
            var brandName = brandObj ? brandObj.BrandName : (discountItem.BrandName || ("Marka #" + discountItem.BrandId));

            AddBrandDiscountRow(discountItem.BrandId, brandName, discountItem.DiscountRate);
        });
    } else {
        // Eğer GetAll içinde UserBrandDiscounts gelmiyorsa ayrı endpoint'ten çekebiliriz:
        loadUserExistingDiscounts(UserId);
    }
}

function PostData(obj) {
    var IsActive = (document.getElementById("isactive").value == 1);

    // Tablodaki marka indirimlerini dizi olarak topla
    var userBrandDiscounts = [];
    $('#brandDiscountTableBody tr').each(function () {
        var brandId = $(this).find('.brand-id').val();
        var discountRate = $(this).find('.brand-discount-input').val();

        if (brandId) {
            userBrandDiscounts.push({
                "UserId": UserId || 0,
                "BrandId": brandId,
                "DiscountRate": parseFloat(discountRate) || 0
            });
        }
    });

    var User = {
        "UserId": UserId,
        "FirstName": document.getElementById("firstname").value,
        "Email": document.getElementById("email").value,
        "LastName": document.getElementById("lastname").value,
        "Password": document.getElementById("password").value,
        "IsActive": IsActive,
        "RoleId": document.getElementById("role").value,
        "Phone": document.getElementById("phone").value,
        "DiscountRate": parseFloat(document.getElementById("discountRate").value) || 0,
        "IsIndividual": document.getElementById("IsIndividual").value == 1,
        "CustomerName": document.getElementById("CustomerName").value,
        "TaxNo": document.getElementById("TaxNo").value,
        "TaxOffice": document.getElementById("TaxOffice").value,
        "Address": document.getElementById("Address").value,
        "CityId": parseInt(document.getElementById("CityId").value) || 0,
        "DistrictId": parseInt(document.getElementById("DistrictId").value) || 0,
        "UserBrandDiscounts": userBrandDiscounts
    };

    var isUpdate = (obj.innerHTML.includes("Güncelle"));
    var url = isUpdate ? '/api/UserApi/Update' : '/api/UserApi/Add';
    var successTitle = isUpdate ? "Kullanıcı Güncelleme!" : "Kullanıcı Kaydetme!";
    var successText = isUpdate ? "Kullanıcınız başarıyla güncellenmiştir." : "Kullanıcınız başarıyla kaydedildi.";

    $.ajax({
        url: url,
        type: 'Post',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(User),
        success: function (data) {
            if (data.state) {
                if (modalTab) modalTab.style.display = "none";
                var backdrops = document.getElementsByClassName("modal-backdrop");
                if (backdrops.length > 0) {
                    backdrops[0].hidden = true;
                }
                Swal.fire({
                    title: successTitle,
                    text: successText,
                    type: "success"
                });
                PageLoad();
            }
            else {
                Swal.fire({
                    title: successTitle,
                    text: "Kullanıcı bilgilerini lütfen kontrol ediniz. İşlem tamamlanamadı.",
                    type: "warning"
                });
            }
        },
        error: function (x) {
            console.log(x);
        }
    });
}

function GetCities() {
    $.ajax({
        url: '/api/UserApi/GetCities',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Cities = data.Cities || [];
            Districts = data.Districts || [];
            document.getElementById("CityId").innerHTML = "<option value='0'>Seçiniz</option>";
            document.getElementById("DistrictId").innerHTML = "<option value='0'>Seçiniz</option>";
            Cities.forEach(function (element) {
                document.getElementById("CityId").innerHTML += "<option value='" + element.CityId + "'>" + element.CityName + "</option>";
            });
        },
        error: function (x) {
            console.clear();
            console.log(x);
        },
        async: false
    });
}

function CityChange() {
    var cityId = document.getElementById("CityId").value;
    var districtSelect = document.getElementById("DistrictId");
    districtSelect.innerHTML = "<option value='0'>Seçiniz</option>";
    Districts.forEach(function (element) {
        if (element.CityId == cityId) {
            districtSelect.innerHTML += "<option value='" + element.DistrictId + "'>" + element.DistrictName + "</option>";
        }
    });
}

// 1. Dropdown için markaları API'den yükleme fonksiyonu
function loadBrandsDropdown() {
    $.ajax({
        url: '/api/BrandApi/GetAll',
        type: 'GET',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            let options = '<option value="">Marka Seçiniz...</option>';

            // API yanıt yapısı farklı ihtimallere karşı güvenli şekilde yakalanıyor
            BrandList = Array.isArray(response) ? response : (response.List || response.data || response.result || []);

            if (BrandList && BrandList.length > 0) {
                BrandList.forEach(b => {
                    options += `<option value="${b.BrandId}">${b.BrandName}</option>`;
                });
            }
            $('#selectBrand').html(options);
        },
        error: function () {
            console.error("Markalar yüklenirken hata oluştu.");
        }
    });
}

// 2. Tabloya dinamik satır ekleme fonksiyonu
// Tabloya dinamik satır ekleme fonksiyonu (Düzeltilmiş versiyonu)
function AddBrandDiscountRow(brandId = '', brandName = '', discountRate = null) {
    let selectedBrandId = brandId || $('#selectBrand').val();
    let selectedBrandName = brandName || $('#selectBrand option:selected').text();

    // Oranı doğru input ID'sinden (önce modalın içindeki özel input, yoksa genel indirim inputu) güvenli şekilde alıyoruz
    let rate = discountRate;
    if (rate === null || rate === undefined || rate === '') {
        // Öncelik marka satırı ekleme inputunda, yoksa genel indirim inputunda
        rate = $('#inputDiscountRate').val() || $('#discountRate').val() || 0;
    }

    if (!selectedBrandId) {
        if (typeof toastr !== 'undefined') {
            toastr.warning("Lütfen bir marka seçin.");
        } else {
            alert("Lütfen bir marka seçin.");
        }
        return;
    }

    let isAlreadyAdded = false;
    $('#brandDiscountTableBody tr').each(function () {
        let existingId = $(this).find('.brand-id').val();
        if (existingId == selectedBrandId) {
            isAlreadyAdded = true;
            return false;
        }
    });

    if (isAlreadyAdded) {
        if (typeof toastr !== 'undefined') {
            toastr.warning("Bu marka zaten listede ekli. Oranı güncelleyebilirsiniz.");
        } else {
            alert("Bu marka zaten listede ekli.");
        }
        return;
    }

    let html = `<tr>
                    <td>
                        ${selectedBrandName}
                        <input type="hidden" class="brand-id" value="${selectedBrandId}" />
                    </td>
                    <td>
                        <input type="number" class="form-control form-control-sm brand-discount-input" 
                               value="${rate}" step="0.01" min="0" max="100" placeholder="0.00">
                    </td>
                    <td class="text-center">
                        <button type="button" class="btn btn-danger btn-sm" onclick="RemoveBrandRow(this)">
                            <i class="fas fa-trash"></i>
                        </button>
                    </td>
                 </tr>`;

    $('#brandDiscountTableBody').append(html);

    // Eklendikten sonra inputları sıfırla
    $('#selectBrand').val('').trigger('change');
    $('#inputDiscountRate').val('');
}

// 3. Tablodan satır silme fonksiyonu
function RemoveBrandRow(button) {
    $(button).closest('tr').remove();
}

// 4. Kullanıcı düzenlenirken alternatif olarak sunucudan marka indirimlerini çekme fonksiyonu
function loadUserExistingDiscounts(userId) {
    if (!userId) return;

    $.ajax({
        url: '/api/UserApi/GetUserBrandDiscounts',
        type: 'GET',
        data: { userId: userId },
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            let data = Array.isArray(response) ? response : (response.data || response.result || response.List || []);
            if (data && data.length > 0) {
                data.forEach(item => {
                    var brandObj = BrandList.find(x => x.BrandId == item.BrandId);
                    var brandName = brandObj ? brandObj.BrandName : (item.BrandName || ("Marka #" + item.BrandId));

                    AddBrandDiscountRow(item.BrandId, brandName, item.DiscountRate);
                });
            }
        },
        error: function (err) {
            console.log("Kullanıcı marka indirimleri getirilemedi:", err);
        }
    });
}