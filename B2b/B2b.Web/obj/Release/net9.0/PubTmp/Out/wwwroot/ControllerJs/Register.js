function Register() {
    var data = {
        FirstName: document.getElementById("FirstName").value,
        LastName: document.getElementById("LastName").value,
        Password: document.getElementById("Password").value,
        Email: document.getElementById("Email").value,
        Phone: document.getElementById("Phone").value,
    }
    $.ajax({
        url: '/api/UserInfoApi/UserRegister',
        type: 'Post',
        dataType: 'Json',
        data: JSON.stringify(data),
        headers: { 'Authorization': localStorage.getItem("token") },
        contentType: 'application/json',
        success: function (data) {
            console.clear();
            console.log(data);
            if (true) {
                Swal.fire({
                    title: "Kayıt İşlemi",
                    text: "Kayıt işleminiz tamamlanmıştır.",
                    icon: "success",
                });
                document.getElementById("FirstName").value = "";
                document.getElementById("LastName").value = "";
                document.getElementById("Password").value = "";
                document.getElementById("Email").value = "";
                document.getElementById("Phone").value = "";
            }
            else {
                Swal.fire({
                    title: "Kayıt İşlemi !",
                    text: data.Message,
                    icon: "error",
                });
            }
           
        },
        error: function () { },
        async: false
    });
}