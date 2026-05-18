import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { Menu, X } from 'lucide-react';
import { useAuth } from '../AuthContext';

export default function NavMenu() {
  const [open, setOpen] = useState(false);
  const { logout } = useAuth();

  return (
    <header className="border-b bg-white shadow-sm mb-4">
      <nav className="container mx-auto px-4 h-14 flex items-center justify-between relative">
        <Link to='/' className="font-semibold text-lg">WebApplication</Link>
        <button
          className="md:hidden p-2 rounded-md hover:bg-gray-100"
          onClick={() => setOpen(prev => !prev)}
          aria-label="Toggle menu"
        >
          {open ? <X className="h-5 w-5" /> : <Menu className="h-5 w-5" />}
        </button>
        <ul className={`${open ? 'flex' : 'hidden'} md:flex flex-col md:flex-row absolute md:static top-14 left-0 right-0 bg-white md:bg-transparent border-b md:border-0 px-4 py-2 md:p-0 gap-1 md:gap-2 z-50`}>
          <li>
            <Link to='/' className="block px-3 py-2 text-sm rounded hover:bg-gray-100" onClick={() => setOpen(false)}>
              Home
            </Link>
          </li>
          <li>
            <Link to='/fetch-data' className="block px-3 py-2 text-sm rounded hover:bg-gray-100" onClick={() => setOpen(false)}>
              Fetch data
            </Link>
          </li>
          <li>
            <button
              className="block w-full text-left px-3 py-2 text-sm rounded hover:bg-gray-100 text-red-600"
              onClick={() => { setOpen(false); logout(); }}
            >
              Logout
            </button>
          </li>
        </ul>
      </nav>
    </header>
  );
}
