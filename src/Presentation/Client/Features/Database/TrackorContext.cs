using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Thaniwasi.Client.Features.ActivityLog;
using Thaniwasi.Client.Features.Categories;
using Thaniwasi.Client.Features.LinkLibrary;
using Thaniwasi.Client.Features.Projects;
using Thaniwasi.Client.Features.SnippetLibrary;
using Thaniwasi.Client.Features.TaskList;

namespace Thaniwasi.Client.Features.Database
{
    public class TrackorContext(DbContextOptions<TrackorContext> options) : DbContext(options)
    {
        public DbSet<ActivityLogItem> ActivityLogItems { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ApplicationSetting> ApplicationSettings { get; set; } 
        public DbSet<TaskListItem> TaskListItems { get; set; }
        public DbSet<CodeSnippet> CodeSnippets { get; set; }
        public DbSet<LinkLibraryItem> Links { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // SQLite does not support expressions of type 'DateTimeOffset' in ORDER BY clauses. Convert the values to a supported type:
            configurationBuilder.Properties<DateTimeOffset>().HaveConversion<DateTimeOffsetToBinaryConverter>();
            configurationBuilder.Properties<DateTimeOffset?>().HaveConversion<DateTimeOffsetToBinaryConverter>();
        }
    }

}
