﻿using Microsoft.EntityFrameworkCore;
using Medical.Client.Features.ActivityLog;

namespace Medical.Client.Features.Database.Repositories;

public class ActivityLogRepository(IDbContextFactory<TrackorContext> db)
{
    public async Task<ActivityLogItem[]> GetActive() 
    {
        using var dbContext = await db.CreateDbContextAsync();

        var items = dbContext.ActivityLogItems
            .Where(x => x.Archived == false)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .ToArray();

        return items;
    }

    public async Task<ActivityLogItem> Save(ActivityLogItem item) 
    {
        using var dbContext = await db.CreateDbContextAsync();

        if (item.Id == 0)
        {
            dbContext.ActivityLogItems.Add(item);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            var tracking = dbContext.ActivityLogItems.Attach(item);
            tracking.State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        return item;
    }

    public async Task Delete(ActivityLogItem item) 
    {
        using var dbContext = await db.CreateDbContextAsync();

        var tracking = dbContext.ActivityLogItems.Attach(item);
        tracking.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }

    public async Task Archive(DateOnly start, DateOnly end) 
    {
        using var dbContext = await db.CreateDbContextAsync();
        
        var items = dbContext.ActivityLogItems
            .Where(x => x.Date >= start)
            .Where(x => x.Date <= end);

        foreach (var item in items)
        {
            item.Archived = true;
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task Unarchive(DateOnly start, DateOnly end)
    {
        using var dbContext = await db.CreateDbContextAsync();

        var items = dbContext.ActivityLogItems
            .Where(x => x.Date >= start)
            .Where(x => x.Date <= end);

        foreach (var item in items)
        {
            item.Archived = false;
        }

        await dbContext.SaveChangesAsync();
    }
}
