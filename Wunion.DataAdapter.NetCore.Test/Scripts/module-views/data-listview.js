var DataListModuleView = (function () {
    function DataListModuleView() {
    }
    DataListModuleView.prototype.init = function (options) {
        var me = this;
        this.$datalistView = $("#data-listView");
        $(".custom-toolsbar").find(".tool-button").on("click", function () {
            me.toolstripItemClick($(this));
        });
        this.reload();
    };
    DataListModuleView.prototype.reload = function (arg) {
        this.loadGroupDropdown();
    };
    DataListModuleView.prototype.fitScreen = function () {
    };
    DataListModuleView.prototype.loadGroupDropdown = function () {
        var me = this;
        var $dropdown = $("#group-dropdown");
        var index = layer.load();
        $dropdown.html("");
        service.get("/api/group/query", function (result) {
            layer.close(index);
            if (result.code === 0x00) {
                $dropdown.append($('<option value="">所有分组</option>'));
                var list = result.data;
                if (!list)
                    return;
                $.each(list, function (index, value) {
                    $dropdown.append($('<option value="' + this.GroupId + '">' + this.GroupName + '</option>'));
                });
                me.loadData();
            }
            else {
                me.$datalistView.html(result.message || "产生错误.");
            }
            layui.form.render("select", "dlist_view");
            layui.form.on("select(ch_database)", function (data) { window.mainApp.changeDatabase(data.value); });
            layui.form.on("select(group_dropdown)", function (data) { me.loadData(data.value); });
        });
    };
    DataListModuleView.prototype.loadData = function (group) {
        if (group)
            layer.alert("分组ID：" + group);
        else
            layer.alert("所有分组！");
    };
    DataListModuleView.prototype.toolstripItemClick = function ($target) {
        var eventName = $target.attr("event-name");
        switch (eventName) {
            case "add":
                layer.alert("添加功能！");
                break;
            case "search":
                layer.alert("搜索功能！");
                break;
        }
    };
    return DataListModuleView;
}());
window.mainApp.setActiveView(new DataListModuleView());
//# sourceMappingURL=data-listview.js.map