var DataListModuleView = (function () {
    function DataListModuleView() {
    }
    DataListModuleView.prototype.init = function (options) {
        this.$datalistView = $("#data-listView");
        this.$datalistView.html('数据列表 田田田');
    };
    DataListModuleView.prototype.reload = function (arg) {
    };
    DataListModuleView.prototype.fitScreen = function () {
    };
    DataListModuleView.prototype.toolstripItemClick = function ($target) {
    };
    return DataListModuleView;
}());
window.mainApp.setActiveView(new DataListModuleView());
//# sourceMappingURL=data-listview.js.map