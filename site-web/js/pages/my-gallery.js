if (!POLYPAINTPRO.login.isLoggedIn()) { //User is not logged in, redirect to login
    window.location.replace('./login.html');
}

$(document).ready(function() {
    var user = POLYPAINTPRO.login.getUser();
    var listEndpoint = POLYPAINTPRO.login.getServer() + "/drawings?owner=" + user.id;
    POLYPAINTPRO.portfolio.getDrawings(listEndpoint);

    $("#gallery-title-name").text("Galerie de " + user.username);
});
