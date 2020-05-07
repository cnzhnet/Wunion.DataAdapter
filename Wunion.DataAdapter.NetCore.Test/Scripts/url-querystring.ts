/// <reference path="types/type.extension.d.ts" />

/** 用于处理 url 查询参数字符串的类. */
class UrlQueryString {
    private rawUrl: string;
    private params: any;
    private queryString: string;
    /** 构造函数. */
    constructor(url: string) {
        this.rawUrl = url;
        this.params = new Object();
        let position: IUrlQueryIndexes = this.getPosition();
        if (position) {
            this.queryString = url.substring(position.start + 1, position.end);
            let parray: string[] = this.queryString.split("&");
            let key: string = "", val: string = "", first: number = -1;
            for (let i = 0; i < parray.length; ++i) {
                first = parray[i].indexOf("=");
                if (first < 2)
                    continue;
                key = parray[i].substring(0, first);
                val = parray[i].substring(first + 1);
                this.params[key] = val;
            }
        }
    }

    /** 获取查询参数的位置索引信息. */
    private getPosition(): IUrlQueryIndexes {
        let startIndex: number = this.rawUrl.indexOf("?");
        if (startIndex === -1)
            return null;
        let endIndex: number = this.rawUrl.indexOf("#");
        if (endIndex === -1)
            endIndex = this.rawUrl.length;
        return { start: startIndex, end: endIndex };
    }

    /** 获取由查询参数组成的字典对象. */
    public getParams(): any { return this.params; }

    /** 移除指定该 url 的查询参数部分，并返回一个全新的 url 字符串. */
    public remove(): string {
        let position: any = this.getPosition();
        if (!position)
            return this.rawUrl;
        return this.rawUrl.substring(0, position.start);
    }

    /** 返回 url 的查询参数部份. */
    public queryPartial(): string {
        return this.queryString;
    }
}