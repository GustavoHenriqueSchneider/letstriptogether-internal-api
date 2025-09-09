namespace WebApi.Models
{
    public class Destination : TrackableEntity //precisei herdar um id para fazer a migration
    // Erro: The entity type 'Destination' requires a primary key to be defined.
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public List<GroupMatch> GroupMatches { get; set; } = new();
    }
}
