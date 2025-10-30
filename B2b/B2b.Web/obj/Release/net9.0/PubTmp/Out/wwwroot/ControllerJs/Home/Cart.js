var Carts = []
$(document).ready(function () {
    formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };
    GetCarts();
});

function GetCarts() {
    $.ajax({
        url: '/api/CartApi/GetAll',
        type: 'Get',
        contentType: 'application/json',
        dataType: 'json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            if (data.State) {
                var result = "";
                Carts = data.List;
                data.List.forEach(function (x) {
                    result += `<tr class="product">
                                        <td>
                                            <img src="${x.ProductImage}" alt="${x.ProductName}" title="${x.ProductName}" class="avatar-md">
                                        </td>
                                        <td>
                                            <h5 class="font-size-14 text-truncate"><a onclick="GetProduct(${x.ProductId})" data-bs-toggle="modal" data-bs-target=".bs-example-modal-xl"  class="text-dark">${x.ProductName}</a></h5>
                                            <p class="mb-0"> <span class="fw-medium">${x.UnitTypeName}</span></p>
                                        </td>
                                        <td><span class="product-price">${formatPrice(x.DiscountedPrice)}</span></td>
                                        <td>
                                            <div class="me-3" style="width: 120px;">
                                                <div class="input-group  bootstrap-touchspin bootstrap-touchspin-injected">
                                                    <input type="number" id="Quantity-${x.ProductId}" min="1" max ="100" disabled value="${x.Quantity}" class="product-quantity form-control" name="demo_vertical">
                                                    <span class="input-group-addon bootstrap-touchspin-vertical-button-wrapper">
                                                        <span class="input-group-btn-vertical">
                                                            <button id="${x.ProductId}-Plus" tabindex="-1" onclick="UpdateCart(this)" class="btn btn-primary bootstrap-touchspin-up " type="button">+</button>
                                                            <button id="${x.ProductId}-Minus" tabindex="-1" onclick="UpdateCart(this)" class="btn btn-primary bootstrap-touchspin-down " type="button">−</button>
                                                        </span>
                                                    </span>

                                                </div>
                                            </div>
                                        </td>
                                        <td><span class="product-line-price">${formatPrice(x.TotalPrice)}</span></td>
                                        <td>
                                            <div class="d-inline-block" data-bs-toggle="tooltip" data-bs-placement="top" aria-label="Remove" data-bs-original-title="Remove">
                                                <a onclick="RemoveCart(${x.CartId})" data-bs-toggle="modal" class="action-icon text-danger"> <i class="mdi mdi-trash-can font-size-18"></i></a>
                                            </div>
                                        </td>
                                    </tr>`;
                });
                document.getElementById("MyCartContent").innerHTML = result;
            }
        },
        error: function (data) { },
        async: false
    });
    CartInfoUpdate();
}

function RemoveCart(productId) {
    $.ajax({
        url: "/api/CartApi/Remove?id=" + productId,
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (x) {
            if (x.State) {
                toastr["success"]("Ürün Sepetten Cıkarılmıştır");
                GetCarts();
                GetAllCart();
            }
            else {
                toastr["warning"]("Ürün Sepetten Çıkarılmadı");
            }
        },
        error: function (x) { }
    });

}
function UpdateCart(obj) {
    var productId = 0;
    var Quantity = 0;
    var cart = {};
    if (obj.id.includes("Plus")) {
        productId = obj.id.replace("-Plus", "");
        cart=Carts.find(x => x.ProductId == productId);
        cart.Quantity += 1; 
    }
    else {
        productId = obj.id.replace("-Minus", "");
      cart=  Carts.find(x => x.ProductId == productId);
        cart.Quantity = cart.Quantity-1;
    }
    $.ajax({
        url: "/api/CartApi/Update",
        type: 'PUT',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(cart),
        success: function (x) {
            if (x.State) {
                GetCarts();
                GetAllCart();
            }
            else {
                toastr["warning"]("Ürün Sepetten Çıkarılmadı");
            }
        },
        error: function (x) { }
    });

}

function CartInfoUpdate() {
    var subtotal = 0;
    var discount = 0;
    var tax = 0;
    var total = 0;
    Carts.forEach(function (x) {
        subtotal += x.TotalPrice;
        discount += x.Price;
        tax += (x.VatPrice * x.Quantity);
    });
    discount = discount - subtotal;
    total = subtotal + tax;
    document.getElementById("cart-subtotal").innerHTML = formatPrice(subtotal);
    document.getElementById("cart-discount").innerHTML = formatPrice(discount);
    document.getElementById("cart-tax").innerHTML = formatPrice(tax);
    document.getElementById("cart-total").innerHTML = formatPrice(total);
}





