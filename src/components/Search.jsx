import React, { useState, useEffect, useMemo } from 'react';
import axios from 'axios';
import { Form, Button, Alert, InputGroup, Container, Row, Col, Card } from 'react-bootstrap';

import { getLabelForOffset, organizeGroceriesByOffset, getOffsets, getOffsetFromDate} from '../utils/DateUtils';
import config from '../config';

function Search() {
    const [searchTerm, setSearchTerm] = useState('');
    const [searchResults, setSearchResults] = useState([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');
    const [token, setToken] = useState('');
    const [groceriesByOffset, setGroceriesByOffset] = useState({});
    const [, setDayOffsets] = useState([]);

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

    const handleSearch = async (e) => {
        e.preventDefault();
        setLoading(true);
        setError('');
        setSearchResults([]);

        try {
            const response = await axiosInstance.get(`/Search/search?name=${searchTerm}`);
            setSearchResults(response.data);
            if (response.data.length === 0) {
                setError('No results found.');
            } else {
                const groceriesByOffset = organizeGroceriesByOffset(response.data);
                setGroceriesByOffset(groceriesByOffset);
                const offsets = getOffsets(groceriesByOffset);
                setDayOffsets(offsets);
            }
        } catch (error) {
            console.error('Error searching:', error);
            setError('Failed to search. Please try again.');
        } finally {
            setLoading(false);
        }
    } 




    return (
        <div>
             
            <Container>
               

                <h2 className="mt-4 mb-3">Search Groceries</h2>
                <Form onSubmit={handleSearch}>
                    <InputGroup className="mb-3">
                        <Form.Control
                            type="text"
                            placeholder="Search for groceries"
                            value={searchTerm}
                            onChange={(e) => setSearchTerm(e.target.value)}
                        />
                        <Button type="submit" variant="primary">
                            {loading ? 'Searching...' : 'Search'}
                        </Button>
                    </InputGroup>
                </Form>

                
                {error && <Alert variant="danger">{error}</Alert>}

                {Object.entries(groceriesByOffset).map(([offset, groceries]) => (
                    <div key={offset}>
                        <Row xs={1} sm={2} md={3} lg={4} className="g-4 mt-3">
                            {searchResults.map((grocery) =>(
                                <Col key={grocery.id}>
                                <Card className="main-font grocery-card"> 
                                    <Card.Body className="bottom-line">
                                        <Card.Title className="text-primary">
                                            {grocery.name}
                                        </Card.Title>
                                        <Card.Text>
                                            <strong>Quantity: </strong>{grocery.quantity} <br />
                                            <strong>Unit Cost: </strong>£{grocery.unitCost.toFixed(2)} <br />
                                            <strong>Total Cost: </strong>£{grocery.totalCost.toFixed(2)} <br />
                                            
                                            
                                            <br />
                                            <footer className="blockquote-footer spacing">
                                                        {getLabelForOffset(getOffsetFromDate(grocery.createdAt), grocery.createdAt)}
                                                    </footer>
                                                    
                                        </Card.Text>
                                    </Card.Body>
                                </Card>
                            </Col>

                            ))}
                        </Row>
                    </div>


                ))}
            </Container>

        </div>

    );
}

export default Search;