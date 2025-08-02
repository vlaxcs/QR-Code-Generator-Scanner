import { useState, useEffect } from "react";
import type { GenerateRequest } from "./types";
import { generateImage } from "./generateImage";

export function useGenerateImage() {
  const [imageUrl, setImageUrl] = useState<string | null>(null);
  const [usedVersion, setVersion] = useState<string| null>(null);
  const [usedEccLevel, setEccLevel] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const generate = async (payload: GenerateRequest) => {
    setLoading(true);
    setError(null);

    try {
      const { image, usedVersion, usedEccLevel } = await generateImage(payload);
      const objectURL = URL.createObjectURL(image);
      setImageUrl(objectURL);
      setVersion(usedVersion);
      setEccLevel(usedEccLevel);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    return () => {
      if (imageUrl) URL.revokeObjectURL(imageUrl);
    };
  }, [imageUrl]);

  return {
    imageUrl,
    usedVersion,
    usedEccLevel,
    loading,
    error,
    generate,
  };
}