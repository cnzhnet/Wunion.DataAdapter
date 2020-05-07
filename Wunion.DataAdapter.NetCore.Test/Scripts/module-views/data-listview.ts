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
        layui.form.render("select", "dlist_view");
        this.$datalistView = $("#data-listView");
        this.$datalistView.html('数据列表 田田田');
    }
    /** 加载或重新加载数据 . */
    public reload(arg?: any): void {
        //
    }
    /** 自动调整子视图的尺寸，以适应浏览器窗口大小. */
    fitScreen(): void {
        //
    }
    toolstripItemClick($target: JQuery<HTMLElement>): void {
        //
    }
}

window.mainApp.setActiveView(new DataListModuleView());