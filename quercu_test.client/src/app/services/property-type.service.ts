import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environment'
import { IPropertyType } from '../Interfaces/IPropertyType';
import { catchError } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class PropertyTypeService {
  private apiUrl = `${environment.apiUrl}/PropertyType`;

  constructor(private http: HttpClient) { }

  getAllPropertyTypes(): Observable<IPropertyType[]> {
    return this.http.get<IPropertyType[]>(this.apiUrl);
  }

  addPropertyType(newProperty: IPropertyType): Observable<IPropertyType> {
    return this.http.post<IPropertyType>(this.apiUrl, newProperty);
  }

  updatePropertyType(id: number, updatedProperty: IPropertyType): Observable<IPropertyType> {
    return this.http.put<IPropertyType>(`${this.apiUrl}/${id}`, updatedProperty);
  }

  deletePropertyType(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Error al eliminar el tipo de propiedad';
        if (error.status === 409) {
          errorMessage = error.error.message;
        }
        return throwError(() => new Error(errorMessage));
      })
    );
  }
}
