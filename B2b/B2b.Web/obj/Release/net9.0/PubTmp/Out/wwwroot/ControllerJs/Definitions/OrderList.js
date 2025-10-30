var DataList = [];
var Users = [];
var OrderDetails = [];
var Data = {};
//ProductId
//Count
//Discount
//Price
//VatPrice
//UnitType
var OrderStatus = 
[
    { "StatusId": 0, Value: "Sipariş Onay Bekleniyor" },
    { "StatusId": 1, Value: "Sipariş Hazırlanıyor" },
    { "StatusId": 2, Value: "Sipariş Onaylandı" },
    { "StatusId": 3, Value: "Kargo da" },
    { "StatusId": 4, Value: "Fatura" }
]
$(document).ready(function () {
    GetCustomers();
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/OrderApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            console.clear();
            console.log(data);
            var columns = [
                { "data": "OrderNo" },
                {
                    "data": "CustomerId",
                    render: function (x) {
                        return Users.find(p => p.UserId == x)?.FirstName + " " + Users.find(p => p.UserId == x)?.LastName;
                    }
                },
                {
                    "data": "OrderDate",
                    render: function (x) {
                        const tarih = new Date(x);

                        return tarih.toLocaleString('tr-TR', {
                         year: 'numeric',
                            month: '2-digit',
                            day: '2-digit',
                            hour12: false // 24 saat formatı için
                        });
                    }
                },
                {
                    "data": "OrderStatus",
                    render: function (x) {
                        return OrderStatus.find(p => p.StatusId == x)?.Value;
                    }
                },
                {
                    "data": "OrderId",
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

function GetCustomers() {
    $.ajax({
        url: '/api/UserApi/GetAll',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            Users = data.data;
            document.getElementById("CustomerId").innerHTML = "";
            Users.forEach(function (x) {
                document.getElementById("CustomerId").innerHTML += `<option value="${x.UserId}">${x.FirstName + " " + x.LastName}</option>`;
            })
        },
        error: function (data) { },
        async: false
    });

}
function FormClean() {
    document.getElementById("BrandName").value = "";
    document.getElementById("ImageUrl").value = "";
    document.getElementById("Queno").value = 0;
    document.getElementById("SilinecekResimler").innerHTML = "";
}

function btnClick(obj) {
    if (obj.className.includes("insert")) {
        document.getElementById("modalTitle").innerHTML = "Sipariş Ekle";
        document.getElementById("btnSumbit").innerHTML = "Yeni Sipariş Kaydet";
        FormClean();
        Data.OrderId = 0;
    }
    else if (obj.className.includes("edit")) {
        document.getElementById("modalTitle").innerHTML = "Sipariş Güncelleme";
        document.getElementById("btnSumbit").innerHTML = "Sipariş Güncelle";
        Data.OrderId = obj.id;
        var subdata = DataList.find(x => x.OrderId == obj.id);
        document.getElementById("OrderNo").value = subdata.OrderNo;
        document.getElementById("OrderStatus").value = subdata.OrderStatus;
        document.getElementById("PaymentType").value = subdata.PaymentType;
        document.getElementById("CustomerId").value = subdata.CustomerId;
        document.getElementById("Total").value = subdata.Total;
        document.getElementById("OrderNote").value = subdata.OrderNote;
        const tarih = new Date(subdata.OrderDate);
        var orderDate= tarih.toLocaleString('tr-TR', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour12: false // 24 saat formatı için
        });
        var sp = orderDate.split(".");
        document.getElementById("OrderDate").value = sp[2] + "-" + sp[1] + "-" + sp[0];
        GetOrderDetails(subdata.OrderId);
        OrderDetailsTableLoad();
    }
    
}
function OrderDetailsTableLoad() {
    var columns = [
        { "data": "ProductName" },
        { "data": "UnitType" },
        {
            "data": function (x) {
                return '<input  class="form-control" id="' + x.OrderDetailId +'-Count" onChange="Change(this)" value="' + x.Count + '"/>';
            }
        },
        {
            "data":  function (x) {
                return '<input  class="form-control"  id="' + x.OrderDetailId +'-Discount" onChange="Change(this)"  value="' + x.Discount + '"/>';
            }
        },
        {
            "data": "Price",
        },
        {
            "data":  function (x) {
                return '<input  class="form-control" id="' + x.OrderDetailId +'-VatPrice"  onChange="Change(this)"  value="' + x.VatPrice + '"/>';
            }
        }
    ];
    DatatablesLoad("details-datatable", OrderDetails, columns)
}

function PostData() {
    Data.OrderNo = document.getElementById("OrderNo").value;
    Data.OrderDate = document.getElementById("OrderDate").value;
    Data.OrderStatus = document.getElementById("OrderStatus").value;
    Data.CustomerId = document.getElementById("CustomerId").value;
    Data.Total = document.getElementById("Total").value;
    Data.PaymentType = document.getElementById("PaymentType").value;
    Data.OrderNote = document.getElementById("OrderNote").value;
    Data.OrderDetails = OrderDetails;
    $.ajax({
        url: '/api/OrderApi/Update',
        type: 'Put',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        data: JSON.stringify(Data),
        contentType: 'application/json',
        success: function (data) {
            if (data.State) {
                Swal.fire({
                    title: "Sipariş Güncelleme",
                    text: "Sipariş başarıyla güncellenmiştir.",
                    icon: "success",
                });
                PageLoad();
            }
            else {
                Swal.fire({
                    title: "Sipariş Güncelleme",
                    text: "Sipariş güncellenirken beklenmedik bir sorun ile karşılaşıldı.",
                    icon: "warning",
                });
            }
        }
    });
  
}

function GetOrderDetails(id) {
    $.ajax({
        url: '/api/OrderApi/GetById?id=' + id,
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            OrderDetails = data.Data.OrderDetails;
            console.clear();
            console.log(OrderDetails);
        },
        error: function (data) { },
        async: false
    });
}
function Change(obj) {
    var data = 0;
    var id = 0;
    if (obj.id.includes("-Count")) {
        id = obj.id.replace("-Count", "");
        OrderDetails.find(x => x.OrderDetailId == id).Count = obj.value;
    }
    else if (obj.id.includes("-Discount")) {
        id = obj.id.replace("-Discount", "");
        OrderDetails.find(x => x.OrderDetailId == id).Discount = obj.value;
    }
    else {
        id = obj.id.replace("-VatPrice", "");
        OrderDetails.find(x => x.OrderDetailId == id).VatPrice = obj.value;
    }
    console.clear();
    console.log(OrderDetails);
}