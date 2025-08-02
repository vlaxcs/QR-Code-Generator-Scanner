import { useState } from "react"
import { NavLink, Link } from "react-router-dom"
import { FaGithub } from 'react-icons/fa';
import Pancakes from "/assets/pancakes.png"

export default function Header() {
  const [isMenuOpen, setIsMenuOpen] = useState(false);

  const toggleMenu = () => {
    setIsMenuOpen(!isMenuOpen);
  };

  return (
    <header className="absolute top-0 left-0 right-0 bg-zinc-900 text-white h-20 shadow-md z-50 opacity-80">
      <div className="max-w-7xl mx-auto px-4 h-full flex items-center justify-between">
        <div className="flex items-center space-x-2">
           <img
                src={Pancakes}
                alt="Logo"
                className="w-12 h-12 object-contain"
                loading="lazy"
            />
          <span className="text-md lg:text-xl font-bold tracking-wide">Pancake's QR</span>
        </div>

        <div className="md:hidden flex items-center">
          <button onClick={toggleMenu} className="hover:cursor-pointer focus:outline-none focus:ring-2 focus:ring-amber-400 p-2 rounded">
            {isMenuOpen ? (
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
              </svg>
            ) : (
              <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16m-7 6h7" />
              </svg>
            )}
          </button>
        </div>

        <nav className="hidden md:flex justify-center items-center space-x-6 text-sm md:text-base px-4">
          <NavLink
            to="/scanner"
            className={({ isActive }) =>
              isActive
                ? "font-bold text-yellow-400"
                : "hover:text-yellow-300 transition-colors"
            }
          >
            Scanner
          </NavLink>
          <NavLink
            to="/generator"
            className={({ isActive }) =>
              isActive
                ? "font-bold text-yellow-400"
                : "hover:text-yellow-300 transition-colors"
            }
          >
            Generator
          </NavLink>
        <Link
            to="https://github.com/vlaxcs/QR-Code-Generator-Scanner"
            target="_blank"
            rel="noopener noreferrer"
            className="scale-200"
            >
            <FaGithub
                className="ml-1 hover:text-amber-400 transition-all 300 ease-in-out"
            />
        </Link>
        </nav>
      </div>

            <nav
        className={`md:hidden fixed inset-0 flex justify-center items-center transform transition-opacity duration-300 ${
          isMenuOpen ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"
        }`}
      >
        <div 
          className="absolute inset-0 bg-black h-full min-h-screen w-full transition-opacity duration-300" 
          style={{ opacity: isMenuOpen ? 1 : 0 }}
          onClick={() => setIsMenuOpen(false)} 
        />
        <div className="z-10 flex flex-col p-4 space-y-4">
          <NavLink
            to="/scanner"
            className={({ isActive }) =>
              isActive
                ? "font-bold text-amber-400 p-4 rounded text-center text-2xl"
                : "text-white hover:text-amber-400 p-4 rounded transition-colors text-center text-2xl"
            }
            onClick={() => setIsMenuOpen(false)}
          >
            Scanner
          </NavLink>
          <NavLink
            to="/generator"
            className={({ isActive }) =>
              isActive
                ? "font-bold text-amber-400 p-4 rounded text-center text-2xl"
                : "text-white hover:text-amber-400 p-4 rounded transition-colors text-center text-2xl"
            }
            onClick={() => setIsMenuOpen(false)}
          >
            Generator
          </NavLink>
        <Link
            to="https://github.com/vlaxcs/QR-Code-Generator-Scanner"
            target="_blank"
            rel="noopener noreferrer"
            className="text-white hover:text-amber-400 p-4 rounded transition-colors flex flex-row items-center text-2xl"
            >
            <FaGithub
                className="mr-3 hover:text-amber-400 transition-colors 300 ease-in-out scale-120"
            /> Repository
        </Link>
        </div>
      </nav>
    </header>
  );
}
