import Header from "./Header/Header";
import { Container, Row, Col, Card, Button } from 'react-bootstrap';
import './LandingPage.css';

function HomePage() {
    return (
        <div className="home-page">
            <Header />
            <Container fluid className="mt-5">
                <Row className="justify-content-center text-center">
                    <Col md={8} lg={6}>
                        <h1 className="display-4 mb-4">Welcome to HouseOs</h1>
                        <p className="lead mb-5">Manage your household groceries with ease</p>
                    </Col>
                </Row>
                <Row className="justify-content-center">
                    <Col md={10} lg={8}>
                        <Card className="shadow-sm">
                            <Card.Body>
                                <h2 className="text-center mb-4 display-9">quick actions </h2>
                                <Row className="g-4 justify-content-center">
                                    <Col md={4}>
                                        <Button variant="outline-primary" size="lg" className="w-100 d-flex align-items-center justify-content-center" href="/Register">Register</Button>
                                    </Col>
                                    <Col md={4}>
                                        <Button variant="outline-success" size="lg" className="w-100 d-flex align-items-center justify-content-center" href="/Login">Log In</Button>
                                    </Col>
                                </Row>
                            </Card.Body>
                        </Card>
                        
                    </Col>
                </Row>

            </Container>
        </div>
    );
}

export default HomePage;