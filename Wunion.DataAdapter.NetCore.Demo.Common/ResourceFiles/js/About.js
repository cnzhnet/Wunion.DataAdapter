// About.js
var About = new Object();
About.CurrentDataBase = "Default";
About.CurrentPage = 1;
About.PageSize = 10;

// 初始化。
About.Initialize = function () {
    $(".navbar").find("a").each(function () {
        $(this).on("click", function () {
            About.ToolbarItemClick(this);
        });
    });
    $("#select-group").on("change", function () {
        About.GetGroupData($("#select-group").val());
    });

    About.GetAllGroup();
};

// 工具栏点击事件处理
About.ToolbarItemClick = function (e) {
    var BtnId = $(e).attr("href");
    switch (BtnId) {
        case "#db-sqlite3":
        case "#db-mssql": 
        case "#db-mysql":
        case "#db-npgsql":
            About.ChangeDataBase(e);
            break;
        case "#add-group":
            About.ShowAddGroup(); /* 在 wwwroot/js/About.Change.js 中 */
            break;
        case "#add-data":
            About.ShowAddData(); /* 在 wwwroot/js/About.Change.js 中 */
            break;
        case "#remove-data":
            About.DeleteData(); /* 在 wwwroot/js/About.Change.js 中 */
            break;
    }
};

// 切换数据库
About.ChangeDataBase = function (e) {
    $.post("/Test/ChangeDataEngine", { key: $(e).attr("data-value") }, function (result) {
        if (result.resultCode == 0x0) {
            About.CurrentDataBase = $(e).attr("data-value");
            $("#curr-db").html("数据库：" + $(e).html());
            About.GetAllGroup();
        }
        else {
            alert(result.message);
        }
    });
}

// 获取所有分组。
About.GetAllGroup = function () {
    $("#select-group").empty();
    $.post("/Test/GetAllGroup", function (result) {
        if (result.resultCode == 0x0) {
            var words = CryptoJS.enc.Base64.parse(result.data);
            var data = $.parseJSON(CryptoJS.enc.Utf8.stringify(words));
            for (var i = 0; i < data.length; ++i) {
                $("#select-group").append($('<option value="' + data[i].GroupId + '">' + data[i].GroupName + '</option>')); 
            }
            About.GetGroupData(data[0].GroupId);
        }
        //else
        //{
        //    alert(result.message);
        //}
    });
};

// 获取指定分组下的数据。
About.GetGroupData = function (groupId) {
    var param = new Object();
    param.pageSize = About.PageSize;
    param.page = About.CurrentPage;
    param.GID = groupId;
    $("#datagrid-panel").html("");
    $.post("/Test/GetDataByGroup", param, function (result) {
        if (result.resultCode == 0x0) {
            // 对数据进行 base64 解码。
            var words = CryptoJS.enc.Base64.parse(result.data);
            var data = $.parseJSON(CryptoJS.enc.Utf8.stringify(words)); 
            // 创建 bootstrap-table 并将数据绑定到其上。
            $("#datagrid-panel").html('<div id="data-grid"></div>');
            $("#data-grid").bootstrapTable({
                striped: true, sortStable: false, pagination: true, paginationLoop: false, classes: "table table-no-bordered table-hover",
                pageNumber: About.CurrentPage, pageSize: About.PageSize, pageList: [3, 5, 10, 15],
                onPageChange: About.OnPageChange, sidePagination: "server", cache: false, idField: "TestId", 
                columns: [[
                    { checkbox: true, field: "", title: "选择", align: "left", halign: "center", width: "30px" },
                    { field: "TestId", title: "编号", align: "left", halign: "center", width: "60px" },
                    { field: "GroupName", title: "分组", align: "left", halign: "center", width: "120px" },
                    {
                        field: "TestName", title: "名称", align: "left", halign: "center", width: "320px", formatter: function (value, row, index) {
                            return '<a href="javascript:;" onclick="About.ShowDetail(' + row.TestId + ')">' + value + '</a>';
                        }
                    },
                    {
                        field: "TestAge", title: "年龄", align: "center", halign: "center", width: "80px", formatter: function (value) {
                            return '<span style="color: #006ac3;"><i class="fa fa-jpy" aria-hidden="true" style="margin-right: 3px;"></i>' + value + '</span>';
                        }
                    },
                    { field: "TestSex", title: "姓别", align: "center", halign: "center", width: "80px" }                    
                ]]
            });
            $("#data-grid").bootstrapTable("load", data); // 加载数据。
        }
        else {
            $("#datagrid-panel").html(result.message);
        }
    });
};
// bootstrap-table 分页事件处理。
About.OnPageChange = function (number, size) {
    About.CurrentPage = number;
    About.PageSize = size;
    About.GetGroupData($("#select-group").val());
};
// 显示详细信息对话框。
About.ShowDetail = function (dataId) {
    $.get("/Test/GetDetails/" + dataId, function (result) {
        if (result.resultCode == 0x0) {
            // 对数据进行 base64 解码。
            var words = CryptoJS.enc.Base64.parse(result.data);
            var data = $.parseJSON(CryptoJS.enc.Utf8.stringify(words)); 

            // 设置对话框内容。
            var photoUrl = "/Test/Photo/" + data.data_id + "?Rnd=" + Math.random();
            $("#dialog-title").html("详细信息");
            $("#dialog-content").html('<div class="row"><div class="col-xs-8 col-sm-6" style="width: auto; padding-top: 6px"><ul style="list-style: none; width: 175px; margin: 0px; padding: 0px;">' +
                '<li>编码：<strong>' + data.data_id + '</strong></li>' +
                '<li>名称：' + data.name + '</li>' +
                '<li>年龄：' + data.age + '</li>' +
                '<li>性别：' + data.sex + '</li>' + '</ul></div>' +
                '<div class="col-xs-4 col-sm-6" style="width: auto;"><a href="' + photoUrl + '" target="_blank"><img alt="照片" src="' + photoUrl + '" style="width: 240px; height: 320px;" /></a></div></div>');

            var $myModal = $("#modalDialog");
            $myModal.find(".modal-dialog").css("width", "480px");
            $myModal.find("#modalbtn-ok").unbind("click");
            $myModal.find("#modalbtn-ok").bind("click", function () {
                $myModal.modal("hide");
            });
            $myModal.modal({ keyboard: false });
            $myModal.modal("show");
        }
        else {
            alert(result.message);
        }
    });
};

$(document).unbind("ready", About.Initialize);
$(document).bind("ready", About.Initialize);