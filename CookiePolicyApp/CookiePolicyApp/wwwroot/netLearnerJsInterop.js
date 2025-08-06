window.netLearnerJsFunctions = {
    acceptMessage: function (cookieString) {
        document.cookie = cookieString;
    },


    hideMessage: function () {
        $("#cookieConsent").fadeOut();
    }
};