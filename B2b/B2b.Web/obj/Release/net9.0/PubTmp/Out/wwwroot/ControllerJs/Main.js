$(document).ready(function () {
    formatPrice = (price) => {
        return price.toFixed(2).replace(/\d(?=(\d{3})+\.)/g, '$&,') + ' TL';
    };
    GetAllCart();
});
function DatatablesLoad(tableName, data, columns) {
    var table = $('#' + tableName).DataTable({
     "data": data,
     "destroy": true,
    "columns": columns,
    "processing": false,
    "serverSide": false,
    "responsive": true,
    "dom": 'Bfrtip',
    "lengthChange": !1,
    "buttons": [ "excel","colvis"],
    "language": {
        "url": '/ControllerJs/language/tr.json',
        "paginate": {
            "previous": "<i class='mdi mdi-chevron-left'>",
            "next": "<i class='mdi mdi-chevron-right'>"
          }
      }
    });
    table.buttons().container()
        .appendTo($('.col-sm-12 .col-md-6:eq(0)', table.table().container()));
}
function DatatablesLoadMainOffer(tableName, data, columns) {
    var table = $('#' + tableName).DataTable({
        "data": data,
        "destroy": true,
        "columns": columns,
        "processing": false,
        "serverSide": false,
        "responsive": false,
        "dom": 'Bfrtip',
        "lengthChange": !1,
        "buttons": ["excel", "colvis"],
        "language": {
            "url": '/ControllerJs/language/tr.json',
            "paginate": {
                "previous": "<i class='mdi mdi-chevron-left'>",
                "next": "<i class='mdi mdi-chevron-right'>"
            }
        },
        scrollX: true, // Yatay kaydırmayı etkinleştir
        scrollCollapse: true,
        fixedColumns: {
            leftColumns: 8  // İlk 5 kolonu sabitle
        }
    });
    table.buttons().container()
        .appendTo($('.col-sm-12 .col-md-6:eq(0)', table.table().container()));
}

function Chekbox(checked, disable) {
    var IsCheced = "";
    var IsDisable = "";
    var drm = "Pasif";
    if (checked) {
        IsCheced = "checked";
        drm = "Aktif";
    }
    if (disable) {
        IsDisable = "disabled";
    }

    return "<div class='custom-control custom-checkbox mb-2'>" +
        "<input type='checkbox' id='autoSizingCheck' class='custom-control-input' " + IsDisable + " " + IsCheced + ">" +
        "<label class='custom-control-label' for='autoSizingCheck'>" + drm + "</label></div>";
}

function TarihFormat(date) {
    if (date != null) {
        return moment(date).format('L');
    }
    else {
        return null;
    }

}



