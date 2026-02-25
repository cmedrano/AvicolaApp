// Autocompletado de localidad y provincia desde código postal usando BD local

// Almacenar referencias de manejadores para poder removerlos después
var georefHandlers = {};

function inicializarGeorefAutocomplete(inputCodigoPostalId, inputLocalidadId, inputProvinciaId) {
    var inputCodigoPostal = document.getElementById(inputCodigoPostalId);
    var inputLocalidad = document.getElementById(inputLocalidadId);
    var inputProvincia = document.getElementById(inputProvinciaId);

    console.log('?? Inicializando Georef Autocomplete para:', {
        codigoPostalId: inputCodigoPostalId,
        localidadId: inputLocalidadId,
        provinciaId: inputProvinciaId
    });

    if (!inputCodigoPostal || !inputLocalidad || !inputProvincia) {
        console.error('?? Elementos no encontrados para el autocompletado:', {
            inputCodigoPostal: !!inputCodigoPostal,
            inputLocalidad: !!inputLocalidad,
            inputProvincia: !!inputProvincia
        });
        return;
    }

    console.log('? Elementos encontrados. Agregando event listener para blur');

    // Crear una clave única para este manejador
    var handlerKey = inputCodigoPostalId + '_blur';

    // Remover manejador anterior si existe
    if (georefHandlers[handlerKey]) {
        inputCodigoPostal.removeEventListener('blur', georefHandlers[handlerKey]);
        console.log('?? Manejador anterior removido');
    }

    // Crear el manejador
    var manejador = function () {
        console.log('?? BLUR EVENT DISPARADO - Código Postal:', inputCodigoPostal.value);
        obtenerLocalidadDesdeCodigoPostal(
            inputCodigoPostal.value,
            inputLocalidad,
            inputProvincia
        );
    };

    // Guardar la referencia
    georefHandlers[handlerKey] = manejador;

    // Agregar el listener
    inputCodigoPostal.addEventListener('blur', manejador);
    console.log('?? Event listener agregado para blur');
}

function removeLocalidadSelectIfExists(inputLocalidad) {
    var selectId = inputLocalidad.id + '_select';
    var existing = document.getElementById(selectId);
    if (existing) {
        existing.removeEventListener && existing.removeEventListener('change', function () { });
        existing.parentNode && existing.parentNode.removeChild(existing);
    }
    inputLocalidad.style.display = 'block';
}

function createLocalidadSelect(inputLocalidad, localidades) {
    removeLocalidadSelectIfExists(inputLocalidad);
    inputLocalidad.style.display = 'none';

    var select = document.createElement('select');
    select.id = inputLocalidad.id + '_select';
    select.className = 'form-select';
    //select.className = inputLocalidad.className + ' form-select mt-2';

    // Option placeholder
    var placeholder = document.createElement('option');
    placeholder.value = '';
    select.appendChild(placeholder);

    localidades.forEach(function (loc) {
        var opt = document.createElement('option');
        opt.value = loc;
        opt.textContent = loc;
        select.appendChild(opt);
    });

    // Cuando el usuario elige, actualizar el inputLocalidad
    select.addEventListener('change', function () {
        inputLocalidad.value = this.value || '';
        inputLocalidad.dispatchEvent(new Event('change'));
        if (this.value) {
            inputLocalidad.classList.remove('is-invalid');
            inputLocalidad.classList.add('is-valid');
        } else {
            inputLocalidad.classList.remove('is-valid');
        }
    });

    // Insertar después del inputLocalidad
    inputLocalidad.parentNode.insertBefore(select, inputLocalidad.nextSibling);
}

function obtenerLocalidadDesdeCodigoPostal(codigoPostal, inputLocalidad, inputProvincia) {
    console.log('?? Iniciando búsqueda para código postal:', codigoPostal);

    if (!codigoPostal || codigoPostal.trim() === '') {
        console.log('? Código postal vacío, limpiando campos');
        inputLocalidad.value = '';
        inputProvincia.value = '';
        removeLocalidadSelectIfExists(inputLocalidad);
        return;
    }

    // Limpiar clases anteriores
    inputLocalidad.classList.remove('is-valid', 'is-invalid');
    inputProvincia.classList.remove('is-valid', 'is-invalid');

    // Mostrar indicador de carga
    inputLocalidad.classList.add('is-loading');
    inputProvincia.classList.add('is-loading');
    console.log('? Mostrando estado de carga');

    var url = '/Clientes/ObtenerLocalidadPorCodigoPostal?codigoPostal=' + encodeURIComponent(codigoPostal);
    console.log('?? URL de consulta:', url);

    fetch(url)
        .then(function (response) {
            console.log('?? Response recibido - Status:', response.status);
            if (!response.ok) {
                throw new Error('HTTP ' + response.status + ': ' + response.statusText);
            }
            return response.json();
        })
        .then(function (data) {
            console.log('?? Respuesta JSON recibida:', data);

            inputLocalidad.classList.remove('is-loading');
            inputProvincia.classList.remove('is-loading');

            if (data.success) {
                console.log('? ÉXITO - Provincia:', data.provincia);
                inputProvincia.value = data.provincia || '';
                inputProvincia.classList.add('is-valid');

                // Si la API devolvió un array de localidades
                if (Array.isArray(data.localidades) && data.localidades.length > 1) {
                    console.log('?? Múltiples localidades recibidas:', data.localidades);
                    // Limpiar input y crear select
                    inputLocalidad.value = '';
                    createLocalidadSelect(inputLocalidad, data.localidades);
                } else if (Array.isArray(data.localidades) && data.localidades.length === 1) {
                    // Un solo resultado
                    removeLocalidadSelectIfExists(inputLocalidad);
                    inputLocalidad.value = data.localidades[0] || '';
                    inputLocalidad.classList.add('is-valid');
                } else if (data.localidad) {
                    // compatibilidad con respuesta antigua que devolvía 'localidad'
                    removeLocalidadSelectIfExists(inputLocalidad);
                    inputLocalidad.value = data.localidad || '';
                    inputLocalidad.classList.add('is-valid');
                } else {
                    removeLocalidadSelectIfExists(inputLocalidad);
                    inputLocalidad.value = '';
                    inputLocalidad.classList.add('is-invalid');
                    inputProvincia.classList.add('is-invalid');
                }

                // Remover las clases de validación después de 3 segundos
                setTimeout(function () {
                    inputLocalidad.classList.remove('is-valid');
                    inputProvincia.classList.remove('is-valid');
                    console.log('?? Removidas clases is-valid después de 3 segundos');
                }, 3000);
            } else {
                console.error('? Error en la búsqueda:', data.mensaje);
                removeLocalidadSelectIfExists(inputLocalidad);
                inputLocalidad.value = '';
                inputProvincia.value = '';
                inputLocalidad.classList.add('is-invalid');
                inputProvincia.classList.add('is-invalid');

                // Remover las clases de validación después de 5 segundos
                setTimeout(function () {
                    inputLocalidad.classList.remove('is-invalid');
                    inputProvincia.classList.remove('is-invalid');
                    console.log('?? Removidas clases is-invalid después de 5 segundos');
                }, 5000);
            }
        })
        .catch(function (error) {
            console.error('?? ERROR en la solicitud fetch:', error);
            inputLocalidad.classList.remove('is-loading');
            inputProvincia.classList.remove('is-loading');
            removeLocalidadSelectIfExists(inputLocalidad);
        });
}
