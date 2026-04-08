export interface CatFact {
  fact: string;
  length: number;
}

export interface CatFactsResponse {
  current_page: number;
  data: CatFact[];
}