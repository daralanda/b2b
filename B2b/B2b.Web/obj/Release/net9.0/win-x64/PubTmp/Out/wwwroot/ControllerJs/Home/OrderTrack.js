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
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/CommerceApi/GetAllOrder',
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
                { "data": "Total" },
                { "data": "OrderNote" },
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



function btnClick(obj) {
    Data.OrderId = obj.id;
    var subdata = DataList.find(x => x.OrderId == obj.id);
    document.getElementById("OrderNo").value = subdata.OrderNo;
    document.getElementById("modalTitle").innerHTML = subdata.OrderNo;
    document.getElementById("OrderStatus").value = subdata.OrderStatus;
    document.getElementById("PaymentType").value = subdata.PaymentType;
    document.getElementById("Total").value = subdata.Total;
    document.getElementById("OrderNote").value = subdata.OrderNote;
    const tarih = new Date(subdata.OrderDate);
    var orderDate = tarih.toLocaleString('tr-TR', {
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
function OrderDetailsTableLoad() {
    var columns = [
        { "data": "ProductName" },
        { "data": "UnitType" },
        {"data":"Count"},
        {"data": "Discount"},
        {"data": "Price",},
        {"data":  "VatPrice"}
    ];
    DatatablesLoad("details-datatable", OrderDetails, columns)
}


function GetOrderDetails(id) {
    $.ajax({
        url: '/api/CommerceApi/GetByOrderId?id=' + id,
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
