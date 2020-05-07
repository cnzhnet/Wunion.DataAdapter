/// <reference types="jquery" />
/// <reference path="types/type.extension.d.ts" />

/** 用于请求服务端 webapi 的对象类型. */
class ServiceRequest {
    private license: string;

    constructor() {
        this.license = "";
    }

    /** 设置请求服务端 webapi 所需的授权信息.
     * @param value 授权信息的文本内空. */
    public setLicense(value: string): void {
        this.license = value;
    }

    /** 以 GET 方式请求指定的webapi（自动包含授权信息）.
     * @param url webapi的地址.
     * @param callback 调用完成后的回调函数. */
    public get(url: string, callback: (result: IWebApiResult) => void): void {
        let settings: any = {
            url: url, type: "GET",
            error: function (jqXHR: JQuery.jqXHR, textStatus: string, errorThrown: string): void {
                if ((typeof callback) === "function")
                    callback({ code: -1, message: "请求指定的服务时产生错误：" + (jqXHR.statusText || jqXHR.statusCode), data: null
            });
            },
            success: function (data: object, textStatus: string, jqXHR: JQuery.jqXHR): void {
                if ((typeof callback) === "function")
                    callback(data as IWebApiResult);
            }
        };
        if (this.license)
            settings.headers = { "license": this.license };
        $.ajax(settings);
    }
    /** 以 post form-data 方式向指定的 webapi 发送请求.
     * @param url webapi的地址.
     * @param data 包含要提交的数据的对象.
     * @param callback 回调函数. */
    public post(url: string, data: object, callback: (result: IWebApiResult) => void): void {
        let settings: any = {
            url: url, type: "POST", data: data,
            error: function (jqXHR: JQuery.jqXHR, textStatus: string, errorThrown: string): void {
                if ((typeof callback) === "function")
                    callback({ code: -1, message: "请求指定的服务时产生错误：" + (jqXHR.statusText || jqXHR.statusCode), data: null });
            },
            success: function (data: object, textStatus: string, jqXHR: JQuery.jqXHR): void {
                if ((typeof callback) === "function")
                    callback(data as IWebApiResult);
            }
        };
        if (this.license)
            settings.headers = { "license": this.license };
        $.ajax(settings);
    }
    /** 以 post 方式向服务器提交 JSON 格式的数据（注：该方法将 JSON 数据写入到 httpRequest 的 body 流，而非传统的键值对形式）.
     * @param url 服务器端 webapi 的地址.
     * @param body 要提交的对象，该对象将以 JSON 格式被写入请求的 body 流中.
     * @param callback 提交完成后的回调函数. */
    public postJson(url: string, body: object, callback: (result: IWebApiResult) => void): void {
        let settings: any = {
            url: url, type: "POST", cache: false, contentType: "application/json",
            processData: false, data: JSON.stringify(body),
            error: function (jqXHR: JQuery.jqXHR, textStatus: string, errorThrown: string): void {
                if ((typeof callback) === "function")
                    callback({ code: -1, message: "请求指定的服务时产生错误：" + (jqXHR.statusText || jqXHR.statusCode), data: null });
            },
            success: function (data: object, textStatus: string, jqXHR: object): void {
                if ((typeof callback) === "function")
                    callback(data as IWebApiResult);
            }
        };
        if (this.license)
            settings.headers = { "license": this.license };
        $.ajax(settings);
    }
    /** 以 multipart/form-data 形式向服务器提交表单数据.
     * @param url 表示webapi地址.
     * @param data 要提交的数据.
     * @param callback 回调函数. */
    public postMultipart(url: string, data: FormData, callback: (result: IWebApiResult) => void): void {
        let settings: any = {
            url: url, type: "POST", contentType: false, processData: false, data: data,
            error: function (jqXHR: JQuery.jqXHR, textStatus: string, errorThrown: string): void {
                if ((typeof callback) === "function")
                    callback({ code: -1, message: "请求指定的服务时产生错误：" + (jqXHR.statusText || jqXHR.statusCode), data: null });
            },
            success: function (data: object, textStatus: string, jqXHR: JQuery.jqXHR): void {
                if ((typeof callback) === "function")
                    callback(data as IWebApiResult);
            }
        };
        if (this.license)
            settings.headers = { "license": this.license };
        $.ajax(settings);
    }
}

service = new ServiceRequest();