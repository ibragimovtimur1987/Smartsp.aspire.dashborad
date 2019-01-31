function startYearListener() {
    var start = $('#startYear').val();
    $('#endYear').val(start);
    $('#endYear').multiselect('rebuild');

    var regions = $('#RegionIds').val()
    console.log(regions);
    if (regions != null && regions.length > 1) {
        $('#endYear').multiselect('disable');
    }
    loadTables();
}
function endYearListener() {
    loadTables();
}
function SelectedMspAreasListener() {
    console.log("SelectedMspAreasListener");
    var selectedMspAreas = $('#SelectedMspAreas').val();
    if (selectedMspAreas == null || selectedMspAreas.length == 0) {
        console.log("selectedMspAreas == null || selectedMspAreas.length == 0");
        $('#SelectedMspCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Направления социальной поддержки »</h3>');
        return;
    }
    reloadCategoriesByArea(selectedMspAreas, loadTables);

}
function SelectedMspCategoriesListener() {
    var selectedMspCategories = $('#SelectedMspCategories').val();
    if (selectedMspCategories == null || selectedMspCategories.length == 0) {
        console.log("selectedMspCategories == null || selectedMspCategories.length == 0");
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Форма социальной поддержки»</h3>');
        return;
    }
    loadTables();
}

function RegionIdsListener() {
    console.log("RegionIdsListener");
    $('#endYear').multiselect('enable');
    var regions = $('#RegionIds').val()
    var start = $('#startYear').val();
    console.log(regions);
    if (regions != null && regions.length > 1) {

        $('#endYear').val(start);
        $('#endYear').multiselect('rebuild');
        $('#endYear').multiselect('disable');
    }
    loadTables();
}
function loadTables() {

    var start = $('#startYear').val();
    var end = $('#endYear').val();
    var selectedMspAreas = $('#SelectedMspAreas').val();
    var selectedMspCategories = $('#SelectedMspCategories').val();
    var regions = $('#RegionIds').val()

    var validated = validateFilter(regions, start, end, selectedMspAreas, selectedMspCategories);

    if (!validated) {
        console.log(" filter not validated");
        return;
    }
    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories);
    if (!parmeters) {
        console.log(" parmeters is null");
        return;
    }
    if (!(parmeters.dataUrl && parmeters.dataParams)) {
        console.log("parmeters.dataUrl || parmeters.dataParams is empty");
        return;
    }
    $.post(
        parmeters.dataUrl,
        parmeters.dataParams,
        function (data) {
            $("#loadingDiv").hide();
            $("#tables").html(data);
        });

}

$(document).on("click", "#printButton", (function () {
    var start = $('#startYear').val();
    var end = $('#endYear').val();
    var selectedMspAreas = $('#SelectedMspAreas').val();
    var selectedMspCategories = $('#SelectedMspCategories').val();
    var regions = $('#RegionIds').val()
    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories);
    if (!parmeters.printDataUrl) {
        console.log("printDataUrl is empty");
        return;
    };
    $.post(
            parmeters.printDataUrl,
            parmeters.dataParams,
            function (response) {
                window.location = '/CompareMCP/Download?fileGuid=' + response["FileGuid"]
                                  + '&filename=' + response["FileName"];
            });
}));
$(document).on("click", "#btnClearSelectedRegions", (function () {
    $('#RegionIds').multiselect('deselectAll', false);
    $('#RegionIds').multiselect('updateButtonText');
}));

