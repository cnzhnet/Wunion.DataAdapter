// 显示添加分组对话框。
About.ShowAddGroup = function () {
    $("#dialog-title").html("添加分组");
    $("#dialog-content").html('<div class="row"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px">组名称：</div>' +
        '<div class="col-xs-4 col-sm-6" style="width: auto;"><input id="GrpName" type="text" class="form-control" style="width: 235px" /></div></div>');

    var $myModal = $("#modalDialog");
    $myModal.find(".modal-dialog").css("width", "360px");
    $myModal.find("#modalbtn-ok").unbind("click");
    $myModal.find("#modalbtn-ok").bind("click", function () {
        About.AddNewGroup();
        $myModal.modal("hide");
    });
    $myModal.modal({ backdrop: 'static', keyboard: false });    
    $myModal.modal("show");
};
// 提交添加分组的动作。
About.AddNewGroup = function () {
    if ($("#GrpName").val() == undefined || $("#GrpName").val() == null || $("#GrpName").val() == "")
        return;
    $.post("/Test/AddNewGroup", { Name: $("#GrpName").val() }, function (result) {
        if (result.resultCode == 0x0)
            About.GetAllGroup(); /* 在 wwwroot/About.js 中 */
        else
            alert(result.message);
    });
};
// 显示添加数据对话框。
About.ShowAddData = function () {
    $("#dialog-title").html("添加数据");
    var ui = '<form id="add-form" method="post" enctype="multipart/form-data">' +
        '<input id="GroupId" type="hidden" value="' + $("#select-group").val() + '" />' +
        '<div class="row" style="padding-top: 8px;"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px">名称：</div>' +
        '<div class="col-xs-4 col-sm-6" style="width: auto;"><input id="TestName" type="text" class="form-control" style="width: 370px" /></div></div>' +
        '<div class="row" style="padding-top: 8px;"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px">年龄：</div>' +
        '<div class="col-xs-4 col-sm-6" style="width: auto;"><input id="TestAge" type="text" class="form-control" style="width: 370px" /></div></div>' +
        '<div class="row" style="padding-top: 8px;"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px">性别：</div>' +
        '<div class="col-xs-4 col-sm-6" style="width: auto;"><select id="TestSex" type="text" class="form-control" style="width: 370px">' +
        '<option value="未知">未知</option><option value="男">男</option><option value="女">女</option></select></div></div>' +
        '<div class="row" style="padding-top: 8px;"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px">照片：</div>' +
        '<div class="col-xs-4 col-sm-6" style="width: auto;"><input id="Photo" type="file" name="Photo" class="form-control" style="width: 370px" /></div></div>' +
        '<div class="row" style="padding-top: 8px; padding-left: 8px;" id="progress-msg"></div>'
        '</form>';
    $("#dialog-content").html(ui);
    var $myModal = $("#modalDialog");
    $myModal.find(".modal-dialog").css("width", "480px");
    $myModal.find("#modalbtn-ok").unbind("click");
    $myModal.find("#modalbtn-ok").bind("click", About.SubmitData);
    $myModal.modal({ backdrop: 'static', keyboard: false });
    $myModal.modal("show");
};
// 提交添加的数据内容。
About.SubmitData = function () {
    var formData = About.GetFormData();
    if (formData == null) // 表单验证不通过时 GetFormData() 方法会返回 null
        return;
    $("#modal-close").attr("disabled", true);
    $("#modal-cancel").attr("disabled", true);
    $("#modalbtn-ok").attr("disabled", true);
    $("#progress-msg").html("正在提交数据，请稍后......");
    // 以下代码修改表单的提交动作。
    $("#add-form").submit(function (event) {
        event.preventDefault();        
        $.ajax({
            type: "POST",
            url: "/Test/AddTestData",
            contentType: false, // 关键设置：当此处设置这 "multipart/form-data" 时，并不会为服务端代码生成完整的请求信息帮而导致服务端代码异常。
            processData: false, // 关键设置：此选项若不禁用则表示由jQuery自动处理表单值，这种情况将导致服务端收不到任何数据。
            data: formData,
            success: function (result) {
                if (result.resultCode == 0x0) {
                    $("#modalDialog").modal("hide");
                    About.GetGroupData($("#select-group").val()); /* 在 wwwroot/About.js 中 */
                }
                else {
                    $("#progress-msg").html(result.message);
                }
                $("#modal-close").attr("disabled", false);
                $("#modal-cancel").attr("disabled", false);
                $("#modalbtn-ok").attr("disabled", false);
            },
            error: function (msg) {
                $("#progress-msg").html("数据提交过程失败。");
            }
        });
        return false;
    });    
    $("#add-form").submit();
};

// 获取并验证表单数据
About.GetFormData = function () {
    var pData = new FormData(); // 将表单组织为 FormData 对象方能实现上传的同时提交数据。
    var fileUpload = $("#Photo").get(0);
    if ($("#GroupId").val() == null || $("#GroupId").val() == "") {
        $("#GroupId").focus();
        return null;
    }
    pData.append("GroupId", $("#GroupId").val());
    if ($("#TestName").val() == null || $("#TestName").val() == "") {
        $("#TestName").focus();
        return null;
    }
    pData.append("TestName", $("#TestName").val());
    if ($("#TestAge").val() == null || $("#TestAge").val() == "") {
        $("#TestAge").focus();
        return null;
    }
    pData.append("TestAge", $("#TestAge").val());
    if ($("#TestSex").val() == null || $("#TestSex").val() == "") {
        $("#TestSex").focus();
        return null;
    }
    pData.append("TestSex", $("#TestSex").val());
    for (var i = 0; i < fileUpload.files.length; ++i) {
        pData.append(fileUpload.files[i].name, fileUpload.files[i]);
    }
    return pData;
};
// 删除选定的数据。
About.DeleteData = function () {
    var filterData = $("#datagrid-panel").find("tbody").find("input:checkbox:checked").map(function () {
        return $(this).val();
    }).get().join(";");
    if (filterData == undefined || filterData == null || filterData == "")
        return;
    if (!confirm("确定要删除选择的数据吗？"))
        return;
    $.post("/Test/DeleteData", { filter: filterData }, function (result) {
        if (result.resultCode == 0x0)
            About.GetGroupData($("#select-group").val()); /* 在 wwwroot/About.js 中 */
        else
            alert(result.message);
    });
};