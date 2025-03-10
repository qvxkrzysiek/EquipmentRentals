import {Component, EventEmitter, Input, numberAttribute, Output} from '@angular/core';
import {DialogComponent} from "@progress/kendo-angular-dialog";
import {FormsModule} from "@angular/forms";
import {TextBoxDirective} from "@progress/kendo-angular-inputs";
import {ButtonComponent} from "@progress/kendo-angular-buttons";
import {EquipmentDTO} from "../../../services/equipment.service";

@Component({
    selector: 'app-equipmentcreate',
    imports: [
        DialogComponent,
        FormsModule,
        TextBoxDirective,
        ButtonComponent
    ],
    templateUrl: './equipmentcreate.component.html',
})
export class EquipmentCreateComponent {
    @Output() equipmentCreated: EventEmitter<EquipmentDTO> = new EventEmitter();
    @Output() dialogClosed: EventEmitter<void> = new EventEmitter();
    @Input({transform: numberAttribute}) modelId: number | undefined;

    public newEquipment: EquipmentDTO = { equipmentId: 0, serialNumber: '', modelId: 0 };

    saveEquipment(): void {
        console.log(this.modelId);
        console.log(this.newEquipment);
        if (this.newEquipment.serialNumber) {
            console.log("this.modelId");
            if (this.modelId != null) {
                this.newEquipment.modelId = this.modelId;
            } else {
                return
            }
            this.equipmentCreated.emit(this.newEquipment);
            this.newEquipment = { equipmentId: 0, serialNumber: '', modelId: 0 };
        }
    }

    cancel(): void {
        this.dialogClosed.emit();
    }
}
