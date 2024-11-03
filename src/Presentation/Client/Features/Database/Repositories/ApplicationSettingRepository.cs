﻿using Microsoft.EntityFrameworkCore;

namespace Medical.Client.Features.Database.Repositories;

public class ApplicationSettingRepository(IDbContextFactory<TrackorContext> db)
{
    public async Task<ApplicationSetting> GetOrAdd(string key, string defaultValue)
    {
        using var dbContext = await db.CreateDbContextAsync();

        var appSetting = dbContext.ApplicationSettings.SingleOrDefault(x => x.Key == key);
        if (appSetting == null) 
        {
            appSetting = new ApplicationSetting { Key = key, Value = defaultValue };
            dbContext.ApplicationSettings.Add(appSetting);
            await dbContext.SaveChangesAsync();
        }

        return appSetting;
    }

    public async Task Update(string key, string value)
    {
        var dbContext = await db.CreateDbContextAsync();
        var existing = dbContext.ApplicationSettings.FirstOrDefault(x => x.Key == key);
        existing.Value = value;
        dbContext.SaveChanges();
    }

    public async Task<ApplicationSetting> Toggle(string key) 
    {
        var dbContext = await db.CreateDbContextAsync();
        var appSetting = dbContext.ApplicationSettings.SingleOrDefault(x => x.Key == key);

        if (appSetting is null || bool.TryParse(appSetting.Value, out bool appSettingValue) == false) 
        {
            throw new ApplicationException($"ApplicationSetting {key} is not a valid boolean setting.");
        }

        appSetting.Value = (!appSettingValue).ToString();
        dbContext.SaveChanges();

        return appSetting;
    }
}
