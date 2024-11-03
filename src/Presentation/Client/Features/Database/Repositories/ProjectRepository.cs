﻿using Microsoft.EntityFrameworkCore;
using Medical.Client.Features.Projects;

namespace Medical.Client.Features.Database.Repositories;

public class ProjectRepository(IDbContextFactory<TrackorContext> db)
{
    public async Task<Project[]> Get()
    {
        using var dbContext = await db.CreateDbContextAsync();
        var items = dbContext.Projects.OrderBy(x => x.Title).ToArray();

        return items;
    }

    public async Task<Project> Save(Project project)
    {
        using var dbContext = await db.CreateDbContextAsync();

        if (project.Id == 0)
        {
            dbContext.Projects.Add(project);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            var tracking = dbContext.Projects.Attach(project);
            tracking.State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        return project;
    }

    public async Task Delete(Project project)
    {
        using var dbContext = await db.CreateDbContextAsync();

        DisconnectActivityLogItemsFromProject(project, dbContext);

        var tracking = dbContext.Projects.Attach(project);
        tracking.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }

    private static void DisconnectActivityLogItemsFromProject(Project project, TrackorContext dbContext)
    {
        foreach (var activityLog in dbContext.ActivityLogItems.Where(x => x.ProjectId == project.Id))
        {
            activityLog.ProjectId = null;
        }
    }
}
