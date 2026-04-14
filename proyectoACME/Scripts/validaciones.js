function enviarFormulario() {
    var inputs = document.querySelectorAll('[data-requerido="true"]');
    var valido = true;

    inputs.forEach(function (input) {
        var id = input.id;
        var err = document.getElementById('err_' + id.replace('campo_', ''));
        if (!input.value.trim()) {
            input.classList.add('input-error');
            if (err) err.classList.add('visible');
            valido = false;
        } else {
            input.classList.remove('input-error');
            if (err) err.classList.remove('visible');
        }
    });

    // Verificar que al menos un campo tenga datos
    if (valido) {
        var todosLosInputs = document.querySelectorAll('#formPublico input[name^="campo_"]');
        var algunCampoConValor = false;
        todosLosInputs.forEach(function (input) {
            if (input.value.trim()) {
                algunCampoConValor = true;
            }
        });

        if (!algunCampoConValor) {
            mostrarModalCampo();
            valido = false;
        }
    }

    if (valido) {
        document.getElementById('formPublico').submit();
    }
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('[data-requerido="true"]').forEach(function (input) {
        input.addEventListener('input', function () {
            var id = this.id;
            var err = document.getElementById('err_' + id.replace('campo_', ''));
            if (this.value.trim()) {
                this.classList.remove('input-error');
                if (err) err.classList.remove('visible');
            }
        });
    });
});

function mostrarModalCampo() {
    document.getElementById('modalCampoVacio').style.display = 'flex';
}

function cerrarModalCampo() {
    document.getElementById('modalCampoVacio').style.display = 'none';
}