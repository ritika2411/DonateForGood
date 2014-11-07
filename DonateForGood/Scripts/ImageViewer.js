
function getTop(e) {
    var offset = e.offsetTop;
    if (e.offsetParent != null) offset += getTop(e.offsetParent);
    return offset;
}
function getLeft(e) {
    var offset = e.offsetLeft;
    if (e.offsetParent != null) offset += getLeft(e.offsetParent);
    return offset;
}
function hideDivImageDisplay() {
    $('#divImageDisplay').html("<img src=''/>");
}

function showDivImageDisplay(img) {
    var leftPos = getLeft(img) + 80;
    var topPos = getTop(img) + 20;
    $('#divImageDisplay').offset({ top: topPos, left: leftPos })
    $('#divImageDisplay').html("<img src='" + img.src + "' width='600px' height='450px'/>");
}

