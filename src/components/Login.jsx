import { useState } from "react";
import { Form, Alert, Container, Row, Col, Card, Button } from 'react-bootstrap';
import axios from 'axios';
import { useNavigate } from "react-router-dom";
import config from '../config.js';

function Login() {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const navigate = useNavigate();

    async function handleSubmit(e) {
        e.preventDefault();
        try {
            const response = await axios.post(`${config.API_URL}/User/authenticate`, { email, password });
            localStorage.setItem('token', response.data.token);
            navigate('/Dashboard');    
        } catch (error) {
            console.error( 'Login failed', error);
            setError('Invalid email or password');    
            }

    
    }

    return (
        <div>
            <Container className="mt-5">
                <Row className="justify-content-md-center">
                    <Col xs={12} md={6}>
                        <Card>
                            <Card.Body>
                                <h2 className="text-center mb-4">Login</h2>
                                {error && <Alert variant="danger">{error}</Alert>}
                                <Form onSubmit={handleSubmit}>
                                    <Form.Group className="mb-3" controlId="formBasicEmail">
                                        <Form.Label>
                                            Email address
                                        </Form.Label>
                                        <Form.Control 
                                            type="email" 
                                            placeholder="Enter email" 
                                            value={email} 
                                            onChange={(e) => setEmail(e.target.value)} 
                                            required
                                        />
                                    </Form.Group>
                                    <Form.Group className="mb-3" controlId="formBasicPassword">
                                        <Form.Label>Password</Form.Label>
                                        <Form.Control 
                                            type="password" 
                                            placeholder="Password" 
                                            value={password} 
                                            onChange={(e) => setPassword(e.target.value)} 
                                            required
                                        />
                                    </Form.Group>
                                    <Button variant="primary" type="submit" className="w-100">
                                        Login
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

export default Login;