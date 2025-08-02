import {useEffect, useRef} from 'react'
import AlertBanner from "~/components/AlertBanner"
import CopyButton from "~/components/CopyButton"

interface ResponseBoxProps {
  error: any;
  message: string | undefined;
  ecclevel: string | undefined;
  version: number | undefined;
}

function ResponseBox({ message, ecclevel, version, error }: ResponseBoxProps) {
    const textareaRef = useRef<HTMLTextAreaElement | null>(null);
    
      const autoResize = () => {
        const textarea = textareaRef.current;
        if (!textarea) return;
    
        textarea.style.height = "auto";
        const maxHeight = 5 * 24;
        textarea.style.height = Math.min(textarea.scrollHeight, maxHeight) + "px";
      };
    
      useEffect(() => {
        autoResize();
      }, []);

      useEffect(() => {
          autoResize();
      }, [message]);

      if (!message){
        autoResize();
      }

    return (
    <>
      <AlertBanner
        message={message}
        error={error}
      />
      <div className="text-white flex items-center justify-center">
        <div className="fixed bottom-10 left-1/2 transform -translate-x-1/2 w-[90%] md:max-w-7xl max-w-full bg-zinc-900 shadow-black drop-shadow-md rounded-4xl flex justify-center items-center p-4 flex-col">
          <textarea
            value={message ? message : ""}
            ref={textareaRef}
            onInput={autoResize}
            className={`w-full h-24 p-2 resize-none text-white placeholder-gray-400 border-none focus:outline-none focus:ring-0 pretty-scroll bg-transparent ${!message ? "pointer-events-none" : ""}`}
            placeholder="Scanned message..."
            readOnly={!!message}
            rows={1}
            maxLength={1000}
          />
          <div className="mt-4 pointer-events-auto flex md:flex-row flex-col md:gap-4 gap-2 justify-between items-center w-full">
            <div className="flex flex-col space-y-2 md:space-y-0 md:space-x-4 md:flex-row w-full">
              <span
                className="text-sm bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
              >
                Error Correction: <span className="text-sm font-bold">{ecclevel ? ecclevel : "Unknown yet"}</span>
              </span>
              <span
                className="text-sm bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
              >
                Version: <span className="text-sm font-bold">{version ? version : "Unknown yet"}</span>
              </span>
            </div>
            { message && <CopyButton text={message} /> }
          </div>
        </div>
      </div>
    </>
  );
};

export default ResponseBox;