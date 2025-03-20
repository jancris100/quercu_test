export interface IOwner {
  id?: number;
  name: string;
  telephone: string;
  email?: string | null;
  identificationNumber: string;
  address?: string | null;
}
