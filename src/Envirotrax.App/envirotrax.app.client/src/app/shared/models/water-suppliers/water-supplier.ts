import { LetterContact } from './letter-contact';
import { LetterAddress } from './letter-return';

export interface WaterSupplier {
  id?: number;
  name?: string;
  domain?: string;
  parent?: WaterSupplier;

  contactName?: string;
  pwsId?: string;
  address?: string;
  city?: string;
  stateId?: number | null;
  zipCode?: string;
  phoneNumber?: string;
  faxNumber?: string;
  emailAddress?: string;

  letterAddress: LetterAddress;  

  letterContact: LetterContact;
}
