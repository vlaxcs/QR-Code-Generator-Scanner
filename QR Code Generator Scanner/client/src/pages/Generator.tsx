import { useRef, useEffect, useState } from "react";

import {useGenerateImage} from "~/api/generateImage/hooks"
import ErrorCorrectionMenu from "~/components/ErrorCorrectionMenu"
import VersionSlider from "~/components/VersionSlider"
import AlertBanner from "~/components/AlertBanner"

interface GeneratorProps {}

function Generator({}: GeneratorProps) {
  const textareaRef = useRef<HTMLTextAreaElement | null>(null);

  const autoResize = () => {
    const textarea = textareaRef.current;
    if (!textarea) return;

    textarea.style.height = "auto";
    const maxHeight = 5 * 24;
    textarea.style.height = Math.min(textarea.scrollHeight, maxHeight) + "px";
  };

  const reset = () => {
    const textarea = textareaRef.current;
    if (!textarea) return;

    textarea.style.height = 40 + "px";
  };

  useEffect(() => {
    autoResize();
  }, []);

  useEffect(() =>{
    reset();
  }, []);

  const [showErrorCorrectionMenu, setShowErrorCorrectionMenu] = useState(false);
  const [showVersionSlider, setShowVersionSlider] = useState(false);

  const [errorCorrectionLevel, setErrorCorrectionLevel] = useState('M');
  const [version, setVersion] = useState(1);

  const handleCorrectionSelect = (level: string) => {
    setErrorCorrectionLevel(level);
    setShowErrorCorrectionMenu(false);
  };

  const handleVersionSelect = (value: string) => {
    setVersion(parseInt(value));
  };

  const eccTable: Record<string, number> = {
    "L": 0,
    "M": 1,
    "Q": 2,
    "H": 3,
  };

  const eccTableResponse: Record<string, string> = {
    "0": "L",
    "1": "M",
    "2": "Q",
    "3": "H",
  };


  const {imageUrl, usedVersion, usedEccLevel, error, generate } = useGenerateImage();
  const handleSubmit = () => {
    const textarea = textareaRef.current;
    if (!textarea) return;
    generate({
      message: textarea.value,
      reqVersion: version,
      reqEccLevel: eccTable[errorCorrectionLevel]
    });
  
    textarea.value = "";
    reset();
  }

  return (
    <>
        <AlertBanner
          message={imageUrl ? imageUrl : undefined}
          error={error ? error : undefined}
        />
        <ErrorCorrectionMenu
          isVisible={showErrorCorrectionMenu}
          onClose={() => setShowErrorCorrectionMenu(false)}
          onSelect={handleCorrectionSelect}
        />
        <VersionSlider
          isVisible={showVersionSlider}
          onClose={() => setShowVersionSlider(false)}
          onSelect={handleVersionSelect}
          value={version}
        />

    <div
    className={`absolute inset-0 bg-black transition-opacity duration-300 ${
        (showErrorCorrectionMenu || showVersionSlider) ? 'opacity-50' : 'opacity-0'
    }`}
    />
    
    {imageUrl && (
      <>
      <div className="relative top-20 lg:top-20 w-full mx-auto flex justify-center items-center flex-col">
            <div
                className="relative w-full max-w-[10rem] lg:max-w-lg rounded-2xl bg-black bg-cover bg-no-repeat bg-center aspect-square transition-all duration-300 ease-in-out"
                style={{
                backgroundImage: imageUrl ? `url(${imageUrl})` : 'none',
            }}></div>

            <div className="p-4 space-y-4 text-center">
              <a href={imageUrl} download="QR.png">
                <button
                  onClick={handleSubmit}
                  className="bg-amber-400 hover:bg-amber-300 hover:cursor-pointer text-black py-2 px-6 rounded-full transition-colors duration-300 ease-in-out"
                  >
                      Download
                </button>
              </a>
            </div>
            {usedEccLevel && usedVersion ? (
                <span
                  className="hover:cursor-pointer bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
                >
                  ECC: {eccTableResponse[usedEccLevel]} | V: {usedVersion}
              </span>
            ) : <></>}
      </div>
      </>
    )}


    <div className="text-white flex items-center justify-center">
      <div className="fixed bottom-10 left-1/2 transform -translate-x-1/2 w-[90%] md:max-w-7xl max-w-full bg-zinc-900 shadow-black drop-shadow-md rounded-4xl flex justify-center items-center p-4 flex-col">
        <textarea
          ref={textareaRef}
          onInput={autoResize}
          className="w-full h-24 p-2 resize-none text-white placeholder-gray-400 border-none focus:outline-none focus:ring-0 pretty-scroll bg-transparent"
          placeholder="Message to encrypt..."
          rows={1}
          maxLength={1000}
        />
        <div className="mt-4 pointer-events-auto flex md:flex-row flex-col md:gap-4 gap-2 justify-between items-center w-full">
          <div className="flex flex-col space-y-2 md:space-y-0 md:space-x-4 md:flex-row w-full">
            <button
              onClick={() => setShowErrorCorrectionMenu(true)}
              className="hover:cursor-pointer text-sm bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
            >
              Error Correction: <span className="text-sm font-bold">{errorCorrectionLevel}</span>
            </button>
            <button
              onClick={() => setShowVersionSlider(true)}
              className="hover:cursor-pointer text-sm bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
            >
              Version: <span className="text-sm font-bold">{version}</span>
            </button>
          </div>

          {/* Resize textarea when generate */}
          <button className="hover:cursor-pointer bg-amber-400 py-2 px-6 rounded-full text-zinc-900 font-boldshadow-md hover:bg-amber-300 transition-colors duration-200 md:w-auto w-full"
            onClick={handleSubmit}>
            Generate
          </button>
        </div>
      </div>
    </div>
    </>
  );
}

export default Generator;