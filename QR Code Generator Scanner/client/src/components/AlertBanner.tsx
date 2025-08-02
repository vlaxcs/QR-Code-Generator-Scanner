import {useEffect, useState} from 'react'

function AlertBanner({ message, error }: { message?: string; error?: string }) {
  const [visible, setVisible] = useState(false);

  useEffect(() => {
    if (message || error) {
      setVisible(true);
      const timer = setTimeout(() => setVisible(false), 2000);
      return () => clearTimeout(timer);
    }
  }, [message, error]);

    return (
      <div
        className={`fixed top-25 z-1 left-1/2 transform -translate-x-1/2 transition-all duration-500 ease-in-out
          w-[90%] md:max-w-7xl max-w-full shadow-black drop-shadow-md rounded-4xl flex justify-center items-center p-4 flex-col
          ${visible ? 'opacity-100 translate-y-0' : 'opacity-0 -translate-y-10'}
          ${message ? 'bg-amber-400 text-black' : ''}
          ${error ? 'bg-amber-400 text-black' : ''}
        `}
      >
        {message && 'Success'}
        {error && `${error}`}
      </div>
  );
}

export default AlertBanner;