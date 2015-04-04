$(function () {
    $("input[class='quantity_spinner']").TouchSpin({min: 0, max: 10000, step: 0.5, decimals: 1, boostat: 5, maxboostedstep: 10 });
});