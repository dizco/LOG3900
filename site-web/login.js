String.prototype.isEmpty = function() {
    return (this.length === 0 || !this.trim());
};

$(document).ready(function() {
    $("#login-form").submit(function(e){
        e.preventDefault();
        var $form = $(this);

        var serverAddress = $form.find("#login-form-ip").val();
        var endpoint = "http://" + serverAddress + "/login";
        var email = $form.find("#login-form-username").val();
        var password = $form.find("#login-form-password").val();

        if (!email.isEmpty() && !password.isEmpty() && !serverAddress.isEmpty()) {
            $.post(endpoint, {email: email, password: password})
                .done(function(data, textStatus, jqXHR) {
                    window.location.replace('./index.html');
                })
                .fail(function( jqXHR, textStatus, errorThrown) {
                    alert("L'adresse spécifiée n'est pas valide. L'adresse du serveur doit être de la forme : XXX.XXX.XXX.XXX:5025");
                    console.log("Failed to login", textStatus, errorThrown);
                });
        }
        else {
            alert("Remplissez tous les champs!");
        }
    });
});
