function showConfirm(message, title) {
    return new Promise(function (resolve) {
        var modal = new bootstrap.Modal(document.getElementById('confirmModal'));
        document.getElementById('confirmModalTitle').textContent = title || 'Confirm';
        document.getElementById('confirmModalBody').textContent = message;

        var okBtn = document.getElementById('confirmModalOk');
        function onOk() {
            modal.hide();
            okBtn.removeEventListener('click', onOk);
            resolve(true);
        }
        okBtn.addEventListener('click', onOk);

        document.getElementById('confirmModal').addEventListener('hidden.bs.modal', function () {
            okBtn.removeEventListener('click', onOk);
            resolve(false);
        }, { once: true });

        modal.show();
    });
}

document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.delete-confirm').forEach(function (form) {
        form.addEventListener('submit', function (e) {
            e.preventDefault();
            var f = this;
            var msg = f.getAttribute('data-confirm-msg') || 'Are you sure?';
            showConfirm(msg, 'Confirm Delete').then(function (ok) {
                if (ok) f.submit();
            });
        });
    });
});