function url_slug(s, opt) {
    s = String(s);
    opt = Object(opt);
    var defaults = {
        'delimiter': '-',
        'limit': undefined,
        'lowercase': true,
        'replacements': {},
        'transliterate': (typeof (XRegExp) === 'undefined') ? true : false
    };
    for (var k in defaults) {
        if (!opt.hasOwnProperty(k)) {
            opt[k] = defaults[k];
        }
    }
    var char_map = {
        // Latin
        'À': 'A', 'Á': 'A', 'Â': 'A', 'Ã': 'A', 'Ä': 'A', 'Å': 'A', 'Æ': 'AE', 'Ç': 'C',
        'È': 'E', 'É': 'E', 'Ê': 'E', 'Ë': 'E', 'Ì': 'I', 'Í': 'I', 'Î': 'I', 'Ï': 'I',
        'Ð': 'D', 'Ñ': 'N', 'Ò': 'O', 'Ó': 'O', 'Ô': 'O', 'Õ': 'O', 'Ö': 'O', 'Ő': 'O',
        'Ø': 'O', 'Ù': 'U', 'Ú': 'U', 'Û': 'U', 'Ü': 'U', 'Ű': 'U', 'Ý': 'Y', 'Þ': 'TH',
        'ß': 'ss',
        'à': 'a', 'á': 'a', 'â': 'a', 'ã': 'a', 'ä': 'a', 'å': 'a', 'æ': 'ae', 'ç': 'c',
        'è': 'e', 'é': 'e', 'ê': 'e', 'ë': 'e', 'ì': 'i', 'í': 'i', 'î': 'i', 'ï': 'i',
        'ð': 'd', 'ñ': 'n', 'ò': 'o', 'ó': 'o', 'ô': 'o', 'õ': 'o', 'ö': 'o', 'ő': 'o',
        'ø': 'o', 'ù': 'u', 'ú': 'u', 'û': 'u', 'ü': 'u', 'ű': 'u', 'ý': 'y', 'þ': 'th',
        'ÿ': 'y',
        // Latin symbols
        '©': '(c)',
        // Greek
        'Α': 'A', 'Β': 'B', 'Γ': 'G', 'Δ': 'D', 'Ε': 'E', 'Ζ': 'Z', 'Η': 'H', 'Θ': '8',
        'Ι': 'I', 'Κ': 'K', 'Λ': 'L', 'Μ': 'M', 'Ν': 'N', 'Ξ': '3', 'Ο': 'O', 'Π': 'P',
        'Ρ': 'R', 'Σ': 'S', 'Τ': 'T', 'Υ': 'Y', 'Φ': 'F', 'Χ': 'X', 'Ψ': 'PS', 'Ω': 'W',
        'Ά': 'A', 'Έ': 'E', 'Ί': 'I', 'Ό': 'O', 'Ύ': 'Y', 'Ή': 'H', 'Ώ': 'W', 'Ϊ': 'I',
        'Ϋ': 'Y',
        'α': 'a', 'β': 'b', 'γ': 'g', 'δ': 'd', 'ε': 'e', 'ζ': 'z', 'η': 'h', 'θ': '8',
        'ι': 'i', 'κ': 'k', 'λ': 'l', 'μ': 'm', 'ν': 'n', 'ξ': '3', 'ο': 'o', 'π': 'p',
        'ρ': 'r', 'σ': 's', 'τ': 't', 'υ': 'y', 'φ': 'f', 'χ': 'x', 'ψ': 'ps', 'ω': 'w',
        'ά': 'a', 'έ': 'e', 'ί': 'i', 'ό': 'o', 'ύ': 'y', 'ή': 'h', 'ώ': 'w', 'ς': 's',
        'ϊ': 'i', 'ΰ': 'y', 'ϋ': 'y', 'ΐ': 'i',
        // Turkish
        'Ş': 'S', 'İ': 'I', 'Ç': 'C', 'Ü': 'U', 'Ö': 'O', 'Ğ': 'G',
        'ş': 's', 'ı': 'i', 'ç': 'c', 'ü': 'u', 'ö': 'o', 'ğ': 'g',
        // Russian
        'А': 'A', 'Б': 'B', 'В': 'V', 'Г': 'G', 'Д': 'D', 'Е': 'E', 'Ё': 'Yo', 'Ж': 'Zh',
        'З': 'Z', 'И': 'I', 'Й': 'J', 'К': 'K', 'Л': 'L', 'М': 'M', 'Н': 'N', 'О': 'O',
        'П': 'P', 'Р': 'R', 'С': 'S', 'Т': 'T', 'У': 'U', 'Ф': 'F', 'Х': 'H', 'Ц': 'C',
        'Ч': 'Ch', 'Ш': 'Sh', 'Щ': 'Sh', 'Ъ': '', 'Ы': 'Y', 'Ь': '', 'Э': 'E', 'Ю': 'Yu',
        'Я': 'Ya',
        'а': 'a', 'б': 'b', 'в': 'v', 'г': 'g', 'д': 'd', 'е': 'e', 'ё': 'yo', 'ж': 'zh',
        'з': 'z', 'и': 'i', 'й': 'j', 'к': 'k', 'л': 'l', 'м': 'm', 'н': 'n', 'о': 'o',
        'п': 'p', 'р': 'r', 'с': 's', 'т': 't', 'у': 'u', 'ф': 'f', 'х': 'h', 'ц': 'c',
        'ч': 'ch', 'ш': 'sh', 'щ': 'sh', 'ъ': '', 'ы': 'y', 'ь': '', 'э': 'e', 'ю': 'yu',
        'я': 'ya',
        // Ukrainian
        'Є': 'Ye', 'І': 'I', 'Ї': 'Yi', 'Ґ': 'G',
        'є': 'ye', 'і': 'i', 'ї': 'yi', 'ґ': 'g',
        // Czech
        'Č': 'C', 'Ď': 'D', 'Ě': 'E', 'Ň': 'N', 'Ř': 'R', 'Š': 'S', 'Ť': 'T', 'Ů': 'U',
        'Ž': 'Z',
        'č': 'c', 'ď': 'd', 'ě': 'e', 'ň': 'n', 'ř': 'r', 'š': 's', 'ť': 't', 'ů': 'u',
        'ž': 'z',
        // Polish
        'Ą': 'A', 'Ć': 'C', 'Ę': 'e', 'Ł': 'L', 'Ń': 'N', 'Ó': 'o', 'Ś': 'S', 'Ź': 'Z',
        'Ż': 'Z',
        'ą': 'a', 'ć': 'c', 'ę': 'e', 'ł': 'l', 'ń': 'n', 'ó': 'o', 'ś': 's', 'ź': 'z',
        'ż': 'z',
        // Latvian
        'Ā': 'A', 'Č': 'C', 'Ē': 'E', 'Ģ': 'G', 'Ī': 'i', 'Ķ': 'k', 'Ļ': 'L', 'Ņ': 'N',
        'Š': 'S', 'Ū': 'u', 'Ž': 'Z',
        'ā': 'a', 'č': 'c', 'ē': 'e', 'ģ': 'g', 'ī': 'i', 'ķ': 'k', 'ļ': 'l', 'ņ': 'n',
        'š': 's', 'ū': 'u', 'ž': 'z'
    };
    for (var k in opt.replacements) {
        s = s.replace(RegExp(k, 'g'), opt.replacements[k]);
    }
    if (opt.transliterate) {
        for (var k in char_map) {
            s = s.replace(RegExp(k, 'g'), char_map[k]);
        }
    }
    var alnum = (typeof (XRegExp) === 'undefined') ? RegExp('[^a-z0-9]+', 'ig') : XRegExp('[^\\p{L}\\p{N}]+', 'ig');
    s = s.replace(alnum, opt.delimiter);
    s = s.replace(RegExp('[' + opt.delimiter + ']{2,}', 'g'), opt.delimiter);
    s = s.substring(0, opt.limit);
    s = s.replace(RegExp('(^' + opt.delimiter + '|' + opt.delimiter + '$)', 'g'), '');
    return opt.lowercase ? s.toLowerCase() : s;
}

