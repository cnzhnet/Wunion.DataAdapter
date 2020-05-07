/// <reference path="../types/layui/index.d.ts" />
/// <reference path="../types/easyui/datagrid.d.ts" />
/// <reference path="../types/type.extension.d.ts" />

/** 数据更表子视图的客户端控制脚本. */
class DataListModuleView implements IModuleView {
    private $datalistView: JQuery<HTMLElement>;

    /** 创建一个 DataListModuleView 的对象实例. */
    constructor() { }
    /** 初始化客户端脚本对象. */
    public init(options?: any): void {
        let me: DataListModuleView = this;
        this.$datalistView = $("#data-listView");
        $(".custom-toolsbar").find(".tool-button").on("click", function (this: HTMLElement): void {
            me.toolstripItemClick($(this));
        });
        this.reload();
    }
    /** 加载或重新加载数据 . */
    public reload(arg?: any): void {
        this.loadGroupDropdown();
    }    
    /** 自动调整子视图的尺寸，以适应浏览器窗口大小. */
    public fitScreen(): void {
        //
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
            layui.form.on("select(group_dropdown)", function (data: any): void { me.loadData(data.value); });
        });
    }
    /** 用于加载数据. */
    private loadData(group?: any): void {
        if (group)
            layer.alert("分组ID：" + group);
        else
            layer.alert("所有分组！");
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
                layer.alert("搜索功能！");
                break;
        }
    }
}

window.mainApp.setActiveView(new DataListModuleView());