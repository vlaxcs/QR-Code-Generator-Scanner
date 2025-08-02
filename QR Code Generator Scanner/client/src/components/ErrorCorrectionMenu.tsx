interface ErrorCorrectionMenuProps {
  isVisible: boolean;
  onClose: () => void;
  onSelect: (level: string) => void;
}

const ErrorCorrectionMenu = ({ isVisible, onClose, onSelect }: ErrorCorrectionMenuProps) => {
  if (!isVisible) return null;

  const eccLevels = [
    { label: 'Low', value: 'L', percentage: '7%' },
    { label: 'Medium', value: 'M', percentage: '15%' },
    { label: 'Quartile', value: 'Q', percentage: '25%' },
    { label: 'High', value: 'H', percentage: '30%' },
  ];

  return (
    <div
      className="fixed inset-0 flex items-center justify-center z-50 animate-fade-in"
      onClick={onClose}
    >
      <div className="absolute inset-0 bg-black opacity-50 transition-opacity duration-300"></div>

      <div
        className="text-gray-200 bg-zinc-800 rounded-2xl shadow-2xl p-4 w-64 transform scale-95 animate-scale-in z-50"
        onClick={(e) => e.stopPropagation()}
      >
        <h3 className="text-xl font-bold mb-4 text-center text-white">
          Error Correction
        </h3>
        {eccLevels.map((item) => (
          <button
            key={item.value}
            className="hover:text-black w-full text-left py-2 px-4 rounded-lg hover:bg-amber-400 transition-colors duration-200 flex justify-between items-center cursor-pointer"
            onClick={() => onSelect(item.value)}
          >
            <span>{item.label}</span>
            <span className="text-sm">{item.percentage}</span>
          </button>
        ))}
      </div>
    </div>
  );
};

export default ErrorCorrectionMenu;
