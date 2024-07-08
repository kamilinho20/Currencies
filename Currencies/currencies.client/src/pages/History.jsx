import { useEffect, useState } from "react";
import { Container, Row, Col } from "react-bootstrap";
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import InputLabel from '@mui/material/InputLabel';
import FormControl from '@mui/material/FormControl';
import dayjs from "dayjs";
import Avatar from "@mui/material/Avatar";
import Stack from "@mui/material/Stack";
import Scatter from "../Scatter";
import ExchangesHistoryTable from "../ExchangesHistoryTable";
export default function History() {
    const initialDate = new Date();
    initialDate.setDate(initialDate.getDate() - 5);
    const [currency, setCurrency] = useState("EUR");
    const [dateFrom, setDateFrom] = useState(initialDate);
    const [dateTo, setDateTo] = useState(new Date());
    const [allCurrencies, setAllCurrencies] = useState([{ code: "EUR", name: "euro" }]);
    const [exchanges, setExchanges] = useState([]);
    useEffect(() => {
        getCurrencies();
    }, []);
    useEffect(() => {
        getExchangeHistory(currency, dateFrom, dateTo);
    }, [currency, dateFrom, dateTo]);


    return <Container className="mt-4">
        <Row className="justify-content-center align-items-center">
            <Col md="auto">
                <FormControl sx={{m: 1, minWidth: 240}}>
                    <InputLabel id="select-label">Currency</InputLabel>
                    <Select label="Currency" labelId="select-label" value={currency} onChange={e => setCurrency(e.target.value)}>
                        {allCurrencies.map(ac =>
                            <MenuItem key={ac.code} value={ac.code}><Stack alignItems="center" direction="row">
                                {ac.code === "XDR" ?
                                    <Avatar className="text-dark" sx={{ width: 32, height: 24 }}><small><small>XDR</small></small></Avatar>
                                    :
                                    <Avatar sx={{ width: 32, height: 24 }} className="border border-1" src={`https://wise.com/public-resources/assets/flags/rectangle/${ac.code.toLowerCase()}.png`} />}
                                <span className="ms-2">{ac.name}</span>
                            </Stack>
                            </MenuItem>
                        )}
                    </Select>
                </FormControl>
            </Col>
            <Col md="auto">
                <DatePicker maxDate={dayjs(dateTo)} label="Date from" value={dayjs(dateFrom)} onChange={newVal => setDateFrom(newVal.toDate())} />
            </Col>
            <Col md="auto">
                <DatePicker label="Date to" value={dayjs(dateTo)} onChange={newVal => setDateTo(newVal.toDate())} />
            </Col>
        </Row>
        <Row>
            <Scatter exchanges={exchanges} currency={currency} />
        </Row>
        <Row className="mt-3">
            <ExchangesHistoryTable exchanges={exchanges} />
        </Row>
    </Container>;

    async function getExchangeHistory(currency, dateFrom, dateTo) {
        const response = await fetch(`api/Currency/GetExchanges?currencyCode=${currency}&dateFrom=${dateFrom.toISOString().slice(0,10)}&dateTo=${dateTo.toISOString().slice(0,10)}`);
        const data = await response.json();
        setExchanges(data.exchangeCurrencyRates
            .map((c, i, a) => i == 0 ? { ...c, askGrow: 0, bidGrow: 0 } : { ...c, askGrow: c.askRate - a[i - 1].askRate, bidGrow: c.bidRate - a[i - 1].bidRate })
            .sort((a,b) => dayjs(b.exDate) - dayjs(a.exDate))
        );
    }

    async function getCurrencies() {
        const response = await fetch("api/Currency/GetAll");
        const data = await response.json();
        setAllCurrencies(data.filter(d => d.code !== "PLN"));
    }
}