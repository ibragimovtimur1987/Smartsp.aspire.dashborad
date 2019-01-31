
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
    console.log(selectedMspAreas);
    if (selectedMspAreas == null || selectedMspAreas.length == 0) {
        console.log("selectedMspAreas == null || selectedMspAreas.length == 0");
        $('#SelectedMspCategories').multiselect('disable');
        $('#SelectedMspSubCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Выберите значение для поля «Направления социальной поддержки »</h3>');
        return;
    }
    reloadCategoriesByArea(selectedMspAreas, function () {
        return reloadSubCategoriesByArea(selectedMspAreas, loadTables);
    });

}
function SelectedMspCategoriesListener() {
    var selectedMspCategories = $('#SelectedMspCategories').val();
    if (selectedMspCategories == null || selectedMspCategories.length == 0) {
        console.log("selectedMspCategories == null || selectedMspCategories.length == 0");
        $('#SelectedMspSubCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Форма социальной поддержки»</h3>');
        return;
    }
    reloadSubCategoriesByCategory(selectedMspCategories, loadTables);
}
function SelectedMspSubCategoriesListener() {
    loadTables();
}
function RegionIdsListener() {
    console.log("RegionIdsListener");
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
//reloadCategoriesByArea(selectedMspAreas);
//reloadSubCategoriesByArea(selectedMspAreas);
//reloadSubCategoriesByCategory(selectedMspCategories)
function loadTables() {

    var start = $('#startYear').val();
    var end = $('#endYear').val();
    var selectedMspAreas = $('#SelectedMspAreas').val();
    var selectedMspCategories = $('#SelectedMspCategories').val();
    var selectedMspSubCategories = $('#SelectedMspSubCategories').val();
    var regions = $('#RegionIds').val()
    

    

    //$("#loadingDiv").show();
    var validated = validateFilter(regions, start, end, selectedMspAreas, selectedMspCategories, selectedMspSubCategories);

    if (!validated) {
        console.log(" filter not validated");
        return;
    }
    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories, selectedMspSubCategories);
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
    var selectedMspSubCategories = $('#SelectedMspSubCategories').val();
    var regions = $('#RegionIds').val()
    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories, selectedMspSubCategories);
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

function validateFilter(regions, start, end, selectedMspAreas, selectedMspCategories, selectedMspSubCategories) {
    
    console.log({ regions: regions, start: start, end: end, selectedMspAreas: selectedMspAreas, selectedMspCategories: selectedMspCategories, selectedMspSubCategories: selectedMspSubCategories });

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

    if (regions != null && regions.length == 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length == 1
            && (selectedMspSubCategories == null || selectedMspSubCategories.length == 0)) {
        console.log("7");
        $('#SelectedMspCategories').multiselect('enable');
        $('#SelectedMspSubCategories').multiselect('enable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Выберите значение для поля «Вид МСП»</h3>');
        return;
    }

    if (regions != null && regions.length == 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && (selectedMspCategories == null || selectedMspCategories.length == 0)) {
        console.log("8");
        $('#SelectedMspCategories').multiselect('enable');
        $('#SelectedMspSubCategories').multiselect('disable');
        $('#endYear').multiselect('enable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Выберите значение для поля «Форма социальной поддержки»</h3>');
        return;
    }
    if (regions != null && regions.length > 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && (selectedMspCategories == null || selectedMspCategories.length == 0)
            && (selectedMspSubCategories == null || selectedMspSubCategories.length == 0)) {
        console.log("88");
        $('#SelectedMspCategories').multiselect('enable');
        $('#SelectedMspSubCategories').multiselect('disable');
        $('#endYear').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Выберите еще одно значение для поля «Направления социальной поддержки » или для полей «форма социальной поддержки» и «Вид МСП»</h3>');
        return;
    }
    if (regions != null && regions.length > 0
            && (selectedMspAreas == null || selectedMspAreas.length == 0)) {
        console.log("9");
        $('#SelectedMspCategories').multiselect('disable');
        $('#SelectedMspSubCategories').multiselect('disable');
        $("#loadingDiv").hide();
        $("#tables").html('<h3 class="warning">Выберите значение для поля «Направления социальной поддержки »</h3>');
        return;
    }

    
    return true;
}

