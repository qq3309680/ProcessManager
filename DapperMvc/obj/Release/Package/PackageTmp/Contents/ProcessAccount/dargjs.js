var Class = {
    //创建类
    create: function () {
        return function () {
            this.initialize.apply(this, arguments);
        };
    },
    create: function (id) {
        return function (id) {
            this.initialize.apply(this, arguments, id);
        };
    }
};
var __A = function (a) {
    //转换数组
    return a ? Array.apply(null, a) : new Array;
};
var __ = function (id) {
    //获取对象
    return document.getElementById(id);
};
Object.extend = function (a, b) {
    //追加方法
    for (var i in b) a[i] = b[i];
    return a;
};
Object.extend(Object, {
    addEvent: function (a, b, c, d) {
        //添加函数
        if (a.attachEvent) a.attachEvent(b[0], c);
        else a.addEventListener(b[1] || b[0].replace(/^on/, ""), c, d || false);
        return c;
    },

    delEvent: function (a, b, c, d) {
        if (a.detachEvent) a.detachEvent(b[0], c);
        else a.removeEventListener(b[1] || b[0].replace(/^on/, ""), c, d || false);
        return c;
    },

    reEvent: function () {
        //获取Event
        return window.event ? window.event : (function (o) {
            do {
                o = o.caller;
            } while (o && !/^\[object[ A-Za-z]*Event\]$/.test(o.arguments[0]));
            return o.arguments[0];
        })(this.reEvent);
    }

});
Function.prototype.bind = function () {
    //绑定事件
    var wc = this; var a = __A(arguments);
    //shift()用于把数组的第一个元素删除，并返回剩余的数组
    var o = a.shift();
    return function () {
        wc.apply(o, a.concat(__A(arguments)));
    };
};
var CDrag = Class.create();
CDrag.IE = /MSIE/.test(window.navigator.userAgent);
//表
CDrag.Table = Class.create();
CDrag.Table.prototype = {
    //列的拖拽暂时不考虑
    initialize: function () {

        //初始化
        var wc = this;
        wc.items = []; //创建列组
    },

    add: function () {
        //添加列
        var wc = this, id = wc.items.length, arg = arguments;

        var colId = arg[1];

        return wc.items[id] = new CDrag.Table.Cols(colId, wc, arg[0]);
    }
};
//列
CDrag.Table.Cols = Class.create();

CDrag.Table.Cols.prototype = {

    initialize: function (id, parent, element) {
        //初始化
        var wc = this;
        wc.items = []; //创建列组
        wc.id = element.id;
        wc.parent = parent;
        wc.element = element;
        var column = parseInt(id.split("_")[1]);
        wc.element.setAttribute("column", column);
    },

    add: function () {

        //添加行
        var wc = this, id = wc.items.length, arg = arguments;

        var rowid = arg[0];

        return wc.items[id] = new CDrag.Table.Rows(rowid, wc, arg[0]);
    },

    ins: function (num, row) {
        //插入行
        var wc = this, items = wc.items, i;

        if (row.parent == wc && row.id < num) num--; //同列向下移动的时候
        for (i = num ; i < items.length ; i++) items[i].id++;

        items.splice(num, 0, row);
        row.id = num, row.parent = wc;

        return row;
    },

    del: function (num) {
        //删除行
        var wc = this, items = wc.items, i;

        if (num >= items.length) return;
        for (i = num + 1; i < items.length ; i++) items[i].id = i - 1;
        return items.splice(num, 1)[0];
    }

};
//行
CDrag.Table.Rows = Class.create();
CDrag.Table.Rows.prototype = {


    initialize: function (id, parent, element) {
        //初始化
        var wc = this, temp;
        wc.id = id;
        wc.parent = parent;

        wc.element = wc.element_init();

        wc.root_id = element;

        temp = wc.element.childNodes[CDrag.IE ? 0 : 1];

        wc.title = temp.childNodes[0];

        wc.reduce = temp.childNodes[1];

        wc.close = temp.childNodes[2];

        wc.content = wc.element.childNodes[CDrag.IE ? 1 : 3];

        wc.mousedown = wc.reduceFunc = wc.closeFunc = null;

        wc.load(database.json);
    },

    element_init: function () {
        //初始化元素
        var wc = this, div = __("root_row").cloneNode(true);
        wc.parent.element.appendChild(div);
        div.style.display = "block";
        return div;
    },

    load: function (datajson) {

        //加载信息--database数据信息

        var wc = this;


        var info = database.parse(wc.root_id, datajson);

        wc.title.innerHTML = info.branch + "_" + info.column + "_" + info.row;

        wc.title.setAttribute("objectId", info.objectid);

        wc.nodeInfo = info;

        //渲染状态
        if (wc.nodeInfo.state == 2) {

            if (wc.nodeInfo.isdelete) {

                $(wc.title).parent().addClass("isdelete");

            } else {

                if (wc.nodeInfo.isnewadd) {

                    $(wc.title).parent().addClass("isnewadd");

                } else {

                    if (wc.nodeInfo.isrolechange) {

                        $(wc.title).parent().addClass("isrolechange");

                    } else if (wc.nodeInfo.issortchange) {

                        $(wc.title).parent().addClass("issortchange");

                    }

                }
            }

        }

        wc.content.innerHTML = info.content;

        wc.element.setAttribute("row", info.row);


    }

};

