var indiceCampo = document.querySelectorAll('#tbodyCampos tr').length;

function agregarCampo() {
    var tbody = document.getElementById('tbodyCampos');
    var tr = document.createElement('tr');
    tr.innerHTML =
        '<td><span class="drag-handle">⠿</span></td>' +
        '<td>' +
        '<input type="hidden" name="Campos[' + indiceCampo + '].Id" value="0" />' +
        '<input type="text" name="Campos[' + indiceCampo + '].NombreCampo" ' +
        'class="campo-input" placeholder="nombre_campo" />' +
        '</td>' +
        '<td>' +
        '<input type="text" name="Campos[' + indiceCampo + '].TituloCampo" ' +
        'class="campo-input" placeholder="Título visible" />' +
        '</td>' +
        '<td style="text-align:center;">' +
        '<input type="checkbox" name="Campos[' + indiceCampo + '].EsRequerido" value="true" />' +
        '</td>' +
        '<td>' +
        '<select name="Campos[' + indiceCampo + '].TipoCampo" class="campo-select">' +
        '<option value="">-- Tipo --</option>' +
        '<option value="Texto">Texto</option>' +
        '<option value="Numero">Número</option>' +
        '<option value="Fecha">Fecha</option>' +
        '</select>' +
        '</td>' +
        '<td>' +
        '<button type="button" class="btn-eliminar-campo" onclick="eliminarCampo(this)">×</button>' +
        '</td>';
    tbody.appendChild(tr);
    indiceCampo++;
}

function eliminarCampo(btn) {
    var tr = btn.closest('tr');
    tr.parentNode.removeChild(tr);
    reindexarCampos();
}

function reindexarCampos() {
    var filas = document.querySelectorAll('#tbodyCampos tr');
    filas.forEach(function (tr, i) {
        tr.querySelectorAll('input, select').forEach(function (el) {
            if (el.name) {
                el.name = el.name.replace(/Campos\[\d+\]/, 'Campos[' + i + ']');
            }
        });
    });
    indiceCampo = filas.length;
}

function guardarEncuesta() {
    var nombre = document.getElementById('Nombre').value.trim();
    if (!nombre) {
        showToast('El nombre de la encuesta es requerido', 'error');
        return;
    }

    var filas = document.querySelectorAll('#tbodyCampos tr');
    if (filas.length === 0) {
        showToast('Debes agregar al menos un campo', 'error');
        return;
    }

    var valido = true;
    filas.forEach(function (tr, i) {
        var nombreCampo = tr.querySelector('input[name*="NombreCampo"]').value.trim();
        var tituloCampo = tr.querySelector('input[name*="TituloCampo"]').value.trim();
        var tipoCampo = tr.querySelector('select[name*="TipoCampo"]').value;
        if (!nombreCampo || !tituloCampo || !tipoCampo) {
            showToast('El campo #' + (i + 1) + ' tiene datos incompletos', 'error');
            valido = false;
        }
    });

    if (valido) {
        document.getElementById('formEncuesta').submit();
    }
}