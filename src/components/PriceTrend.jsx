import React from 'react';
import { BarChart } from '@mui/x-charts/BarChart';
// import { Card } from 'react-bootstrap';



function PriceTrend({ PriceTrendData }) {
    

    // if (!PriceTrendData || typeof PriceTrendData !== 'object' || Object.keys(PriceTrendData).length === 0) {
    //     console.log('No data available');
    //     return (
    //         <Container>
    //             <Row>
    //                 <Col>
    //                     <Card>
    //                         <Card.Body>
    //                             <Card.Title>Price Trend</Card.Title>
    //                             <Card.Text>No data available</Card.Text>
    //                         </Card.Body>
    //                     </Card>
    //                 </Col>
    //             </Row>
    //         </Container>
    //     );
    // }

    const parseDate = (dateString) => {
        const date = new Date(dateString);
        return isNaN(date.getTime()) ? null : date;
    };

    const items = Object.keys(PriceTrendData);
    console.log('Items:', items);

    const allDates = [...new Set(Object.values(PriceTrendData).flatMap(data => 
        data.map(d => d.date)
    ))]
    .map(parseDate)
    .filter(date => date !== null)
    .sort((a, b) => a - b);

    console.log('All dates:', allDates);

    // if (allDates.length === 0) {
    //     console.log('No valid dates found');
    //     return (
    //         <Container>
    //             <Row>
    //                 <Col>
    //                     <Card>
    //                         <Card.Body>
    //                             <Card.Title>Price Trend</Card.Title>
    //                             <Card.Text>No valid dates found in the data</Card.Text>
    //                         </Card.Body>
    //                     </Card>
    //                 </Col>
    //             </Row>
    //         </Container>
    //     );
    // }

    const chartData = allDates.map(date => {
        const dataPoint = { date };
        items.forEach(item => {
            const priceData = PriceTrendData[item].find(p => parseDate(p.date)?.getTime() === date.getTime());
            dataPoint[item] = priceData ? priceData.price : null;
        });
        return dataPoint;
    });

    console.log('Chart data:', chartData);

    const series = items.map(item => ({
        dataKey: item,
        label: item,
        valueFormatter: (value) => value != null ? `£${value.toFixed(2)}` : 'N/A',
    }));

    console.log('Generated series:', series);

    const containerStyle = {
        display: 'flex',
        flexDirection: 'column',
        alignItems: 'center',
        justifyContent: 'center',
        padding: '1rem',
        marginBottom: '0.1rem',
        border: '1px solid #ccc',
        borderRadius: '5px',
        boxShadow: '0 2px 8px rgba(0, 0, 0, 0.1)',
        width: '105%',
        position: 'relative',
        right: '20px',
        down: '120px',
        
    };

    const chartStyle = {
        width: '100%',
        maxWidth: '1220px',
    };

    return (
        <div style={containerStyle} >
            <div style={chartStyle}>
                <h3>Price Trend</h3>
                <p>
                    This chart shows the price trend of items over time.
                </p>
                          
                                    <BarChart
                                        dataset={chartData}
                                        xAxis={[{
                                            scaleType: 'band',
                                            dataKey: 'date',
                                            tickLabelStyle: {fontSize: 12, angle: 0, textAnchor: 'start'},
                                            valueFormatter: (value) => {
                                                if (value instanceof Date) {
                                                    return value.toLocaleDateString(undefined, {year: 'numeric', month: 'short'});
                                                }
                                                return 'invalid date';
                                            }
                                        }]}
                                        series={series}
                                        yAxis={[{label: 'Price (£)'}]}
                                        slotProps={{
                                            tooltip: { show: true, snap: true, snapThreshold: 10, anchor: 'x', align: 'y' },
                                            legend:{
                                                direction: 'row',
                                                position: {vertical: 'top', horizontal: 'right'},
                                                padding: 10,
                                                itemMarkWidth: 20,  
                                                itemMarkHeight: 2,
                                                markGap: 5,
                                                itemGap: 10,
                                                labelStyle: {fontSize: 10},
                                            }
                                        
                                        }}
                                        layout="vertical"
                                        height={500}
                                        margin={{top: 50, right: 20, bottom: 70, left: 40}}
                                    />
                </div>
                       
                        
        </div>
    );
}

export default PriceTrend;