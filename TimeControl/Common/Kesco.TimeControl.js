function SetInit(paramAddFunc) {
    if (paramAddFunc == 0) {
        SetTableDivSize();
        if (window.innerWidth == undefined) {
            window.onresize = SetTableDivSize;
        } else {
            window.onresize = function() { SetTableDivSize(); };
        }
    } else if (paramAddFunc == 1) {
        SetTableDetailsSize();
        if (window.innerWidth == undefined) {
            window.onresize = SetTableDetailsSize;
        } else {
            window.onresize = function() { SetTableDetailsSize(); };
        }
    }
}

function SetTableDivSize() {
    var vHeight = 0;
    vHeight = $(window).height() - 45;
    var tableDiv, mtable, descDiv, intervalList, mrow_1, mrow_2, btnPrint;

    if (document.getElementById("tableDiv")) tableDiv = document.getElementById("tableDiv");
    if (document.getElementById("mtable")) mtable = document.getElementById("mtable");
    if (document.getElementById("descDiv")) descDiv = document.getElementById("descDiv");
    if (document.getElementById("intervalList")) intervalList = document.getElementById("intervalList");
    if (document.getElementById("mrow_1")) mrow_1 = document.getElementById("mrow_1");
    if (document.getElementById("mrow_2")) mrow_2 = document.getElementById("mrow_2");
    if (document.getElementById("btnPrint")) btnPrint = document.getElementById("btnPrint");

    if (tableDiv == null || mtable == null || descDiv == null || mrow_1 == null || mrow_2 == null) return;
    tableDiv.style.display = "block";
    tableDiv.style.top = mrow_1.clientHeight + mrow_2.clientHeight + 6 + "px";

    if (intervalList != null) {
        //var mHeight = 0;
        var heightFilter = mrow_1.clientHeight + mrow_2.clientHeight;
        var heightGrid = vHeight - heightFilter;
        if (intervalList.clientHeight > tableDiv.clientHeight) {
            if (heightGrid < 300) {
                tableDiv.style.height = 300 + "px";
            } else {
                if (heightGrid < intervalList.clientHeight) {
                    tableDiv.style.height = heightGrid + "px";
                } else {
                    tableDiv.style.height = intervalList.clientHeight + "px";
                }
                //tableDiv.style.height = intervalList.clientHeight + "px";
            }
            //mHeight = tableDiv.clientHeight;
        } else {
            tableDiv.style.height = intervalList.clientHeight + "px";
            //mHeight = intervalList.clientHeight;
        }
        //descDiv.style.top = mHeight + heightFilter + "px";
        //descDiv.style.width = tableDiv.style.width;
    } else {
        descDiv.style.display = "none";
        if (btnPrint != null)
            btnPrint.style.display = "none";
    }
}

function SetTableDetailsSize() {
    var vHeight = 0;
    vHeight = $(window).height() - 220;
    var tableDiv, mtable, descDiv, intervalList, btnPrint;

    if (document.getElementById("tableDiv")) tableDiv = document.getElementById("tableDiv");
    if (document.getElementById("mtable")) mtable = document.getElementById("mtable");
    if (document.getElementById("descDiv")) descDiv = document.getElementById("descDiv");
    if (document.getElementById("intervalList")) intervalList = document.getElementById("intervalList");
    if (document.getElementById("btnPrint")) btnPrint = document.getElementById("btnPrint");

    if (tableDiv == null || mtable == null || descDiv == null) return;

    if (intervalList) {
        //var mHeight = 0;
        if (intervalList.clientHeight > tableDiv.clientHeight) {
            if (vHeight < 300) {
                tableDiv.style.height = 300 + "px";
            } else {
                tableDiv.style.height = vHeight + "px";
                //tableDiv.style.height = intervalList.clientHeight + "px";
            }
            //mHeight = tableDiv.clientHeight;
        } else {
            tableDiv.style.height = intervalList.clientHeight + "px";
            //mHeight = intervalList.clientHeight;
        }
        //descDiv.style.top = mHeight + mtable.clientHeight + "px";
        //descDiv.style.width = tableDiv.style.width;
    } else {
        descDiv.style.display = "none";
        if (btnPrint != null)
            btnPrint.style.display = "none";
    }
}

