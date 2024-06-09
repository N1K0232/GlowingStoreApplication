function me(language) {
    Alpine.data("me", () => ({
        user: {
            firstName: '',
            lastName: '',
            email: ''
        },

        reset: function () {
            this.user = {};
        },

        get: async function () {
            var accessToken = window.localStorage.getItem('access_token');
            var response = await fetch('api/me', {
                method: "GET",
                headers: {
                    "Content-type": "application/json",
                    "Accept-Language": language,
                    "Authorization": `bearer ${accessToken}`
                }
            });

            var content = await response.json();
            var errorMessage = GetErrorMessage(response.status, content);

            if (errorMessage == null) {
                alert(`Welcome ${content.firstName} ${content.lastName}`);
            }
            else {
                alert(errorMessage);
            }
        }
    }))
}