import { Offcanvas, Nav, } from 'react-bootstrap';
import { useNavigate } from 'react-router-dom';
import { useState } from 'react';
import { Menu, CircleGauge, ShoppingBasket, Search, LogOut } from 'lucide-react';

import './Header.css';

function MainHeader() {
    const navigate = useNavigate();
    const [show, setShow] = useState(false);

    function handleClose() {

        setShow(false);
        document.querySelector('.toggle-button').classList.remove('hidden');
    }

    function handleShow() {
        setShow(true);
        document.querySelector('.toggle-button').classList.add('hidden');
    }

    function handleSearch() {
        navigate('/search');
    }

    function handleGroceries() {
        navigate('/Dashboard');
    }

    function handleLogout() {
        localStorage.removeItem('token');
        navigate('/Login');
    }

    function handleAnalytics() {
        navigate('/analytics');
    }

    function SidebarContent () {
        return(
          <>
                <div className='sidebar-header'>
                  <h3 className>owa</h3>
                </div>
                <Nav className='flex-column nav-links'>
                      <Nav.Link 
                        onClick={handleAnalytics}
                        className='d-flex p-3' 
                      >
                          <CircleGauge size={20} className='me-2'/>
                          Dashboard
                      </Nav.Link>
                      <Nav.Link onClick={handleGroceries} className='d-flex p-3' >
                          <ShoppingBasket size={20} className='me-2'/>
                          Groceries
                      </Nav.Link>
                      <Nav.Link onClick={handleSearch} className='d-flex p-3' >
                          <Search size={20} className='me-2'/>
                          Search
                      </Nav.Link>
                      <Nav.Link onClick={handleLogout} className='d-flex p-3' >
                          <LogOut size={20} className='me-2'/>
                          Logout
                      </Nav.Link>
                  </Nav>
          
          </>

        );
    }

    return(

      <div>

          <Menu 
            onClick={handleShow} 
            className="d-lg-none toggle-button top-0 start-0 m-3" 
            size={24}/>

          <Offcanvas 
            show={show} 
            onHide={handleClose} 
            responsive='lg'
            className= 'flex-column position-fixed sidebar bg-gray-500'

            
          >
              <Offcanvas.Header closeButton>
              </Offcanvas.Header>

              <Offcanvas.Body className='p0 d-flex flex-column'>
                  <SidebarContent />
              </Offcanvas.Body>
          </Offcanvas>
         
      </div>
      
      



      
      // {/* // <div className='sidebar'>
      // //   <div className='sidebar-header'>
      // //     <h4>owa</h4>
      // //   </div>
      // //   <Nav className='flex-column'>
      // //     <Nav.Link onClick={handleAnalytics}>
      // //             Dashboard
      // //           </Nav.Link>
      // //           <Nav.Link onClick={handleGroceries} >
      // //             Groceries
      // //            </Nav.Link>
      // //            <Nav.Link onClick={handleSearch} >
      // //              Search
      // //             </Nav.Link>
      // //            <Nav.Link onClick={handleLogout}>
      // //             Logout
      // //           </Nav.Link>

      // //   </Nav>

      // // </div> */}

    );

}

export default MainHeader;