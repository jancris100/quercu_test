import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environment';
import { IOwner } from '../Interfaces/IOwner';
import { HttpErrorResponse } from '@angular/common/http';
import { catchError } from 'rxjs/operators'

@Injectable({
  providedIn: 'root'
})
export class OwnerService {
  private apiUrl = `${environment.apiUrl}/Owner`;

  constructor(private http: HttpClient) { }

  getAllOwners(): Observable<IOwner[]> {
    return this.http.get<IOwner[]>(this.apiUrl);
  }

  private handleError(error: HttpErrorResponse) {
    let errorMessage = 'Error desconocido';
    if (error.error?.message) {
      errorMessage = error.error.message;
    }
    return throwError(() => new Error(errorMessage));
  }

  addOwner(newOwner: IOwner): Observable<IOwner> {
    return this.http.post<IOwner>(this.apiUrl, newOwner).pipe(
      catchError(this.handleError)
    );
  }

  updateOwner(id: number, updatedOwner: IOwner): Observable<IOwner> {
    return this.http.put<IOwner>(`${this.apiUrl}/${id}`, updatedOwner).pipe(
      catchError(this.handleError)
    );
  }

  deleteOwner(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = 'Error al eliminar el dueño';
        if (error.status === 409) {
          errorMessage = error.error.message;
        }
        return throwError(() => new Error(errorMessage));
      })
    );
  }

}
