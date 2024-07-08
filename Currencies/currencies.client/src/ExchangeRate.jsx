import Chip from "@mui/material/Chip";
import { FaAngleUp, FaAngleDown, FaMinus } from "react-icons/fa6";

const formatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'PLN',
    minimumFractionDigits: 4,
    maximumFractionDigits: 4
});

export default function ExchangeRate({grow, rate}) {
    if (grow > 0.0) {
        return <Chip color="success" icon={<FaAngleUp />} variant="outlined" label={formatter.format(rate)} />;
    } else if (grow === 0.0) {
        return <Chip color="default" icon={<FaMinus />} variant="outlined" label={formatter.format(rate)} />;
    } else if (grow < 0.0) {
        return <Chip color="error" icon={<FaAngleDown />} variant="outlined" label={formatter.format(rate)} />;
    }
    return <Chip color="default"  label={formatter.format(rate)} />;
};