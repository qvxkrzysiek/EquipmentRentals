import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { AuthService } from './auth.service';

export interface RentalRequest {
  equipmentModelId: number;
  startDate: string;
  endDate: string;
}

export interface Rental {
  rentalId: number;
  rentalDate: string;
  returnDate: string;
  isReturned: boolean;
  userId: number;
  userEmail: string;
  equipmentId: number;
  equipmentModelName: string;
  equipmentSerialNumber: string;
}

@Injectable({
  providedIn: 'root'
})
export class RentalService {
  private apiUrl = 'https://localhost:7014/api/Rentals';

  constructor(private http: HttpClient, private authService: AuthService) {}

  getAllRentals(): Observable<Rental[]> {
    const headers = this.authService.getHttpHeader();
    return this.http.get<Rental[]>(this.apiUrl, { headers });
  }

  returnRental(rentalId: number): Observable<void> {
    const headers = this.authService.getHttpHeader();
    return this.http.put<void>(`${this.apiUrl}/${rentalId}/return`, {}, { headers });
  }

  createRental(rental: RentalRequest): Observable<any> {
    const headers = this.authService.getHttpHeader();
    return this.http.post<any>(this.apiUrl, rental, { headers });
  }
}