function validateFilter(regions, start, end, selectedMspAreas, selectedMspCategories) {

    console.log({ regions: regions, start: start, end: end, selectedMspAreas: selectedMspAreas, selectedMspCategories: selectedMspCategories });

    if (!regions) {
        $('#yearError2').show();
        $("#tables").hide();
        return;
    } else {
        $('#yearError2').hide();
        $("#tables").show();
    }

    if (end < start) {
        $('#yearError1').show();
        $("#tables").hide();
        return;
    } else {
        $('#yearError1').hide();
        $("#tables").show();
    }

    if ((start != null && start == 0) || (end != null && end == 0)) {
        $('#yearError3').show();
        $("#tables").hide();
        return;
    } else {
        $('#yearError3').hide();
        $("#tables").show();
    }

    if (selectedMspAreas != null && selectedMspAreas.length > 1) {
        console.log("not validated 1");
        $('#SelectedMspCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Допускается только одно значение для поля «Направления социальной поддержки »</h3>');
        return;
    }

    if (selectedMspCategories != null && selectedMspCategories.length > 1) {
        console.log("not validated 2");
        $("#loadingDiv").hide();
        $("#tables").html('<h3  class="warning">Допускается только одно значение для поля «Форма социальной поддержки»</h3>');
        return;
    }

    if (selectedMspAreas != null && selectedMspAreas.length == 1
            && (selectedMspCategories == null || selectedMspCategories.length == 0)) {
        console.log("not validated 3");
        $('#SelectedMspCategories').multiselect('enable');
        $('#endYear').multiselect('enable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Форма социальной поддержки»</h3>');
        return;
    }

    if (selectedMspAreas == null || selectedMspAreas.length == 0) {
        console.log("not validated 4");
        $('#SelectedMspCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Направления социальной поддержки »</h3>');
        return;
    }


    return true;
}

function getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories) {
    var dataUrl, dataParams, printDataUrl;

    if (regions != null && regions.length == 1
        && selectedMspAreas != null && selectedMspAreas.length == 1
        && selectedMspCategories != null && selectedMspCategories.length == 1) {
        //- Если выбран 1 субъект, то может быть выбран только 1 Направления социальной поддержки , только 1 категория внутри типа МСП, любой период
        console.log("1");
        dataUrl = "/CompareMCP/MSPCompareByCriteria";
        printDataUrl = "/CompareMCP/PrintMSPCompareByCriteria";
        dataParams = { region: regions, area: selectedMspAreas, category: selectedMspCategories, startYear: start, endYear: end };

        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length > 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length == 1
            && start == end) {
        //Если выбрано 2 и более субъекта, то может быть выбран только 1 Направления социальной поддержки , только 1 категория внутри типа МСП, период может быть равен только одному году
        console.log("2");
        dataUrl = "/CompareMCP/MSPCompareByCriteria2";
        printDataUrl = "/CompareMCP/PrintMSPCompareByCriteria2";
        dataParams = { regions: regions, area: selectedMspAreas, category: selectedMspCategories, year: start };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }




}
function reloadCategoriesByArea(type, callback) {
    console.log("reloadCategoriesByArea");
    var arr = { "type": type };
    var p = $.param(arr, true);
    $.get('/CompareMCP/GetMspCategoriesByArea/', p, function (data) {
        $('#SelectedMspCategories').empty();
        console.log("GetMspCategoriesByArea");
        $.each(data, function (i, row) {
            $('#SelectedMspCategories').append('<option value="' + row.Id + '">' + row.Title + '</option>')
        });
        $('#SelectedMspCategories').multiselect('rebuild');
        callback();
    });
}
function reloadSubCategoriesByArea(type, callback) {
    console.log("reloadSubCategoriesByArea");

    var arr = { "type": type };
    var p = $.param(arr, true);
    $.get('/CompareMCP/GetMspSubCategoriesByArea/', p, function (data) {
        $('#SelectedMspSubCategories').empty();
        console.log("GetMspSubCategoriesByArea");
        $.each(data, function (i, row) {
            $('#SelectedMspSubCategories').append('<option value="' + row.Id + '">' + row.Title + '</option>')
        });
        $('#SelectedMspSubCategories').multiselect('rebuild');
        callback();
    });
}
function reloadSubCategoriesByCategory(category, callback) {
    var arr = { "category": category };
    var p = $.param(arr, true);
    $.get('/CompareMCP/GetMspSubCategoriesByCategory/', p, function (data) {
        $('#SelectedMspSubCategories').empty();
        console.log("GetMspSubCategoriesByCategory");
        $.each(data, function (i, row) {
            $('#SelectedMspSubCategories').append('<option value="' + row.Id + '">' + row.Title + '</option>')
        });
        $('#SelectedMspSubCategories').multiselect('rebuild');
        callback();
    });
}
//var selectedMspCategories = $('#SelectedMspCategories').val();
//var selectedMspSubCategories = $('#SelectedMspSubCategories').val();