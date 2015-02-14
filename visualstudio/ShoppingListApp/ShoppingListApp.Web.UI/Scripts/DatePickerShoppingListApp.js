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
        //culture insensitive date format yyyy-mm-dd accepted everywhere.
        format: 'Y-m-d'
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
        format: 'Y-m-d'
    });
});