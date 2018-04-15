if (!POLYPAINTPRO.login.isLoggedIn()) { //User is not logged in, redirect to login
    window.location.replace('./login.html');
}

$(document).ready(function() {
    var user = POLYPAINTPRO.login.getUser();
    $("#profile-title-name").text(user.username);

    $("#password-reset-form").submit(function(e){
        e.preventDefault();
        var $form = $(this);

        var endpoint = POLYPAINTPRO.login.getServer() + "/account/password";

        var oldPassword = $form.find("#old-form-password").val();
        var password = $form.find("#new-form-password1").val();
        var passwordConfirmation = $form.find("#new-form-password2").val();
        var $resultElement = $("#password-reset-result");

        if (!oldPassword.isEmpty() && !password.isEmpty() && !passwordConfirmation.isEmpty()) {
            disableForm($form);

            $.ajax({
                url: endpoint,
                type: "put",
                data: {"old-password": oldPassword, "password": password, "confirm-password": passwordConfirmation},
                success: function(data, textStatus, jqXHR) {
                    resetForm($form);
                    $resultElement.attr("data-notify-type", "success")
                        .attr("data-notify-msg", "Mot de passe changé avec succès.")
                        .html("");
                    SEMICOLON.widget.notifications($resultElement);
                },
                error: function(jqXHR, textStatus, errorThrown) {
                    console.log("Failed to reset password", jqXHR.responseText, textStatus, errorThrown);
                    enableForm($form);

                    var errorMessage = "Une erreur est survenue lors du changement de mot de passe. ";
                    if (jqXHR.status === 422) {
                        var errors = JSON.parse(jqXHR.responseText);
                        errorMessage += buildHintsString(errors)
                    }
                    else if (jqXHR.status === 401) {
                        errorMessage += "Vous n'êtes pas autorisé à faire cette modification. ";
                    }
                    $resultElement.attr("data-notify-type", "error")
                        .attr("data-notify-msg", errorMessage)
                        .attr("data-notify-timeout", 10000)
                        .html("");
                    SEMICOLON.widget.notifications($resultElement);
                }
            });
        }
        else {
            $resultElement.attr("data-notify-type", "error")
                .attr("data-notify-msg", "Remplissez tous les champs!")
                .attr("data-notify-timeout", 10000)
                .html("");
            SEMICOLON.widget.notifications($resultElement);
        }
    });

    function resetForm($form) {
        enableForm($form);
        $form.find("input").each(function(index, element) {
            $(element).val("");
        });
    }

    function disableForm($form) {
        $form.find("input").each(function(index, element) {
            $(element).prop("disabled", true);
        });
        $form.find("button").each(function(index, element) {
            $(element).prop("disabled", true);
        });
    }

    function enableForm($form) {
        $form.find("input").each(function(index, element) {
            $(element).prop("disabled", false);
        });
        $form.find("button").each(function(index, element) {
            $(element).prop("disabled", false);
        });
    }

    function buildHintsString(error) {
        var hintsString = "";
        error.hints.forEach(function(hint) {
            hintsString += "- " + hint.msg + "\n";
        });
        return hintsString;
    }
});
