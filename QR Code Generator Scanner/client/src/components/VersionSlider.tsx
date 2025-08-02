interface VersionSliderProps {
  isVisible: boolean;
  onClose: () => void;
  onSelect: (value: string) => void;
  value: number;
}

const VersionSlider = ({ isVisible, onClose, onSelect, value }: VersionSliderProps) => {
  if (!isVisible) return null;

  return (
    <div
      className="fixed inset-0 flex items-center justify-center z-50 animate-fade-in"
      onClick={onClose}
    >
      <div
        className="text-gray-200 bg-zinc-800 rounded-2xl shadow-2xl p-6 w-80 transform scale-95 animate-scale-in z-50"
        onClick={(e) => e.stopPropagation()}
      >
        <h3 className="text-xl font-bold mb-4 text-center">Version: {value}</h3>
        <input
          type="range"
          min="1"
          max="40"
          value={value}
          onChange={(e) => onSelect(e.target.value)}
          className="w-full h-1 rounded-lg appearance-none cursor-pointer accent-amber-400 bg-zinc-600"
        />
      </div>
    </div>
  );
};

export default VersionSlider;