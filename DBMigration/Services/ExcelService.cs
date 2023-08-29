using DBMigration.Models;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Data;

namespace DBMigration.Services
{
    public class ExcelService : IExcelService
    {
        public void SaveDataSetAsExcel(DataSet dataset, string excelFilePath)
        {
            using (SpreadsheetDocument document = SpreadsheetDocument.Create(excelFilePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();
                Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                UInt32Value sheetCount = 0;

                foreach (DataTable table in dataset.Tables)
                {
                    sheetCount++;

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    Worksheet worksheet = new Worksheet();
                    SheetData sheetData = new SheetData(); // worksheetPart.Worksheet.GetFirstChild<SheetData>();
                    ConvertDataTableToWorksheet(table, ref sheetData);
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = sheetCount, Name = table.TableName };
                    sheets.AppendChild(sheet);
                    workbookPart.Workbook.Save();
                }
                document.Save();
            }
        }

        public Row ConvertDataRowToSheetRow(DataRow dataRow, string[] columns) {
            Row newRow = new Row();
            foreach (string col in columns)
            {
                Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dataRow[col].ToString());
                            newRow.AppendChild(cell);
            }

            return newRow;
        }

        public void ConvertDataTableToWorksheet(DataTable dataTable, ref SheetData sheetData) {
            Row headerRow = new Row();

            List<string> columns = new List<string>();
            foreach (System.Data.DataColumn column in dataTable.Columns)
            {
                columns.Add(column.ColumnName);

                Cell cell = new Cell();
                cell.DataType = CellValues.String;
                cell.CellValue = new CellValue(column.ColumnName);
                headerRow.AppendChild(cell);
            }

            sheetData.AppendChild(headerRow);
            

            foreach (DataRow dsrow in dataTable.Rows)
            {
                sheetData.AppendChild(ConvertDataRowToSheetRow(dsrow, columns.ToArray()));
            }
        }

    }
}