//$(".text-editor").summernote({
//    height: 300,
//    minHeight: null,
//    maxHeight: null,
//    focus: !0,
//    lang: 'tr-TR'
//})

var Imgdata = [];
function FileUpload(obj) {
    var snc = "";

    var postFile = Array.from(document.getElementById(obj.id).files);
    var data = new FormData();
    if (postFile != null && postFile.length > 0) {
        postFile.forEach(function (x) {
            data.append('file', x);
        })
        $.ajax({
            url: '/upload/upload-file',
            processData: false,
            contentType: false,
            data: data,
            headers: { 'Authorization': localStorage.getItem("token") },
            type: 'Post',
            success: function (data) {
                PostFiles = data.Img;
                Imgdata = data.Img;
                snc = '<a id="' + data.Img + '" class="' + data.Img + '" onclick="DeleteFile(this)">' + data.Img + ' X Sil </a>';
                document.getElementById("SilinecekResimler").innerHTML = '<img src="' + data.Img + '" class="' + data.Img + '" id="silinecekimg" width="80" height="auto"/>';;

            },
            async: false
        });
        return Imgdata;
    }
    else {
        return null;
    }

}
function DeleteFile(obj) {

    Imgdata.forEach(function (x) {
        if (x.FileId == obj.id) {
            $.ajax({
                url: '/upload/delete-file',
                data: JSON.stringify(x),
                type: 'Post',
                contentType: 'application/json',
                dataType: 'json',
                success: function (data) {
                    if (data.state) {
                        var Xdata = $("." + obj.id);
                        for (var i = 0; i < Xdata.length; i++) {
                            Xdata[i].remove();
                        }
                        Imgdata.splice(x, 1);
                    }
                },
                async: false
            });
        }
    })

}
function DeleteSingleFile(obj) {
    //$.ajax({
    //    url: '/upload/delete-file',
    //    data: JSON.stringify(obj.id),
    //    type: 'Post',
    //    contentType: 'application/json',
    //    headers: { 'Authorization': localStorage.getItem("token") },
    //    dataType: 'json',
    //    success: function (data) {
    //        if (data.state) {

    //            if (obj.parentElement != undefined) {
    //                obj.parentElement.innerHTML = "";
    //            }
    //        }
    //    },
    //    async: false
    //});
   
        Swal.fire({
            title: "Silmek istediğinize eminmisiniz?",
            text: "Kalıcı olarak silme işlemi yapılacaktır. Geri alınamaz !",
            icon: "warning",
            showCancelButton: !0,
            confirmButtonText: "Evet, Sil!",
            cancelButtonText: "Hayır, Silme!",
            customClass: {
                confirmButton: "btn btn-success mt-2",
                cancelButton: "btn btn-danger ms-2 mt-2"
            },
            buttonsStyling: !1
        }).then(function (t) {
            t.value ? Swal.fire({
                title: "Silini!",
                text: "Dosyanız silindi.",
                icon: "success"
            }) : t.dismiss === Swal.DismissReason.cancel && Swal.fire({
                title: "Silme işlemi",
                text: "Silme İşlemi İptal edildi",
                icon: "error"
            })
        })
    

}