function displayTimeExit(disp, inx) {
    var obj = document.getElementById("timeExit_TD3");
    if (obj != null)
        obj.style.display = (disp == "0" ? "none" : (inx == "1" ? "inline-block" : "none"));

    obj = document.getElementById("timeExit_TD1");
    if (obj != null)
        obj.style.display = (disp == "0" ? "none" : "inline-block");

    obj = document.getElementById("timeExit_TD2");
    if (obj != null)
        obj.style.display = (disp == "0" ? "none" : "inline-block");
}

function UTC2Local() {
    for (i = 0; i < document.all.length; i++) {
        if (document.all(i).id.indexOf("utctime") != -1) {
            var temp = v4_toLocalTime(document.all(i).innerHTML, "hh:mi:ss", "", true);
            if (document.all(i).id.indexOf("utctimeForce") != -1) {
                if (temp == "00:00:00") {
                    temp = "24:00:00";
                }
            }
            document.all(i).innerHTML = temp;
        }
        if (document.all(i).innerHTML == "Incorrect date format")
            document.all(i).innerHTML = "";
    }
}

function UTC2LocalTimeDetails() {
    for (i = 0; i < document.all.length; i++) {
        
        if (document.all(i).id.indexOf("utctime") != -1) {
            var temp = v4_toLocalTime(document.all(i).innerHTML, "hh:mi:ss", "", true);
            if (document.all(i).id.indexOf("utctimeForce") != -1) {
                if (temp == "00:00:00") {
                    temp = "24:00:00";
                }
            }
            document.all(i).innerHTML = temp;
        }
        if (document.all(i).innerHTML == "Incorrect date format")
            document.all(i).innerHTML = "";
    }
}

function mouseOver(id, userPhoto) {
    var X;
    var Y;
    if (document.body.clientHeight > 330) {
        if (window.event.clientY < document.body.clientHeight / 2)
            Y = document.body.scrollTop + window.event.clientY + 20;
        else
            Y = document.body.scrollTop + window.event.clientY - 165;
        X = window.event.clientX - 20;
    } else {
        X = 120;
        Y = document.body.scrollTop + 20;
    }
    document.getElementById("lookup").innerHTML =
        "<div class= \"bgMain\" style =\"WIDTH: 100px; BACKGROUND-COLOR: #ffffff; FILTER: alpha(opacity=100); FONT-WEIGHT: bold; TEXT-DECORATION: none; COLOR: black; POSITION: absolute; Z-INDEX: 1000; TOP = " +
        Y +
        "px;  LEFT = " +
        X +
        "px \"><img style = \"FLOAT: left;\" align=left width='120px' src='" +
        userPhoto +
        "?id=" +
        id +
        "&w=120'><span style =\"FLOAT: left; TEXT-DECORATION: none\"></span></div>";
}

function ShowWaitLayer(cmd) {
    cmdasync("cmd", cmd);
}

function HideWaitLayer() {
    //$("*").css("cursor", "auto");
}

function mouseOut() {
    document.getElementById("lookup").innerHTML = "";
}

function ShowFotoToolTip(setCheck, userPhoto, id, path) {
    if (setCheck)
        gi("cbShowFoto_" + id).checked = !gi("cbShowFoto_" + id).checked;
    if (gi("cbShowFoto_" + id).checked) {
        gi("dFoto_" + id).innerHTML = "<img width=\"120px\" onclick=\"ShowFotoToolTip(true, '', " +
            id +
            ");\" src=\"" +
            userPhoto +
            "?id=" +
            id +
            "&w=120\" \>";
        SetCookieTc(path, true);
    } else {
        gi("dFoto_" + id).innerHTML = "";
        SetCookieTc(path, false);
    }
}

function SetCookieTc(path, id) {
    var url = path + "?savePhoto=1&photo=" + id;
    $.support.cors = true;
    $.ajax({
        type: "POST",
        url: url,
        crossDomain: true,
        xhrFields: { withCredentials: true }
    });
}

function HideCalcType() {
    calc_type.innerHTML = "";
}

function ItemClick(id, path) {
    var l = screen.availHeight / 2 - 200;
    var r = screen.availWidth / 2 - 300;
    v4_windowOpen(path + "?id=" + id,
        "_blank",
        "scrollbars = yes,height=400,width=600,resizable = yes,toolbar=no,menubar=no,location=no,left=" +
        l +
        ",top=" +
        r);
}

function sendMessage(eMail) {
    var obj = new ActiveXObject("Messenger.MessengerApp");
    if (obj) obj.LaunchIMUI(eMail);
}