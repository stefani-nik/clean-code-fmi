$(function () {
    $('.alert-success').click(function () {
        $(this).fadeOut();
    });
    setTimeout(function () {
        $('.alert-success').fadeOut();
    }, 2000);
});