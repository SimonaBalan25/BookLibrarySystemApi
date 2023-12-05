export interface Book {
  ID : number;
  ISBN: string;
  title: string;
  publisher: string;
  releaseYear: number;
  genre: string;
  status: number;
  numberOfCopies: number;
  loanedQuantity: number;
  numberOfPages: number;
  authors: string[];
}
