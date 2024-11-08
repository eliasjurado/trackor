﻿using Microsoft.EntityFrameworkCore;
using Medical.Client.Features.LinkLibrary;

namespace Medical.Client.Features.Database.Repositories;

public class LinkLibraryRepository(IDbContextFactory<TrackorContext> db)
{
    public async Task<LinkLibraryItem[]> Search(string searchTerm)
    {
        using var dbContext = await db.CreateDbContextAsync();
        var snippets = dbContext.Links
            .Where(x => EF.Functions.Like(x.Label, $"%{searchTerm}%")
                     || EF.Functions.Like(x.Description, $"%{searchTerm}%")
                     || EF.Functions.Like(x.Url, $"%{searchTerm}%"))
            .OrderBy(x => x.Label)
            .ToArray();

        return snippets;
    }

    public async Task<LinkLibraryItem> Save(LinkLibraryItem link)
    {
        using var dbContext = await db.CreateDbContextAsync();

        if (link.Id == 0)
        {
            dbContext.Links.Add(link);
            await dbContext.SaveChangesAsync();
        }
        else
        {
            var tracking = dbContext.Links.Attach(link);
            tracking.State = EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        return link;
    }

    public async Task Delete(LinkLibraryItem link)
    {
        using var dbContext = await db.CreateDbContextAsync();
        var tracking = dbContext.Links.Attach(link);
        tracking.State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
    }
}
