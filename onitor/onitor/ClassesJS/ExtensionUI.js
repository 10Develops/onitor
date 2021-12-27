var videoElem = document.querySelector('video');
var audioElem = document.querySelector('audio');
if (videoElem !== null || audioElem !== null) {
    TaskHandler.sendData('PageHaveMedia');
}

var onitorMeta = document.querySelector('meta[name=onitor-theme]');
if (onitorMeta != null && onitorMeta.getAttribute('content') == 'true') {
    TaskHandler.sendData('SupportingTheme');
}

window.alert = function (t) {
    HandlerUI.sendData('typeAlert:' + t);
    return true;
}

var prevScrollpos = 0;
var isTopBarShown = true;
window.addEventListener("scroll", function () {
    var t = window.pageYOffset;

    if (!document.webkitIsFullScreen) {
        if ((Math.abs(prevScrollpos - t) > 34 && prevScrollpos > t || t < 34) && !isTopBarShown) {
            HandlerUI.sendData('ShowTopBar');
            isTopBarShown = true;
        }
        else if (Math.abs(prevScrollpos - t) > 34 && prevScrollpos < t && isTopBarShown) {
            HandlerUI.sendData('HideTopBar');
            isTopBarShown = false;
        }
    }

    prevScrollpos = t <= 0 ? 0 : t;
}, false);

window.addEventListener("keydown", function (t) {
    if (1 == t.ctrlKey) {
        if ('61' == t.keyCode || '107' == t.keyCode) {
            t.preventDefault();

            HandlerUI.sendData('eventType:ZP');
        }
        else if ('109' == t.keyCode) {
            t.preventDefault();

            HandlerUI.sendData('eventType:ZM');
        }
        else if ('79' == t.keyCode) {
            t.preventDefault();

            HandlerUI.sendData('FocusOnAddressBar');
        }
        else if ('83' == t.keyCode) {
            t.preventDefault();

            HandlerUI.sendData('SavePageAs');
        }
        /*else if ('88' == t.keyCode) {
            document.execCommand('copy');
            document.execCommand('delete');
        }
        else if ('67' == t.keyCode) {
            document.execCommand('copy');
        }
        else if ('86' == t.keyCode) {
            t.preventDefault();
            document.execCommand('insertText', false, window.clipboardData.getData('text'));
            document.activeElement.focus();
        }
        else if ('65' == t.keyCode) {
            HandlerUI.sendData('SelectAll');
        }*/
    }

    switch (t.keyCode) {
        case 116:
            HandlerUI.sendData('RefreshPage');
            break;
        case 122:
            HandlerUI.sendData('ShowFullScreen');
            break;
    }
}, false);

var selStart, selEnd;
window.addEventListener("mouseup", function (t) {
    var g = t.target;
    if (g.tagName == 'INPUT' && g.selectionStart != g.selectionEnd) {
        selStart = g.selectionStart;
        selEnd = g.selectionEnd;
        HandlerUI.sendData(selStart + " " + selEnd);
    }

    if (3 == t.which) {
 
    }
}, false);

window.addEventListener("wheel", function (t) {
    if (1 == t.ctrlKey) {
        if (t.deltaY < 0) {
            HandlerUI.sendData('eventType:ZP');
        } else if (0 < t.deltaY) {
            HandlerUI.sendData('eventType:ZM');
        }
        t.preventDefault();
    }
}, false);

document.addEventListener('webkitfullscreenchange', function (t) {
    if (document.webkitIsFullScreen) {
        HandlerUI.sendData('HideTopBar');
    }
    else {
        if (isTopBarShown) {
            HandlerUI.sendData('ShowTopBar');
        }
    }
}, false);