CDrag.prototype = {

    initialize: function (id) {

        var child = document.createElement("div");

        child.id = id;
        child.className = "root row";
        child.setAttribute("branch", parseInt(id.replace("root", "")) + 1);
        __("node-content").appendChild(child);
        //初始化成员
        var wc = this;
        wc.table = new CDrag.Table; //建立表格对象
        wc.iFunc = wc.eFunc = null;
        wc.obj = { on: { a: null, b: "" }, row: null, left: 0, top: 0 };
        wc.temp = { row: null, div: document.createElement("div") };//拖拽的div，站位div
        wc.temp.div.setAttribute(CDrag.IE ? "className" : "class", "CDrag_temp_div");
        wc.temp.div.innerHTML = "&nbsp;";
    },

    reMouse: function (a) {
        //获取鼠标位置
        var e = Object.reEvent();
        return {
            x: document.documentElement.scrollLeft + e.clientX,
            y: document.documentElement.scrollTop + e.clientY
        };
    },

    rePosition: function (o) {
        //获取元素绝对位置
        var __x = __y = 0;
        do {
            __x += o.offsetLeft;
            __y += o.offsetTop;
        } while ((o = o.offsetParent)); // && o.tagName != "BODY"
        return { x: __x, y: __y };
    },

    sMove: function (o) {
        //当拖动开始时设置参数

        var wc = this;
        if (wc.iFunc || wc.eFinc) return;

        var mouse = wc.reMouse(), obj = wc.obj, temp = wc.temp, div = o.element, position = wc.rePosition(div);

        obj.row = o;
        obj.on.b = "me";
        obj.left = mouse.x - position.x;
        obj.top = mouse.y - position.y;

        if (!$(div).find(".title").hasClass("isdelete")) {


            temp.row = document.body.appendChild(div.cloneNode(true)); //复制预拖拽对象

            with (temp.row.style) {
                //设置复制对象
                position = "absolute";
                left = mouse.x - obj.left + "px";
                top = mouse.y - obj.top + "px";
                zIndex = 100;
                opacity = "0.3";
                filter = "alpha(opacity:30)";
            }

            with (temp.div.style) {
                //设置站位对象
                height = div.clientHeight + "px";
                width = div.clientWidth + "px";
                float = "left";
            }

            var dragnoderow = o.element.children[0].children[0].innerHTML.split("_")[2];

            temp.div.setAttribute("row", dragnoderow);//设置站位div所在row


            wc.move_start_position = position;//拖动的节点初始位置

            wc.node_element = o.element;

            div.parentNode.replaceChild(temp.div, div);

            wc.iFunc = Object.addEvent(document, ["onmousemove"], wc.iMove.bind(wc));
            wc.eFunc = Object.addEvent(document, ["onmouseup"], wc.eMove.bind(wc));
            document.onselectstart = new Function("return false");
        }
    },

    iMove: function () {
        //当鼠标移动时设置参数

        var wc = this, mouse = wc.reMouse(), cols = wc.table.items, obj = wc.obj, temp = wc.temp,
         row = obj.row, div = temp.row, t_position, t_cols, t_rows, i, j;

        var m_row_position = wc.move_start_position;

        var m_cols = wc.obj.row.parent.items;

        with (div.style) {
            left = mouse.x - obj.left + "px";
            top = mouse.y - obj.top + "px";
        }

        var other_move = obj.row.parent.element.getElementsByClassName("move");

        if (other_move.length != 0) {

            for (var i = 0; i < other_move.length; i++) {

                t_position = wc.rePosition(other_move[i]);

                if (mouse.x > t_position.x && mouse.x < t_position.x + 100 && (!$(other_move[i]).find(".title").hasClass("isdelete"))) {

                    var temp_div = other_move[i];

                    var emptydivrow = temp.div.getAttribute("row");

                    var changenoderow = temp_div.getAttribute("row");

                    //变更对应数据
                    database.updateDatabase(temp_div, emptydivrow, "顺序变更");

                    temp_div.children[0].children[0].innerHTML = temp_div.children[0].children[0].innerHTML.split("_")[0] + "_" + temp_div.children[0].children[0].innerHTML.split("_")[1] + "_" + emptydivrow;

                    temp_div.setAttribute("row", emptydivrow);
                    temp_div.parentNode.replaceChild(temp_div, temp.div);

                    if (emptydivrow < changenoderow) {
                        //向左
                        if (i + 1 == other_move.length) {

                            other_move[i].parentNode.appendChild(temp.div);

                        } else {

                            other_move[i].parentNode.insertBefore(temp.div, other_move[i + 1]);

                        }


                    } else {
                        //向右
                        if (i == 0) {

                            other_move[i].parentNode.insertBefore(temp.div, other_move[0]);

                        } else {

                            other_move[i].parentNode.insertBefore(temp.div, other_move[i]);

                        }
                    }

                    temp.div.setAttribute("row", changenoderow);

                }



            }

        }

    },

    eMove: function () {
        //当鼠标释放时设置参数

        var wc = this, obj = wc.obj, temp = wc.temp, row = obj.row, div = row.element, o_cols, n_cols, number;

        var emptydivrow = temp.div.getAttribute("row");

        div.setAttribute("row", emptydivrow);

        //变更对应数据
        database.updateDatabase(div, emptydivrow, "顺序变更");

        div.children[0].children[0].innerHTML = div.children[0].children[0].innerHTML.split("_")[0] + "_" + div.children[0].children[0].innerHTML.split("_")[1] + "_" + emptydivrow;

        temp.div.parentNode.replaceChild(div, temp.div);
        temp.row.parentNode.removeChild(temp.row);
        delete temp.row;

        Object.delEvent(document, ["onmousemove"], wc.iFunc);
        Object.delEvent(document, ["onmouseup"], wc.eFunc);
        document.onselectstart = wc.iFunc = wc.eFunc = null;
    },

    add: function (o) {
        //添加对象
        var wc = this;

        o.mousedown = Object.addEvent(o.title, ["onmousedown"], wc.sMove.bind(wc, o));
        // o.reduceFunc = Object.addEvent(o.reduce, ["onclick"], wc.reduce.bind(wc, o));
        o.closeFunc = Object.addEvent(o.close, ["onclick"], wc.close.bind(wc, o));

    },

    close: function (o) {

        //删除对象
        var wc = this, parent = o.parent;

        Object.delEvent(o.close, ["onclick"], o.closeFunc);
        Object.delEvent(o.reduce, ["onclick"], o.reduceFunc);
        Object.delEvent(o.title, ["onmousedown"], o.mousedown);
        o.closeFunc = o.reduceFunc = o.mousedown = null;

        var row = o.element.getAttribute("row");

        for (var i = 0; i < o.parent.items.length; i++) {

            if (row == o.parent.items.length) {
                //最后一个节点

            } else {

                var otherRow = o.parent.items[i].element;

                if (row < otherRow.getAttribute("row") && (!$(otherRow).find(".title").hasClass("isdelete"))) {

                    //变更对应数据
                    database.updateDatabase(otherRow, otherRow.getAttribute("row") - 1, "顺序变更");

                    o.parent.items[i].element.setAttribute("row", otherRow.getAttribute("row") - 1);

                    otherRow.children[0].children[0].innerHTML = otherRow.children[0].children[0].innerHTML.split("_")[0] + "_" + otherRow.children[0].children[0].innerHTML.split("_")[1] + "_" + parseInt(otherRow.getAttribute("row"));

                }

            }

        }

        var node_id = $(o.element).find(".title_a").html();
        if (o.nodeInfo.isnewadd) {
            //删除对应数据
            database.updateDatabase(o.element, row, "删除");
        } else {
            //打上删除标记
            database.updateDatabase(o.element, row, "删除标记");
        }



        parent.element.removeChild(o.element);

        parent.del(o.id);

        delete o;

        calcTableHeaderWidth();
        columnAlign();

    },

    returnMaxId: function (id) {
        //返回列中的最大id
        var wc = this;
        for (var i = 0; i < wc.table.items.length; i++) {
            if (id == wc.table.items[i].id) {
                //wc.table.items[i].items.push({ id: wc.table.items[i].items.length, root_id: id + "_" + wc.table.items[i].items.length });
                return wc.table.items[i].items.length + 1;
            }
        }
    },

    addnodeevent: function (o) {

        var wc = this;

        var div = o.offsetParent;

        var cols;

        for (var i = 0; i < wc.table.items.length; i++) {
            if (wc.table.items[i].id == div.id) {
                cols = wc.table.items[i];
            }
        }

        var id = wc.returnMaxId(div.id);
        var branch = div.id.split("_")[0];
        var column = parseInt(div.id.split("_")[1]);

        //获取类型
        var type = $(".root-title-cell[column='" + column + "'] ").html();

        var databaseitem = { objectid: $.GetGUID(), id: div.id + "_" + id, title: div.id + "_" + id, content: "新节点", type: type, branch: branch, column: column, row: id, oldrow: id, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false };


        database.json.push(databaseitem);

        wc.add(cols.add(databaseitem.objectid));


        calcTableHeaderWidth();
        columnAlign();
    },
    //上移按钮
    upColumnSort: function (o) {

        var branch = parseInt(o.getAttribute("branch"));

        var upElement = document.getElementById("root" + parseInt(branch - 1));

        var downElement = document.getElementById("root" + parseInt(branch - 2));

        if (downElement != null) {


            $(".root").remove();

            $.each(database.json, function (key, val) {
                if (val.branch == branch) {
                    val.branch = branch - 1;
                }
                else if (val.branch == branch - 1) {
                    val.branch = branch;
                }
                val.issortchange = true;
                val.state = 2;
                val.id = val.branch + "_" + val.column + "_" + val.row;

            });

            var upGo = function (fieldData, index) {

                if (index != 0) {

                    fieldData[index] = fieldData.splice(index - 1, 1, fieldData[index])[0];

                } else {

                    fieldData.push(fieldData.shift());

                }

            }

            $.each(database.colsJson, function (key, val) {

                var colsBranch = val[0].cols.split('_')[0];

                if (colsBranch == branch) {
                    $.each(val, function (colKey, colVal) {
                        colVal.cols = (branch - 1) + "_" + colVal.cols.split('_')[1];
                        $.each(colVal.rows, function (rowKey, rowVal) {
                            rowVal.id = (branch - 1) + "_" + rowVal.id.split('_')[1] + "_" + rowVal.id.split('_')[2]
                        });
                    });
                }
                else if (colsBranch == branch - 1) {
                    $.each(val, function (colKey, colVal) {
                        colVal.cols = branch + "_" + colVal.cols.split('_')[1];
                        $.each(colVal.rows, function (rowKey, rowVal) {
                            rowVal.id = branch + "_" + rowVal.id.split('_')[1] + "_" + rowVal.id.split('_')[2]
                        });
                    });

                }


            });

            upGo(database.colsJson, branch - 1);

            $.each(database.colsJson, function (key, val) {


                var wc = new CDrag("root" + key);

                wc.parse(val, "root" + key);

            });

        }



    },
    //下移按钮
    downColumnSort: function (o) {

        var branch = parseInt(o.getAttribute("branch"));

        var upElement = document.getElementById("root" + parseInt(branch));

        var downElement = document.getElementById("root" + parseInt(branch - 1));


        if (upElement != null) {

            $(".root").remove();

            $.each(database.json, function (key, val) {
                if (val.branch == branch) {
                    val.branch = branch + 1;
                }
                else if (val.branch == branch + 1) {
                    val.branch = branch;
                }
                val.state = 2;
                val.issortchange = true;
                val.id = val.branch + "_" + val.column + "_" + val.row;

            });

            var downGo = function (fieldData, index) {

                if (index != fieldData.length - 1) {

                    fieldData[index] = fieldData.splice(index + 1, 1, fieldData[index])[0];

                } else {

                    fieldData.unshift(fieldData.splice(index, 1)[0]);

                }

            }


            $.each(database.colsJson, function (key, val) {

                var colsBranch = val[0].cols.split('_')[0];

                if (colsBranch == branch) {
                    $.each(val, function (colKey, colVal) {
                        colVal.cols = (branch + 1) + "_" + colVal.cols.split('_')[1];
                        $.each(colVal.rows, function (rowKey, rowVal) {
                            rowVal.id = (branch + 1) + "_" + rowVal.id.split('_')[1] + "_" + rowVal.id.split('_')[2]
                        });
                    });
                }
                else if (colsBranch == branch + 1) {
                    $.each(val, function (colKey, colVal) {
                        colVal.cols = branch + "_" + colVal.cols.split('_')[1];
                        $.each(colVal.rows, function (rowKey, rowVal) {
                            rowVal.id = branch + "_" + rowVal.id.split('_')[1] + "_" + rowVal.id.split('_')[2]
                        });
                    });

                }


            });

            downGo(database.colsJson, branch - 1);

            $.each(database.colsJson, function (key, val) {

                var wc = new CDrag("root" + key);

                wc.parse(val, "root" + key);

            });

        }

    },

    parse: function (o, rootid) {


        //初始化成员
        try {

            var wc = this, table = wc.table, cols, rows, div, i, j;

            //创建列div
            for (i = 0 ; i < o.length ; i++) {

                var child = document.createElement("div");
                child.id = o[i].cols;
                child.className = "cell";
                __(rootid).appendChild(child);
                //创建每一次的增加单元格的“加号”
                var addchild = document.createElement("div");
                addchild.id = "addnode" + o[i].cols;
                addchild.innerHTML = "+";
                addchild.className = "addnode";
                __(child.id).appendChild(addchild);

                //绑定“加号”事件
                Object.addEvent(addchild, ["onclick"], wc.addnodeevent.bind(wc, addchild));

                if (i == 0) {
                    //添加排序按钮
                    var sortBtn = document.createElement("div");
                    sortBtn.id = o[i].cols.split('_')[0] + "_0";
                    sortBtn.className = "sortCell";
                    __(child.id).appendChild(sortBtn);
                    var upBtn = document.createElement("div");
                    upBtn.id = "up_" + o[i].cols.split('_')[0] + "_0";
                    upBtn.className = "iconfont icontop-s upBtn";
                    upBtn.setAttribute("title", "上移");
                    upBtn.setAttribute("branch", o[i].cols.split('_')[0]);

                    var downBtn = document.createElement("div");
                    downBtn.id = "down_" + o[i].cols.split('_')[0] + "_0";
                    downBtn.className = "iconfont iconbelow-s downBtn";
                    downBtn.setAttribute("title", "下移");
                    downBtn.setAttribute("branch", o[i].cols.split('_')[0]);

                    __(sortBtn.id).appendChild(upBtn);
                    __(sortBtn.id).appendChild(downBtn);


                    //绑定“排序按钮”事件
                    Object.addEvent(upBtn, ["onclick"], wc.upColumnSort.bind(wc, upBtn));
                    Object.addEvent(downBtn, ["onclick"], wc.downColumnSort.bind(wc, downBtn));


                }

                if (i < o.length - 1) {
                    var childnbsp = document.createElement("div");
                    childnbsp.className = "r_nbsp";
                    childnbsp.innerHTML = "&nbsp;";
                    __(rootid).appendChild(childnbsp);
                }

            }

            //增加列及行
            for (i = 0 ; i < o.length ; i++) {

                div = __(o[i].cols), cols = table.add(div, o[i].cols);

                for (j = 0 ; j < o[i].rows.length ; j++) {

                    wc.add(cols.add(o[i].rows[j].objectid));
                }
            }

            //重新计算列宽
            calcTableHeaderWidth();
            columnAlign();
        } catch (exp) {
            console.dir(exp);
        }
    }


};

