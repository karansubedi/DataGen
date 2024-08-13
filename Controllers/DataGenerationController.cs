using DataGen.APIHelper;
using DataGen.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System.Text.Json;
using System.Text;

namespace DataGen.Controllers
{
        [Route("DataGeneration")]
        public class DataGenerationController : Controller
        {
            [HttpGet("create")]
            public IActionResult Index()
            {
                return View();
            }

            [HttpGet("sql")]  
            public IActionResult SQL()
            {
             return View();  
            }

            [HttpGet("chat")]
            public IActionResult Chat()
            {
             return View();
            }

        [HttpPost("/DataGeneration/upload")]
        public async Task<IActionResult> Upload(IFormFile sqlFile)
        {
            if (sqlFile == null || sqlFile.Length == 0)
            {
                return BadRequest("No File Uploaded.");
            }

            string filecontent;

            using(var streamReader = new StreamReader(sqlFile.OpenReadStream(), Encoding.UTF8))
            {
                filecontent = await streamReader.ReadToEndAsync();

                string token = await LLAMAAPIHelper.getToken();

                filecontent = string.Format("{0} I have a SQL table definition above for a table. Please generate a sample data set with 5 entries that would fit into this table. The data should include realistic values for each field, including valid dates, unique identification numbers, and email addresses. The CustomerID should auto-increment, starting at 1. Also, keep data heterogenous with nepalese names and others with keeping Null values so that data looks real. Please provide a file in response containing INSERT statements with realistic values for each field, including valid dates, unique identification numbers, and email addresses. The CustomerID should auto-increment, starting at 1.", filecontent);

                string answer = await LLAMAAPIHelper.PromptingLLAMA(filecontent, token);

                var apiResponse = JsonSerializer.Deserialize<ApiResponse>(answer);  

                if(apiResponse != null && apiResponse.status== 200)
                {
                    apiResponse.response = apiResponse.response.Replace("\n", "");
                    
                    int startIndex = apiResponse.response.IndexOf("```sql")+6;

                    var sqlContent = apiResponse.response.Substring(startIndex);

                    int endIndex = sqlContent.IndexOf(";```")+1;

                    sqlContent = sqlContent.Substring(0, endIndex);

                    var byteArray = System.Text.Encoding.UTF8.GetBytes(sqlContent);
                    var stream = new MemoryStream(byteArray);

                    
                    return File(stream, "application/sql", "kyc_sample_data.sql");

                }

                else
                {
                    return StatusCode(500, "Error processing the file.");
                }
            }
            
        }

        [HttpPost("/DataGeneration/submitDynamicForm")]
        public async Task<IActionResult> submitDynamicForm(string[] FieldName , string[] DataType, string[] FieldDescription)
        {

            var formData = new List<FormFieldData>();

            for(int i = 0; i < FieldName.Length; i++)
            {
                formData.Add(
                    new FormFieldData
                    {
                        FieldName = FieldName[i],
                        DataType = DataType[i], 
                        FieldDescription = FieldDescription[i]
                    });
            }

            var jsonString = JsonSerializer.Serialize(formData);

            string question = string.Format("Given the following JSON structure that describes the fields for a SQL table {0}" +
                " Please generate a SQL `INSERT INTO` statement with at least 20 different synthesized values for the FieldNames column. The generated values should " +
                "closely resemble common Nepalese origin, taking into account the examples provided in the FieldDescription. Ensure that the generated values are diverse, reflecting varities." +
                "The SQL structure should adapt to the number of fields provided in the JSON and generate at least 20 rows of synthesized data." +
                "For example, if the JSON contains `name` and `age`, the SQL should look like:```sqlINSERT INTO YourTableName (name, age) VALUES ('Name1', Age1), ('Name2', Age2), ... ;", jsonString);

            string token = await LLAMAAPIHelper.getToken();

            string answer = await LLAMAAPIHelper.PromptingLLAMA(question, token);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(answer);

            if (apiResponse.status == 200)
            {
                apiResponse.response = apiResponse.response.Replace("\n", "");

                int startIndex = apiResponse.response.IndexOf("```sqlINSERT") + 6;

                var sqlContent = apiResponse.response.Substring(startIndex);

                int endIndex = sqlContent.IndexOf(";```") + 1;

                sqlContent = sqlContent.Substring(0, endIndex);

                var byteArray = System.Text.Encoding.UTF8.GetBytes(sqlContent);
                var stream = new MemoryStream(byteArray);

                return File(stream, "application/sql", "data.sql");

            }
            else
            {
                return StatusCode(500, "Error processing the file.");
            }

        }

        [HttpPost("/DataGeneration/chatWithLLAMA")]
        public async Task<IActionResult> chatWithLLAMA(string chat)
        {
            
            string token = await LLAMAAPIHelper.getToken();

            string prompt = string.Format("I need to generate an SQL insert script based on the following description provided by the user: " +
                "'{0}'. The insert script should include at least 20 rows of data, with each row containing heterogeneous values." +
                " Identify the necessary columns for the dataset based on the description.    Assume reasonable data types for each column (e.g., INT, VARCHAR, DATE, etc.)." +
                " Generate the INSERT INTO SQL statement for the appropriate table.   Ensure that the values for each column are diverse to reflect heterogeneous data containing nepalese origin names, add null values as well." +
                "   Provide the final SQL script that includes the table creation and the insert statements.Example user input could be something like 'account data' or 'customer details'. Generate the SQL script accordingly without any create table scripts" +
                "also return sql without mentioning column names in values section but only in Insert into command.",chat);

            string answer = await LLAMAAPIHelper.PromptingLLAMA(prompt, token);

            var apiResponse = JsonSerializer.Deserialize<ApiResponse>(answer);

            if (apiResponse.status == 200)
            {
                apiResponse.response = apiResponse.response.Replace("\n", "");

                int startIndex = apiResponse.response.IndexOf("```sqlINSERT") + 6;

                var sqlContent = apiResponse.response.Substring(startIndex);

                int endIndex = sqlContent.IndexOf(";```") + 1;

                sqlContent = sqlContent.Substring(0, endIndex);

                var byteArray = System.Text.Encoding.UTF8.GetBytes(sqlContent);
                var stream = new MemoryStream(byteArray);

                return File(stream, "application/sql", "data.sql");

            }
            else
            {
                return StatusCode(500, "Error processing the file.");
            }
        }

        }
}
