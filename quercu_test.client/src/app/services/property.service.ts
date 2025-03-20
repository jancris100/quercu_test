import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '../../../environment';
import { IProperty } from '../Interfaces/IProperty';

@Injectable({
  providedIn: 'root'
})
export class PropertyService {
  private apiUrl = `${environment.apiUrl}/Property`;

  constructor(private http: HttpClient) { }

  getAllProperties(): Observable<IProperty[]> {
    return this.http.get<IProperty[]>(this.apiUrl).pipe(
      catchError(this.handleError)
    );
  }

  addProperty(newProperty: IProperty): Observable<IProperty> {
    return this.http.post<IProperty>(this.apiUrl, newProperty).pipe(
      catchError(this.handleError)
    );
  }

  updateProperty(id: number, updatedProperty: IProperty): Observable<IProperty> {
    const dto = {
      id: updatedProperty.id,
      propertyTypeId: updatedProperty.propertyTypeId,
      ownerId: updatedProperty.ownerId,
      number: updatedProperty.number,
      address: updatedProperty.address,
      area: updatedProperty.area,
      constructionArea: updatedProperty.constructionArea
    };

    return this.http.put<IProperty>(`${this.apiUrl}/${id}`, dto).pipe(
      catchError(this.handleError)
    );
  }

  deleteProperty(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError(this.handleError)
    );
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Error desconocido';
    if (error.error?.message) {
      errorMessage = error.error.message;
    }
    return throwError(() => new Error(errorMessage));
  }
}
