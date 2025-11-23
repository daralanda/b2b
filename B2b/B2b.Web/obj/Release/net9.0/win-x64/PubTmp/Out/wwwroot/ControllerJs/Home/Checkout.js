var confirm = document.getElementById("v-pills-confir");
var shipping = document.getElementById("v-pills-shipping");
var payment = document.getElementById("v-pills-payment");
$(document).ready(function () {
    formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };
    GetCarts();
    GetAllCheckNumber();
    confirm.style.display = "none";
    payment.style.display = "none";
    shipping.style.display = "block";
});
function GetCarts() {
    $.ajax({
        url: '/api/UserInfoApi/GetById',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            document.getElementById("billing-name").value = data.data.CustomerName;
            document.getElementById("billing-Tax").value = data.data.TaxNo;
            document.getElementById("billing-Tax-Office").value = data.data.TaxOffice;
            document.getElementById("billing-email-address").value = data.data.Email;
            document.getElementById("billing-phone").value = data.data.Phone;
            document.getElementById("City").value = data.Cities.find(x=>x.CityId=data.data.CityId).CityName;
            document.getElementById("District").value = data.Districts.find(x => x.DistrictId = data.data.DistrictId).DistrictName;
            document.getElementById("billing-address").value = data.data.Address;
        },
        error: function (x) { },
        async: false
    });
}
function GetAllCheckNumber() {
    $.ajax({
        url: '/api/AccountNumberApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            document.getElementById("AccountInformation").innerHTML = "";
            var result = "";
            data.List.forEach(function (x) {
                result += `<tr>
        <td><img src="${x.Logo}" alt="${x.Bank}" height="18"></td>
        <td>${x.Bank}</td>
        <td>${x.AccountName}</td>
        <td>${x.AccountNo}</td>
        <td>${x.BranchCode}</td>
        <td>${x.Currency}</td>
        <td>${x.IBAN}</td>
      </tr>`;
            })
            document.getElementById("AccountInformation").innerHTML = result;
        }
    });
}
function Next(obj) {
    var defaultCss = "tab-pane fade";
    if (obj=="Payment") {
        shipping.style.display = "none";
        shipping.className = defaultCss;
        payment.style.display = "block";
        payment.className += "active show";
        confirm.style.display = "none";
        confirm.className = defaultCss;
    }
    else if (obj == "Shipping") {
        shipping.style.display = "block";
        shipping.className += "active show";
        payment.style.display = "none";
        payment.className = defaultCss;
        confirm.style.display = "none";
        confirm.className = defaultCss;
    }
    else if (obj == "Confirm") {
        if (!IsCreditCart && PaymentTypeId ==1) {
            Swal.fire({
                title: "Kredi Kartı Ödeme",
                text: "Ödeme yapmadan bu adımda ilerleyemezsiniz. Eğer ödeme yaptıysanız işlem nosuyla geri dönüş sağlayınız lütfen",
                icon: "warning"
            });
        }
        else {
            var data = {
                "PaymentType": PaymentTypeId,
                "OrderNote": document.getElementById("example-textarea").value
            }
            $.ajax({
                url: '/api/OrderApi/CreateOrder',
                type: 'Post',
                contentType: 'application/json',
                dataType: 'json',
                data: JSON.stringify(data),
                headers: { 'Authorization': localStorage.getItem("token") },
                success: function (x) {
                    if (x.State) {
                        confirm.style.display = "block";
                        confirm.className += "active show";
                        payment.style.display = "none";
                        payment.className = defaultCss;
                        shipping.style.display = "none";
                        shipping.className = defaultCss;
                        GetAllCart();
                        GetOrder(x.Data.OrderId);
                        Swal.fire({
                            title: "Sipariş",
                            text: "Siparişiniz Başarıyla oluşturulmuştur.",
                            icon: "success"
                        });
                    }
                },
                error: function (x) { },
                async: false
            });


        }

    }
}
var PaymentTypeId = 1;
IsCreditCart = false;
function PaymentType(obj) {
    PaymentTypeId = obj;
}

function GetOrder(obj) {
    $.ajax({
        url: '/api/OrderApi/GetById?id=' + obj,
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            if (data.State) {
                var result = "";
                var row = "";
                var Discounts = 0;
                var Kdv = 0;
                var SubTotal = 0;
                var Total = 0;
                data.Data.OrderDetails.forEach(function (x) {
                    row += `<tr>

                                      <td><img src="${x.ProductImg}" alt="${x.ProductName}" title="${x.ProductName}" class="avatar-md"></td>
                                     <td>
                                         <h5 class="font-size-14 text-truncate"><a  class="text-dark">${x.ProductName}</a></h5>
                                     </td>
	                                 <td>${x.UnitType}</td>
	                                 <td>${x.Count}</td>
                                     <td>${formatPrice(x.Discount)}</td>
                                     <td>${formatPrice(x.Count * x.Discount)}</td>
                                 </tr>`;
                    Discounts += (x.Price - x.Discount) * x.Count;
                    Kdv += x.VatPrice;
                    SubTotal += (x.Discount * x.Count) ;
                })

                result = row + ` <tr><td colspan="5"><h6 class="m-0 text-end">İndirim:</h6></td><td>${formatPrice(Discounts)}</td></tr>
                                <tr><td colspan="5"><h6 class="m-0 text-end">Kdv:</h6></td><td>${formatPrice(Kdv)}</td></tr>
                                <tr><td colspan="5"><h6 class="m-0 text-end">Sub Total:</h6></td><td>${formatPrice(SubTotal)}</td></tr>
                                <tr>
                                     <td colspan="6">
                                         <div class="bg-info-subtle p-3 rounded">
                                             <h5 class="font-size-14 text-info mb-0"><i class="fas fa-shipping-fast me-2"></i> Kargo <span class="float-end">Ücretsiz</span></h5>
                                         </div>
                                     </td>
                                 </tr>
                                 <tr><td colspan="5"><h6 class="m-0 text-end">Genel Toplam :</h6></td><td>${formatPrice(data.Data.Total)}</td></tr>`;
                document.getElementById("confrmTableContent").innerHTML = result;
            }
        },
        error: function (data) { },
        async: false
    });

}
