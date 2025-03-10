import { Component, Input, OnInit } from '@angular/core';
import { EquipmentDTO } from "../../../services/equipment.service";
import { EquipmentService } from "../../../services/equipment.service";
import {FormsModule} from "@angular/forms";
import {ButtonComponent} from "@progress/kendo-angular-buttons";
import {
    CellTemplateDirective,
    ColumnComponent,
    DataBindingDirective,
    GridComponent
} from "@progress/kendo-angular-grid";
import {NgIf} from "@angular/common";
import {TextBoxDirective} from "@progress/kendo-angular-inputs";
import {skip} from "rxjs";
import {NotifyService} from "../../../services/notify.service";
import {EquipmentCreateComponent} from "../../dialog/equipmentcreate/equipmentcreate.component";

@Component({
  selector: 'app-table-equipment',
    imports: [
        FormsModule,
        ButtonComponent,
        CellTemplateDirective,
        ColumnComponent,
        NgIf,
        GridComponent,
        TextBoxDirective,
        DataBindingDirective,
        EquipmentCreateComponent,
        EquipmentCreateComponent
    ],
  templateUrl: './equipments.component.html'
})
export class EquipmentListComponent implements OnInit {
  @Input() modelId: number | null = null;
  public equipmentList: EquipmentDTO[] = [];
  public editedEquipmentId: number | null = null;
  public newEquipment: EquipmentDTO = { equipmentId: 0, serialNumber: '', modelId: 0};
  public showAddModelDialog: boolean = false;

  constructor(private equipmentService: EquipmentService, private notify: NotifyService) { }

  ngOnInit(): void {
    if (this.modelId) {
      this.loadData();
    }
  }

  loadData(): void {
    this.equipmentService.getEquipmentByModelId(this.modelId).subscribe(data => {
      this.equipmentList = data;
    });
  }

  editEquipmentHandler(dataItem: EquipmentDTO): void {
    this.editedEquipmentId = dataItem.equipmentId;
    this.newEquipment = { ...dataItem };
  }

  saveEquipmentHandler(): void {
    if (!this.newEquipment.equipmentId) return;

    this.equipmentService.updateEquipment(this.newEquipment).subscribe(
        () => {
          this.loadData();
            this.notify.showNotification('success', `Sprzęt nr${this.editedEquipmentId} został zmieniony`);
          this.editedEquipmentId = null;
        },
        error => {
            this.notify.showNotification('error', error.error);
        }
    );
  }

  cancelEquipmentHandler(): void {
    this.editedEquipmentId = null;
  }

  removeEquipmentHandler(equipmentId: number): void {
    this.equipmentService.deleteEquipment(equipmentId).subscribe(
        () => {
          this.equipmentList = this.equipmentList.filter(item => item.equipmentId !== equipmentId);
            this.notify.showNotification('success', `Sprzęt nr${equipmentId} został usunięty`);
        },
        error => {
            this.notify.showNotification('error', error.error);
        }
    );
  }

    openAddModelDialog(): void {
        this.showAddModelDialog = true;
    }

    closeAddModelDialog(): void {
        this.showAddModelDialog = false;
    }

    addNewModel(newEquipment: EquipmentDTO): void {
        this.equipmentService.createEquipment(newEquipment).subscribe(
            () => {
                this.loadData();
                this.closeAddModelDialog();
                this.notify.showNotification('success', 'Nowy sprzęt został utworzony!');
            },
            error => {
                this.notify.showNotification('error', error.error);
            }
        );
    }
}
