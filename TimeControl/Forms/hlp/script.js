function SetDefaultPage() {
    if (parent == null) return;

    var data = {};

    if (parent.window.location.search) {
        var pair = (parent.location.search.substr(1)).split("&");
        for (var i = 0; i < pair.length; i ++) {
            var param = pair[i].split("=");
            data[param[0]] = param[1];
        }
    }
    if (data["page"] != null) {
        var objP = document.getElementById(data["page"]);
        if (objP) {

            var ie = getIEVersion();
            if (ie > -1)
                objP.click();
            else
                fireEvent(objP, "click");
            objP.focus();
        }
    }
}

function fireEvent(obj, evt) {

    var fireOnThis = obj;
    if (document.createEvent) {
        var evObj = document.createEvent("MouseEvents");
        evObj.initEvent(evt, true, false);
        fireOnThis.dispatchEvent(evObj);
    } else if (document.createEventObject) {
        fireOnThis.fireEvent("on" + evt);
    }
}

function OpenPageInHelp(pageId) {
    OpenPageInHelp(pageId, undefined);
}

function OpenPageInHelp(pageId, anchor) {
    if (!parent || !parent.index || !parent.index.document.getElementById(pageId)) {
        alert("Ссылка на страницу с id='" + pageId + "' не найдена или у документа неверная структура!");
        return;
    }

    var obj = parent.index.document.getElementById(pageId);

    var oldHref = obj.href;

    if (obj !== undefined && anchor !== undefined)
        obj.href += "#" + anchor;

    var ie = getIEVersion();
    if (ie > -1)
        obj.click();
    else
        fireEvent(obj, "click");
    obj.href = oldHref;
    obj.focus();
}

function getIEVersion() {
    var rv = -1; // Return value assumes failure.
    if (navigator.appName == "Microsoft Internet Explorer") {
        var ua = navigator.userAgent;
        var re = new RegExp("MSIE ([0-9]{1,}[\.0-9]{0,})");
        if (re.exec(ua) != null)
            rv = parseFloat(RegExp.$1);
    }
    return rv;
}