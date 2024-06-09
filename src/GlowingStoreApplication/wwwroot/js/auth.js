function auth(language) {
    Alpine.data("auth", () => ({
        firstName: '',
        lastName: '',
        email: '',
        userName: '',
        password: '',
        confirmPassword: '',
        isBusy: false,
        isPersistent: false,
        passwordErrorMessage: '',

        clear: function () {
            this.firstName = '';
            this.lastName = '';
            this.email = '';
            this.userName = '';
            this.password = '';
            this.confirmPassword = '';
            this.isBusy = false;
            this.isPersistent = false;
            this.passwordErrorMessage = '';
        },

        cancel: function () {
            this.clear();
            document.window.href = '/';
        },

        login: async function () {
            this.isBusy = true;

            try {
                var request = {
                    userName: this.userName,
                    password: this.password,
                    isPersistent: this.isPersistent
                };

                var response = await fetch('/api/auth/login', {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                var content = await response.json();
                var errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    window.localStorage.setItem('access_token', content.accessToken);
                }
                else {
                    alert(errorMessage);
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        register: async function () {
            this.isBusy = true;

            if (!this.checkPassword()) {
                this.passwordErrorMessage = "the passwords aren't matching";
                this.isBusy = false;
                return;
            }

            try {
                var request = {
                    firstName: this.firstName,
                    lastName: this.lastName,
                    email: this.email,
                    password: this.password
                };

                var response = await fetch('/api/auth/register', {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                if (response.ok) {
                    window.location.href = '/index';
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        reset: async function () {
            this.isBusy = true;

            try {
                var request = { email: this.email };
                var response = await fetch('/api/auth/resetpassword', {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                var content = await response.json();
                var errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    window.localStorage.setItem('email', this.email);
                    window.localStorage.setItem('reset_token', content.token);
                    window.location.href = '/UpdatePassword';
                } 

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        updatePassword: async function () {
            this.isBusy = true;

            if (!this.checkPassword()) {
                this.passwordErrorMessage = "the passwords aren't matching";
                this.isBusy = false;
                return;
            }

            try {
                var email = window.localStorage.getItem('email');
                var token = window.localStorage.getItem('reset_token');

                var request = {
                    email: email,
                    token: token,
                    newPassword: this.password
                };

                var response = await fetch('/api/auth/updatepassword', {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Accept-Language": language
                    },
                    body: JSON.stringify(request)
                });

                var content = await response.json();
                var errorMessage = GetErrorMessage(response.status, content);

                if (errorMessage == null) {
                    window.localStorage.removeItem('email');
                    window.localStorage.removeItem('reset_token');
                    window.location.href = '/index';
                }
                else {
                    alert(errorMessage);
                }

            } catch (error) {
                alert(error);
            }
            finally {
                this.isBusy = false;
            }
        },

        invalidLoginForm: function () {
            return this.userName.trim().length === 0 && this.password.trim().length === 0;
        },

        invalidRegisterForm: function () {
            return this.firstName.trim().length === 0 && this.lastName.trim().length === 0
                && this.email.trim().length === 0 && this.password.trim().length === 0
                && this.confirmPassword.trim().length === 0;
        },

        checkPassword: function () {
            return this.confirmPassword.trim().toUpperCase() === this.password.trim().toUpperCase();
        }
    }));
}