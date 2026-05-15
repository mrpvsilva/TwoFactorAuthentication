import React from 'react';
import { Container } from 'reactstrap';
import { Routes, Route, Navigate } from 'react-router-dom';
import NavMenu from './NavMenu';
import Home from './Home';
import FetchData from './FetchData';

export default function Layout() {
  return (
    <div>
      <NavMenu />
      <Container>
        <Routes>
          <Route path='Home' element={<Home />} />
          <Route path='fetch-data' element={<FetchData />} />
          <Route path='*' element={<Navigate to='/Home' replace />} />
        </Routes>
      </Container>
    </div>
  );
}
