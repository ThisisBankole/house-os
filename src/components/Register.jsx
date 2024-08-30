import { useState } from "react";
import { Form, Button, Container, Row, Col, Alert, Card } from "react-bootstrap";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import config from '../config.js';


function Register() {
    const [user, setUser] = useState({
        FirstName: '',
        LastName: '',
        Email: '',
        Password: ''
    });
    const [error, setError] = useState('');
    const navigate = useNavigate();
    const handleChange = (e) => {
        setUser(u => ({ ...u, [e.target.name]: e.target.value }));
    }

    async function handleSubmit(e) {
        e.preventDefault();
        try {
            await axios.post(`${config.API_URL}/User/add`, user);
            navigate('/Login');
        } catch (error) {
            console.error('Registration failed', error);
            setError('Registration failed');
        }
    }


    return (
        <div>
            <Container className="mt-5">
                <Row className="justify-content-md-center">
                    <Col xs={12} md={6}>
                        <Card>
                            <Card.Body>
                                <h2 className="text-center mb-4">Register</h2>
                                {error && <Alert variant="danger">{error}</Alert>}
                                <Form onSubmit={handleSubmit}>
                                   
                                    <Form.Group className="mb-3" controlId="formFirstName">
                                        <Form.Label>First Name</Form.Label>
                                        <Form.Control
                                            type="text"
                                            name="FirstName"
                                            placeholder="Enter first name"
                                            value={user.firstName}
                                            onChange={handleChange}
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="mb-3" controlId="formLastName">
                                        <Form.Label> Last Name </Form.Label>
                                        <Form.Control
                                            type="text"
                                            name="LastName"
                                            placeholder="Enter last name"
                                            value={user.lastName}
                                            onChange={handleChange}
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group className="mb-3" controlId="formEmail">
                                        <Form.Label>Email Address</Form.Label>
                                        <Form.Control
                                            type="email"
                                            name="Email"
                                            placeholder="Enter email"
                                            value={user.email}
                                            onChange={handleChange}
                                            required
                                        />
                                    </Form.Group>

                                    <Form.Group>
                                        <Form.Label>Password</Form.Label>
                                        <Form.Control
                                            type="password"
                                            name="Password"
                                            placeholder="Password"
                                            value={user.password}
                                            onChange={handleChange}
                                            required
                                        />
                                    </Form.Group>
                                    
                                    <Button variant="primary" type="submit" className="w-100">
                                        Register
                                    </Button>

                                </Form>
                            </Card.Body>
                        </Card>
                    </Col>
                </Row>
            </Container>

        </div>
    );

};

export default Register;