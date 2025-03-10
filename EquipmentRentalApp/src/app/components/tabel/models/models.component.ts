import {Component, OnInit} from '@angular/core';
import { ModelDTO, ModelsService } from "../../../services/models.service";
import { FormsModule } from "@angular/forms";
import {
  CellTemplateDirective,
  ColumnComponent, DataBindingDirective,
  DetailTemplateDirective,
  GridComponent,
} from '@progress/kendo-angular-grid';
import {CurrencyPipe, NgIf} from "@angular/common";
import {TextBoxDirective} from "@progress/kendo-angular-inputs";
import {ButtonComponent} from "@progress/kendo-angular-buttons";
import {EquipmentListComponent} from "../equipments/equipments.component";
import {ModelCreateComponent} from "../../dialog/modelcreate/modelcreate.component";
import {NotifyService} from "../../../services/notify.service";

@Component({
  selector: 'app-table-models',
  templateUrl: './models.component.html',
  imports: [FormsModule, GridComponent, ColumnComponent, CellTemplateDirective, NgIf, TextBoxDirective, CurrencyPipe, ButtonComponent, EquipmentListComponent, DetailTemplateDirective, DataBindingDirective, ModelCreateComponent],
})
export class ModelsComponent implements OnInit {
  public models: ModelDTO[] = [];
  public newModel: ModelDTO = { modelId: 0, name: '', description: '', price: 0 };
  public editedModelId: number | null = null;
  public showAddModelDialog: boolean = false;

  constructor(private modelsService: ModelsService, private notify: NotifyService) { }

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.modelsService.getModels().subscribe(data => {
      this.models = data;
    });
  }

  editModelHandler(dataItem: ModelDTO): void {
    this.editedModelId = dataItem.modelId;
    this.newModel = { ...dataItem };
  }

  saveModelHandler(): void {
    if (!this.newModel.modelId) return;

    this.modelsService.updateModel(this.newModel).subscribe(
        () => {
          this.loadData();
          this.notify.showNotification('success', `Model nr${this.editedModelId} został zmieniony`);
          this.editedModelId = null;
        },
        error => {
          this.notify.showNotification('error', error.error);
        }
    );
  }

  cancelModelHandler(): void {
    this.editedModelId = null;
  }

    removeModelHandler(modelId: number): void {
        this.modelsService.deleteModel(modelId).subscribe(
            () => {
                this.models = this.models.filter(model => model.modelId !== modelId);
                this.notify.showNotification('success', `Model nr${modelId} został usunięty`);
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

  addNewModel(newModel: ModelDTO): void {
    this.modelsService.createModel(newModel).subscribe(
        () => {
          this.loadData();
          this.closeAddModelDialog();
          this.notify.showNotification('success', 'Nowy model został utworzony!');
        },
        error => {
          this.notify.showNotification('error', error.error);
        }
    );
  }
}

