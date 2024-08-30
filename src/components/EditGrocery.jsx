import React, { useState, useEffect, useMemo } from 'react';
import { useParams, useLocation, useNavigate } from 'react-router-dom';
import axios from 'axios';
import { Form, Button, Alert, InputGroup, Container } from 'react-bootstrap';
import config from '../config';


function EditGrocery() {
    const { id } = useParams();
    const location = useLocation();
    const navigate = useNavigate();
    const [grocery, setGrocery] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [token, setToken] = useState('');

    useEffect(() => {
        const storedToken = localStorage.getItem('token');
        if (storedToken) {
            setToken(storedToken);
        }
    }, []);

    const axiosInstance = useMemo(() => axios.create({
        baseURL: config.API_URL,
        headers: {
            'Authorization': `Bearer ${token}`
        }
    }), [token]);

    useEffect(() => {
        const fetchGrocery = async () => {
            if (!token) return; // Don't fetch if there's no token
            
            setLoading(true);
            try {
                let groceryData;
                if (location.state?.grocery) {
                    groceryData = JSON.parse(location.state.grocery);
                } else {
                    const response = await axiosInstance.get(`/Grocery/${id}`);
                    groceryData = response.data;
                }
                setGrocery(groceryData);
            } catch (error) {
                console.error('Error fetching grocery:', error);
                setError('Failed to load grocery data. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        fetchGrocery();
    }, [id, location.state, axiosInstance, token]);

    const handleInputChange = (event) => {
        const { name, value } = event.target;
        setGrocery(prev => ({ ...prev, [name]: value }));
    };

    const handleSubmit = async (event) => {
        event.preventDefault();
        try {
            await axiosInstance.put(`/Grocery/${id}`, grocery);
            navigate('/Dashboard');
        } catch (error) {
            console.error('Error updating grocery:', error);
            setError('Failed to update grocery. Please try again.');
        }
    };

    if (loading) return <div>Loading...</div>;
    if (error) return <Alert variant="danger">{error}</Alert>;
    if (!grocery) return <Alert variant="warning">No grocery found</Alert>;

    return (
        <div>
        
        <Container className="mb-5 mt-3">
            
            <h2 className="mb-3">Edit Grocery</h2>
            <Form onSubmit={handleSubmit}>
                {/* Form fields remain the same */}
                <Form.Group className="mb-3">
                    <Form.Label htmlFor="name">Name</Form.Label>
                    <Form.Control
                        id="name"
                        name="name"
                        value={grocery.name}
                        onChange={handleInputChange}
                        required
                    />
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label htmlFor="quantity">Quantity</Form.Label>
                    <Form.Control
                        id="quantity"
                        name="quantity"
                        type="number"
                        min="0"
                        step="0.01"
                        value={grocery.quantity}
                        onChange={handleInputChange}
                        required
                    />
                </Form.Group>

                <Form.Group className="mb-3">
                    <Form.Label htmlFor="totalCost">Total Cost</Form.Label>
                    <InputGroup>
                        <InputGroup.Text>£</InputGroup.Text>
                        <Form.Control
                            id="totalCost"
                            name="totalCost"
                            type="number"
                            min="0"
                            step="0.01"
                            value={grocery.totalCost}
                            onChange={handleInputChange}
                            required
                        />
                    </InputGroup>
                </Form.Group>

                <Button variant="primary" type="submit" className="me-2">
                    Save Changes
                </Button>
                <Button variant="secondary" onClick={() => navigate('/Dashboard')}>
                    Cancel
                </Button>
            </Form>
        </Container>
    </div>
    )
}

export default EditGrocery;