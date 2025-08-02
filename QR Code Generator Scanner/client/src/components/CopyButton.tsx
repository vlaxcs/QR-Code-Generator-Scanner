import {useState} from 'react'

function CopyButton({ text }: { text: string }) {
  const [copied, setCopied] = useState(false);

  const handleCopy = async () => {
    try {
      await navigator.clipboard.writeText(text);
      setCopied(true);
      setTimeout(() => setCopied(false), 2000);
    } catch (err) {
      console.error('Copy failed', err);
    }
  };

  return (
    <button 
      className="hover:cursor-pointer bg-amber-400 py-2 px-6 rounded-full text-zinc-900 font-boldshadow-md hover:bg-amber-300 transition-colors duration-200 md:w-auto w-full"
      onClick={handleCopy}>
        {copied ? 'Copied!' : 'Copy'}
    </button>
  );
}

export default CopyButton;