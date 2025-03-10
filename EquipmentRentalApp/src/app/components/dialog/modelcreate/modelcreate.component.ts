import {Component, EventEmitter, Output} from '@angular/core';
import {ModelDTO} from "../../../services/models.service";
import {DialogComponent} from "@progress/kendo-angular-dialog";
import {FormsModule} from "@angular/forms";
import {TextBoxDirective} from "@progress/kendo-angular-inputs";
import {ButtonComponent} from "@progress/kendo-angular-buttons";

@Component({
  selector: 'app-modelcreate',
  imports: [
    DialogComponent,
    FormsModule,
    TextBoxDirective,
    ButtonComponent
  ],
  templateUrl: './modelcreate.component.html',
})
export class ModelCreateComponent {
  @Output() modelCreated: EventEmitter<ModelDTO> = new EventEmitter();
  @Output() dialogClosed: EventEmitter<void> = new EventEmitter();

  public newModel: ModelDTO = { modelId: 0, name: '', description: '', price: 0 };

  saveModel(): void {
    if (this.newModel.name && this.newModel.description && this.newModel.price) {
      this.modelCreated.emit(this.newModel);
      this.newModel = { modelId: 0, name: '', description: '', price: 0 };
    }
  }

  cancel(): void {
    this.dialogClosed.emit();
  }
}
