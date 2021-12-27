var contentStyle = document.body.style;
contentStyle.setProperty('-ms-content-zooming', 'none');
contentStyle.setProperty('-ms-touch-action', 'pan-x pan-y');
contentStyle.fontFamily = 'Segoe UI';

window.alert = function (alertMessage) { NotifyApp.sendData('typeAlert:' + alertMessage); };
window.confirm = function (confirmMessage) { NotifyApp.sendData('typeConfirm:' + confirmMessage); };
window.prompt = function (promptMessage) { NotifyApp.sendData('typePrompt:' + promptMessage); };
window.oncontextmenu = function (e) {
    var targElem = e.target;
    if (!((targElem.hasAttribute('contextmenu') || targElem.hasAttribute('mouseover') || targElem.hasAttribute('touchstart')
        || targElem.hasAttribute('touchmove') || targElem.hasAttribute('touchend')) && window.getSelection().toString() == '')) {
        e.preventDefault();
        NotifyApp.showContextMenu(e.clientX, e.clientY);
    }
}

var prevScrollpos = -38;
window.onscroll = function () {
    var currentScrollPos = window.pageYOffset;
    if (Math.abs(prevScrollpos - currentScrollPos) > 57 && currentScrollPos != 0 && !document.fullscreen) {
        if (prevScrollpos > currentScrollPos) {
            NotifyApp.sendData('ShowTopBar');
            prevScrollpos = currentScrollPos;
        } else {
            NotifyApp.sendData('HideTopBar');
            prevScrollpos = currentScrollPos;
        }
    }
    else if (currentScrollPos == 0 && !document.fullscreen) {
        NotifyApp.sendData('ShowTopBar');
        prevScrollpos = currentScrollPos;
    }
}

document.onkeydown = function (e) {
    if (e.ctrlKey == true) {
        if (e.keyCode == '61' || e.keyCode == '107') {
            NotifyApp.sendData('eventType:ZP');
            e.preventDefault();
        }
        if (e.keyCode == '109') {
            NotifyApp.sendData('eventType:ZM');
            e.preventDefault();
        }
    }
}

document.onwheel = function (e) {
    if (e.ctrlKey == true) {
        if (e.deltaY < 0) {
            NotifyApp.sendData('eventType:ZP');
            e.preventDefault();
        } else if (e.deltaY > 0) {
            NotifyApp.sendData('eventType:ZM');
            e.preventDefault();
        }
        e.preventDefault();
    }
}

document.onclick = function (e) {
    e.target.focus();
}

var dragging = false;
var tapCount = 0;

document.ontouchstart = function (e) {
    if (dragging == true) {
        tapCount = 0;
    }
    dragging = false;
    if (e.target != null) {
        e.target.focus();
    }
}

document.ontouchmove = function (e) {
    dragging = true;
    tapCount = 0;
    if (e.target != null) {
        e.target.focus();
    }
}

document.ontouchend = function (e) {
    var ed = e || window.event;
    ed = ed.target || ed.srcElement;

    var targElem = e.target;
    var isCheckedElement = (targElem.childNodes[0].nodeValue == null && (!targElem.hasAttribute('click') || !targElem.hasAttribute('dblclick')
        || !targElem.hasAttribute('touchend')) && (targElem.tagName === 'DIV' || targElem.tagName === 'BODY' || targElem.tagName === 'HTML'));

    if (dragging == false && isCheckedElement) {
        tapCount++;

        var children = targElem.parentNode;
        var dbltapable = (children.nodeValue == null && (!children.hasAttribute('click') || !children.hasAttribute('dblclick')
            || !children.hasAttribute('touchend')) && (children.tagName === 'DIV' || children.tagName === 'BODY' || children.tagName === 'HTML'));

        if (!dbltapable) {
            tapCount = 0;
        }

        setTimeout(function () { tapCount = 0; }, 300);
        if (tapCount == 2 && window.getSelection().toString() == '' && dbltapable) {
            NotifyApp.sendData('eventType:TZ');
            tapCount = 0;
        }
    }

    if (e.target != null) {
        e.target.focus();
    }
}

window.focus();