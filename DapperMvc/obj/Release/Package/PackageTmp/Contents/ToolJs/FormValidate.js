var FormValidate = {
    form: [],
    data: [],
    Init: function (formId) {
        this.form = document.getElementById(formId);
    },


    //必填验证
    RequireValidate: function (node) {
        var value = $(node).val();
        if (value != "" && value != null && value != undefined) {
            $(node).parent().removeClass("has-error").removeClass("has-feedback");
            $(node).parent().find("span").remove();
            return true;
        } else {
            $(node).parent().addClass("has-error").addClass("has-feedback");
            $(node).after('<span class="form-control-feedback" aria-hidden="true">必填</span>');
            return false;
        }
    },
    //提交验证
    Validate: function (formId) {
        var _this = this;
        _this.form = document.getElementById(formId);
        var flag = true;
        $(_this.form).find("input,select").each(function () {
            //console.dir($(this));
            if ($(this).data("require")) {
                flag = _this.RequireValidate($(this));
                //console.dir(flag);
                if (!flag) {
                    return flag;
                }
            }
        });
        //alert(flag);
        return flag;
    }
};
//FormValidate.Init();