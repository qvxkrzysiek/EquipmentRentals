import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {AuthService} from "./auth.service";

export interface EquipmentDTO {
  equipmentId: number;
  serialNumber: string;
  modelId: number;
}

@Injectable({
  providedIn: 'root'
})
export class EquipmentService {

  private apiUrl = 'https://localhost:7014/api/Equipment';

  constructor(private http: HttpClient, private authService: AuthService) { }

  getEquipmentByModelId(modelId: number | null): Observable<EquipmentDTO[]> {
    const headers = this.authService.getHttpHeader();
    return this.http.get<EquipmentDTO[]>(`${this.apiUrl}/model/${modelId}`, { headers });
  }

  createEquipment(newEquipment: EquipmentDTO): Observable<EquipmentDTO> {
    const headers = this.authService.getHttpHeader();
    return this.http.post<EquipmentDTO>(this.apiUrl, newEquipment, { headers });
  }

  updateEquipment(updatedEquipment: EquipmentDTO): Observable<void> {
    const headers = this.authService.getHttpHeader();
    return this.http.put<void>(`${this.apiUrl}/${updatedEquipment.equipmentId}`, updatedEquipment, { headers });
  }

  deleteEquipment(equipmentId: number): Observable<void> {
    const headers = this.authService.getHttpHeader();
    return this.http.delete<void>(`${this.apiUrl}/${equipmentId}`, { headers });
  }
}
