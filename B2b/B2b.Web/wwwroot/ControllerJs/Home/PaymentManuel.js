var DataList = [];
$(document).ready(function () {
    PageLoad();
});
function PageLoad() {
    $.ajax({
        url: '/api/PaymentApi/GetAllHistory',
        type: 'Get',
        dataType: 'Json',
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            DataList = data.List;
            var columns = [
                { "data": "MerchantPaymentId" },
                { "data": "CreateDate" },
                { "data": "Amount" },
                {
                    "data": "State",
                    render: function (x) { return x==true?"Ödeme Yapıldı" : "Ödeme Yapılmadı" }
                }
            ];
            DatatablesLoad("datatables", DataList, columns);
        }
    });
}


function ManuelPos() {

    var paymentAmount = parseFloat(document.getElementById("Amount").value.replace(',', '.')); 
    $.ajax({
        url: '/api/PaymentApi/GetManuelToken',
        type: 'Post',
        contentType: 'application/json',
        dataType: 'json',
        data: JSON.stringify(paymentAmount),
        headers: { 'Authorization': localStorage.getItem("token") },
        success: function (data) {
            if (data.SessionToken != null) {
                var hppUrl = "https://pos.vakifpays.com.tr/payment/" + data.SessionToken;
                window.location.href = hppUrl;
            }

        },
        error: function (x) {
            console.clear();
            console.log(x);
        },
        async: false
    });
}