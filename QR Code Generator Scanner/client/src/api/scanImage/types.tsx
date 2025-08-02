export interface ScanResponse {
  message: string;
  errorCorrectionLevel: number;
  version: number;
}

export interface Message {
  title: string;
  body: string;
}