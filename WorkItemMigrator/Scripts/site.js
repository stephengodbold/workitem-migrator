
function foldSection(sectionSelector, foldspeed) {
    var section = $(sectionSelector);

    if (section.is(':hidden')) {
        section.slideDown(foldspeed);
        $('.fold-prompt').hide(foldspeed);
    } else {
        section.slideUp(foldspeed);
        $('.fold-prompt').show(foldspeed);
    }
}

function determineView() {
    if ($('#detail').data('workitem-id') === 0) {
        $('#Id').val('');
        $('#search').show();
        $('#detail').hide();
    } else {
        $('#detail').show();
        $('#search').hide();
    }
}

function displayMessage() {
    if ($('#message').text() === '') {
        $('#messaging').hide();
    } else {
        $('#messaging').show();
    }
}