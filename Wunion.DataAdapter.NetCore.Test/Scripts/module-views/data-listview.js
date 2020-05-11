var DataListModuleView = (function () {
    function DataListModuleView() {
        this.pageSize = 3;
        this.currentPage = 1;
        this.searchMode = false;
        this.tableView = null;
    }
    DataListModuleView.prototype.init = function (options) {
        var me = this;
        this.$datalistView = $("#data-listView");
        $(".custom-toolsbar").find(".tool-button").on("click", function () {
            me.toolstripItemClick($(this));
        });
        var $keywords = $("#search-keywords");
        $keywords.unbind("keydown");
        $keywords.on("keydown", function (e) {
            var event = e;
            if (!event)
                event = window.event;
            if (!event)
                return;
            if ((event.keyCode || event.which) === 0x0d)
                me.search();
        });
        this.reload();
    };
    DataListModuleView.prototype.reload = function (arg) {
        this.loadGroupDropdown();
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
            layui.form.on("select(group_dropdown)", function (data) {
                me.groupId = !(data.value) ? null : parseInt(data.value);
                me.loadData();
            });
        });
    };
    DataListModuleView.prototype.search = function () {
        this.searchMode = !($("#search-keywords").val()) ? false : true;
        this.loadData();
    };
    DataListModuleView.prototype.loadData = function () {
        var requestUrl;
        var parameters = new Object();
        parameters.page = this.currentPage;
        parameters.pagesize = this.pageSize;
        if (this.searchMode) {
            requestUrl = "/api/data/search";
            parameters.keywords = $("#search-keywords").val();
        }
        else {
            requestUrl = "/api/data/list";
        }
        if (this.groupId)
            parameters.group = this.groupId;
        this.tableView = null;
        $("#pagination").html("");
        var me = this;
        var index = layer.load();
        service.post(requestUrl, parameters, function (result) {
            layer.close(index);
            if (result.code === 0x00) {
                me.dataBind(result.data);
            }
            else {
                me.$datalistView.html(result.message);
            }
        });
    };
    DataListModuleView.prototype.dataBind = function (data) {
        if (!data)
            return;
        var me = this;
        this.$datalistView.html('<table id="data-table"></table>');
        this.tableView = this.$datalistView.find("#data-table").datagrid({
            autoRowHeight: false, striped: true, rowStyler: function (index, row) { return "height: 32px;"; },
            selectOnCheck: false, checkOnSelect: false, rownumbers: true, data: data.Items, columns: [[
                    { title: "编号", field: "TestId", width: 75, align: "center", halign: "center" },
                    {
                        title: "名称", field: "TestName", width: 210, align: "left", halign: "center", formatter: function (value, row, index) {
                            var version = Math.floor(Math.random() * 1000000);
                            return '<a href="/api/data/' + row.TestId + '/picture?v=' + version + '" target="_blank" title="点击查看图片.">' + value + '</a>';
                        }
                    },
                    { title: "分组", field: "GroupName", width: 160, align: "left", halign: "center" },
                    { title: "龄", field: "TestAge", width: 52, align: "center", halign: "center" },
                    { title: "性征", field: "TestSex", width: 60, align: "center", halign: "center" },
                    {
                        title: "管理", field: "undefined", width: "140", align: "center", halign: "center", formatter: function (value, row, index) {
                            var content = '<a class="cell-link" href="javascript:;" data-id="' + row.TestId + '" event-name="edit"><i class="layui-icon">&#xe642;</i>编辑</a>';
                            content += '<a class="cell-link" href="javascript:;" data-id="' + row.TestId + '" event-name="delete" style="margin-left: 12px;"><i class="layui-icon">&#xe640;</i>删除</a>';
                            return content;
                        }
                    }
                ]],
            onSelect: function (index, row) { $(this).datagrid("unselectRow", index); }
        });
        this.$datalistView.find(".cell-link").on("click", function () {
            var $link = $(this);
            me.cellLinkEvent(parseInt($link.attr("data-id")), $link.attr("event-name"));
        });
        layui.laypage.render({
            elem: "pagination", count: data.Total, limit: this.pageSize, curr: data.Page,
            first: "|&lt;", prev: "&lt;", next: "&gt;", last: "&gt;|", groups: 12,
            limits: [3, 5, 10, 15, 20], layout: ["count", "limit", "prev", "page", "next"],
            jump: function (arg, first) {
                if (first)
                    return;
                me.pageSize = arg.limit;
                me.currentPage = arg.curr;
                me.loadData();
            }
        });
        this.fitScreen();
    };
    DataListModuleView.prototype.cellLinkEvent = function (dataId, event) {
        switch (event) {
            case "edit":
                window.mainApp.changeView("Shared/DataEditor?id=" + dataId + "");
                break;
            case "delete":
                break;
        }
    };
    DataListModuleView.prototype.fitScreen = function () {
        if (!(this.tableView))
            return;
        var $parent = $(".module-view").find(".container");
        var th = $parent.height();
        th -= $(".module-view").find(".custom-toolsbar").height();
        th -= $(".module-view").find(".pagination-bar").height();
        this.tableView.datagrid('resize', { width: $parent.width(), height: th });
    };
    DataListModuleView.prototype.toolstripItemClick = function ($target) {
        var eventName = $target.attr("event-name");
        switch (eventName) {
            case "add":
                window.mainApp.changeView("Shared/DataEditor");
                break;
            case "search":
                this.search();
                break;
        }
    };
    return DataListModuleView;
}());
window.mainApp.setActiveView(new DataListModuleView());
//# sourceMappingURL=data-listview.js.map