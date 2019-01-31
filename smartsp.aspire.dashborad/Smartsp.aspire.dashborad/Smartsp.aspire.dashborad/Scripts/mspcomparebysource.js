function startYearListener() {
    console.log("startYearListener");
    var start = $('#startYear').val();
    $('#endYear').val(start);
    $('#endYear').multiselect('rebuild');
    var regions = $('#RegionIds').val()
    if (regions != null && regions.length > 1) {
        $('#endYear').multiselect('disable');
    }
    loadTables();
}
function endYearListener() {
    console.log("endYearListener");
    loadTables();
}
function SelectedMspAreasListener() {
    console.log("SelectedMspAreasListener");
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
//reloadCategoriesByArea(selectedMspAreas);
//reloadSubCategoriesByArea(selectedMspAreas);
//reloadSubCategoriesByCategory(selectedMspCategories)
function loadTables() {

    var start = $('#startYear').val();
    var end = $('#endYear').val();
    var selectedMspAreas = $('#SelectedMspAreas').val();

    var regions = $('#RegionIds').val()
    

    

    //$("#loadingDiv").show();
    var validated = validateFilter(regions, start, end, selectedMspAreas);

    if (!validated) {
        console.log(" filter not validated");
        return;
    }
    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas);
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
    var regions = $('#RegionIds').val();

    var parmeters = getFilterParameters(regions, start, end, selectedMspAreas);
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

function validateFilter(regions, start, end, selectedMspAreas) {
    
    console.log({ regions: regions, start: start, end: end, selectedMspAreas: selectedMspAreas });

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

    
    if (regions != null && regions.length > 0
            && (selectedMspAreas == null || selectedMspAreas.length == 0)) {
        console.log("9");
        $("#loadingDiv").hide();
        $("#tables").html('<h3 style="color:green!important" class="warning">Выберите значение для поля «Направления социальной поддержки »</h3>');
        return;
    }
    
    return true;
}

function getFilterParameters(regions, start, end, selectedMspAreas) {
    var dataUrl, dataParams, printDataUrl;

    if (regions != null && regions.length == 1 && start == end) {
        //Если выбран 1 субъект и 1 год, то Направления социальной поддержки  может быть выбран любой (т.е. 2 типа одновременно или 1 тип на выбор)
        console.log("1");


        dataUrl = "/CompareMCP/MSPCompareBySource";
        printDataUrl = "/CompareMCP/PrintMSPCompareBySource";
        dataParams = { regions: regions, areas: selectedMspAreas, year: start};

        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length == 1 && start != end) {
        //Если выбран 1 субъект и 2 и более года, то Направления социальной поддержки  может быть выбран любой (т.е. 2 типа одновременно или 1 тип на выбор)
        console.log("2");


        dataUrl = "/CompareMCP/MSPCompareBySource2";
        printDataUrl = "/CompareMCP/PrintMSPCompareBySource2";
        dataParams = { regions: regions, areas: selectedMspAreas, startYear: start, endYear: end };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }

    if (regions != null && regions.length > 1 && start == end) {
        //Если выбрано 2 и более субъект, то период может быть равен только одному году, а Направления социальной поддержки  может быть выбран любой (т.е. 2 типа одновременно или 1 тип на выбор)
        console.log("3");

        dataUrl = "/CompareMCP/MSPCompareBySource3";
        printDataUrl = "/CompareMCP/PrintMSPCompareBySource3";
        dataParams = { regions: regions, areas: selectedMspAreas, year: start };
        return {
            dataUrl: dataUrl,
            dataParams: dataParams,
            printDataUrl: printDataUrl
        }
    }
}


