import { Component, EventEmitter, Input, Output } from '@angular/core';
import {DialogComponent} from "@progress/kendo-angular-dialog";
import {ButtonComponent} from "@progress/kendo-angular-buttons";

@Component({
  selector: 'app-ask',
  templateUrl: './ask.component.html',
  imports: [
    DialogComponent,
    ButtonComponent,
    ButtonComponent,
    DialogComponent,
    ButtonComponent
  ]
})
export class AskComponent {
  @Input() message: string = "Czy na pewno chcesz kontynuować?";
  @Output() confirmed = new EventEmitter<boolean>();

  closeDialog(result: boolean): void {
    this.confirmed.emit(result);
  }
}
