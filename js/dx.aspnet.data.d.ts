import CustomStore from "devextreme/data/custom_store";

export type AfterSend = (operation: string, ajaxSettings: JQueryAjaxSettings, jqxhr: JQueryXHR) => void;
export type XhrFinished = (operation: string, ajaxSettings: JQueryAjaxSettings, jqxhr: JQueryXHR) => void;

interface Options {
    key?: string|Array<string>,
    errorHandler?: (e: Error) => void,

    loadUrl?: string,
    loadParams?: Object,
    loadMethod?: string,

    updateUrl?: string,
    updateMethod?: string,

    insertUrl?: string,
    insertMethod?: string,

    deleteUrl?: string,
    deleteMethod?: string,

    onBeforeSend?: (operation: string, ajaxSettings: JQueryAjaxSettings) => void,
    onAfterSend?: AfterSend
    onXhrFinished?: XhrFinished
    onAjaxError?: (e: { xhr: JQueryXHR, error: string | Error }) => void
}

export function createStore(options: Options): CustomStore;
