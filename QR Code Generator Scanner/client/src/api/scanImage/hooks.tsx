import { useState } from "react";
import type { ScanResponse } from "./types";
import { scanImage } from "./scanImage";

export const useScan = () => {
  const [result, setResult] = useState<ScanResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const scan = async (file: File) => {
    setLoading(true);
    setError(null);
    try {
      const res = await scanImage(file);
      setResult(res);
    } catch (err: any) {
      setError(err.message || "Scan failed");
    } finally {
      setLoading(false);
    }
  };

  return { result, loading, error, scan };
};
