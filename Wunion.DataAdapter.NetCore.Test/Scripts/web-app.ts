/// <reference path="types/layui/index.d.ts" />
/// <reference path="types/type.extension.d.ts" />

/** The main client control script of this web application.
 * 当前 Web 应用程序的主客户端控制脚本. */
class WebApp implements IWebApp {
    private currModule: IModuleView;

    /** 创建一个 WebApp 的对象实例. */
    constructor() {
        this.currModule = null;
        $(window).unbind("resize");
        $(window).on("resize", function (this: Window & typeof globalThis): void {
            if (!(window.mainApp))
                return;
            if (window.mainApp.getActiveView())
                window.mainApp.getActiveView().fitScreen();
        });
        $(document).ready(() => { this.init(); });
    }
    /** 初始化客户端脚本. */
    private init(): void {
        layer.alert("running init!");
    }
    public setActiveView(module: IModuleView): void { this.currModule = module; }
    public getActiveView(): IModuleView { return this.currModule; }
    public changeView(viewName: string, options?: any): void {
        throw new Error("Method not implemented.");
    }
    public userLogout(confirm?: boolean): void {
        throw new Error("Method not implemented.");
    }
}

if (!(window.mainApp))
    window.mainApp = new WebApp();