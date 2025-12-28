import { LetterContact } from './letter-contact';
import { LetterReturn } from './letter-return';

export interface WaterSupplier {
  id?: number;
  name?: string;
  domain?: string;
  parent?: WaterSupplier;

  contactName?: string;
  pwsId?: string;
  address?: string;
  city?: string;
  state?: string;
  zipCode?: string;
  phone?: string;
  fax?: string;
  email?: string;

  letterReturn: LetterReturn;

  letterContact: LetterContact;
}
