import {Injectable, TemplateRef, ViewChild} from '@angular/core';
import {NotificationService, NotificationSettings} from "@progress/kendo-angular-notification";

@Injectable({
    providedIn: 'root'
})
export class NotifyService {

    @ViewChild("notification", { read: TemplateRef })
    public notificationTemplate: TemplateRef<any> | undefined;

    public state: NotificationSettings = {
        content: "Your data has been saved.",
        type: { style: "success", icon: true },
        animation: { type: "slide", duration: 400 },
        hideAfter: 3000,
        cssClass: "custom-notification"
    };

    constructor(private notificationService: NotificationService) { }

    public showNotification(type: any, text: string): void {
        switch (type) {
            case "success":
                this.state.content = text;
                this.state.type = { style: "success", icon: true };
                break;
            case "error":
                this.state.content = text;
                this.state.type = { style: "error", icon: true };
                break;
            case "warning":
                this.state.content = text;
                this.state.type = { style: "warning", icon: true };
                break;
            case "info":
                this.state.content = text;
                this.state.type = { style: "info", icon: true };
                break;
            case "default":
                this.state.content = text;
                this.state.type = { style: "none", icon: true };
                break;
        }
        this.notificationService.show(this.state);
    }
}
