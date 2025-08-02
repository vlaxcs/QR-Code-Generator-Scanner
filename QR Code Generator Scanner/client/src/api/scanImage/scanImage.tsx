const BASE_URL = import.meta.env.VITE_SERVICE_URL;

import type { ScanResponse } from "./types";

export const scanImage = async (file: File): Promise<ScanResponse> => {
  const formData = new FormData();
  formData.append("image", file);

  const response = await fetch(`${BASE_URL}/scan`, {
    method: "POST",
    headers: {
      Accept: "application/json"
    },
    body: formData
  });

  if (!response.ok) {
    throw new Error(`Server error: ${response.statusText}`);
  }

  const json = await response.json();
  return json as ScanResponse;
};