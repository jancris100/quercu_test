import { Component, OnInit } from '@angular/core';
import { PropertyService } from '../services/property.service';
import { OwnerService } from '../services/owner.service';
import { PropertyTypeService } from '../services/property-type.service';
import { MatTableDataSource } from '@angular/material/table';
import { IProperty } from '../Interfaces/IProperty';
import { IOwner } from '../Interfaces/IOwner';
import { IPropertyType } from '../Interfaces/IPropertyType';
import { IPropertyCreate } from '../Interfaces/IPropertyCreate';

@Component({
  selector: 'app-properties',
  templateUrl: './properties.component.html',
  styleUrls: ['./properties.component.css'],
  standalone: false
})
export class PropertiesComponent implements OnInit {
  displayedColumns: string[] = ['no', 'number', 'address', 'area', 'constructionArea', 'owner', 'propertyType', 'actions'];
  dataSource = new MatTableDataSource<IProperty>([]);
  editingId: number | null = null;

  showValidationMessage = false;
  successMessage = '';
  errorTimeout: any;

  owners: IOwner[] = [];
  propertyTypes: IPropertyType[] = [];

  newProperty: IProperty = {
    propertyTypeId: 0,
    ownerId: 0,
    number: '',
    address: '',
    area: null,
    constructionArea: null
  };

  constructor(
    private propertyService: PropertyService,
    private ownerService: OwnerService,
    private propertyTypeService: PropertyTypeService
  ) { }

  ngOnInit() {
    this.loadData();
  }

  private loadData() {
    this.propertyService.getAllProperties().subscribe({
      next: (properties) => this.dataSource.data = properties,
      error: (err) => console.error("Error cargando propiedades:", err)
    });

    this.ownerService.getAllOwners().subscribe({
      next: (owners) => this.owners = owners,
      error: (err) => console.error("Error cargando dueños:", err)
    });

    this.propertyTypeService.getAllPropertyTypes().subscribe({
      next: (types) => this.propertyTypes = types,
      error: (err) => console.error("Error cargando tipos:", err)
    });
  }

  addProperty() {

    if (!this.isFormValid()) {
      this.showValidationMessage = true;
      this.setErrorTimeout();
      return;
    }
    const propertyToSend: IPropertyCreate = {
      propertyTypeId: Number(this.newProperty.propertyTypeId),
      ownerId: Number(this.newProperty.ownerId),
      number: this.newProperty.number,
      address: this.newProperty.address,
      area: this.newProperty.area,
      constructionArea: this.newProperty.constructionArea
    };

    this.propertyService.addProperty(this.newProperty).subscribe({
      next: (property) => {
        this.dataSource.data = [...this.dataSource.data, property];
        this.clearForm();
        this.loadData();
        this.showSuccessMessage('Propiedad agregada exitosamente');
      },
      error: (err) => console.error("Error agregando propiedad:", err)
    });
  }

  updateProperty(id: number) {
    const property = this.dataSource.data.find(p => p.id === id);
    if (property) {
      const isAddressInvalid = !property.address || property.address.trim().length < 5;
      const isAreaInvalid = property.area === null || property.area <= 0.1;

      if (isAddressInvalid || isAreaInvalid) {
        this.showValidationMessage = true;
        this.setErrorTimeout();
        return;
      }

      this.propertyService.updateProperty(id, property).subscribe({
        next: () => {
          this.editingId = null;
          this.showSuccessMessage('Propiedad actualizada exitosamente');
        },
        error: (err) => console.error("Error actualizando propiedad:", err)
      });
    }
  }

  deleteProperty(id: number) {
    this.propertyService.deleteProperty(id).subscribe({
      next: () => {
        this.dataSource.data = this.dataSource.data.filter(p => p.id !== id);
        this.showSuccessMessage('Propiedad eliminada exitosamente');
      },
      error: (err) => console.error("Error eliminando propiedad:", err)
    });
  }

  isFormValid(): boolean {
      const isValid = this.newProperty.propertyTypeId > 0 &&
      this.newProperty.ownerId > 0 &&
      this.newProperty.number.trim().length >= 1 &&
      this.newProperty.address.trim().length >= 5 &&
      this.newProperty.area != null && this.newProperty.area > 0.1; 
    if (this.newProperty.constructionArea != null) { 
      return isValid && (this.newProperty.constructionArea >= 0);
    }

    return isValid;
  }

  private clearForm() {
    this.newProperty = {
      propertyTypeId: 0,
      ownerId: 0,
      number: '',
      address: '',
      area: null,
      constructionArea: null 
    };
    this.showValidationMessage = false;
  }

  toggleEdit(property: IProperty) {
    this.editingId = this.editingId === property.id ? null : (property.id || null);
  }

  private showSuccessMessage(message: string) {
    this.successMessage = message;
    setTimeout(() => this.successMessage = '', 3000);
  }

  private setErrorTimeout() {
    if (this.errorTimeout) clearTimeout(this.errorTimeout);
    this.errorTimeout = setTimeout(() => {
      this.showValidationMessage = false;
    }, 3000);
  }
}