function generateUUID() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        let r = Math.random() * 16 | 0,
            v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

function Login(domain) {
    var user = {
        "Email": document.getElementById("email").value,
        "Password": document.getElementById("password").value
    }
    $.ajax({
        url: "/api/LoginApi/Login",
        type: 'Post',
        dataType: 'Json',
        contentType: 'application/json',
        data: JSON.stringify(user),
        success: function (x) {
            localStorage.setItem("token", x.data.Token);
            window.location.href=domain + x.url;
        },
        error: function (x) { console.log(x) }
    });
}
function Register(domain) {
    var user = {
        "FirstName": document.getElementById("FirstName").value,
        "LastName": document.getElementById("LastName").value,
        "Email": document.getElementById("Email").value,
        "Password": document.getElementById("Password").value,
        "Phone": document.getElementById("Phone").value

    }
    $.ajax({
        url: "/api/LoginApi/Register",
        type: 'Post',
        dataType: 'Json',
        contentType: 'application/json',
        data: JSON.stringify(user),
        success: function (x) {
            if (x.State) {
                Swal.fire({
                    title: "Kullanıcı Kayıt ",
                    text: "Kullanıcınız başarıyla kayıt edilmiştir.",
                    type: "success"
                })
                window.location.replace(domain);

            }
            else {
                Swal.fire({
                    title: "Kullanıcı Kayıt",
                    text: x.Message,
                    type: "error"
                })
            }
        },
        error: function (x) { console.log(x) },
        async: false
    });
}
function LogOut(domain) {
    $.ajax({
        url: "/api/LoginApi/LogOut",
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (x) {
            if (x.state) {
                localStorage.removeItem("token");
                localStorage.clear();
                window.location.replace(domain);
            }
        },
        error: function (x) { console.log(x) },
        async: false
    });
}

