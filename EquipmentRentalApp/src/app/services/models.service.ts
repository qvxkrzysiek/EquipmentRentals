import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import {AuthService} from "./auth.service";

export interface ModelDTO {
    modelId: number;
    name: string;
    description: string;
    price: number;
}

export interface ModelsInStockDTO {
    modelId: number;
    name: string;
    description: string;
    price: number;
    quantity: number;
    stockQuantity: number;
}

@Injectable({
    providedIn: 'root'
})
export class ModelsService {

    private apiUrl = 'https://localhost:7014/api/Models';

    constructor(private http: HttpClient, private authService: AuthService) { }

    getModelsStock(): Observable<ModelsInStockDTO[]> {
        return this.http.get<ModelsInStockDTO[]>(`${this.apiUrl}/stock`);
    }

    getModels(): Observable<ModelDTO[]> {
        const headers = this.authService.getHttpHeader();
        return this.http.get<ModelDTO[]>(this.apiUrl, { headers });
    }

    createModel(newModel: ModelDTO): Observable<ModelDTO> {
        const headers = this.authService.getHttpHeader();
        return this.http.post<ModelDTO>(this.apiUrl, newModel, { headers });
    }

    updateModel(newModel: ModelDTO) {
        const headers = this.authService.getHttpHeader();
        return this.http.put<void>(`${this.apiUrl}/${newModel.modelId}`, newModel, { headers });
    }

    deleteModel(modelId: number) {
        const headers = this.authService.getHttpHeader();
        return this.http.delete<void>(`${this.apiUrl}/${modelId}`, { headers });
    }
}