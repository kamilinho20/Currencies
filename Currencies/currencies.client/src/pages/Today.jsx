import { Container, Row, Col, Table } from "react-bootstrap";
import { useState, useEffect } from "react";
import Exchange from "../ExchangeRow";
export default function Today() {
    const [exchanges, setExchanges] = useState();

    useEffect(() => {
        getExchangesData();
    }, []);

    return <Container className="mt-4">
        <Row>
            <Col>
                <Table striped bordered>
                    <thead>
                        <tr>
                            <th>Symbol</th>
                            <th>Currency</th>
                            <th>Ask price</th>
                            <th>Bid price</th>
                        </tr>
                    </thead>
                    <tbody>
                        {exchanges ? exchanges.map(ex => <Exchange key={ex.code} exchange={ex} />) : <tr><th colSpan="4">Loading...</th></tr>}
                    </tbody>
                </Table>
            </Col>
        </Row>
    </Container>;

    async function getExchangesData() {
        const response = await fetch('api/Currency/GetLast');
        const data = await response.json();
        setExchanges(data);
    }
}