function recover(domain) {
    var user = {
        "Email": document.getElementById("Email").value,
    }
    $.ajax({
        url: "/api/LoginApi/PasswordRecover",
        type: 'Post',
        dataType: 'Json',
        contentType: 'application/json',
        data: JSON.stringify(user),
        success: function (x) {
            if (x.State) {
                Swal.fire({
                    title: "Kullanıcı Şifre Sıfırlama ",
                    text: x.Message,
                    type: "success"
                })
                window.location.replace(domain);

            }
            else {
                Swal.fire({
                    title: "Kullanıcı Şifre Sıfırlama ",
                    text: x.Message,
                    type: "error"
                })
            }
        },
        error: function (x) { console.log(x) },
        async: false
    });
}

$('#mainSlider').owlCarousel({
    loop: true,
    margin: 10,
    nav: true,
    dots: false,
    autoHeight:false,
    autoplay:true,
    responsive: {
        0: {
            items: 1
        }
    }
})
$(".owl-prev").html('<i class="fa fa-chevron-left"></i>');
$(".owl-next").html('<i class="fa fa-chevron-right"></i>');

toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-bottom-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": 300,
    "hideDuration": 1000,
    "timeOut": 5000,
    "extendedTimeOut": 1000,
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}

function AddCart(productId, count,price) {
    var cart = {
        "ProductId": productId,
        "Quantity": count,
        "Price": price
    }
    $.ajax({
        url: "/api/CartApi/Add",
        type: 'Post',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(cart),
        contentType: 'application/json',
        success: function (x) {
            if (x.State) {
                toastr["success"]("Ürün Sepete Eklendi");
                GetAllCart();
            }
            else {
                toastr["error"]("Ürün Sepete Eklenemedi")
            }
        },
        error: function (x) { console.log(x) },
        async: false
    });
}
function GetAllCart() {
    $.ajax({
        url: "/api/CartApi/GetAll",
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (x) {
            var Total = 0;
            var CartCount = 0;
            var content = document.getElementById("CartContent");
            content.innerHTML = "";
            x.List.forEach(function (item) {
                content.innerHTML += `
                                                            <div class="d-flex mt-2">
                                                                <div class="flex-shrink-0 me-3">
                                                                    <img src="${item.ProductImage}" class="rounded-circle avatar-xs" alt="${item.ProductName}">
                                                                </div>
        
                                                                <div class="flex-grow-1 d-flex align-items-center justify-content-between">
                                                                    <div>
                                                                        <h6 class="mt-0 mb-1">${item.ProductName}</h6>
                                                                        <div class="font-size-12 text-muted">
                                                                            <p class="mb-1">${item.Quantity} x ${formatPrice(item.DiscountedPrice)}</p>
                                                                        </div>
                                                                    </div>
                                                                    <a class="cart-remove" id="${item.CartId}">
                                                                        <i class="mdi mdi-trash-can font-size-16 text-danger me-1"></i>
                                                                    </a>
                                                                </div>
                                                                </div>
                                                        `;
                CartCount += item.Quantity;
                Total += item.TotalPrice;

            })
            document.getElementById("CartProductCount").innerHTML = CartCount;
            document.getElementById("CartTotal").innerHTML = formatPrice(Total);
        },
        error: function (x) { console.log(x) },
        async: false
    });
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
                GetAllCart();
            }
            else {
                toastr["warning"]("Ürün Sepetten Çıkarılmadı");
            }
        },
        error: function (x) { }
    });

}