calcTableHeaderWidth = function () {
    //计算对应表头宽度
    var parentHeaderWidth = 100;

    var headerDiv = document.getElementsByClassName("root-title-cell");

    var cols = document.getElementsByClassName("cell");


    for (var i = 0; i < headerDiv.length; i++) {

        for (j = 0 ; j < cols.length ; j++) {


            div = cols[j];

            if (headerDiv[i].getAttribute("column") == div.getAttribute("column")) {


                if (div.offsetWidth >= headerDiv[i].offsetWidth) {

                    headerDiv[i].style.width = div.offsetWidth + "px";

                }

            }

        }
        parentHeaderWidth += headerDiv[i].offsetWidth;
    }


    document.getElementsByClassName("root-title")[0].style.width = parentHeaderWidth + "px";

    var rootArray = document.getElementsByClassName("root");
    for (var i = 0; i < rootArray.length; i++) {
        rootArray[i].style.width = parentHeaderWidth + "px";
    }


}

columnAlign = function () {

    var headerDiv = document.getElementsByClassName("root-title-cell");

    var cols = document.getElementsByClassName("cell");

    for (j = 0 ; j < cols.length ; j++) {

        div = cols[j];

        for (var i = 0; i < headerDiv.length; i++) {

            if (headerDiv[i].getAttribute("column") == div.getAttribute("column")) {

                var offsetleft = div.style.left == '' ? 0 : parseInt(div.style.left.substr(0, div.style.left.length - 2));

                div.style.left = parseInt(headerDiv[i].offsetLeft - div.offsetLeft + offsetleft) + "px";

            }
        }


    }

}



