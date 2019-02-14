// override jquery validate plugin defaults
(function ($) {
    var defaultOptions = {
        errorElement: 'div',
        validClass: 'is-valid',
        errorClass: 'is-invalid',
        highlight: function (element, errorClass, validClass) {
            $(element)
                .removeClass(validClass)
                .addClass(errorClass);
        },
        unhighlight: function (element, errorClass, validClass) {
            $(element)
                .removeClass(errorClass)
                .addClass(validClass);

        },
        submitHandler: function (form) {
            // do other things for a valid form
            $(form).addClass('was-validated');
            form.submit();
        },
        invalidHandler: function (event, validator) {
            $(form).addClass('was-validated');
        },
        error: function (label, element) {
            debugger;
        },
        success: function (label) {
            debugger;
        }
    };

    $.validator.setDefaults(defaultOptions);

    $.validator.unobtrusive.options = {
        errorClass: defaultOptions.errorClass,
        validClass: defaultOptions.validClass,
    };

    $("form").each(function () {
        $(this).addClass('needs-validation');
        var validator = $(this).validator(defaultOptions);
    });
})(jQuery);