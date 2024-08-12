namespace DataGen.Model
{
        public class Root
        {
            public string? access_token { get; set; }
            public string? token_type { get; set; }
        }

    public class ApiResponse
    {
        public int status { get; set; }
        public string? message { get; set; }
        public string? response { get; set; }
    }
}
