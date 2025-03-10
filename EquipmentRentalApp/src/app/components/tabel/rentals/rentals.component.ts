import { Component, OnInit } from '@angular/core';
import { RentalService, Rental } from '../../../services/rental.service';
import {
  CellTemplateDirective,
  ColumnComponent,
  DataBindingDirective,
  GridComponent
} from "@progress/kendo-angular-grid";
import {DatePipe, NgClass, NgIf} from "@angular/common";
import {AskComponent} from "../../dialog/ask/ask.component";
import {ButtonComponent} from "@progress/kendo-angular-buttons";
import {NotifyService} from "../../../services/notify.service";

@Component({
  selector: 'app-table-rentals',
  templateUrl: './rentals.component.html',
    imports: [
        GridComponent,
        DataBindingDirective,
        ColumnComponent,
        CellTemplateDirective,
        NgIf,
        DatePipe,
        NgClass,
        AskComponent,
        ButtonComponent
    ]
})
export class RentalsComponent implements OnInit {
    public rentals: Rental[] = [];
    public selectedRentalId: number | null = null;

    constructor(private rentalService: RentalService, private notifyService: NotifyService) {}

    ngOnInit(): void {
    this.loadData();
    }

    loadData(): void {
    this.rentalService.getAllRentals().subscribe(data => {
      this.rentals = data;
    });
    }

    returnRental(rentalId: number): void {
    this.rentalService.returnRental(rentalId).subscribe(() => {
        this.rentals = this.rentals.map(rental => rental.rentalId === rentalId ? { ...rental, isReturned: true } : rental)
        this.notifyService.showNotification('success', `Sprzęt został oznaczony jako zwrócony`);
    });
    }

    confirmReturn(rentalId: number): void {
    console.log(rentalId);
    this.selectedRentalId = rentalId;
    }

    handleConfirmation(result: boolean): void {
    if (result && this.selectedRentalId !== null) {
      this.returnRental(this.selectedRentalId);
    }
    this.selectedRentalId = null;
    }
}
