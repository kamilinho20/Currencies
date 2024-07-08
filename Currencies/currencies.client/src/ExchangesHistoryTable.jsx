import { Table } from "react-bootstrap";
import ExchangeRate from "./ExchangeRate";
import dayjs from "dayjs";
export default function ExchangesHistoryTable({exchanges }) {
    return <Table striped bordered>
        <thead>
            <tr>
                <th>Date</th>
                <th>Ask rate</th>
                <th>Bid rate</th>
            </tr>
        </thead>
        <tbody>
            {exchanges.length ?
                exchanges.map((ex, i) => <tr key={i}>
                    <td>{dayjs(ex.exDate).toDate().toLocaleDateString()}</td>
                    <td>{<ExchangeRate grow={ex.askGrow} rate={ex.askRate} />}</td>
                    <td>{<ExchangeRate grow={ex.bidGrow} rate={ex.bidRate} />}</td>
                </tr>)
                :
                <tr>
                    <td colSpan="3">No data</td>
                </tr>
            }
        </tbody>
    </Table>;
}