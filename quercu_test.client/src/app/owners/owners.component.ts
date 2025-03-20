import { Component, OnInit } from '@angular/core';
import { OwnerService } from '../services/owner.service';
import { MatTableDataSource } from '@angular/material/table';
import { IOwner } from '../Interfaces/IOwner';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ConfirmationDialogComponent } from '../helpers/confirmation-dialog.component';

@Component({
  selector: 'app-owners',
  templateUrl: './owners.component.html',
  styleUrls: ['./owners.component.css'],
  standalone: false
})
export class OwnersComponent implements OnInit {
  displayedColumns: string[] = ['no', 'name', 'telephone', 'identificationNumber', 'email', 'address', 'actions'];
  dataSource = new MatTableDataSource<IOwner>([]);
  editingId: number | null = null;
  private errorTimeout: any;

  showValidationMessage = false;
  successMessage = '';
  requiredFieldsEmpty = false;
  nameHasError = false;
  telephoneHasError = false;
  identificationHasError = false;
  emailHasError = false;
  identificationError = false;

  newOwner: IOwner = {
    name: '',
    telephone: '',
    identificationNumber: '',
    email: null, 
    address: null 
  };

  constructor(private ownerService: OwnerService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,) { }

  ngOnInit() {
    this.loadOwners();
  }

  loadOwners() {
    this.ownerService.getAllOwners().subscribe({
      next: (owners) => this.dataSource.data = owners,
      error: (err) => console.error("Error loading owners:", err)
    });
  }

  onlyNumbers(event: KeyboardEvent): boolean {
    const charCode = event.key.charCodeAt(0);
    return (charCode >= 48 && charCode <= 57);
  }

  onlyLetters(event: KeyboardEvent): boolean {
    const charCode = event.key.charCodeAt(0);
    return (charCode >= 65 && charCode <= 90) ||  
      (charCode >= 97 && charCode <= 122) ||
      charCode === 32; 
  }

  onPaste(event: ClipboardEvent, type: 'number' | 'text') {
    const data = event.clipboardData?.getData('text/plain') || '';

    if (type === 'number' && !/^\d+$/.test(data)) {
      event.preventDefault();
    }

    if (type === 'text' && !/^[a-zA-Z\s]+$/.test(data)) {
      event.preventDefault();
    }
  }

  addOwner() {
    this.identificationError = false;

    if (!this.isFormValid()) {
      this.showValidationMessage = true;
      this.setErrorTimeout();
      return;
    }

    this.ownerService.addOwner(this.prepareOwnerData()).subscribe({
      next: (owner) => {
        this.dataSource.data = [...this.dataSource.data, owner];
        this.clearForm();
        this.showSuccessMessage('Dueño agregado exitosamente');
      },
      error: (err) => {
        if (err.message.includes('identificación')) {
          this.identificationError = true;
          this.showValidationMessage = true;
          this.setErrorTimeout();
        }
        console.error("Error al agregar:", err);
      }
    });
  }

  updateOwner(id: number) {
    this.identificationError = false;
    const owner = this.dataSource.data.find(o => o.id === id);

    if (owner) {
      this.ownerService.updateOwner(id, owner).subscribe({
        next: () => {
          this.editingId = null;
          this.showSuccessMessage('Dueño actualizado exitosamente'); 
        },
        error: (err) => {
          if (err.message.includes('identificación')) {
            this.identificationError = true;
            this.showValidationMessage = true;
            this.setErrorTimeout();
          }
          console.error("Error al actualizar:", err);
        }
      });
    }
  }

  deleteOwner(id: number) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      width: '350px',
      data: '¿Estás seguro de eliminar este dueño?'
    });

    dialogRef.afterClosed().subscribe((result: boolean) => {
      if (result) {
        this.ownerService.deleteOwner(id).subscribe({
          next: () => {
            this.dataSource.data = this.dataSource.data.filter(o => o.id !== id);
            this.showSuccessMessage('Dueño eliminado exitosamente');
          },
          error: (err) => {
            this.snackBar.open(err.message, 'Cerrar', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
          }
        });
      }
    });
  }

  private prepareOwnerData(): IOwner {
    return {
      ...this.newOwner,
      email: this.newOwner.email?.trim() || null,
      address: this.newOwner.address?.trim() || null
    };
  }

   isFormValid(): boolean {

    this.requiredFieldsEmpty = !this.newOwner.name?.trim() ||
      !this.newOwner.telephone?.trim() ||
      !this.newOwner.identificationNumber?.trim();

    this.nameHasError = !this.newOwner.name || this.newOwner.name.length < 3;
    this.telephoneHasError = !/^\d{7,}$/.test(this.newOwner.telephone);
    this.identificationHasError = !/^\d{5,}$/.test(this.newOwner.identificationNumber);
    this.newOwner.email = this.newOwner.email?.trim() || null;
    this.newOwner.address = this.newOwner.address?.trim() || null;

    return !this.requiredFieldsEmpty &&
      !this.nameHasError &&
      !this.telephoneHasError &&
      !this.identificationHasError &&
      !this.emailHasError;
  }

  validateEmail(email: string): boolean {
    const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return re.test(email.toLowerCase());
  }

  clearForm() {
    this.newOwner = {
      name: '',
      telephone: '',
      identificationNumber: '',
      email: null, 
      address: null
    };
    this.showValidationMessage = false;
  }

  toggleEdit(owner: IOwner) {
    this.editingId = this.editingId === owner.id ? null : (owner.id || null);
  }

  private setErrorTimeout() {
    if (this.errorTimeout) clearTimeout(this.errorTimeout);
    this.errorTimeout = setTimeout(() => {
      this.showValidationMessage = false;
      this.identificationError = false;
    }, 3000);
  }

  showSuccessMessage(message: string) {
    this.successMessage = message;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000);
  }
}
