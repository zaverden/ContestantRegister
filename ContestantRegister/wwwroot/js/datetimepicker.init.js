$(document).ready(() => {
    moment.locale($('html').attr('lang'));
    $('.apply-datetimepicker').datetimepicker();
    $('.apply-datetimepicker-date').datetimepicker({ format: 'L' });
    $('.apply-datetimepicker-time').datetimepicker({ format: 'LT' });
});

