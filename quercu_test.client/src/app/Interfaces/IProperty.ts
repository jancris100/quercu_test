import { IOwner } from '../Interfaces/IOwner';
import { IPropertyType } from '../Interfaces/IPropertyType';

export interface IProperty {
  id?: number;
  propertyTypeId: number;
  ownerId: number;
  number: string;
  address: string;
  area: number | null;
  constructionArea?: number | null;
  owner?: IOwner;         
  propertyType?: IPropertyType;
}

