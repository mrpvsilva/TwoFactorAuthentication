import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link, useNavigate } from 'react-router-dom';
import './NavMenu.css';

export default function NavMenu() {
  const [collapsed, setCollapsed] = useState(true);
  const navigate = useNavigate();

  const handleLogout = (e) => {
    e.preventDefault();
    localStorage.removeItem('token');
    navigate('/login');
  };

  return (
    <header>
      <Navbar className="navbar-expand-sm navbar-toggleable-sm ng-white border-bottom box-shadow mb-3" light>
        <Container>
          <NavbarBrand tag={Link} to='/'>WebApplication</NavbarBrand>
          <NavbarToggler onClick={() => setCollapsed(prev => !prev)} className="ms-2" />
          <Collapse className="d-sm-inline-flex flex-sm-row-reverse" isOpen={!collapsed} navbar>
            <ul className="navbar-nav flex-grow">
              <NavItem>
                <NavLink tag={Link} className="text-dark" to='/'>Home</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to='/fetch-data'>Fetch data</NavLink>
              </NavItem>
              <NavItem>
                <NavLink tag={Link} className="text-dark" to='#' onClick={handleLogout}>Logout</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
}
