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
                {
                    "data": function (x) {
                        return `<img src="${x.Logo} " alt="${x.Bank} " height="18">`;
                    }
                },
                { "data": "Bank" },
                { "data": "BranchCode" },
                { "data": "AccountNo" },
                { "data": "AccountName" },
                { "data": "Currency" },
                { "data": "IBAN" }
            ];
            DatatablesLoad("datatables", DataList, columns);
        }
    });
}


