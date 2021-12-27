window.addEventListener('contextmenu', function (t) {
    t.preventDefault();
    var e = t.target;
    if (('ontouchstart' in window) == false) {
        if (!e.hasAttribute('contextmenu')) {
            HandlerUI.showContextMenu(t.clientX, t.clientY);
        }
    }
    else {
        if (!(((e.tagName == 'UL' || e.tagName == 'LI') && window.getSelection().toString().length == 0) || e.tagName == 'PROGRESS' || e.type == 'range')
            && (!e.hasAttribute('mousemove') || !e.hasAttribute('touchmove') || !(e.hasAttribute('touchend') && window.getSelection().toString() == ''))) {
            HandlerUI.showContextMenu(t.clientX, t.clientY);
        }
    }
}, false);

/*document.addEventListener('mouseup', function (t) {
    var inputSelection = undefined;
    if (document.activeElement.tagName === "TEXTAREA" || (document.activeElement.tagName === "INPUT" && document.activeElement.type === "text")) {
        inputSelection = document.activeElement.value.substring(document.activeElement.selectionStart, document.activeElement.selectionEnd);
    }

    if (document.getSelection().toString().length > 0 || inputSelection != undefined && inputSelection.length > 0) {
        HandlerUI.showSelectionMenu(t.clientX, t.clientY);
    }
    else {
        HandlerUI.sendData("HideSelectionMenu");
    }
}, false);*/