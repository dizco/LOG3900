if (POLYPAINTPRO.login.isLoggedIn()) {
    window.location.replace('./index.html');
}

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
                    POLYPAINTPRO.login.saveServer("http://" + serverAddress);
                    POLYPAINTPRO.login.saveUser(data.objectId, email);
                    window.location.replace('./index.html');
                })
                .fail(function( jqXHR, textStatus, errorThrown) {
                    alert("Identifiant, mot de passe ou adresse de serveur invalide. Assurez vous que l'adresse du serveur soit de la forme : XXX.XXX.XXX.XXX:5025");
                });
        }
        else {
            alert("Remplissez tous les champs!");
        }
    });
});
