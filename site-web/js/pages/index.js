if (!POLYPAINTPRO.login.isLoggedIn()) { //User is not logged in, redirect to login
    window.location.replace('./login.html');
}

$(document).ready(function() {
    var listEndpoint = POLYPAINTPRO.login.getServer() + "/drawings?visibility=public";
    POLYPAINTPRO.portfolio.getDrawings(listEndpoint);
});
