import {useState, useEffect} from 'react'

import { useScan } from "~/api/scanImage/hooks"
import type { ScanResponse } from "~/api/scanImage/types";

interface UploadProps {
  onResult: React.Dispatch<React.SetStateAction<ScanResponse | null>>;
  onLoading: React.Dispatch<React.SetStateAction<boolean>>;
  onError: React.Dispatch<React.SetStateAction<string | null>>;
}

export default function Upload({ onResult, onLoading, onError }: UploadProps) {
    const [file, setFile] = useState<File | null>(null);
    const [imageUrl, setImageUrl] = useState<string | null>(null);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        const selectedFile = event.target.files?.[0];
        if (selectedFile && selectedFile.type === 'image/png') {
            setFile(selectedFile);
        const url = URL.createObjectURL(selectedFile);
            setImageUrl(url);
        } else {
            alert('Only PNG files are allowed');
        }
    };

    const { result, loading, error, scan } = useScan();
    const handleSubmit = () => {
        if (file) {
            scan(file);
        }
    };

    useEffect(() => {
        onResult(result);
    }, [result]);

    useEffect(() => {
        onLoading(loading);
    }, [loading]);

    useEffect(() => {
        onError(error);
    }, [error]);


    const handleUploadOther = () => {
        onResult(null);
        onError(null);
        onLoading(false);
        setFile(null);
        setImageUrl(null);
    };

    return (
        <>
        <div className="relative top-20 lg:top-20 w-full mx-auto flex justify-center items-center flex-col">
            <div
                className="relative w-full max-w-[10rem] lg:max-w-lg rounded-2xl bg-black bg-cover bg-no-repeat bg-center aspect-square transition-all duration-300 ease-in-out"
                style={{
                backgroundImage: imageUrl ? `url(${imageUrl})` : 'none',
            }}
            />

            <div className="p-4 space-y-4 text-center">
            {!file ? (
                <label className="inline-block bg-amber-400 hover:bg-amber-300 text-black py-2 px-6 rounded-full cursor-pointer transition-colors duration-300 ease-in-out">
                    Upload file
                    <input
                        type="file"
                        accept=".png"
                        className="hidden"
                        onChange={handleFileChange}
                    />
                </label>
            ) : (
                <div className="space-y-2">
                    <div className="flex flex-col lg:flex-row justify-center gap-4">
                        
                        <button
                        onClick={handleUploadOther}
                        className="hover:cursor-pointer bg-zinc-800 text-gray-200 py-2 px-6 rounded-full shadow-md hover:bg-zinc-700 transition-colors duration-200 border border-zinc-700"
                        >
                            Upload other file
                        </button>

                        <button
                        onClick={handleSubmit}
                        className="bg-amber-400 hover:bg-amber-300 hover:cursor-pointer text-black py-2 px-6 rounded-full transition-colors duration-300 ease-in-out"
                        >
                            Scan
                        </button>
                    </div>
                </div>
            )}
            </div>
        </div>
    </>
    );
}