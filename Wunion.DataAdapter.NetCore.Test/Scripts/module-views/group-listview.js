var GroupListView = (function () {
    function GroupListView() {
    }
    GroupListView.prototype.init = function (options) {
        var me = this;
        layui.form.render("select", "group_listView");
        layui.form.on("select(ch_database)", function (data) {
            window.mainApp.changeDatabase(data.value);
        });
        $(".custom-toolsbar").find(".tool-button").on("click", function () {
            me.toolstripItemClick($(this));
        });
        this.reload();
    };
    GroupListView.prototype.reload = function (arg) {
        var me = this;
        var index = layer.load();
        service.get("/api/group/Query", function (result) {
            layer.close(index);
            var $groupList = $(".group-list-view");
            if (result.code !== 0x00) {
                $groupList.html('<li class="last-item">' + result.message + '</li>');
                return;
            }
            var items = result.data;
            if (!items) {
                $groupList.html('<li class="last-item">目前尚未创建分组信息.</li>');
                return;
            }
            if (items.length < 1) {
                $groupList.html('<li class="last-item">目前尚未创建分组信息.</li>');
                return;
            }
            $groupList.html("");
            $.each(items, function (index, value) {
                var $li = $('<li><h2>' + this.GroupName + '</h2></li>');
                var $toolbox = $('<div class="item-toolbox"></div>');
                $toolbox.append($('<a href="javascript:;" data-id="' + this.GroupId + '" event-name="delete">删除<i class="layui-icon">&#xe640;</i></a>'));
                if (index == (items.length - 1))
                    $li.addClass("last-item");
                $li.append($toolbox);
                $groupList.append($li);
                $toolbox.find("a").on("click", function () {
                    var $target = $(this);
                    var grpId = parseInt($target.attr("data-id"));
                    switch ($target.attr("event-name")) {
                        case "delete":
                            me.delete(grpId);
                            break;
                    }
                });
            });
        });
    };
    GroupListView.prototype.delete = function (gid) {
        var me = this;
        layer.confirm("是否要删除此分组？", { title: '<strong>分组删除确认</strong>' }, function (index, layero) {
            layer.close(index);
            service.get("/api/group/" + gid + "/delete", function (result) {
                if (result.code === 0x00)
                    me.reload();
                else
                    layer.alert(result.message, { icon: 2, title: "错误信息" });
                ;
            });
        });
    };
    GroupListView.prototype.fitScreen = function () { };
    GroupListView.prototype.showAddDialog = function () {
        var ui = '<div style="margin: 0px; padding: 12px;"><h4 style="margin-bottom: 8px;">请输入分组名称：</h4>';
        ui += '<div><input type="text" class="layui-input" style="width: 210px;" id="group-name" autocomplete="off" />';
        ui += '</div></div>';
        var me = this;
        layer.open({
            type: 1, title: '<strong>添加分组对话框</strong>', btn: ["确认", "取消"], content: ui,
            yes: function (index, layero) {
                layer.close(index);
                var loading = layer.load();
                service.post("/api/group/add", { name: layero.find("#group-name").val() }, function (result) {
                    layer.close(loading);
                    if (result.code === 0x00)
                        me.reload();
                    else
                        layer.alert(result.message, { icon: 2, title: "错误信息" });
                });
            }
        });
    };
    GroupListView.prototype.toolstripItemClick = function ($target) {
        switch ($target.attr("event-name")) {
            case "add":
                this.showAddDialog();
                break;
        }
    };
    return GroupListView;
}());
window.mainApp.setActiveView(new GroupListView());
//# sourceMappingURL=group-listview.js.map