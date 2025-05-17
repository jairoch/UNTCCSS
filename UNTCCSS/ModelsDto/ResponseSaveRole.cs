using UNTCCSS.Data;

namespace UNTCCSS.ModelsDto
{
    public class ResponseSaveRole
    {
        public ApplicationRole Role { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
