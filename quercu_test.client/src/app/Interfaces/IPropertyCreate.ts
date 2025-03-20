export interface IPropertyCreate {
  propertyTypeId: number;
  ownerId: number;
  number: string;
  address: string;
  area: number | null;
  constructionArea?: number | null;
}

