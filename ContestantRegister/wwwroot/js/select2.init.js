$(document).ready(() => {
    $.fn.select2.defaults.set("theme", "bootstrap");
    $.fn.select2.defaults.set("allowClear", true);
    $.fn.select2.defaults.set("placeholder",
        function () {
            return $(this).data('placeholder');
        });
    $('.apply-select2').select2();
});

