using FileTracking.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.OleDb;

namespace FileTracking
{
    public class DashboardController : Controller
    {
        private readonly dbFileTrackerContext context;
        public IConfiguration configuration { get; set; }
        private IWebHostEnvironment environment { get; set; }
        public DashboardController(IConfiguration configuration, IWebHostEnvironment environment, dbFileTrackerContext context)
        {
            this.configuration = configuration;
            this.environment = environment;
            this.context = context;
        }

        [HttpPost]
        public IActionResult Submit(IFormFile postedFile)
        {
            try
            {
                if (postedFile != null)
                {
                    //Create a Folder.
                    string path = Path.Combine(this.environment.WebRootPath, "Uploads");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    //Save the uploaded Excel file.
                    string fileName = Path.GetFileName(postedFile.FileName);
                    string filePath = Path.Combine(path, fileName);
                    using (FileStream stream = new FileStream(filePath, FileMode.Create))
                    {
                        postedFile.CopyTo(stream);
                    }

                    //Read the connection string for the Excel file.
                    string conString = configuration.GetSection("AppSettings").GetSection("ExcelConString").Value;
                    DataTable dt = new DataTable();
                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT BARCODE,FILENAME,DEPARTMENT,'" + DateTime.UtcNow + "' AS UPLOADDATE From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();
                            }
                        }
                    }

                    //Insert the Data read from the Excel file to Database Table.
                    conString = configuration.GetSection("AppSettings").GetSection("ConnectionString").Value;

                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.
                            sqlBulkCopy.DestinationTableName = "master";

                            //[OPTIONAL]: Map the Excel columns with that of the database table.
                            sqlBulkCopy.ColumnMappings.Add("BARCODE", "BARCODE");
                            sqlBulkCopy.ColumnMappings.Add("FILENAME", "FILENAME");
                            sqlBulkCopy.ColumnMappings.Add("DEPARTMENT", "DEPARTMENT");
                            sqlBulkCopy.ColumnMappings.Add("UPLOADDATE", "UPLOADDATE");

                            con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            con.Close();
                        }
                    }
                    ViewData["IsSuccess"] = true;
                    return View("Upload");
                }
                else
                {
                    ViewData["IsSuccess"] = false;
                    return View("Upload");
                }
            }
            catch (Exception ex)
            {
                ViewData["IsSuccess"] = false;
                return View("Search");
            }
        }
        public IActionResult Search(string barCode)
        {
            if (barCode == null)
            {
                return View();
            }
            var result = context.Masters.Where(x => x.Barcode == barCode).ToList();
            if (result.Count > 0)
            {
                ViewData["DataFound"] = false;
            }
            return View(result);
        }

        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Save(Master data)
        {
            try
            {
                if (data == null)
                {
                    ViewData["IsSave"] = false;
                    return View("Upload");
                }
                data.Uploaddate = DateTime.UtcNow;
                context.Masters.Add(data);
                var count = context.SaveChanges();
                if (count > 0)
                {
                    ViewData["IsSave"] = true;
                    return View("Upload");
                }
                ViewData["IsSave"] = false;
                return View("Upload");
            }
            catch (Exception)
            {
                ViewData["IsSave"] = false;
                return View("Upload");
            }
        }
    }
}
