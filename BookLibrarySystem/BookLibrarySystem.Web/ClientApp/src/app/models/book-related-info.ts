import { Book } from "./book";

export interface BookWithRelatedInfo extends Book {

  isReserved:boolean;
}
