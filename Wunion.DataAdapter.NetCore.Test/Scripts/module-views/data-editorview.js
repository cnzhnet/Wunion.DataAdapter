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
        layui.form.render("select", "data_editor");
    };
    DataEditorView.prototype.toolstripItemClick = function ($target) {
        switch ($target.attr("event-name")) {
            case "goback":
                window.mainApp.changeView("Shared/DataList");
                break;
            case "accept":
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