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
        $("#upload-file").on("change", function (event: JQuery.ChangeEvent<HTMLElement, undefined, HTMLElement, HTMLElement>): void {
            let input: HTMLInputElement = <HTMLInputElement>event.target;
            if (input.files.length < 1)
                return;
            let $showFile: JQuery<HTMLElement> = $(".photo-toolbox").find(".show-file");
            if ($showFile.length < 1) {
                $showFile = $('<li class="show-file"></li>');
                $(".photo-toolbox").append($showFile);
            }
            $showFile.html('[选定的本地照片]：' + input.files.item(0).name + '<a class="remove-lnk" href="javascript:;"><i class="layui-icon">&#x1006;</i></a>');
            $showFile.find(".remove-lnk").on("click", function (this: HTMLElement): void {
                $("#upload-file").val('');
                let xxx: HTMLInputElement = $("#upload-file").get(0) as HTMLInputElement;
                $showFile.remove();
                console.clear();
                console.log(xxx.files);
            });
        });
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
        $("#upload-file").val('');
        $(".photo-toolbox").find(".show-file").remove();
        layui.form.render("select", "data_editor");
    }
    /** 获取编辑的实体对象. */
    private getEditObject(): object {
        let obj: any = new Object();
        let target: HTMLElement, $target: JQuery<HTMLElement>;
        let propName: string, value: string;
        let inputs: JQuery<HTMLElement> = $("input,select");
        for (let i = 0; i < inputs.length; ++i) {
            target = inputs.get(i);
            $target = $(target);
            propName = $target.attr("name");
            if (!propName)
                continue;
            value = $target.val() as string;
            if (value) {
                switch (propName) {
                    case "GroupId":
                        obj[propName] = parseInt(value);
                        break;
                    case "TestAge":
                        obj[propName] = parseFloat(value);
                        break;
                    default:
                        obj[propName] = value;
                        break;
                }
            }
            else {
                if (target.hasAttribute("required")) {
                    layer.alert($target.attr("data-prompt") || "数据有效性验证失败，请检查必填项.");
                    return null;
                }
            }
        }
        return obj;
    }
    /** 用于提交数据. */
    private accept(): void {
        let entity: any = this.getEditObject();
        if (!entity)
            return;
        let me: DataEditorView = this;
        let serverUrl: string = !this.entity ? "/api/data/add" : "/api/data/" + this.entity.TestId + "/update";
        let uploadFile: HTMLInputElement = <HTMLInputElement>$("#upload-file").get(0);
        let loading: number = layer.load();
        let callback: (result: IWebApiResult) => void = (result) => {
            layer.close(loading);
            if (result.code === 0x00) {
                if (me.entity) {
                    layer.alert("已成功更新数据！", { title: "提示信息" }, function (index: number, layero: JQuery<HTMLElement>): void {
                        layer.close(index);
                        window.mainApp.changeView("Shared/DataList");
                    });
                }
                else {
                    layer.confirm("已成功添加数据，请选择接下来的操作！", { title: "提示信息", btn: ["继续添加", "返回列表"] },
                        function (index: number, layero: JQuery<HTMLElement>): void { // 继续添加
                            layer.close(index);
                            me.reload();
                        },
                        function (incex: number, layero: JQuery<HTMLElement>): void { window.mainApp.changeView("Shared/DataList"); }
                    );
                }
            }
            else {
                layer.alert(result.message, { icon: 2, title: "错误信息" });
            }
        };
        if (uploadFile.files.length > 0) {
            let data: FormData = new FormData();
            for (let prop in entity)
                data.append(prop, entity[prop]);
            data.append("TestPhoto", uploadFile.files.item(0));            
            service.postMultipart(serverUrl, data, callback);
        }
        else {
            service.postJson(serverUrl, entity, callback);
        }
    }
    /** 工具栏按钮事件处理程序. */
    public toolstripItemClick($target: JQuery<HTMLElement>): void {
        switch ($target.attr("event-name")) {
            case "goback":
                window.mainApp.changeView("Shared/DataList");
                break;
            case "accept":
                this.accept();
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