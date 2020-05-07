/// <reference path="types/layui/index.d.ts" />
/// <reference path="types/type.extension.d.ts" />
/// <reference path="url-querystring.ts" />

/** The main client control script of this web application.
 * 当前 Web 应用程序的主客户端控制脚本. */
class WebApp implements IWebApp {
    private currModule: IModuleView;
    private focusedNavi: HTMLElement;
    private $moduleView: JQuery<HTMLElement>;

    /** 创建一个 WebApp 的对象实例. */
    constructor() {
        this.currModule = null;
        this.$moduleView = null;
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
        let me: WebApp = this;
        let $navi: JQuery<HTMLElement> = $(".top-navbar").find("a");
        $navi.each(function (this: HTMLElement, index: number, elem: HTMLElement): void {
            let $target: JQuery<HTMLElement> = $(this);
            if ($target.hasClass("item-active"))
                me.focusedNavi = this;
            $target.on("click", function (this: HTMLElement): void {
                me.topNavbarItem_Click(this);
            });
        });
        this.$moduleView = $(".module-view");
        if (this.focusedNavi)
            this.topNavbarItem_Click(this.focusedNavi);
    }
    public setActiveView(module: IModuleView): void { this.currModule = module; }
    public getActiveView(): IModuleView { return this.currModule; }
    /** 切换当前的模块子视图.
     * @param viewName 模块子视图的 url（可包含查询参数）. */
    public changeView(viewName: string, options?: any): void {
        if (!viewName)
            return;
        let me: WebApp = this;
        let linkElem: HTMLLinkElement = options as HTMLLinkElement;
        let urlQuery: UrlQueryString = new UrlQueryString(viewName);
        let requestUrl: string = "/moduleView/Get";
        if (urlQuery.queryPartial())
            requestUrl += "?" + urlQuery.queryPartial();
        $.post(requestUrl, { name: urlQuery.remove() }, function (result: any): void {
            me.setActiveView(null);
            me.$moduleView.html(result as string);          
            if (linkElem) {
                if (me.focusedNavi)
                    $(me.focusedNavi).removeClass("item-active");
                $(linkElem).addClass("item-active");
                me.focusedNavi = linkElem;
            }
            if (me.getActiveView())
                me.getActiveView().init(urlQuery.getParams());
        });
    }
    /** 用于请求切换数据库.
     * @param kind 数据库类型. */
    public changeDatabase(kind: string): void {
        if (!kind)
            return;
        let me: WebApp = this;
        let index: number = layer.load();
        service.post("/api/data/ChangeDataBase", { kind: kind }, function (result: IWebApiResult): void {
            layer.close(index);
            if (result.code === 0x00) {
                if (me.getActiveView())
                    me.getActiveView().reload();
            }
            else {
                layer.alert(result.message, { icon: 2, title: "错误信息" });
            }
        });
    }
    /** 用于处理顶部导航元素的点击事件.
     * @param target 被点击的导航元素. */
    private topNavbarItem_Click(target: HTMLElement): void {
        let $target: JQuery<HTMLElement> = $(target);
        this.changeView($target.attr("data-view"), target);
    }

    public userLogout(confirm?: boolean): void {
        throw new Error("Method not implemented.");
    }
}

if (!(window.mainApp))
    window.mainApp = new WebApp();