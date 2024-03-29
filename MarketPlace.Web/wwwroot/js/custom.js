﻿function open_waiting(selector = 'body') {
    $(selector).waitMe({
        effect: 'facebook',
        text: 'لطفا صبر کنید ...',
        bg: 'rgba(255,255,255,0.7)',
        color: '#000'
    });
}

function close_waiting(selector = 'body') {
    $(selector).waitMe('hide');
}

function ShowMessage(title, text, theme) {
    window.createNotification({
        closeOnClick: true,
        displayCloseButton: false,
        positionClass: 'nfc-bottom-right',
        showDuration: 4000,
        theme: theme !== '' ? theme : 'success'
    })({
        title: title !== '' ? title : 'اعلان',
        message: decodeURI(text)
    });
}

$(document).ready(function () {
    var editors = $("[ckeditor]");
    if (editors.length > 0) {
        $.getScript('/js/ckeditor.js', function () {
            $(editors).each(function (index, value) {
                var id = $(value).attr('ckeditor');
                ClassicEditor.create(document.querySelector('[ckeditor="' + id + '"]'),
                    {
                        toolbar: {
                            items: [
                                'heading',
                                '|',
                                'bold',
                                'italic',
                                'link',
                                '|',
                                'fontSize',
                                'fontColor',
                                '|',
                                'imageUpload',
                                'blockQuote',
                                'insertTable',
                                'undo',
                                'redo',
                                'codeBlock'
                            ]
                        },
                        language: 'fa',
                        table: {
                            contentToolbar: [
                                'tableColumn',
                                'tableRow',
                                'mergeTableCells'
                            ]
                        },
                        licenseKey: '',
                        simpleUpload: {
                            // The URL that the images are uploaded to.
                            uploadUrl: '/Uploader/UploadImage'
                        }

                    })
                    .then(editor => {
                        window.editor = editor;
                    }).catch(err => {
                        console.error(err);
                    });
            });
        });
    }
});

function fillPageId(pageId) {
    $('#PageId').val(pageId);
    $('#filter-form').submit();
}

$("[main_category_checkbox]").on('change', function (e) {
    var isChecked = $(this).is(':checked');
    var selectedCategoryId = $(this).attr('main_category_checkbox');
    if (isChecked) {
        $('#sub_categories_' + selectedCategoryId).slideDown(200);
    } else {
        $('[parent-category-id="' + selectedCategoryId + '"]').prop('checked', false);
        $('#sub_categories_' + selectedCategoryId).slideUp(200);
    }
});

$("#add_color_button").on("click", function (e) {
    e.preventDefault();
    var colorName = $("#product_color_name_input").val();
    var colorPrice = $("#product_color_price_input").val();
    var colorCode = $("#product_color_code_input").val();

    if (colorName !== "" && colorPrice !== "" && colorCode !== "") {
        var currentColorCount = $("#list_of_product_colors tr");
        var index = currentColorCount.length;

        var isExistSelectedColor = $('[color-name-hidden-input][value="' + colorName + '"');
        if (isExistSelectedColor.length === 0) {
            var colorNameNode = `<input type="hidden" value="${colorName}" name="ProductColors[${index}].ColorName" color-name-hidden-input="${colorName}-${colorPrice}" >`;
            var colorPriceNode = `<input type="hidden" value="${colorPrice}" name="ProductColors[${index}].Price" color-price-hidden-input="${colorName}-${colorPrice}" >`;
            var colorCodeNode = `<input type="hidden" value="${colorCode}" name="ProductColors[${index}].ColorCode" color-code-hidden-input="${colorName}-${colorPrice}" >`;
            $("#create_product_form").append(colorNameNode);
            $("#create_product_form").append(colorPriceNode);
            $("#create_product_form").append(colorCodeNode);

            var colorTableNode = `<tr color-table-item="${colorName}-${colorPrice}">
                                    <td>${colorName}</td>
                                    <td>${colorPrice}</td> 
                                    <td style="text-align: -webkit-center;">
                                        <div style="border-radius: 50%; width: 40px; height: 40px; background-color:${colorCode};"></div>
                                    </td>
                                    <td> <a class="btn btn-danger text-white" onclick="removeProductColor('${colorName}-${colorPrice}')">حذف</a> </td>
                                </tr>`;
            $("#list_of_product_colors").append(colorTableNode);

            $("#product_color_name_input").val("");
            $("#product_color_price_input").val("");
            $("#product_color_code_input").val("");
        } else {
            ShowMessage("اخطار", "رنگ وارد شده تکراری می باشد", "warning");
            $("#product_color_name_input").val("").focus();
        }
    } else {
        ShowMessage("اخطار", "لطفا نام رنگ و قیمت آن را به درستی وارد نمایید", "warning");
    }
});

