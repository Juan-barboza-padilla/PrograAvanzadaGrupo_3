$(document).ready(function () {
    $(".adminCheckbox").change(function () {
        const userId = $(this).data("user-id");
        const isAdmin = $(this).is(":checked");

        $.ajax({
            url: '/Admin/ToggleAdminRole',
            type: 'POST',
            data: {
                id: userId,
                isAdmin: isAdmin
            },
            success: function (result) {
                alert('Rol actualizado con éxito');
            },
            error: function (error) {
                alert('Ocurrió un error al actualizar el rol');
            }
        });
    });
});