function getFilterParameters(regions, start, end, selectedMspAreas, selectedMspCategories, selectedMspSubCategories) {
    var dataUrl, dataParams, printDataUrl;

    if (regions != null && regions.length == 1
        && selectedMspAreas != null && selectedMspAreas.length == 2) {
        //Если выбран 1 субъект и 2 типа МСП одновременно, то выбор категорий и подкатегорий не доступен, период может быть выбран любой
        console.log("1");
        $('#endYear').multiselect('enable');

        $('#SelectedMspCategories').multiselect('disable');
        $('#SelectedMspSubCategories').multiselect('disable');

        dataUrl = "/CompareMCP/MSPCompareByType";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType";
        dataParams = { region: regions, categories: selectedMspAreas, startYear: start, endYear: end };

        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length > 1
            && selectedMspAreas != null && selectedMspAreas.length == 2
            && start == end) {
        //если выбрано 2 и более субъекта и 2 типа МСП одновременно, то выбор категорий и подкатегорий не доступен, а период может быть равен только одному году
        console.log("2");
        $('#endYear').multiselect('disable');
        $('#SelectedMspCategories').multiselect('disable');
        $('#SelectedMspSubCategories').multiselect('disable');

        dataUrl = "/CompareMCP/MSPCompareByType2";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType2";
        dataParams = { regions: regions, categories: selectedMspAreas, year: start };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length == 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length > 1) {
        //если выбран 1 субъект, 1 Направления социальной поддержки , 2 и более категории внутри типа МСП, то выбор подкатегорий не доступен, период может быть любой
        console.log("3");
        $('#endYear').multiselect('enable');

        $('#SelectedMspSubCategories').multiselect('disable');

        dataUrl = "/CompareMCP/MSPCompareByType3";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType3";
        dataParams = { region: regions, type: selectedMspAreas, category: selectedMspCategories, startYear: start, endYear: end };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length > 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length > 1
            && start == end) {
        //если выбрано 2 и более субъекта, 1 Направления социальной поддержки , 2 и более категории внутри типа МСП, то выбор подкатегорий не доступен, а период может быть равен только одному году
        console.log("4");
        $('#endYear').multiselect('disable');
        $('#SelectedMspSubCategories').multiselect('disable');

        dataUrl = "/CompareMCP/MSPCompareByType4";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType4";
        dataParams = { year: start, type: selectedMspAreas, category: selectedMspCategories, regions: regions };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length == 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length == 1
            && selectedMspSubCategories != null && selectedMspSubCategories.length > 0) {
        //если выбран 1 субъект, 1 Направления социальной поддержки , 1 категория внутри типа МСП, то выбор подкатегорий доступен, период может быть любой
        console.log("5");
        $('#endYear').multiselect('enable');

        dataUrl = "/CompareMCP/MSPCompareByType5";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType5";
        dataParams = { region: regions, type: selectedMspAreas, category: selectedMspCategories, subcategories: selectedMspSubCategories, startYear: start, endYear: end };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length > 1
            && selectedMspAreas != null && selectedMspAreas.length == 1
            && selectedMspCategories != null && selectedMspCategories.length == 1
            && selectedMspSubCategories != null && selectedMspSubCategories.length > 0
            && start == end) {
        //если выбрано 2 и более субъекта, 1 Направления социальной поддержки , 1 категория внутри типа МСП, то выбор категорий доступен, а период может быть равен только одному году
        console.log("6");
        $('#endYear').multiselect('disable');

        dataUrl = "/CompareMCP/MSPCompareByType6";
        printDataUrl = "/CompareMCP/PrintMSPCompareByType6";
        dataParams = { regions: regions, type: selectedMspAreas, category: selectedMspCategories, subcategories: selectedMspSubCategories, year: start };
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