$("#add_feature_button").on("click", function (e) {
    e.preventDefault();
    var feature = $("#product_feature_input").val();
    var featureValue = $("#product_feature_value_input").val();

    if (feature !== "" && featureValue !== "") {
        var currentFeaturesCount = $("#list_of_product_features tr");
        var index = currentFeaturesCount.length;

        var isExistSelectedFeature = $('[feature-hidden-input][value="' + feature + '"');
        if (isExistSelectedFeature.length === 0) {
            var featureNode = `<input type="hidden" value="${feature}" name="ProductFeatures[${index}].Feature" feature-hidden-input="${feature}-${featureValue}" >`;
            var featureValueNode = `<input type="hidden" value="${featureValue}" name="ProductFeatures[${index}].FeatureValue" feature-value-hidden-input="${feature}-${featureValue}" >`;
            $("#create_product_form").append(featureNode);
            $("#create_product_form").append(featureValueNode);

            var featureTableNode = `<tr feature-table-item="${feature}-${featureValue}">
                                    <td>${feature}</td>
                                    <td>${featureValue}</td>
                                    <td> <a class="btn btn-danger text-white" onclick="removeProductFeature('${feature}-${featureValue}')">حذف</a> </td>
                                </tr>`;
            $("#list_of_product_features").append(featureTableNode);

            $("#product_feature_input").val("");
            $("#product_feature_value_input").val("");
        } else {
            ShowMessage("اخطار", "ویژگی وارد شده تکراری می باشد", "warning");
            $("#product_feature_input").val("").focus();
        }
    } else {
        ShowMessage("اخطار", "لطفا نام ویژگی و مقدار آن را به درستی وارد نمایید", "warning");
    }
});

function removeProductColor(index) {
    $('[color-name-hidden-input="' + index + '"]').remove();
    $('[color-price-hidden-input="' + index + '"]').remove();
    $('[color-code-hidden-input="' + index + '"]').remove();
    $('[color-table-item="' + index + '"]').remove();
    reOrderProductColorHiddenInputs();
}

function removeProductFeature(index) {
    $('[feature-hidden-input="' + index + '"]').remove();
    $('[feature-value-hidden-input="' + index + '"]').remove();
    $('[feature-table-item="' + index + '"]').remove();
    reOrderProductFeatureHiddenInputs();
}

function reOrderProductColorHiddenInputs() {
    var hiddenColors = $('[color-name-hidden-input]');
    $.each(hiddenColors, function (index, value) {
        var hiddenColor = $(value);
        var colorId = $(value).attr('color-name-hidden-input');
        var hiddenPrice = $('[color-price-hidden-input="' + colorId + '"]');
        var hiddenCode = $('[color-code-hidden-input="' + colorId + '"]');
        $(hiddenColor).attr('name', 'ProductColors[' + index + '].ColorName');
        $(hiddenPrice).attr('name', 'ProductColors[' + index + '].Price');
        $(hiddenCode).attr('name', 'ProductColors[' + index + '].ColorCode');
    });
}

function reOrderProductFeatureHiddenInputs() {
    var hiddenFeatures = $('[feature-hidden-input]');
    $.each(hiddenFeatures, function (index, value) {
        var hiddenFeature = $(value);
        var featureId = $(value).attr('feature-hidden-input');
        var hiddenfeatureValue = $('[feature-value-hidden-input="' + featureId + '"]');
        $(hiddenFeature).attr('name', 'ProductFeatures[' + index + '].Feature');
        $(hiddenfeatureValue).attr('name', 'ProductFeatures[' + index + '].FeatureValue');
    });
}

$("#OrderBy").on("change", function () {
    $("#filter-form").submit();
});

function changeProductPriceBasedOnColor(colorId, priceOfColor, colorName) {
    const basePrice = parseInt($("#ProductBasePrice").val(), 0);
    $(".current_price").html((basePrice + priceOfColor) + " تومان" + " ( " + colorName + " )");
    $("#add_product_to_order_ProductColorId").val(colorId);
}

$("#number_of_products_in_basket").on("change", function (e) {
    var numberOfProduct = parseInt(e.target.value, 0);
    $("#add_product_to_order_Count").val(numberOfProduct);
});

function onSuccessAddProductToOrder(res) {
    if (res.status == "Success")
        ShowMessage("اعلان", res.message, res.status.toLowerCase());
    else
        ShowMessage("اعلان", res.message, "warning");

    close_waiting();
}

$("#submitOrderForm").on("click", function () {
    $("#addProductToOrderForm").submit();
    open_waiting();
});

function removeProductfromOrder(detailId) {
    $.get("/user/remove-order-item/" + detailId).then(res => {
        location.reload();
    });
}

// one way to change count of order detail
//function checkDetailCount() {
//    $("input[order-detail-count]").on("change", function (event) {
//        open_waiting();
//        var detailId = $(this).attr("order-detail-count");
//        $.get("/user/change-detail-count/" + detailId + "/" + event.target.value).then(res => {
//            $("#user-open-order-wrapper").html(res);
//            close_waiting();
//        });
//    });
//}

//checkDetailCount();

// another way
function changeOpenOrderDetailCount(event, detailId) {
    open_waiting();
    $.get("/user/change-detail-count/" + detailId + "/" + event.target.value).then(res => {
        $("#user-open-order-wrapper").html(res);
        close_waiting();
    });
}