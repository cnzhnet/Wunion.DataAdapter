/// <reference path="../types/layui/index.d.ts" />
/// <reference path="../types/crypto-js/index.d.ts" />
/// <reference path="../types/type.extension.d.ts" />

/** 数据编辑器子视图的客户端控制脚本. */
class DataEditorView implements IModuleView {
    private entity: any;

    /** 创建一个 DataEditorView 的对象实例. */
    constructor() {
        this.entity = null;
    }
    /** 初始化客户端脚本.
     * @param options 初始化客户端脚本可能需要的参数. */
    public init(options?: any): void {
        let me: DataEditorView = this;
        $(".custom-toolsbar").find("button").on("click", function (this: HTMLElement): void {
            me.toolstripItemClick($(this));
        });
        let context: string = $("#data-entity").val() as string;
        if (context) {
            let words: any = CryptoJS.enc.Base64.parse(context);
            context = CryptoJS.enc.Utf8.stringify(words);
            this.entity = $.parseJSON(context);
        }
        if (this.entity)
            $("#tool-accept").html('<i class="layui-icon">&#xe605;</i>提交数据更新');
        else
            $("#tool-accept").html('<i class="layui-icon">&#xe605;</i>提交新增记录');
        this.reload();        
    }
    /** 清空/重置数据. */
    public reload(arg?: any): void {
        let isEdit: boolean = (this.entity !== undefined && this.entity !== null);
        let me: DataEditorView = this;
        $(".data-editor").find("input").each(function (this: HTMLElement, index: number, elem: HTMLElement): void {
            let $target: JQuery<HTMLElement> = $(this);
            if ($target.attr("type").toLowerCase() === "hidden")
                return;
            let propName: string = $target.attr("name");
            if (!propName)
                return;
            if (isEdit)
                $target.val(me.entity[propName]);
            else
                $target.val('');
        });
        $("#group-id").val(isEdit ? me.entity.GroupId : '');
        $("#test-sex").val(isEdit ? me.entity.TestSex : '');
        let $seePicture: JQuery<HTMLElement> = $("#see-picture");
        if (isEdit) {
            let version: number = Math.floor(Math.random() * 1000000);
            $seePicture.attr("href", "/api/data/" + this.entity.TestId + "/picture?v=" + version + "");
            $seePicture.attr("target", "_blank");
        }
        else {
            $seePicture.attr("href", "javascript:;");
            $seePicture.removeAttr("target");
        }
        layui.form.render("select", "data_editor");
    }
    
    public toolstripItemClick($target: JQuery<HTMLElement>): void {
        switch ($target.attr("event-name")) {
            case "goback":
                window.mainApp.changeView("Shared/DataList");
                break;
            case "accept":
                break;
            case "reset":
                this.reload();
                break;
        }
    }
    /** 未使用的指口方法. */
    public fitScreen(): void { }
}

window.mainApp.setActiveView(new DataEditorView());