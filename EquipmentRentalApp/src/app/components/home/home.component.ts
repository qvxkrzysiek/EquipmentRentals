import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { GridModule } from '@progress/kendo-angular-grid';
import { ButtonsModule } from '@progress/kendo-angular-buttons';
import { InputsModule } from '@progress/kendo-angular-inputs';
import { DatePickerModule } from '@progress/kendo-angular-dateinputs';
import { RentalService } from '../../services/rental.service';
import { ModelsService, ModelsInStockDTO } from "../../services/models.service";
import { LogoutComponent } from "../logout/logout.component";
import { NotifyService } from "../../services/notify.service";

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  standalone: true,
  imports: [CommonModule, GridModule, ButtonsModule, InputsModule, DatePickerModule, LogoutComponent]
})
export class HomeComponent implements OnInit {
  public groupedEquipments: ModelsInStockDTO[] = [];
  public skip = 0;
  public pageSize = 10;
  public selectedProduct: ModelsInStockDTO | null = null;

  public today: Date = new Date();
  public rentalStartDate: Date = new Date();
  public rentalEndDate: Date = new Date();

  public minDate: Date = new Date();
  public maxDate: Date = new Date();
  public equipmentModelId: number = 0;

  constructor(
      private modelsService: ModelsService,
      private rentalService: RentalService,
      private notifyService: NotifyService
  ) {}

  ngOnInit(): void {
    this.loadData();
  }

  loadData(): void {
    this.modelsService.getModelsStock().subscribe(data => {
      this.groupedEquipments = data;
    });
  }

  onRentClick(item: ModelsInStockDTO): void {
    this.selectedProduct = item;
    this.equipmentModelId = item.modelId;
    this.setMinAndMaxDates();
  }

  setMinAndMaxDates() {
    this.minDate.setDate(this.today.getDate() + 7);
    this.maxDate.setDate(this.today.getDate() + 30);
    this.rentalStartDate = this.today;
    this.rentalEndDate.setDate(this.today.getDate() + 7);
  }

  onRent(): void {
    const rentalData = {
      equipmentModelId: this.equipmentModelId,
      startDate: this.rentalStartDate.toISOString(),
      endDate: this.rentalEndDate.toISOString()
    };

    this.rentalService.createRental(rentalData).subscribe({
      next: () => {
        this.notifyService.showNotification('success', `Pomyślnie zarezerwowano sprzęt`);
      },
      error: (error) => {
        this.notifyService.showNotification('error', error.error);
      }
    });
  }
}