$('#CartContent').on('click', '.cart-remove', function () {
    var productId = this.id;
    // Silme işlemini başlat
    RemoveCart(productId);
});
var ProductId = 0;
function GetProduct(id) {
    ProductId = id;
    var row = {};
    $.ajax({
        url: "/api/CommerceApi/GetProductInfo?id=" + id,
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (x) {
            row = x.Data;
        },
        error: function (x) { },
        async: false

    });
    var content = document.getElementById("unitTable");
    var tabContent = document.getElementById("v-pills-tabContent");
    var tab = document.getElementById("v-pills-tab");
    document.getElementById("ProductName").innerText = "Ürün Adı : " + row.ProductName;
    document.getElementById("productDetailModal").innerText = row.ProductName;
    document.getElementById("ProductCode").innerText = "Ürün Kodu : " + row.ProductCode;
    document.getElementById("Category").innerText = "Kategori : " + row.CategoryName;
    document.getElementById("Brand").innerText = "Marka : " + row.BrandName;
    document.getElementById("Price").innerText = row.Price.toFixed(2) + " TL";
    document.getElementById("DiscountedPrice").innerText = row.DiscountedPrice.toFixed(2) + " TL";
    document.getElementById("DiscountedRate").innerText = "İndirim Oranı : %" + row.TotalDiscountRate;
    document.getElementById("Description").innerText = row.Description;
    $.ajax({
        url: '/api/CommerceApi/GetProduct?ProductId=' + id,
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            content.innerHTML = '';
            response.List.forEach(function (item) {
                if (item.IsDefault) {
                    content.innerHTML += `<tr class="red-row">
                    <td>${item.UnitTypeName}</td>
                    <td>${item.Price.toFixed(2)} TL</td>
                    <td>${item.DiscountedPrice.toFixed(2)} TL</td>
                </tr>`;
                }
                else {
                    content.innerHTML += `<tr>
                    <td>${item.UnitTypeName}</td>
                    <td>${item.Price.toFixed(2)} TL</td>
                    <td>${item.DiscountedPrice.toFixed(2)} TL</td>
                </tr>`;
                }
            });
        },
        error: function (err) { },
        async: false
    });
    $.ajax({
        url: '/api/CommerceApi/GetProductImage?ProductId=' + id,
        type: 'Get',
        dataType: 'Json',
        contentType: 'application/json',
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (response) {
            tabContent.innerHTML = '';
            tab.innerHTML = '';
            response.List.forEach(function (item, index) {
                if (index == 0) {
                    tab.innerHTML += `<a class="nav-link active" id="${item.ProductImageId}-tab" data-bs-toggle="pill" href="#${item.ProductImageId}" role="tab" aria-controls="${item.ProductImageId}" aria-selected="true">
                                                    <img src="${item.ImageUrl}" alt="" class="img-fluid mx-auto d-block rounded">
                                                </a>`;
                    tabContent.innerHTML += `  <div class="tab-pane fade show active" id="${item.ProductImageId}" role="tabpanel" aria-labelledby="${item.ProductImageId}-tab">
                                                    <div>
                                                        <img src="${item.ImageUrl}" alt="" class="img-fluid mx-auto d-block">
                                                    </div>
                                                </div>`;
                }
                else {
                    tab.innerHTML += `<a class="nav-link" id="${item.ProductImageId}-tab" data-bs-toggle="pill" href="#${item.ProductImageId}" role="tab" aria-controls="${item.ProductImageId}" aria-selected="false" tabindex="-1">
                                                    <img src="${item.ImageUrl}" alt="" class="img-fluid mx-auto d-block rounded">
                                                </a>`;
                    tabContent.innerHTML += `  <div class="tab-pane fade" id="${item.ProductImageId}" role="tabpanel" aria-labelledby="${item.ProductImageId}-tab">
                                                    <div>
                                                        <img src="${item.ImageUrl}" alt="" class="img-fluid mx-auto d-block">
                                                    </div>
                                                </div>`;
                }
            });

        },
        error: function (err) { },
        async: false
    });

//  document.getElementById("DetailProduct").dataset.value = id;
}


function DetailAddCart() {
    var Quantity = document.getElementById("Count").value;
    console.clear();
    var cart = {
        "ProductId": ProductId,
        "Quantity": Quantity,
    }
    $.ajax({
        url: "/api/CartApi/Add",
        type: 'Post',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(cart),
        contentType: 'application/json',
        success: function (x) {
            if (x.State) {
                toastr["success"]("Ürün Sepete Eklendi");
                GetAllCart();
            }
            else {
                toastr["error"]("Ürün Sepete Eklenemedi")
            }
        },
        error: function (x) { console.log(x) },
        async: false
    });
    location.reload();
}