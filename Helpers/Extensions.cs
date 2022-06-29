using System.Text.Json;

namespace navision.api.Helpers
{
  public static class Extensions
    {
        public static void AddApplicationError(this HttpResponse response, string message){
            response.Headers.Add("Application-Error", message);
            response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
            response.Headers.Add("Access-Control-Allow-Origin", "*");
        }

        public static void AppPagination(this HttpResponse response, int currentPage,
            int itemsPerPage, int totalItems, int totalPages){

                try
                {
                    var paginationHeader = new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);
                    
                    // var formatter = new JsonSerializerOptions{
                    //     PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    //     WriteIndented = true
                    // };
                    // response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader, formatter));

                    var formatter = new JsonSerializerOptions{
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };
                    response.Headers.Add("Pagination",JsonSerializer.Serialize(paginationHeader, formatter));
                    
                    response.Headers.Add("Access-Control-Expose-Headers", "Pagination");   
                }
                catch (System.Exception ex)
                {
                    throw new Exception(ex.Message);
                }                
            }
    }
}