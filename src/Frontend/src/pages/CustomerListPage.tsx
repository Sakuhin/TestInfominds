import {
  Paper,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Typography,
  styled,
  tableCellClasses,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Button, Stack } from "@mui/material";

interface CustomerListQuery {
  id: number;
  name: string;
  address: string;
  email: string;
  phone: string;
  iban: string;
  code: string;
  description: string;
}

export default function CustomerListPage() {
  const [list, setList] = useState<CustomerListQuery[]>([]);
  const [searchText, setSearchText] = useState("");


useEffect(() => {
  fetch(`/api/customers/list?SearchText=${encodeURIComponent(searchText)}`)
    .then((response) => response.json())
    .then((data) => {
      setList(data as CustomerListQuery[]);
    });
}, [searchText]);

  const handleExportXml = () => {
  const xmlContent = `
<customers>
  ${list
    .map((c) => {
      return `
    <customer>
      <id>${c.id}</id>
      <name>${escapeXml(c.name)}</name>
      <address>${escapeXml(c.address)}</address>
      <email>${escapeXml(c.email)}</email>
      <phone>${escapeXml(c.phone)}</phone>
      <iban>${escapeXml(c.iban)}</iban>
      <code>${escapeXml(c.code)}</code>
      <description>${escapeXml(c.description)}</description>
    </customer>`;
    })
    .join("")}
</customers>`.trim();

  const blob = new Blob([xmlContent], { type: "application/xml" });
  const url = URL.createObjectURL(blob);
  const link = document.createElement("a");
  link.href = url;
  link.download = "customers.xml";
  link.click();
  URL.revokeObjectURL(url);
  
  // test
  console.log("XML Content:", xmlContent);
};

const escapeXml = (unsafe: string): string =>
  unsafe
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&apos;");

  return (
    <>
      <Typography variant="h4" sx={{ textAlign: "center", mt: 4, mb: 4 }}>
        Customers
      </Typography>

      <Stack direction="row" spacing={2} justifyContent="space-between" sx={{ mb: 2 }}>
        <input
          type="text"
          placeholder="Cerca per nome o email"
          value={searchText}
          onChange={(e) => setSearchText(e.target.value)}
          style={{ padding: '8px', flexGrow: 1 }}
        />
        <Button variant="outlined" onClick={handleExportXml}>
          Export XML
        </Button>
      </Stack>

      <TableContainer component={Paper}>
        <Table sx={{ minWidth: 650 }} aria-label="simple table">
          <TableHead>
            <TableRow>
              <StyledTableHeadCell>Name</StyledTableHeadCell>
              <StyledTableHeadCell>Address</StyledTableHeadCell>
              <StyledTableHeadCell>Email</StyledTableHeadCell>
              <StyledTableHeadCell>Phone</StyledTableHeadCell>
              <StyledTableHeadCell>Iban</StyledTableHeadCell>
              <StyledTableHeadCell>Code</StyledTableHeadCell>
              <StyledTableHeadCell>Description</StyledTableHeadCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {list.map((row) => (
              <TableRow
                key={row.id}
                sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
              >
                <TableCell>{row.name}</TableCell>
                <TableCell>{row.address}</TableCell>
                <TableCell>{row.email}</TableCell>
                <TableCell>{row.phone}</TableCell>
                <TableCell>{row.iban}</TableCell>
                <TableCell>{row.code}</TableCell>
                <TableCell>{row.description}</TableCell>

              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>
    </>
  );

}

const StyledTableHeadCell = styled(TableCell)(({ theme }) => ({
  [`&.${tableCellClasses.head}`]: {
    backgroundColor: theme.palette.primary.light,
    color: theme.palette.common.white,
  },
}));


