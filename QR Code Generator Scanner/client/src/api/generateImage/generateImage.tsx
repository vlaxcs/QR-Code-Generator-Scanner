const BASE_URL = import.meta.env.VITE_SERVICE_URL;

import type { GenerateResponse, GenerateRequest } from "./types"

export async function generateImage(payload: GenerateRequest): Promise<GenerateResponse> {
  const response = await fetch(`${BASE_URL}/generate`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(payload),
  });

  if (!response.ok) {
    const err = await response.json();
    throw new Error(err.error || "QR generation failed");
  }

  const usedVersion = response.headers.get("usedVersion");
  const usedEccLevel = response.headers.get("usedEccLevel");
  const image = await response.blob();

  return { image, usedVersion, usedEccLevel };
}