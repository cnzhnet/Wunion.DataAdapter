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
        layer.alert("删除" + gid);
    };
    GroupListView.prototype.fitScreen = function () { };
    GroupListView.prototype.toolstripItemClick = function ($target) {
    };
    return GroupListView;
}());
window.mainApp.setActiveView(new GroupListView());
//# sourceMappingURL=group-listview.js.map