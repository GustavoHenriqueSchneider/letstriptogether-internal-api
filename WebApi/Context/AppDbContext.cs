using Microsoft.EntityFrameworkCore;
using WebApi.Models;
namespace WebApi.Context
    //Realiza a comunicação entre as entidades para as tabelas do BD
{
    public class AppDbContext : DbContext //classe do entity framework core
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } //User para tabela Users
        public DbSet<Group> Groups { get; set; } //Group para tabela Groups etc...
        public DbSet<GroupMember> GroupMembers { get; set; } 
        public DbSet<GroupInvitation> GroupInvitations { get; set; } 
        public DbSet<GroupMatch> GroupMatches { get; set; }
        public DbSet<Destination> Destinations { get; set; }
    }
}
