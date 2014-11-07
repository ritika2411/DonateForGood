
    function SetDropDownValue(categoryType)
    {
        
        var catId = document.getElementById("ddlCategory");
        catId.selectedIndex = -1;
        for (var i = 0; i < catId.options.length; i++)
        {
            if (catId.options[i].childNodes['0'].data == categoryType)
            {
                catId.selectedIndex = i;
                catId.options[i].selected = true;
                break;
            }
        }
    }
function ClearText(field, title) {
    if ($(field).val() == title)
        $(field).val("");
}
function EnterText(field, title) {
    if ($(field).val() == '')
        $(field).val(title);
}
function Validate(form) {
    var DisplayMyPhoneNo = document.getElementById('chkDisplayMyPhoneNo');
    var DisplayMyEmail = document.getElementById('chkDisplayMyEmail');
    var PreferencePickUp = document.getElementById('rdbPickUp');
    var PreferenceDrop = document.getElementById('rdbDrop');
    var validStatus = false;
    if ((DisplayMyPhoneNo.checked) || (DisplayMyEmail.checked)) {
        validstatus = true;
    }
    else
    {
        alert("Check either Display My PhoneNo/Email");
        return false;
    }
    if ((PreferencePickUp.checked) || (PreferenceDrop.checked)) {
        validstatus = true;
    }
    else
    {
        alert("Check either PickUp/Drop");
        return false;
    }
    return validStatus;
}