//分支操作
var addBranch = Class.create();

addBranch.prototype = {

    maxBranch: function () {
        return parseInt($(".root").length) + 1;
    },

    initialize: function () {

    },

    getJson: function () {

        var jsonTemplate = [
            { objectid: $.GetGUID(), id: this.maxBranch() + "_1_1", content: "无", type: "分支条件", branch: this.maxBranch(), column: 1, row: 1, oldrow: 1, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_2_1", content: "发起人", type: "发起", branch: this.maxBranch(), column: 2, row: 1, oldrow: 1, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_2_2", content: "直接上级", type: "发起", branch: this.maxBranch(), column: 2, row: 2, oldrow: 2, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_3_1", content: "专业角色", type: "专业审批", branch: this.maxBranch(), column: 3, row: 1, oldrow: 2, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_4_1", content: "权责角色", type: "权责审批", branch: this.maxBranch(), column: 4, row: 1, oldrow: 1, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_5_1", content: "执行知会角色", type: "执行/知会", branch: this.maxBranch(), column: 5, row: 1, oldrow: 1, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" },

            { objectid: $.GetGUID(), id: this.maxBranch() + "_6_1", content: "归档", type: "归档", branch: this.maxBranch(), column: 6, row: 1, oldrow: 1, state: 2, processname: decodeURIComponent($.GetLocationParams("WorkFlowName")), processcode: $.GetLocationParams("WorkFlowCode"), isdelete: false, isnewadd: true, isrolechange: false, issortchange: false, version: "1.0" }];

        return jsonTemplate;

    },

    add: function () {

        //创建新分支
        var o = this;

        var databaseJson = o.getJson();

        var maxBranch = this.maxBranch();

        for (var i = 0; i < databaseJson.length; i++) {
            database.json.push(databaseJson[i]);
        }


        var json = [{
            cols: maxBranch + "_1",
            rows: [
             { id: maxBranch + "_1_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_1_1" }).objectid }
            ]
        }, {
            cols: maxBranch + "_2",
            rows: [
             { id: maxBranch + "_2_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_2_1" }).objectid },
             { id: maxBranch + "_2_2", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_2_2" }).objectid }

            ]
        }, {
            cols: maxBranch + "_3",
            rows: [
                { id: maxBranch + "_3_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_3_1" }).objectid }
            ]
        }, {
            cols: maxBranch + "_4",
            rows: [
             { id: maxBranch + "_4_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_4_1" }).objectid }
            ]

        }, {
            cols: maxBranch + "_5",
            rows: [
             { id: maxBranch + "_5_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_5_1" }).objectid }
            ]
        }, {
            cols: maxBranch + "_6",
            rows: [
             { id: maxBranch + "_6_1", objectid: _.find(databaseJson, function (val) { return val.id == maxBranch + "_6_1" }).objectid }
            ]
        }];
        database.colsJson.push(json);

        var wc = new CDrag("root" + parseInt(this.maxBranch() - 1));

        wc.parse(json, "root" + parseInt(this.maxBranch() - 2));



    },

}



