export interface GenerateRequest {
  message: string;
  reqVersion: number;
  reqEccLevel: number;
}

export interface GenerateResponse{ 
  image: Blob;
  usedVersion: string | null;
  usedEccLevel: string | null;
}