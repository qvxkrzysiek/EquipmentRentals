import {Component} from '@angular/core';
import {ModelsComponent} from "../tabel/models/models.component";
import {RentalsComponent} from "../tabel/rentals/rentals.component";
import {LogoutComponent} from "../logout/logout.component";

@Component({
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  standalone: true,
    imports: [
        ModelsComponent,
        RentalsComponent,
        LogoutComponent,
    ],
})
export class AdminComponent {

}
