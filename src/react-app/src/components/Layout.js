import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import NavMenu from './NavMenu';
import Home from './Home';
import FetchData from './FetchData';

export default function Layout() {
  return (
    <div>
      <NavMenu />
      <main className="container mx-auto px-4 py-4">
        <Routes>
          <Route path='Home' element={<Home />} />
          <Route path='fetch-data' element={<FetchData />} />
          <Route path='*' element={<Navigate to='/Home' replace />} />
        </Routes>
      </main>
    </div>
  );
}
