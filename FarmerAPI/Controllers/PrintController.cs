using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FarmerAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Print")]
    public class PrintController : Controller
    {       
        private IHostingEnvironment _hostingEnvironment;

        public PrintController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/Print
        [HttpGet]
        public FileStreamResult TestGet()
        {
            // 1. 開啟 Excel 檔案，取得工作簿
            var memoryStream = new MemoryStream();
            using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
            {
                var workbookPart = document.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "MySheet_1"
                });

                // 2. 取得工作簿中的工作表，並透過 Linq 判斷工作表是否存在
                //Sheet theSheet = document.WorkbookPart.Workbook.Descendants<Sheet>().
                //  Where(s => s.Name == "MySheet_1").FirstOrDefault();
                //var theSheet = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                //if (theSheet != null)
                //{
                //    // 3. 建立 Cell 物件，設定寫入位置，格式，資料
                //    Cell cell = new Cell()
                //    {
                //        CellReference = "C1",
                //        DataType = new EnumValue<CellValues>(CellValues.String),
                //        //CellValue = new CellValue("我是理查德")
                //    };

                //    // 4. 建立 Row 物件，將 Cell 加入
                //    Row theRow = new Row();
                //    //theRow.InsertAt(cell, 3);

                //    // 5. 將 Row 加入工作表中
                //    //Worksheet ws = ((WorksheetPart)(document.WorkbookPart.GetPartById(theSheet.Id))).Worksheet;
                //    //SheetData sheetData = ws.GetFirstChild<SheetData>();
                //    //sheetData.Append(theRow);
                //    theSheet.Append(theRow);
                //}

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();
                for (int i = 0; i < 10; i++)
                {
                    Row row = new Row();
                    for (int j = 0; j < 10; j++)
                    {
                        Cell dataCell = new Cell
                        {
                            CellValue = new CellValue($"{i + 1}行{j + 1}列"),
                            DataType = new EnumValue<CellValues>(CellValues.String)
                        };
                        row.AppendChild(dataCell);
                    }
                    sheetData.Append(row);
                }


                // Save to memorystream and close
                workbookPart.Workbook.Save();
                document.Close();         
            }

            string FileName = "stkp402-"+ DateTime.Now.ToString("yyyyMMdd") + ".xlsx";

            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            { FileDownloadName = FileName };
        }



        [HttpGet("[action]")]
        // Customer => 客戶代號
        // Variety  => 品種
        // Type => 種類
        // Brand => 品牌
        // ProductName => 品名
        // BeginDate => 起始交易日期
        // EndDate => 結束交易日期
        // PrintType => 列印別
        public FileResult DemoGet(string Customer, string Variety, string Type,
            string Brand, string PrintType, string ProductName, string Company = "公司名",
            string BeginDate = "2018-7-19", string EndDate = "2018-7-25")
        {                       
            // 轉日期格式
            BeginDate = Convert.ToDateTime(BeginDate).ToString("yyyy.MM.dd");
            EndDate = Convert.ToDateTime(EndDate).ToString("yyyy.MM.dd");

            // Open the demo file store on memory temporarily
            string DemoFile = _hostingEnvironment.ContentRootPath + @"\Templates\demo.xlsx";
            byte[] byteArray = System.IO.File.ReadAllBytes(DemoFile);
            MemoryStream memoryStream = new MemoryStream();
            memoryStream.Write(byteArray, 0, byteArray.Length);

            // Open a SpreadsheetDocument based on a stream.
            using (var document = SpreadsheetDocument.Open(memoryStream, true))
            {
                WorkbookPart workbookPart = document.WorkbookPart;

                // 字母表(Excel的行號)
                char[] alpha = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

                // 1. 取得工作簿中的工作表，並透過 Linq 判斷工作表是否存在
                // get sheet by name
                Sheet sheet = workbookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == "工作表1").FirstOrDefault();

                // get worksheetpart by sheet id
                WorksheetPart worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;

                // The SheetData object will contain all the data.
                SheetData sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // 2. 修改Excel內容資料
                if (sheetData != null)
                {           
                    // 修改表頭資訊
                    Cell CompanyCell = GetCell(sheetData, "A", 1);
                    CompanyCell.CellValue = new CellValue(Company);
                    CompanyCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    Cell TitleCell = GetCell(sheetData, "A", 2);
                    TitleCell.CellValue = new CellValue(BeginDate + "當日銷貨統計表");
                    TitleCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    Cell TypeCell = GetCell(sheetData, "E", 2);
                    TypeCell.CellValue = new CellValue("stkp402");
                    TypeCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    Cell MadeCell = GetCell(sheetData, "E", 4);
                    MadeCell.CellValue = new CellValue("製表日期：" + DateTime.Now.ToString("yyyy.MM.dd"));
                    MadeCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    Cell RangeCell = GetCell(sheetData, "A", 4);
                    RangeCell.CellValue = new CellValue("日期區間：" + BeginDate + " - " + EndDate);
                    RangeCell.DataType = new EnumValue<CellValues>(CellValues.String);

                    // 插入內容
                    int RowCount = 10;
                    int ColumnCount = 5;
                    int HeaderCount = 5;
                    for (int i = 0; i < RowCount; i++)
                    {
                        InsertRow(sheetData, (uint)(HeaderCount + i));        //表頭有5列，在其後加入空白Row在第6列
                        Row row = GetRow(sheetData, (uint)(HeaderCount + 1 + i)); //RowIndex開頭為1，加入的第6列RowIndex為6
                        for (int j = 0; j < ColumnCount; j++)
                        {
                            Cell dataCell = new Cell
                            {
                                CellValue = new CellValue($"{j + 1}行{i + 1}列"),

                                DataType = new EnumValue<CellValues>(CellValues.String),
                                CellReference = $"{alpha[j]}{row.RowIndex}"
                            };
                            row.AppendChild(dataCell);
                        }                      
                    }                    

                    // 修改合計的Row
                    Cell SumCountCell = GetCell(sheetData, "C", (uint)sheetData.Elements<Row>().Count());
                    SumCountCell.CellValue = new CellValue("1111");
                    SumCountCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                    Cell SumMomeyCell = GetCell(sheetData, "D", (uint)sheetData.Elements<Row>().Count());
                    SumMomeyCell.CellValue = new CellValue("1111");
                    SumMomeyCell.DataType = new EnumValue<CellValues>(CellValues.Number);
                }

                //於Memory中存檔
                document.Save();
            }

            //----輸出檔案----//
            string FileName = "當日銷貨統計表-" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
            //memory讀取位置歸零
            memoryStream.Seek(0, SeekOrigin.Begin);

            return new FileStreamResult(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
            { FileDownloadName = FileName };
        }

        private static Cell GetCell(SheetData sheetData, string columnName, uint rowIndex)
        {
            Row row = GetRow(sheetData, rowIndex);

            if (row == null) return null;

            var FirstRow = row.Elements<Cell>().Where(c => string.Compare
            (c.CellReference.Value, columnName +
            rowIndex, true) == 0).FirstOrDefault();

            if (FirstRow == null) return null;

            return FirstRow;
        }

        private static Row GetRow(SheetData sheetData, uint rowIndex)
        {
            Row row = sheetData
                .Elements<Row>().FirstOrDefault(r => r.RowIndex == rowIndex);
            if (row == null)
            {
                throw new ArgumentException(String.Format("No row with index {0} found in spreadsheet", rowIndex));
            }
            return row;
        }

        static void InsertRow(SheetData sheetData, uint rowIndex)
        {
            Row refRow = GetRow(sheetData, rowIndex);
            ++rowIndex;

            //Cell cell1 = new Cell() {
            //    CellReference = "A" + rowIndex,
            //    CellValue = new CellValue { Text = "" }
            //};         
            Row newRow = new Row()
            {
                RowIndex = rowIndex
            };
            //newRow.Append(cell1);
            for (int i = sheetData.Elements<Row>().Count(); i >= (int)rowIndex; i--)
            {
                var row = sheetData.Elements<Row>().Where(r => r.RowIndex.Value == i).FirstOrDefault();
                row.RowIndex++;
                foreach (Cell c in row.Elements<Cell>())
                {
                    string refer = c.CellReference.Value;
                    int num = Convert.ToInt32(Regex.Replace(refer, @"[^\d]*", ""));
                    num++;
                    string letters = Regex.Replace(refer, @"[^A-Z]*", "");
                    c.CellReference.Value = letters + num;
                }
            }
            sheetData.InsertAfter(newRow, refRow);
            //ws.Save();
        }

        // GET: api/Print/ResponseGet
        [HttpGet("[action]")]
        public HttpResponseMessage ResponseGet(int id)
        {
            /**responese解析成json，此法要再調整*/

            string DemoFile = _hostingEnvironment.ContentRootPath + @"/Templates/demo.xlsx";

            if (System.IO.File.Exists(DemoFile) || true)
            {
                var memoryStream = new MemoryStream();
                using (var document = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook))
                {
                    var workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    worksheetPart.Worksheet = new Worksheet(new SheetData());

                    var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    sheets.Append(new Sheet()
                    {
                        Id = workbookPart.GetIdOfPart(worksheetPart),
                        SheetId = 1,
                        Name = "Test Sheet 1"
                    });                 

                    workbookPart.Workbook.Save();
                    document.Close();

                    //    using (var fileStream = new FileStream(_hostingEnvironment.ContentRootPath + "test.xlsx", FileMode.Create))
                    //    {
                    //        memoryStream.WriteTo(fileStream);
                    //    }

                    //存到記憶體
                    //memoryStream.Seek(0, SeekOrigin.Begin); //功能同下
                    memoryStream.Position = 0; //Rewind the stream position back to zero, so it's ready for next reader;

                    //回傳封包
                    HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StreamContent(memoryStream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = "理查德" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx";
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

                    return response;
                }

                
            }
            return new HttpResponseMessage(HttpStatusCode.BadRequest);
        }
        
      
    }
}
