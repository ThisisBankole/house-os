import { Navbar, Nav, Container } from 'react-bootstrap';

import './Header.css';

function Header() {
    return(
        <Navbar bg="primary" expand="lg" data-bs-theme="dark">
        <Container>
          <Navbar.Brand href="#home">HouseOs</Navbar.Brand>
          <Navbar.Toggle aria-controls="basic-navbar-nav" />
          <Navbar.Collapse id="basic-navbar-nav">
            <Nav className="me-auto">
              <Nav.Link href="#groceries">
                Groceries
              </Nav.Link>
              <Nav.Link href="#add-item">
                Add Item
              </Nav.Link>
              <Nav.Link href="#scan-receipt">
                Scan Receipt
              </Nav.Link>
            </Nav>
          </Navbar.Collapse>
        </Container>
      </Navbar>

    );

}

export default Header;