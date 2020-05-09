/// <reference path="../types/layui/index.d.ts" />
/// <reference path="../types/easyui/datagrid.d.ts" />
/// <reference path="../types/type.extension.d.ts" />

/** 数据更表子视图的客户端控制脚本. */
class DataListModuleView implements IModuleView {
    private $datalistView: JQuery<HTMLElement>;
    private pageSize: number;
    private searchMode: boolean;
    private currentPage: number;
    private groupId: number | null;
    private tableView: JQuery<HTMLElement>;

    /** 创建一个 DataListModuleView 的对象实例. */
    constructor() {
        this.pageSize = 3;
        this.currentPage = 1;
        this.searchMode = false;
        this.tableView = null;
    }
    /** 初始化客户端脚本对象. */
    public init(options?: any): void {
        let me: DataListModuleView = this;        
        this.$datalistView = $("#data-listView");
        $(".custom-toolsbar").find(".tool-button").on("click", function (this: HTMLElement): void {
            me.toolstripItemClick($(this));
        });
        let $keywords: JQuery<HTMLElement> = $("#search-keywords");
        $keywords.unbind("keydown");
        $keywords.on("keydown", function (this: HTMLElement, e: Event): void {
            let event: KeyboardEvent = e as KeyboardEvent;
            if (!event)
                event = window.event as KeyboardEvent;
            if (!event)
                return;
            if ((event.keyCode || event.which) === 0x0d)
                me.search();
        });
        this.reload();
    }
    /** 加载或重新加载数据 . */
    public reload(arg?: any): void {
        this.loadGroupDropdown();
    }        
    /** 加载分组下拉选项. */
    private loadGroupDropdown(): void {
        let me: DataListModuleView = this;
        let $dropdown: JQuery<HTMLElement> = $("#group-dropdown");
        let index: number = layer.load();
        $dropdown.html("");
        service.get("/api/group/query", function (result: IWebApiResult): void {
            layer.close(index);
            if (result.code === 0x00) {
                $dropdown.append($('<option value="">所有分组</option>'));
                let list: any[] = result.data as any[];
                if (!list)
                    return;
                $.each<any>(list, function (this: any, index: number, value: any): any {
                    $dropdown.append($('<option value="' + this.GroupId + '">' + this.GroupName + '</option>'));
                });
                me.loadData();
            }
            else {
                me.$datalistView.html(result.message || "产生错误.");
            }
            layui.form.render("select", "dlist_view");
            layui.form.on("select(ch_database)", function (data: any): void { window.mainApp.changeDatabase(data.value); });
            layui.form.on("select(group_dropdown)", function (data: any): void {
                me.groupId = !(data.value) ? null : parseInt(data.value);
                me.loadData();
            });
        });
    }
    /** 搜索信息. */
    private search(): void {
        this.searchMode = !($("#search-keywords").val()) ? false : true;
        this.loadData();
    }
    /** 用于加载数据. */
    private loadData(): void {
        let requestUrl: string;
        let parameters: any = new Object();
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
        let me: DataListModuleView = this;
        let index: number = layer.load();
        service.post(requestUrl, parameters, function (result: IWebApiResult): void {
            layer.close(index);
            if (result.code === 0x00) {
                me.dataBind(result.data);
            }
            else {
                me.$datalistView.html(result.message);
            }
        });
    }
    /** 绑定数据列表.
     * @param data 包含分页信息的对象. */
    private dataBind(data: any): void {
        if (!data)
            return;
        let me: DataListModuleView = this;
        this.$datalistView.html('<table id="data-table"></table>');
        this.tableView = this.$datalistView.find("#data-table").datagrid({
            autoRowHeight: false, striped: true, rowStyler: function (index: number, row: any): any { return "height: 32px;"; },
            selectOnCheck: false, checkOnSelect: false, rownumbers: true, data: data.Items, columns: [[
                { title: "编号", field: "TestId", width: 75, align: "center", halign: "center" },
                { title: "名称", field: "TestName", width: 210, align: "left", halign: "center" },
                { title: "分组", field: "GroupName", width: 160, align: "left", halign: "center" },
                { title: "龄", field: "TestAge", width: 52, align: "center", halign: "center" },
                { title: "性征", field: "TestSex", width: 60, align: "center", halign: "center" },
                {
                    title: "管理", field: "undefined", width: "140", align: "center", halign: "center", formatter: function (value: number | string, row: any, index: number): any {
                        let content: string = '<a class="cell-link" href="javascript:;" data-id="' + row.TestId + '" event-name="edit"><i class="layui-icon">&#xe642;</i>编辑</a>'; //style="margin-right: 4px;"
                        content += '<a class="cell-link" href="javascript:;" data-id="' + row.TestId + '" event-name="delete" style="margin-left: 12px;"><i class="layui-icon">&#xe640;</i>删除</a>';
                        return content;
                    }
                }
            ]],
            onSelect: function (index: number, row: any): void { $(this).datagrid("unselectRow", index); }
        });        
        this.$datalistView.find(".cell-link").on("click", function (this: HTMLElement): void {
            let $link: JQuery<HTMLElement> = $(this);
            me.cellLinkEvent(parseInt($link.attr("data-id")), $link.attr("event-name"));
        });
        layui.laypage.render({
            elem: "pagination", count: data.Total, limit: this.pageSize, curr: data.Page,
            limits: [3, 5, 10, 15, 20], layout: ["count", "limit", "prev", "page", "next"],
            jump: function (arg: any, first: boolean): void {
                if (first)
                    return;
                me.pageSize = arg.limit;
                me.currentPage = arg.curr;
                me.loadData();
            }
        });
        this.fitScreen();
    }
    /** 数据网格的管理单元格连接点击事件处理.
     * @param dataId 相关的数据ID.
     * @param event 事件名称. */
    private cellLinkEvent(dataId: number, event: string): void {
        switch (event) {
            case "edit":
                break;
            case "delete":
                break;
        }
    }
    /** 自动调整子视图的尺寸，以适应浏览器窗口大小. */
    public fitScreen(): void {
        if (!(this.tableView))
            return;
        let $parent: JQuery<HTMLElement> = $(".module-view").find(".container");
        let th: number = $parent.height();
        th -= $(".module-view").find(".custom-toolsbar").height();
        th -= $(".module-view").find(".pagination-bar").height();
        this.tableView.datagrid('resize', { width: $parent.width(), height: th });
    }
    /** 工具栏按钮事件处理.
     * @param $target 被点击的工具栏按钮. */
    toolstripItemClick($target: JQuery<HTMLElement>): void {
        let eventName: string = $target.attr("event-name");
        switch (eventName) {
            case "add":
                layer.alert("添加功能！");
                break;
            case "search":
                this.search();
                break;
        }
    }
}

window.mainApp.setActiveView(new DataListModuleView());