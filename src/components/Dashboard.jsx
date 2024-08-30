import { Nav , Dropdown, Alert, Card, Row, Col, Modal, Button, Form, Offcanvas, Container, Spinner, ProgressBar} from "react-bootstrap";
import { useState, useEffect, useCallback, useMemo } from "react";
import axios from "axios";
// import MainHeader from "./Header/MainHeader";

import { useNavigate } from "react-router-dom";
import { Receipt } from 'react-bootstrap-icons';
import config from "../config";



function Dashboard() {
    const [groceries, setGroceries] = useState({});
    const [dayOffset, setDayOffset] = useState([]);
    const [currentDayOffset, setCurrentDayOffset] = useState(null);
    const [, setStartDate] = useState(new Date());
    const [hasGroceries, setHasGroceries] = useState(false);
    const [, setMessage] = useState('');
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [deleteGroceryId, setDeleteGroceryId] = useState('');
    const [showOffcanvas, setShowOffcanvas] = useState(false);
    const [selectedGrocery, setSelectedGrocery] = useState('');
    const [scannedGroceries, setScannedGroceries] = useState([]);
    const [showScanModal, setShowScanModal] = useState(false);
    const [token, setToken] = useState('');
    const [isUploading, setIsUploading] = useState(false);
    const [uploadProgress, setUploadProgress] = useState(0);
    const navigate = useNavigate();

    useEffect(() => {
        const storedToken = localStorage.getItem('token');
        if (storedToken) {
            setToken(storedToken);
        }
    }, []);

  

    const axiosInstance = useMemo( () => axios.create({
        baseURL: config.API_URL,
        headers: {
            'Authorization': `Bearer ${token}`
        }
    }), [token]);
    

   
    useEffect(() => {
            const interceptor = axiosInstance.interceptors.request.use((config)=> {
            config.headers['Authorization'] = `Bearer ${token}`;
            return config;
        });

        return () => {
            axiosInstance.interceptors.request.eject(interceptor);
        };
   
    }, [axiosInstance, token]);
   

    const getGroceries = useCallback( async ()  => {
        if(!token) return;
        try {
            const response = await axiosInstance.get('/Grocery');
            const allGroceries = response.data;

            if (allGroceries.length === 0) {
                setHasGroceries(false);
                setMessage('You don\'t have any groceries added yet.');
                return;
            }

            setHasGroceries(true);
            const now = new Date();
            now.setHours(0, 0, 0, 0);


            const minDate = new Date(Math.min(...allGroceries.map(g => new Date(g.createdAt))));
            minDate.setHours(0, 0, 0, 0);
            setStartDate(minDate);

            const groceriesByOffset = allGroceries.reduce((acc, grocery) => {
                const createdAt = new Date(grocery.createdAt);
                createdAt.setHours(0, 0, 0, 0);
                const offset = Math.floor(( now - createdAt) / (1000 * 60 * 60 * 24));
                if (!acc[offset]) {
                    acc[offset] = [];
                }
                acc[offset].push(grocery);
                return acc;
            }, {});


        setGroceries(groceriesByOffset);
        const offsets = Object.keys(groceriesByOffset).map(Number).sort((a, b) => a - b);
        setDayOffset(offsets.map(offset => ({
            offset,
            label: getLabelForOffset(offset, now)
        })));

        if (currentDayOffset === null && offsets.length > 0) {
            setCurrentDayOffset(offsets[0]);
        }

        } catch (error) {
            console.error('Failed to get groceries', error);
            setMessage('Failed to get groceries');
        }
    }, [axiosInstance, token, currentDayOffset]);
        


    useEffect(() => {
        if (token) {
            getGroceries();
        }
    }, [token, getGroceries]);


    // useEffect(() => {
    //     getGroceries();
    // }, [currentDayOffset, getGroceries]);

    function getLabelForOffset(offset, now) {
        if (offset === 0) return 'Today';
        if (offset === 1) return 'Yesterday';
        const date = new Date(now.getTime() - offset * 24 * 60 * 60 * 1000);
        return date.toLocaleDateString();
    };

    async function handleDeleteGrocery() {
        try {
            await axiosInstance.delete(`/Grocery/${deleteGroceryId}`);
            setMessage('Grocery deleted successfully');
            setShowDeleteModal(false);
            getGroceries();
        } catch (error) {
            console.error('Failed to delete grocery', error);
            setMessage('Failed to delete grocery');
        } }

    const handleEditClick = (grocery) => {
        navigate(`/edit/${grocery.id}`, { state: { grocery: JSON.stringify(grocery) } });
        setShowOffcanvas(false);
    };

    async function handleFileUpload(e){
        const file = e.target.files[0];
        if(!file)return;

        setIsUploading(true);
        setUploadProgress(0);
        setMessage('');

        const formData = new FormData();
        formData.append('file', file);

        try {
            const response = await axiosInstance.post('/Grocery/Scan', formData, {
                headers: {'Content-Type': 'multipart/form-data',
                            'Authorization' : `Bearer ${token}`

                },
                onUploadProgress: (progressEvent) => {
                    const percentCompleted = Math.round((progressEvent.loaded * 100) / progressEvent.total);
                    setUploadProgress(percentCompleted);
                }
            });
            if(response.data && response.data.length > 0) {

                setScannedGroceries(response.data);
                setShowScanModal(true);
                setMessage('Groceries scanned successfully');

                await getGroceries();
            } else {
                setMessage('No groceries found in the image');

            }
        } catch (error) {
            console.error('Failed to scan groceries', error);
            setMessage('Failed to scan groceries: ' + error.message);
        } finally {
            setIsUploading(false);
            setUploadProgress(0);
        }
        
    }

    // function handleScanGrocery() {
    //     setGroceries(prev => ({
    //         ...prev,
    //         [new Date().toISOString()]: [ ...scannedGroceries, ...(prev[new Date().toISOString()] || [])]
    //     }));
    //     setShowScanModal(false);
    //     getGroceries();
    // }


    function showGroceryDetails(grocery) {
        setSelectedGrocery(grocery);
        setShowOffcanvas(true);
    }


    return (
        <div >

           {/* <MainHeader/> */}

           <div >
        
                <Container fluid className="card-container">
                    <Nav variant="tabs" className="mb-3 main-font position-relative">
                        {dayOffset.slice(0,6).map( (day, index) => (
                            <Nav.Item key={index}>
                                <Nav.Link
                                    active={currentDayOffset === day.offset}
                                    onClick={() => setCurrentDayOffset(day.offset)}
                                >
                                    {day.label}
                                </Nav.Link>
                            </Nav.Item>
                        ))}

                        {dayOffset.length > 6 && (
                            
                            <Dropdown as={Nav.Item}>
                                <Dropdown.Toggle as={Nav.Link}>More</Dropdown.Toggle>
                                <Dropdown.Menu>
                                    {dayOffset.slice(6).map((day, index) => (
                                        <Dropdown.Item
                                            key={index + 6}
                                            
                                            onClick={() => setCurrentDayOffset(day.offset)}
                                        
                                        >
                                            {day.label}

                                        </Dropdown.Item>
                                    ))}
                                </Dropdown.Menu>


                            </Dropdown>
                        )}
                        <Nav.Item className="ms-auto">
                            <Form.Label htmlFor="file-upload" className="mb-0">
                                <Button 
                                    variant="primary"
                                    className="main-font d-flex align-items-center"
                                    disabled={isUploading}
                                    as="div"
                                >
                                    {isUploading ? (
                                        <Spinner animation="border" size="sm" className="me-2" />
                                    ) : (
                                        <Receipt size={24} className="me-2" />
                                    )}
                                    Upload Receipt

                                </Button>
                                <Form.Control
                                    type="file"
                                    id="file-upload"
                                    accept="image/*"
                                    onChange={handleFileUpload}
                                    style={{display: 'none'}}
                                    disabled={isUploading}
                                />
                            </Form.Label>

                        </Nav.Item>
                    </Nav>
            
                    {!hasGroceries ? ( 
                        <Alert variant="info">You don't have any groceries added yet.</Alert>
                        ) : (
                        <div>
                        
                            <Row xs={1} sm={2} md={3} lg={4} className="g-4">
                                {groceries[currentDayOffset]?.map((grocery) => (
                                    <Col key={grocery.id}>
                                        <Card
                                            className="main-font grocery-card"
                                        
                                            onClick={() => showGroceryDetails(grocery)}
                                        >
                                            <Card.Body className="bottom-line">
                                                <Card.Title className="text-primary">
                                                    {grocery.name}
                                                </Card.Title>
                                                <Card.Text className="main-font">
                                                    <strong>Quantity: </strong>{grocery.quantity} <br />
                                                    <strong>Unit Cost: </strong>£{grocery.unitCost.toFixed(2)} <br />
                                                    <strong>Total Cost: </strong>£{grocery.totalCost.toFixed(2)} <br />
                                                    {/* <br />
                                                    <br />
                                                    <footer className="blockquote-footer spacing">
                                                        {getLabelForOffset(currentDayOffset, startDate)}
                                                    </footer> */}
                                                </Card.Text>
                                            </Card.Body>
                                        </Card>
                                    </Col>
                                ))}
                            </Row>
                        </div>
                    )}
                </Container>


                    <Modal show={showDeleteModal} onHide={() => setShowDeleteModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Confirm Delete</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            Are you sure you want to delete this grocery?
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowDeleteModal(false)}>
                                No
                            </Button>
                            <Button variant="danger" onClick={handleDeleteGrocery}>
                                Yes
                            </Button>
                        </Modal.Footer>
                    </Modal>

                    <Offcanvas 
                        show={showOffcanvas} 
                        onHide={() => setShowOffcanvas(false)}
                        placement="top"
                        className="offcanvas-custom-top"
                        
                    >
                        <Offcanvas.Header closeButton>
                            <Offcanvas.Title>{selectedGrocery?.name}</Offcanvas.Title>
                        </Offcanvas.Header>
                        <Offcanvas.Body>
                            {selectedGrocery && (
                                <>
                                <div className="border-bottom">
                                <p><strong>Quantity:</strong> {selectedGrocery.quantity}</p>
                                <p><strong>Unit Cost:</strong> {selectedGrocery.unitCost}</p>
                                <p><strong>Total Cost:</strong> {selectedGrocery.totalCost}</p>
                                <br />
                                <br />
                                </div>
                                <br />
                                <div className="d-flex flex-column align-items-center mt-3">
                                <Button
                                    className="btn-custom-lg mb-3"
                                    variant="danger"
                                    onClick={() => {
                                    setDeleteGroceryId(selectedGrocery.id);
                                    setShowDeleteModal(true);
                                    setShowOffcanvas(false);
                                    }}
                                >
                                    Delete
                                </Button>
                                <Button
                                    variant="primary"
                                    className="btn-custom-lg"
                                    onClick={() => handleEditClick(selectedGrocery)}
                                >
                                    Edit
                                </Button>
                                </div>
                                </>
                            )}
                        </Offcanvas.Body>
                    </Offcanvas>

                    <Modal show={showScanModal} onHide={() => setShowScanModal(false)}>
                        <Modal.Header closeButton>
                            <Modal.Title>Scanned Groceries</Modal.Title>
                        </Modal.Header>
                        <Modal.Body>
                            <div className="border-bottom">
                                {scannedGroceries.map((grocery, index) => (
                                    <p key={index}> {grocery.name}: {grocery.quantity} x ${grocery.unitCost} </p>
                                ))}   

                            </div>   
                        </Modal.Body>
                        <Modal.Footer>
                            <Button variant="secondary" onClick={() => setShowScanModal(false)}>
                                Close
                            </Button>
                        </Modal.Footer>
                    </Modal>

                    <Modal show={isUploading} centered backdrop="static" keyboard={false}>
                        <Modal.Body className="text-center">
                            <Spinner animation="border" role="status" className="mb-2" >
                                <span className="visually-hidden">Loading...</span>
                            </Spinner>
                            <p>Extraction in progress...</p>
                            <ProgressBar now={uploadProgress} label={`${uploadProgress}%`} />
                            
                        </Modal.Body>

                    </Modal>

             </div>
        </div>
    );
}

export default Dashboard;