var RowsControl = {
    Init: function () {
        alert("Init");
    },
    Add: function () {
        alert("Add");
    }
}
; (function ($) {
    $.extend(RowsControl);

    $.fn.extend({
        /**
        type:插入类型：last-最后一行，up-当前行的上一行，down-当前行的下一行
        currRows:添加的行为第几行
        column:添加多少列
        colWidthArr：每一列的宽度数组
        rowCount：需要添加几行，默认1行
        */
        "AddRow": function (type, currRows, column, colWidthArr, rowCount) {

            var appendHtml = "";
            if (rowCount != undefined) {
                for (var i = 0; i < rowCount; i++) {
                    appendHtml += '<div class="row mainFormData" Rows="' + parseInt(currRows + i) + '"  style="min-height: 33px;">';
                    for (var j = 0; j < column; j++) {
                        appendHtml += '<div id="div_' + parseInt(currRows + i) + '_' + parseInt(j + 1) + '" colWidth="' + colWidthArr[j] + '"  class="col-md-' + colWidthArr[j] + ' fixdInfo field-div"  ondragover="allowDrop(event)"   ondrop="drop(event)"  oncontextmenu="onContextMenu(this,event)"> <input id="input_' + parseInt(currRows + i) + '_' + parseInt(j + 1) + '" class="field-input" type="text"   value=""  /><span id="span_' + parseInt(currRows + i) + '_' + parseInt(j + 1) + '" class="field-span" ondragenter="dragEnter(event)"   ondragend="dragEnd(event)" ondragleave="dragLeave(event)" ondragstart="dragBegin(event)"  draggable="true">请填写字段名称</span></div>';
                    }
                    appendHtml += "</div>";
                }
            } else {
                appendHtml += '<div class="row mainFormData" Rows="' + parseInt(currRows) + '"  style="min-height: 33px;">';
                for (var i = 0; i < column; i++) {
                    appendHtml += '<div id="div_' + currRows + '_' + parseInt(i + 1) + '" colWidth="' + colWidthArr[i] + '"  class="col-md-' + colWidthArr[i] + ' fixdInfo field-div" ondragover="allowDrop(event)"   ondrop="drop(event)"  oncontextmenu="onContextMenu(this,event)"  oncontextmenu="onContextMenu(this,event)"><input  id="input_' + parseInt(currRows + i) + '_' + parseInt(j + 1) + '" class="field-input" type="text" value=""  /> <span   id="span_' + parseInt(currRows + i) + '_' + parseInt(j + 1) + '" class="field-span" ondragenter="dragEnter(event)"   ondragend="dragEnd(event)" ondragleave="dragLeave(event)" ondragstart="dragBegin(event)"  draggable="true">请填写字段要求</span></div>';
                }
                appendHtml += "</div>";
            }
            if (type == "last") {
                return this.append(appendHtml);
            } else {
                if (type == "up") {
                    $(this).ChangeRowSort(currRows, 1);
                    $(this).find(".mainFormData[rows='" + (parseInt(currRows) + 1) + "']").before(appendHtml);
                } else {
                    $(this).ChangeRowSort((parseInt(currRows)), 1);
                    $(this).find(".mainFormData[rows='" + (parseInt(currRows) - 1) + "']").after(appendHtml);
                }

            }

        },
        "ChangeRowSort": function (beginRow, changeCount) {
            var targetRows = [];
            var targetRow = $(this).find(".mainFormData[rows='" + beginRow + "']");
            if (targetRow.length > 0) {
                targetRows.push(targetRow[0]);
                if ($(targetRow).nextAll().length > 0) {
                    $.each($(targetRow).nextAll(), function (key, val) {
                        targetRows.push(val);
                    });
                }
            }
            $.each(targetRows, function (key, val) {
                $(val).attr("rows", parseInt($(val).attr("rows")) + changeCount);
            });


        }

    });

})(jQuery);