namespace navision.api.Models
{
  public class Token
  {
    public string Access_Token { get; set; } = string.Empty;
    public string Token_Type { get; set; } = string.Empty;
    public int Expires_In { get; set; }
  }
}