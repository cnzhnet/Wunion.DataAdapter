/** 表示 webapi 返回的结果的类型接口. */
interface IWebApiResult {
    /** 状态码. */
    code: number;
    /** 错误信息 */
    message: string;
    /** webapi 返回的数据（当 code 为 0 值时该属性有值）. */
    data: any
}
/** 表示应用程序主页面的规范化接口. */
interface IWebApp {
    /** 设置当前活动的子视图.
    * @param module 实现子视图控制的客户端脚本对象. */
    setActiveView(module: IModuleView): void;
    /** 获取当前活动的的子视图控制脚本对象. */
    getActiveView(): IModuleView;
    /** 切换当前的模块视图.
     * @param viewName 视图名称.
     * @param options 该视图可能需要的参数. */
    changeView(viewName: string, options?: any): void;
    /** 用于请求切换数据库.
     * @param kind 数据库类型. */
    changeDatabase(kind: string): void;
    /** 登出(注销)当前登录的用户账户.
     * @param confirm 在注销时是否弹出确认框. */
    userLogout(confirm?: boolean): void;
}
/** 表示应用程序的子视图接口规范. */
interface IModuleView {
    /** 完成子视图的脚本初始化.
     * @param options 初始化时可能需要的参数选项. */
    init(options?: any): void;
    /** 加载/重新加载子视图的数据内容.
     * @param arg 加载子视图数据可能需要的参数. */
    reload(arg?: any): void;
    /** 使子视图的尺寸大小适应整体布局. */
    fitScreen(): void;
    /** 当工具栏上的按钮元素的点击事件处理程序.
     * @param $target 被点击的工具栏按钮元素的 jQuery 对象. */
    toolstripItemClick($target: JQuery<HTMLElement>): void;
}
/** 表示 url 查询参数信息索引信息的类型接口. */
interface IUrlQueryIndexes {
    /** url 查询参数的起始位置索引. */
    start: number;
    /** url 查询参数的结束位置索引. */
    end: number;
}

interface Window {
    /** 表示对主界面视图控制脚本对象的扩展. */
    mainApp: IWebApp;
}

declare var service: ServiceRequest;