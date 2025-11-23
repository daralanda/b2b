function RecoverPw() {
    $.ajax({
        url: '/api/UserInfoApi/RecorveryPassword?email=' + document.getElementById("useremail").value,
        type: 'Post',
        dataType: 'Json',
        data: {} ,
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {

            if (data.State) {
                Swal.fire({
                    title: "Şifre Sıfırlama",
                    text: "Şifreniz mail adresinize iletilmiştir.",
                    icon: "success",
                });
                document.getElementById("useremail").value = "";
            }
            else {
                Swal.fire({
                    title: "Şifre Sıfırlama !",
                    text: data.Message,
                    icon: "error",
                });
            }
        },
        error: function () { },
        async: false
    });
}