import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PropertiesComponent } from './properties/properties.component';
import { PropertyTypesComponent } from './property-types/property-types.component';
import { OwnersComponent } from './owners/owners.component';

const routes: Routes = [
  { path: '', redirectTo: '/properties', pathMatch: 'full' },
  { path: 'properties', component: PropertiesComponent },
  { path: 'property-types', component: PropertyTypesComponent },
  { path: 'owners', component: OwnersComponent },
  { path: '', redirectTo: '/properties', pathMatch: 'full' },  
  { path: '**', redirectTo: '/properties' },  
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
