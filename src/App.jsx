//import logo from './logo.svg';
import { BrowserRouter as Router, Route, Routes} from 'react-router-dom';
import './App.css';
import HomePage from './components/HomePage';
import Dashboard from './components/Dashboard';
import Login from './components/Login';
import Register from './components/Register';
import EditGrocery from './components/EditGrocery';
import Search from './components/Search';
import Analytics from './Analytics';
import Layout from './components/Layout';


//import { Switch } from 'nunjucks/src/nodes';

function App() {
  return (
    <Router>
      <Routes>
     
        <Route path="/" element={ <HomePage /> } />
        <Route path="/Login" element={ <Login /> } />
        <Route path="/Register" element={ <Register /> } />




        <Route element={ <Layout /> }>
          <Route path="/Dashboard" element={ <Dashboard /> } />
          
          <Route path="/edit/:id" element={ <EditGrocery /> } />
          <Route path="/search" element={ <Search /> } />
          <Route path="/analytics" element={ <Analytics /> } />
        </Route>
        
      </Routes>
    </Router>
  );
}

export default App;
