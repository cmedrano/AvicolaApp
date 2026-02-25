var validacionEmailNuevoUrl = '/Clientes/ValidarEmail';

document.addEventListener('DOMContentLoaded', function() {
    var inputEmailNuevo = document.getElementById('inputEmailNuevo');
    var errorEmailNuevo = document.getElementById('errorEmailNuevo');
    var formNuevoCliente = document.getElementById('formNuevoCliente');
    var emailValido = true;
    var emailValidationTimer = null;

    function validarEmailNuevo() {
        var email = inputEmailNuevo.value.trim();

        if (email === '') {
            emailValido = true;
            errorEmailNuevo.style.display = 'none';
            inputEmailNuevo.classList.remove('is-invalid');
            return;
        }

        var url = validacionEmailNuevoUrl + '?email=' + encodeURIComponent(email);
        fetch(url)
            .then(function(response) { return response.json(); })
            .then(function(data) {
                if (data.valido) {
                    emailValido = true;
                    errorEmailNuevo.style.display = 'none';
                    inputEmailNuevo.classList.remove('is-invalid');
                } else {
                    emailValido = false;
                    errorEmailNuevo.textContent = data.mensaje;
                    errorEmailNuevo.style.display = 'block';
                    inputEmailNuevo.classList.add('is-invalid');
                }
            })
            .catch(function(error) { console.error('Error:', error); });
    }

    inputEmailNuevo.addEventListener('change', function() {
        validarEmailNuevo();
    });

    inputEmailNuevo.addEventListener('keyup', function() {
        clearTimeout(emailValidationTimer);
        emailValidationTimer = setTimeout(function() {
            validarEmailNuevo();
        }, 500);
    });

    var modalNuevoCliente = document.getElementById('modalNuevoCliente');
    if (modalNuevoCliente) {
        modalNuevoCliente.addEventListener('show.bs.modal', function() {
            console.log('?? Modal NUEVO CLIENTE ABIERTO - Inicializando autocompletado');
            
            formNuevoCliente.reset();
            emailValido = true;
            errorEmailNuevo.style.display = 'none';
            inputEmailNuevo.classList.remove('is-invalid');

            // Inicializar autocompletado de Georef para crear nuevo cliente
            if (typeof inicializarGeorefAutocomplete === 'function') {
                console.log('? inicializarGeorefAutocomplete es una función');
                inicializarGeorefAutocomplete(
                    'inputCodigoPostalNuevo',
                    'inputLocalidadNueva',
                    'inputProvinciaNueva'
                );
            } else {
                console.error('? inicializarGeorefAutocomplete NO ES UNA FUNCIÓN');
            }
        });
    }

    formNuevoCliente.addEventListener('submit', function(e) {
        e.preventDefault();

        var email = inputEmailNuevo.value.trim();
        
        if (email !== '' && !emailValido) {
            errorEmailNuevo.textContent = 'Por favor, corrija el email antes de continuar';
            errorEmailNuevo.style.display = 'block';
            inputEmailNuevo.focus();
            return false;
        }

        this.submit();
    });
});
