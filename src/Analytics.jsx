import { Container, Row, Col, Dropdown, Card} from "react-bootstrap";
import { useState, useEffect, useMemo } from "react";
import axios from "axios";
// import MainHeader from "./components/Header/MainHeader";
import PriceTrend from "./components/PriceTrend";
import config from "./config";


const timePeriods = [
    // { value: 'today', label: 'Today' },
    { value: 'thisWeek', label: 'Last 7 days' },
    { value: 'thisMonth', label: 'Last month' },
    { value: 'thisYear', label: 'Last year' },
    { value: 'allTime', label: 'All time' }
];



function Analytics() {
    const [analyticsData, setAnalyticsData] = useState(null);
    const [countPeriod, setCountPeriod] = useState('thisWeek');
    const [costPeriod, setCostPeriod] = useState('thisWeek');
    const [token, setToken] = useState('');
    const [priceTrend, setPriceTrend] = useState(null);


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
        const fetchAnalyticsData = async () => {
            if (!token) return; // Don't fetch if there's no token

            try {
                const [cardResponse, priceTrendResponse] = await Promise.all([
                        axiosInstance.get('/Analytics'),
                        axiosInstance.get('Analytics/pricetrends')
                ]);
                setAnalyticsData(cardResponse.data);
                setPriceTrend(priceTrendResponse.data);
                console.log(cardResponse.data);
                console.log(priceTrendResponse.data);
                
            } catch (error) {
                console.error('Error fetching grocery count:', error);
            }
        };

        fetchAnalyticsData();
    }, [axiosInstance, token]);

   


    const handlePeriodChange = (type, period) => {
        if (type === 'count') {
            setCountPeriod(period);
        } else {
            setCostPeriod(period);
        }
    }

    const getDisplayData = (period, type) => {
        console.log('Getting display data for', period, type);
        if (!analyticsData || !analyticsData[period]) 
            {
                console.log('No data for', period);
                return 0; 
            }
            const value = type === 'count' ? analyticsData[period].count : analyticsData[period].totalSpent;
            console.log('Value:', value);
            return value;
    }

    return (
        <div >

                {/* <MainHeader /> */}

            <div >
             
                <Container fluid className="card-container">
            
                    <Row>
                        <div className="card-ctn-a">
                            <Col md={4} className="mb-3">
                                <Card className="main-font grocery-card">
                                    <Card.Body>
                                        <Card.Title >
                                            Grocery Count
                                        </Card.Title>
                                        <Card.Text className="display-4 ac-text">{getDisplayData(countPeriod, 'count')}</Card.Text>
                                        <Dropdown className="mt-3">
                                            <Dropdown.Toggle  id="dropdown-count">
                                                {timePeriods.find(p => p.value === countPeriod).label}
                                            </Dropdown.Toggle>

                                            <Dropdown.Menu className="a-dropdown">
                                                {timePeriods.map(period => (
                                                    <Dropdown.Item 
                                                        key={period.value} 
                                                        onClick={() => handlePeriodChange('count', period.value)}>
                                                        {period.label}
                                                    </Dropdown.Item>
                                                ))}
                                            </Dropdown.Menu>

                                        </Dropdown>
                                    </Card.Body>
                                </Card>
                            </Col>
                            <Col md={4} className="mb-3">
                                <Card  className="main-font grocery-card">
                                    <Card.Body>
                                        <Card.Title>
                                            Total Spent
                                        </Card.Title>
                                        <Card.Text className="display-4 ac-text">£{getDisplayData(costPeriod, 'cost')}</Card.Text>
                                        <Dropdown className="mt-3 ">
                                            <Dropdown.Toggle  id="dropdown-cost">
                                                {timePeriods.find(p => p.value === costPeriod).label}
                                            </Dropdown.Toggle>

                                            <Dropdown.Menu className="">
                                                {timePeriods.map(period => (
                                                    <Dropdown.Item 
                                                        key={period.value} 
                                                        onClick={() => handlePeriodChange('cost', period.value)}>
                                                        {period.label}
                                                    </Dropdown.Item>
                                                ))}
                                            </Dropdown.Menu>

                                        </Dropdown>

                                    </Card.Body>
                                </Card>
                            </Col>
                        </div>
                    </Row>
                    <Row className="justify-content-center main-font">
                        <Col xs={12} md={10} lg={8} >
                            {priceTrend && <PriceTrend PriceTrendData={priceTrend} />}
                        </Col>
                    </Row>

                </Container>
            </div>
        </div>
    )
    






}

export default Analytics;