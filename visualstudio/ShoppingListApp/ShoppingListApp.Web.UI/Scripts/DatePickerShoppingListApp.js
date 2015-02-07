$(function () {
    jQuery('#_datetimepicker_en-US').datetimepicker();
});

$(function () {
    jQuery('#_datetimepicker_de-DE').datetimepicker({
        lang: 'de',
        i18n: {
            de: { // German
                months: [
					'Januar', 'Februar', 'März', 'April', 'Mai', 'Juni', 'Juli', 'August', 'September', 'Oktober', 'November', 'Dezember'
                ],
                dayOfWeek: [
					"So", "Mo", "Di", "Mi", "Do", "Fr", "Sa"
                ]
            }
        },
        timepicker: false,
        //forced to take the en-US format of date and time otherwise model binding does not work and date is rejected or worst taken wrong!
        // month is taken as day and day is taken as month during data binding!
        format: 'm/d/Y'
    });
});

$(function () {
    jQuery('#_datetimepicker_fr-FR').datetimepicker({
        lang: 'fr',
        i18n: {
            fr: { //French
                months: [
					"Janvier", "Février", "Mars", "Avril", "Mai", "Juin", "Juillet", "Août", "Septembre", "Octobre", "Novembre", "Décembre"
                ],
                dayOfWeek: [
					"Dim", "Lun", "Mar", "Mer", "Jeu", "Ven", "Sam"
                ]
            }
        },
        timepicker: false,
        format: 'm/d/Y'
    });
});