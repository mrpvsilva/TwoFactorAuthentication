import React, { useState } from 'react';
import { Collapse, Container, Navbar, NavbarBrand, NavbarToggler, NavItem, NavLink } from 'reactstrap';
import { Link } from 'react-router-dom';
import { useAuth } from '../AuthContext';
import './NavMenu.css';

export default function NavMenu() {
  const [collapsed, setCollapsed] = useState(true);
  const { logout } = useAuth();

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
                <NavLink tag={Link} className="text-dark" to='/reset-totp'>Reset TOTP</NavLink>
              </NavItem>
              <NavItem>
                <NavLink href='#' className="text-dark" onClick={(e) => { e.preventDefault(); logout(); }}>Logout</NavLink>
              </NavItem>
            </ul>
          </Collapse>
        </Container>
      </Navbar>
    </header>
  );
}
