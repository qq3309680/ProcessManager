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
        "AddRow": function (currRows, column, colWidthArr, rowCount) {

            var appendHtml = "";
            if (rowCount != undefined) {
                for (var i = 0; i < rowCount; i++) {
                    appendHtml += '<div class="row mainFormData" Rows="' + parseInt(currRows + i + 1) + '"  style="min-height: 33px;">';
                    for (var j = 0; j < column; j++) {
                        appendHtml += '<div id="div_' + parseInt(currRows + i + 1) + '_' + parseInt(j + 1) + '" colWidth="' + colWidthArr[j] + '"  class="col-md-' + colWidthArr[j] + ' fixdInfo field-div"  ondragover="allowDrop(event)"   ondrop="drop(event)"  oncontextmenu="onContextMenu(this,event)"> <input id="input_' + parseInt(currRows + i + 1) + '_' + parseInt(j + 1) + '" class="field-input" type="text"   value=""  /><span id="span_' + parseInt(currRows + i + 1) + '_' + parseInt(j + 1) + '" class="field-span" ondragenter="dragEnter(event)"   ondragend="dragEnd(event)" ondragleave="dragLeave(event)" ondragstart="dragBegin(event)"  draggable="true">请填写字段名称</span></div>';
                    }
                    appendHtml += "</div>";
                }
            } else {
                appendHtml += '<div class="row mainFormData" Rows="' + parseInt(currRows + 1) + '"  style="min-height: 33px;">';
                for (var i = 0; i < column; i++) {
                    appendHtml += '<div id="div_' + currRows + '_' + parseInt(i + 1) + '" colWidth="' + colWidthArr[i] + '"  class="col-md-' + colWidthArr[i] + ' fixdInfo field-div" ondragover="allowDrop(event)"   ondrop="drop(event)"  oncontextmenu="onContextMenu(this,event)"  oncontextmenu="onContextMenu(this,event)"><input  id="input_' + parseInt(currRows + i + 1) + '_' + parseInt(j + 1) + '" class="field-input" type="text" value=""  /> <span   id="span_' + parseInt(currRows + i + 1) + '_' + parseInt(j + 1) + '" class="field-span" ondragenter="dragEnter(event)"   ondragend="dragEnd(event)" ondragleave="dragLeave(event)" ondragstart="dragBegin(event)"  draggable="true">请填写字段要求</span></div>';
                }
                appendHtml += "</div>";
            }
            return this.append(appendHtml);
        }
    });

})(jQuery);