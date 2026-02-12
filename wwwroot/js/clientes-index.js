var validacionEmailEditarUrl = '/Clientes/ValidarEmail';

document.addEventListener('DOMContentLoaded', function() {
    var inputNombre = document.getElementById('inputNombre');
    var inputNombreFantasia = document.getElementById('inputNombreFantasia');
    var debounceTimer = null;

    function buscarDinamicamente() {
        var nombre = inputNombre.value.trim();
        var fantasia = inputNombreFantasia.value.trim();

        if (debounceTimer) {
            clearTimeout(debounceTimer);
        }

        var debeHacerBusqueda = (nombre.length >= 3 || fantasia.length >= 3 || (nombre.length === 0 && fantasia.length === 0));

        if (!debeHacerBusqueda) {
            return;
        }

        debounceTimer = setTimeout(function() {
            var url = '/Clientes/Index?page=1&searchNombre=' + encodeURIComponent(nombre) + '&searchFantasia=' + encodeURIComponent(fantasia);

            fetch(url)
                .then(function(response) { return response.text(); })
                .then(function(html) {
                    var parser = new DOMParser();
                    var doc = parser.parseFromString(html, 'text/html');

                    var newTable = doc.querySelector('#tablaClientesBody');
                    var newPaginationContainer = doc.querySelector('.d-flex.justify-content-between.align-items-center.mt-4');

                    var currentTable = document.querySelector('#tablaClientesBody');
                    var currentPaginationContainer = document.querySelector('.d-flex.justify-content-between.align-items-center.mt-4');

                    if (newTable && currentTable) {
                        currentTable.innerHTML = newTable.innerHTML;
                        reattachEditButtons();
                    }

                    if (newPaginationContainer && currentPaginationContainer) {
                        currentPaginationContainer.innerHTML = newPaginationContainer.innerHTML;
                        agregarEventListenersPaginacion();
                    } else if (!newPaginationContainer && currentPaginationContainer) {
                        currentPaginationContainer.style.display = 'none';
                    }
                })
                .catch(function(error) { console.error('Error:', error); });
        }, 300);
    }

    function reattachEditButtons() {
        var editButtons = document.querySelectorAll('[data-bs-target="#modalEditarCliente"]');
        editButtons.forEach(function(button) {
            button.addEventListener('click', function(e) {});
        });
    }

    function agregarEventListenersPaginacion() {
        var paginationLinks = document.querySelectorAll('.pagination a');
        paginationLinks.forEach(function(link) {
            link.addEventListener('click', function(e) {
                e.preventDefault();
                var url = this.href;

                fetch(url)
                    .then(function(response) { return response.text(); })
                    .then(function(html) {
                        var parser = new DOMParser();
                        var doc = parser.parseFromString(html, 'text/html');

                        var newTable = doc.querySelector('#tablaClientesBody');
                        var newPaginationContainer = doc.querySelector('.d-flex.justify-content-between.align-items-center.mt-4');

                        var currentTable = document.querySelector('#tablaClientesBody');
                        var currentPaginationContainer = document.querySelector('.d-flex.justify-content-between.align-items-center.mt-4');

                        if (newTable && currentTable) {
                            currentTable.innerHTML = newTable.innerHTML;
                            reattachEditButtons();
                        }

                        if (newPaginationContainer && currentPaginationContainer) {
                            currentPaginationContainer.innerHTML = newPaginationContainer.innerHTML;
                            agregarEventListenersPaginacion();
                        }
                    })
                    .catch(function(error) { console.error('Error:', error); });
            });
        });
    }

    inputNombre.addEventListener('keyup', function() {
        buscarDinamicamente();
    });

    inputNombreFantasia.addEventListener('keyup', function() {
        buscarDinamicamente();
    });

    var modalEditarCliente = document.getElementById('modalEditarCliente');
    var editClienteEmail = document.getElementById('editClienteEmail');
    var errorEmailEditar = document.getElementById('errorEmailEditar');
    var formEditarCliente = document.getElementById('formEditarCliente');
    var editClienteId = document.getElementById('editClienteId');
    var emailValidarTimer = null;
    var emailEditarValido = true;

    modalEditarCliente.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;

        var clienteId = button.getAttribute('data-cliente-id');
        var clienteNombre = button.getAttribute('data-cliente-nombre');
        var clienteTelefono = button.getAttribute('data-cliente-telefono');
        var clienteEmail = button.getAttribute('data-cliente-email');
        var clienteCelular = button.getAttribute('data-cliente-celular');
        var clienteDomicilio = button.getAttribute('data-cliente-domicilio');
        var clienteLocalidad = button.getAttribute('data-cliente-localidad');
        var clienteProvincia = button.getAttribute('data-cliente-provincia');
        var clienteCodigoPostal = button.getAttribute('data-cliente-codigo-postal');
        var clienteDNI = button.getAttribute('data-cliente-dni');
        var clienteCUIT = button.getAttribute('data-cliente-cuit');
        var clienteFantasia = button.getAttribute('data-cliente-fantasia');
        var clienteCategoria = button.getAttribute('data-cliente-categoria');
        var clienteOperacionesContado = button.getAttribute('data-cliente-operaciones-contado');
        var clienteInhabilitado = button.getAttribute('data-cliente-inhabilitado');

        editClienteId.value = clienteId;
        document.getElementById('editClienteNombre').value = clienteNombre;
        document.getElementById('editClienteTelefono').value = clienteTelefono || '';
        editClienteEmail.value = clienteEmail || '';
        document.getElementById('editClienteCelular').value = clienteCelular || '';
        document.getElementById('editClienteDomicilio').value = clienteDomicilio || '';
        document.getElementById('editClienteLocalidad').value = clienteLocalidad || '';
        document.getElementById('editClienteProvincia').value = clienteProvincia || '';
        document.getElementById('editClienteCodigoPostal').value = clienteCodigoPostal || '';
        document.getElementById('editClienteDNI').value = clienteDNI || '';
        document.getElementById('editClienteCUIT').value = clienteCUIT || '';
        document.getElementById('editClienteFantasia').value = clienteFantasia || '';
        document.getElementById('editClienteCategoria').value = clienteCategoria || '';
        document.getElementById('editOperacionesContado').checked = clienteOperacionesContado === 'True';
        document.getElementById('editInhabilitado').checked = clienteInhabilitado === 'True';
        
        errorEmailEditar.style.display = 'none';
        editClienteEmail.classList.remove('is-invalid');
        emailEditarValido = true;
    });

    editClienteEmail.addEventListener('change', function() {
        validarEmailEditar();
    });

    editClienteEmail.addEventListener('keyup', function() {
        clearTimeout(emailValidarTimer);
        emailValidarTimer = setTimeout(function() {
            validarEmailEditar();
        }, 500);
    });

    function validarEmailEditar() {
        var email = editClienteEmail.value.trim();
        var clienteId = editClienteId.value;

        if (email === '') {
            emailEditarValido = true;
            errorEmailEditar.style.display = 'none';
            editClienteEmail.classList.remove('is-invalid');
            return;
        }

        var url = validacionEmailEditarUrl + '?email=' + encodeURIComponent(email) + '&clienteId=' + clienteId;
        fetch(url)
            .then(function(response) { return response.json(); })
            .then(function(data) {
                if (data.valido) {
                    emailEditarValido = true;
                    errorEmailEditar.style.display = 'none';
                    editClienteEmail.classList.remove('is-invalid');
                } else {
                    emailEditarValido = false;
                    errorEmailEditar.textContent = data.mensaje;
                    errorEmailEditar.style.display = 'block';
                    editClienteEmail.classList.add('is-invalid');
                }
            })
            .catch(function(error) { console.error('Error:', error); });
    }

    formEditarCliente.addEventListener('submit', function(e) {
        e.preventDefault();

        var email = editClienteEmail.value.trim();
        if (email !== '' && !emailEditarValido) {
            errorEmailEditar.textContent = 'Por favor, corrija el email antes de continuar';
            errorEmailEditar.style.display = 'block';
            return;
        }

        this.submit();
    });

    var modalConfirmarEliminar = document.getElementById('modalConfirmarEliminar');
    var clienteIdToDelete = null;

    modalConfirmarEliminar.addEventListener('show.bs.modal', function (event) {
        var button = event.relatedTarget;
        clienteIdToDelete = button.getAttribute('data-cliente-id');
    });

    document.getElementById('btnConfirmarEliminar').addEventListener('click', function() {
        if (clienteIdToDelete) {
            var form = document.createElement('form');
            form.method = 'POST';
            form.action = '/Clientes/EliminarCliente';

            var input = document.createElement('input');
            input.type = 'hidden';
            input.name = 'id';
            input.value = clienteIdToDelete;

            form.appendChild(input);
            document.body.appendChild(form);
            form.submit();
        }
    });

    // Modal Ver Cliente
    var modalVerCliente = document.getElementById('modalVerCliente');
    if (modalVerCliente) {
        modalVerCliente.addEventListener('show.bs.modal', function (event) {
            var button = event.relatedTarget;

            var clienteNombre = button.getAttribute('data-cliente-nombre');
            var clienteTelefono = button.getAttribute('data-cliente-telefono');
            var clienteEmail = button.getAttribute('data-cliente-email');
            var clienteCelular = button.getAttribute('data-cliente-celular');
            var clienteDomicilio = button.getAttribute('data-cliente-domicilio');
            var clienteLocalidad = button.getAttribute('data-cliente-localidad');
            var clienteProvincia = button.getAttribute('data-cliente-provincia');
            var clienteCodigoPostal = button.getAttribute('data-cliente-codigo-postal');
            var clienteDNI = button.getAttribute('data-cliente-dni');
            var clienteCUIT = button.getAttribute('data-cliente-cuit');
            var clienteFantasia = button.getAttribute('data-cliente-fantasia');
            var clienteCategoria = button.getAttribute('data-cliente-categoria');
            var clienteOperacionesContado = button.getAttribute('data-cliente-operaciones-contado');
            var clienteInhabilitado = button.getAttribute('data-cliente-inhabilitado');

            document.getElementById('verClienteNombreTitle').textContent = clienteNombre;
            document.getElementById('verClienteNombre').value = clienteNombre;
            document.getElementById('verClienteTelefono').value = clienteTelefono || '';
            document.getElementById('verClienteEmail').value = clienteEmail || '';
            document.getElementById('verClienteCelular').value = clienteCelular || '';
            document.getElementById('verClienteDomicilio').value = clienteDomicilio || '';
            document.getElementById('verClienteLocalidad').value = clienteLocalidad || '';
            document.getElementById('verClienteProvincia').value = clienteProvincia || '';
            document.getElementById('verClienteCodigoPostal').value = clienteCodigoPostal || '';
            document.getElementById('verClienteDNI').value = clienteDNI || '';
            document.getElementById('verClienteCUIT').value = clienteCUIT || '';
            document.getElementById('verClienteFantasia').value = clienteFantasia || '';
            document.getElementById('verClienteCategoria').value = clienteCategoria || '';
            document.getElementById('verOperacionesContado').checked = clienteOperacionesContado === 'True';
            document.getElementById('verInhabilitado').checked = clienteInhabilitado === 'True';
        });
    }
});
