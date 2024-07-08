import dayjs from "dayjs";
import { ScatterChart } from '@mui/x-charts/ScatterChart';

export default function Scatter({ exchanges, currency }) {
    const currencyFormatter = new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency,
        minimumFractionDigits: 4
    }).format;
    return (
        <ScatterChart
            height={300}
            series={[
                {
                    label: 'Ask rate',
                    data: exchanges.map((ex, i) => ({ x: dayjs(ex.exDate).toDate(), y: ex.askRate, id: i })),
                    valueFormatter: (v) => `${dayjs(v.x).format('ll')} ${v === null ? '-' : currencyFormatter(v.y)}`,
                    highlightScope: {
                        highlighted: 'item',
                        faded: 'global'
                    }
                },
                {
                    label: 'Bid rate',
                    data: exchanges.map((ex, i) => ({ x: dayjs(ex.exDate).toDate(), y: ex.bidRate, id: i })),
                    valueFormatter: (v) => `${dayjs(v.x).format('ll')} ${v === null ? '-' : currencyFormatter(v.y)}`,
                    highlightScope: {
                        highlighted: 'item',
                        faded: 'global'
                    }
                },
            ]}
            grid={{ vertical: true, horizontal: true }}
            xAxis={[{
                scaleType: 'time',
                tickMinStep: 3600 * 1000 * 24,
                valueFormatter: (value, context) => context.location === 'tick'
                    ? dayjs(value).format('DD/MM')
                    : dayjs(value).format('ll'),
            }]}
            tooltip={{
                trigger: 'item'
            }} />
    );
}