Object.addEvent(window, ["onload"], function () {

    database.json = JSON.parse($("#nodeList").val());

    database.colsJson = JSON.parse($("#nodeData").val());

    $.each(database.colsJson, function (key, val) {


        var wc = new CDrag("root" + key);

        wc.parse(val, "root" + key);

    });

    var AddBranch = new addBranch();

    Object.addEvent(document.getElementById("addBranch"), ["click"], AddBranch.add.bind(AddBranch));


});


var database = {

    json: $("#nodeList").val(),

    colsJson: $("#nodeData").val(),

    parse: function (id, jsondata) {
        //昂应该用AJAX查找然后返回数据..我这里就拿..json串模拟好了嘿

        var wc = this, i;

        for (i in jsondata) {
            if (jsondata[i].objectid == id) {

                return { id: jsondata[i].id, content: jsondata[i].content, type: jsondata[i].type, branch: jsondata[i].branch, column: jsondata[i].column, row: jsondata[i].row, processname: jsondata[i].processname, processcode: jsondata[i].processcode, state: jsondata[i].state, oldrow: jsondata[i].oldrow, isnewadd: jsondata[i].isnewadd, isdelete: jsondata[i].isdelete, isrolechange: jsondata[i].isrolechange, issortchange: jsondata[i].issortchange, objectid: jsondata[i].objectid, version: jsondata[i].version };

            }
        }
    },

    updateDatabase: function (o, row, type) {

        //变更对应数据
        $.each(database.json, function (key, val) {

            if (val.id == $(o).find(".title_a").html() && val.isdelete != true) {

                database.json = _.without(database.json, val);

                val.state = 2;

                if (type == "删除") {

                    val.isnewadd = true;
                    val.isdelete = true;

                } else {
                    switch (type) {

                        case "删除标记":
                            val.isnewadd = false;
                            val.isdelete = true;
                            break;
                        case "顺序变更":
                            val.issortchange = true;
                            break;

                    }


                }

                val.oldrow = val.row;

                val.row = parseInt(row);

                val.id = val.branch + "_" + val.column + "_" + val.row;

                database.json.push(val);


                return false;

            }

        });

    },

    updateRoleName: function (o, roleName) {


        $.each(database.json, function (key, val) {

            if (val.objectid == $(o).find(".title_a").attr("objectid")) {

                database.json = _.without(database.json, val);

                val.content = roleName;

                val.state = 2;

                val.isrolechange = true;

                database.json.push(val);

                return false;

            }

        });

    }

};

