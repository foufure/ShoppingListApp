﻿$(function () {
    $("input[class='quantity_spinner']").TouchSpin({min: 0, max: 10000, step: 1, decimals: 0, boostat: 10, maxboostedstep: 20 });
});