import React from 'react';
import { Outlet } from 'react-router-dom';
import MainHeader from './Header/MainHeader'; // Adjust this path if necessary

function Layout() {
  return (
    <div className="app-container">
      <MainHeader />
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  );
}

export default Layout;



// className="dash-page"
// className="dash-main-content"
// className="analytics-page"
// className="main-content"