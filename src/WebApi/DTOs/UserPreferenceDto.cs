using System.Collections.Generic;

namespace WebApi.DTOs
{
    public class UserPreferenceDto
    {
        public string? Theme { get; set; } 
        public List<string> FavoriteCategories { get; set; } = new();
        
    }
}
