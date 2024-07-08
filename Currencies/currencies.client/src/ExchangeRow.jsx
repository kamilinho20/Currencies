import { FaArrowRightLong } from "react-icons/fa6";
import Avatar from '@mui/material/Avatar';
import Stack from '@mui/material/Stack';
import ExchangeRate from "./ExchangeRate";

function CurrencyExchangeIcon({ code }) {
    return <Stack direction="row" spacing={1} className="justify-content-center">
        {code != "XDR" ?
            <Avatar sx={{ width: 36, height: 28 }} className="border border-1" src={`https://wise.com/public-resources/assets/flags/rectangle/${code.toLowerCase()}.png`} alt={code} />
            :
            <Avatar sx={{ width: 36, height: 28 }} className="text-dark"><small>{code}</small></Avatar>
        }
        <FaArrowRightLong className="my-auto" />
        <Avatar sx={{ width: 36, height: 28 }} className="border border-1" src={`https://wise.com/public-resources/assets/flags/rectangle/pln.png`} alt={code} />
    </Stack>;
}
export default function Exchange({ exchange }) {
    if (!exchange.exchangeCurrencyRates.length) return;
    const today = exchange.exchangeCurrencyRates[0];

    const { bidRate, askRate } = today;
    return <tr>
        <td className="h6 text-center">
            <CurrencyExchangeIcon code={exchange.code} />
        </td>
        <td className="h6 align-middle">{exchange.name} ({exchange.code})</td>
        <td className="h5 align-middle"><ExchangeRate rate={askRate} /></td>
        <td className="h5 align-middle"><ExchangeRate rate={bidRate} /></td>
    </tr>;
}

                