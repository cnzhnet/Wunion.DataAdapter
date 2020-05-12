/// <reference path="../types/layui/index.d.ts" />
/// <reference path="../types/type.extension.d.ts" />

/** 分组列表子视图的客户端控制脚本. */
class GroupListView implements IModuleView {
    /** 创建一个 GroupListView 的对象实例. */
    constructor() { }
    /** 初始化客户端脚本对象. */
    public init(options?: any): void {
        let me: GroupListView = this;
        layui.form.render("select", "group_listView");
        layui.form.on("select(ch_database)", function (data: any): void {
            window.mainApp.changeDatabase(data.value);
        });
        $(".custom-toolsbar").find(".tool-button").on("click", function (this: HTMLElement): void {
            me.toolstripItemClick($(this));
        });
        this.reload();
    }
    /** 加载/重新加载数据. */
    public reload(arg?: any): void {
        let me: GroupListView = this;
        let index: number = layer.load();
        service.get("/api/group/Query", function (result: IWebApiResult): void {
            layer.close(index);
            let $groupList: JQuery<HTMLElement> = $(".group-list-view");
            if (result.code !== 0x00) {
                $groupList.html('<li class="last-item">' + result.message + '</li>');
                return;
            }
            let items: any[] = result.data as any[];
            if (!items) {
                $groupList.html('<li class="last-item">目前尚未创建分组信息.</li>');
                return;
            }
            if (items.length < 1) {
                $groupList.html('<li class="last-item">目前尚未创建分组信息.</li>');
                return;
            }
            $groupList.html("");
            $.each<any>(items, function (this: any, index: number, value: any): void {
                let $li: JQuery<HTMLElement> = $('<li><h2>' + this.GroupName + '</h2></li>');
                let $toolbox: JQuery<HTMLElement> = $('<div class="item-toolbox"></div>');
                $toolbox.append($('<a href="javascript:;" data-id="' + this.GroupId + '" event-name="delete">删除<i class="layui-icon">&#xe640;</i></a>'));
                if (index == (items.length - 1))
                    $li.addClass("last-item");
                $li.append($toolbox);
                $groupList.append($li);
                $toolbox.find("a").on("click", function (this: HTMLElement): void {
                    let $target: JQuery<HTMLElement> = $(this);
                    let grpId: number = parseInt($target.attr("data-id"));
                    switch ($target.attr("event-name")) {
                        case "delete":
                            me.delete(grpId);
                            break;
                    }
                });
            });
        });
    }
    /** 删除指定的分组信息. */
    private delete(gid: number): void {
        let me: GroupListView = this;
        layer.confirm("是否要删除此分组？", { title: '<strong>分组删除确认</strong>' },
            function (index: number, layero: JQuery<HTMLElement>): void {
                layer.close(index);
                service.get("/api/group/" + gid + "/delete", function (result: IWebApiResult): void {
                    if (result.code === 0x00)
                        me.reload();
                    else
                        layer.alert(result.message, { icon: 2, title: "错误信息" });
                });
            }
        );
    }
    /** 未使用的方法. */
    public fitScreen(): void { }
    /** 显示分组添加对话框. */
    private showAddDialog(): void {
        let ui: string = '<div style="margin: 0px; padding: 12px;"><h4 style="margin-bottom: 8px;">请输入分组名称：</h4>';
        ui += '<div><input type="text" class="layui-input" style="width: 210px;" id="group-name" autocomplete="off" />';
        ui += '</div></div>';
        let me: GroupListView = this;
        layer.open({
            type: 1, title: '<strong>添加分组对话框</strong>', btn: ["确认", "取消"], content: ui,
            yes: function (index: number, layero: JQuery<HTMLElement>): void {
                layer.close(index);
                let loading: number = layer.load();
                service.post("/api/group/add", { name: layero.find("#group-name").val() }, function (result: IWebApiResult): void {
                    layer.close(loading);
                    if (result.code === 0x00)
                        me.reload();
                    else
                        layer.alert(result.message, { icon: 2, title: "错误信息" });
                });
            }
        });
    }
    /** 工具栏按钮事件处理程序.
     * @param $target 被点击的工具栏按钮. */
    public toolstripItemClick($target: JQuery<HTMLElement>): void {
        switch ($target.attr("event-name")) {
            case "add":
                this.showAddDialog();
                break;
        }
    }
}

window.mainApp.setActiveView(new GroupListView());