var DataEditorView = (function () {
    function DataEditorView() {
        this.entity = null;
    }
    DataEditorView.prototype.init = function (options) {
        var me = this;
        $(".custom-toolsbar").find("button").on("click", function () {
            me.toolstripItemClick($(this));
        });
        var context = $("#data-entity").val();
        if (context) {
            var words = CryptoJS.enc.Base64.parse(context);
            context = CryptoJS.enc.Utf8.stringify(words);
            this.entity = $.parseJSON(context);
        }
        if (this.entity)
            $("#tool-accept").html('<i class="layui-icon">&#xe605;</i>提交数据更新');
        else
            $("#tool-accept").html('<i class="layui-icon">&#xe605;</i>提交新增记录');
        $("#upload-file").on("change", function (event) {
            var input = event.target;
            if (input.files.length < 1)
                return;
            var $showFile = $(".photo-toolbox").find(".show-file");
            if ($showFile.length < 1) {
                $showFile = $('<li class="show-file"></li>');
                $(".photo-toolbox").append($showFile);
            }
            $showFile.html('[选定的本地照片]：' + input.files.item(0).name + '<a class="remove-lnk" href="javascript:;"><i class="layui-icon">&#x1006;</i></a>');
            $showFile.find(".remove-lnk").on("click", function () {
                $("#upload-file").val('');
                var xxx = $("#upload-file").get(0);
                $showFile.remove();
                console.clear();
                console.log(xxx.files);
            });
        });
        this.reload();
    };
    DataEditorView.prototype.reload = function (arg) {
        var isEdit = (this.entity !== undefined && this.entity !== null);
        var me = this;
        $(".data-editor").find("input").each(function (index, elem) {
            var $target = $(this);
            if ($target.attr("type").toLowerCase() === "hidden")
                return;
            var propName = $target.attr("name");
            if (!propName)
                return;
            if (isEdit)
                $target.val(me.entity[propName]);
            else
                $target.val('');
        });
        $("#group-id").val(isEdit ? me.entity.GroupId : '');
        $("#test-sex").val(isEdit ? me.entity.TestSex : '');
        var $seePicture = $("#see-picture");
        if (isEdit) {
            var version = Math.floor(Math.random() * 1000000);
            $seePicture.attr("href", "/api/data/" + this.entity.TestId + "/picture?v=" + version + "");
            $seePicture.attr("target", "_blank");
        }
        else {
            $seePicture.attr("href", "javascript:;");
            $seePicture.removeAttr("target");
        }
        $("#upload-file").val('');
        $(".photo-toolbox").find(".show-file").remove();
        layui.form.render("select", "data_editor");
    };
    DataEditorView.prototype.getEditObject = function () {
        var obj = new Object();
        var target, $target;
        var propName, value;
        var inputs = $("input,select");
        for (var i = 0; i < inputs.length; ++i) {
            target = inputs.get(i);
            $target = $(target);
            propName = $target.attr("name");
            if (!propName)
                continue;
            value = $target.val();
            if (value) {
                switch (propName) {
                    case "GroupId":
                        obj[propName] = parseInt(value);
                        break;
                    case "TestAge":
                        obj[propName] = parseFloat(value);
                        break;
                    default:
                        obj[propName] = value;
                        break;
                }
            }
            else {
                if (target.hasAttribute("required")) {
                    layer.alert($target.attr("data-prompt") || "数据有效性验证失败，请检查必填项.");
                    return null;
                }
            }
        }
        return obj;
    };
    DataEditorView.prototype.accept = function () {
        var entity = this.getEditObject();
        if (!entity)
            return;
        var me = this;
        var serverUrl = !this.entity ? "/api/data/add" : "/api/data/" + this.entity.TestId + "/update";
        var uploadFile = $("#upload-file").get(0);
        var loading = layer.load();
        var callback = function (result) {
            layer.close(loading);
            if (result.code === 0x00) {
                if (me.entity) {
                    layer.alert("已成功更新数据！", { title: "提示信息" }, function (index, layero) {
                        layer.close(index);
                        window.mainApp.changeView("Shared/DataList");
                    });
                }
                else {
                    layer.confirm("已成功添加数据，请选择接下来的操作！", { title: "提示信息", btn: ["继续添加", "返回列表"] }, function (index, layero) {
                        layer.close(index);
                        me.reload();
                    }, function (incex, layero) { window.mainApp.changeView("Shared/DataList"); });
                }
            }
            else {
                layer.alert(result.message, { icon: 2, title: "错误信息" });
            }
        };
        if (uploadFile.files.length > 0) {
            var data = new FormData();
            for (var prop in entity)
                data.append(prop, entity[prop]);
            data.append("TestPhoto", uploadFile.files.item(0));
            service.postMultipart(serverUrl, data, callback);
        }
        else {
            service.postJson(serverUrl, entity, callback);
        }
    };
    DataEditorView.prototype.toolstripItemClick = function ($target) {
        switch ($target.attr("event-name")) {
            case "goback":
                window.mainApp.changeView("Shared/DataList");
                break;
            case "accept":
                this.accept();
                break;
            case "reset":
                this.reload();
                break;
        }
    };
    DataEditorView.prototype.fitScreen = function () { };
    return DataEditorView;
}());
window.mainApp.setActiveView(new DataEditorView());
//# sourceMappingURL=data-editorview.js.map