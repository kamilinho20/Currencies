import { Nav, Navbar, Container} from "react-bootstrap";
import { BrowserRouter as Router, Route, Routes, NavLink } from "react-router-dom";

import './App.css';
import Today from './pages/Today';
import History from './pages/History';
import { LocalizationProvider } from '@mui/x-date-pickers';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs'

function App() {
    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <Router>
                <Navbar expand="lg" className="navbar-dark bg-dark w-100">
                    <Container fluid className=" d-flex justify-content-center">
                        <Navbar.Brand className="px-2 ms-4"><span className="m-0 h4">NBP Exchange Rates</span></Navbar.Brand>
                        <Navbar.Toggle aria-controls="basic-navbar-nav" />
                        <Navbar.Collapse className="mx-auto">
                            <Nav activeKey="/">
                                <Nav.Item>
                                    <NavLink className="nav-link" to="/" end>Most recent</NavLink>
                                </Nav.Item>
                                <Nav.Item>
                                    <NavLink className="nav-link" to="/historical">History</NavLink>
                                </Nav.Item>

                            </Nav>
                        </Navbar.Collapse>
                    </Container>
                </Navbar>
                <Routes>
                    <Route exact path="/" element={<Today />} />
                    <Route path="/historical" element={<History />} />
                </Routes>
            </Router>
        </LocalizationProvider>
    );
}

export default App;