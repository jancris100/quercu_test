import { Component, OnInit } from '@angular/core';
import { PropertyTypeService } from '../services/property-type.service';
import { MatTableDataSource } from '@angular/material/table';
import { IPropertyType } from '../Interfaces/IPropertyType';
import { ConfirmationDialogComponent } from '../helpers/confirmation-dialog.component';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-property-types',
  templateUrl: './property-types.component.html',
  styleUrls: ['./property-types.component.css'],
  standalone: false
})
export class PropertyTypesComponent implements OnInit {
  displayedColumns: string[] = ['no', 'description', 'actions'];
  dataSource = new MatTableDataSource<IPropertyType>([]);
  newPropertyDescription: string = '';
  selectedPropertyTypeId: number | null = null; 
  editingId: number | null = null;
  showValidationMessage = false;
  successMessage = '';
  constructor(private propertyTypeService: PropertyTypeService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,) { }

  ngOnInit() {
    this.loadPropertyTypes();
  }

  loadPropertyTypes() {
    this.propertyTypeService.getAllPropertyTypes().subscribe(
      propertyTypes => {
        this.dataSource.data = propertyTypes;
      },
      error => {
        console.error("Error al cargar los tipos de propiedad:", error);
      }
    );
  }


  async addPropertyType() {
    if (!this.newPropertyDescription?.trim()) {
      this.showValidationMessage = true;
      return;
    }

    this.propertyTypeService.addPropertyType({ description: this.newPropertyDescription })
      .subscribe({
        next: (createdPropertyType) => {
          this.dataSource.data = [...this.dataSource.data, createdPropertyType];
          this.newPropertyDescription = '';
          this.showSuccessMessage('Tipo de propiedad agregado exitosamente');
        },
        error: (error) => console.error("Error al agregar:", error)
      });
  }

  editPropertyType(id: number) {
    const propertyToEdit = this.dataSource.data.find(property => property.id === id);
    if (propertyToEdit) {
      this.selectedPropertyTypeId = id;
      this.newPropertyDescription = propertyToEdit.description; 
    }
  }


  async updatePropertyType(id: number) {
    const property = this.dataSource.data.find(p => p.id === id);
    if (property) {
      this.propertyTypeService.updatePropertyType(id, property)
        .subscribe({
          next: () => {
            this.editingId = null;
            this.showSuccessMessage('Tipo de propiedad actualizado exitosamente');
          },
          error: (error) => console.error("Error al actualizar:", error)
        });
    }
  }

  deletePropertyType(id: number) {
    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: '¿Estás seguro de eliminar este tipo de propiedad?'
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.propertyTypeService.deletePropertyType(id).subscribe({
          next: () => {
            this.dataSource.data = this.dataSource.data.filter(pt => pt.id !== id);
            this.showSuccessMessage('Tipo eliminado exitosamente');
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

  private showSuccessMessage(message: string) {
    this.successMessage = message;
    setTimeout(() => {
      this.successMessage = '';
    }, 3000); 
  }


  toggleEdit(element: IPropertyType) {
    if (this.editingId === element.id) {
      this.editingId = null;
    } else {
      this.editingId = element.id!; 
    }
  }

}
