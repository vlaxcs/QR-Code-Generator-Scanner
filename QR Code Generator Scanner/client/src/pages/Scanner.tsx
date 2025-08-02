import { useState } from "react";

import Upload from "~/components/Upload";
import ResponseBox from "~/components/ResponseBox";
import type { ScanResponse } from "~/api/scanImage/types";

function Scanner() {
  const [result, setResult] = useState<ScanResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const eccLevels = ["L", "M", "Q", "H"];

  return (
    <>
        <Upload
            onResult={setResult}
            onLoading={setLoading}
            onError={setError}
        />

        <ResponseBox
            error={error}
            message={result?.message}
            ecclevel={
            typeof result?.errorCorrectionLevel === "number" &&
            result.errorCorrectionLevel >= 0 &&
            result.errorCorrectionLevel < eccLevels.length
                ? eccLevels[result.errorCorrectionLevel]
                : undefined
            }
            version={result?.version}
        />
    </>
  );
}

export default Scanner;