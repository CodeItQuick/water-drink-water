using Microsoft.EntityFrameworkCore;
using TbdFriends.WaterDrinkWater.Data.Contexts;
using TbdFriends.WaterDrinkWater.Data.Contracts;
using TbdFriends.WaterDrinkWater.Data.Models;
using TbdFriends.WaterDrinkWater.Data.Results;

namespace TbdFriends.WaterDrinkWater.Data.Repositories;

public class GroupRepository(IDbContextFactory<ApplicationDbContext> factory) : IGroupRepository
{
    public void Add(Group group)
    {
        using var context = factory.CreateDbContext();

        context.Groups.Add(group);

        context.SaveChanges();
    }

    public Group? GetByName(string name, int accountId)
    {
        using var context = factory.CreateDbContext();

        return context.Groups.FirstOrDefault(g => g.Name == name && g.OwnerId == accountId);
    }

    public IEnumerable<Group> GetGroups(int accountId)
    {
        using var context = factory.CreateDbContext();

        return context.Groups
            .Where(g => g.OwnerId == accountId || g.Memberships.Any(m => m.AccountId == accountId))
            .ToList();
    }

    public Group? GetByCode(string code)
    {
        using var context = factory.CreateDbContext();

        return context.Groups.FirstOrDefault(g => g.Code == code);
    }

    public void AddMembership(Membership membership)
    {
        using var context = factory.CreateDbContext();

        context.Memberships.Add(membership);

        context.SaveChanges();
    }

    public IEnumerable<GroupResult> GetAllGroupsWithMembersAndDailyProgress(int accountId)
    {
        using var context = factory.CreateDbContext();

        var timeZoneLookup = GetTimeZoneLookup(context);

        var memberPreferencesByGroupId =
            context.Groups
                .Where(g => g.OwnerId == accountId || g.Memberships.Any(m => m.AccountId == accountId))
                .SelectMany(g => g.Memberships)
                .Select(m => new
                {
                    m.GroupId,
                    Id = m.AccountId,
                    m.Account.Name,
                    m.Account.Preferences.TimeZoneId,
                    m.Account.Preferences.TargetFluidOunces
                })
                .GroupBy(m => m.GroupId)
                .ToList();

        var membersWithCalculatedDailyProgressByGroup =
            from @group in memberPreferencesByGroupId
            join groupInfo in context.Groups on @group.Key equals groupInfo.Id
            select new GroupResult
            {
                Id = groupInfo.Id,
                Name = groupInfo.Name,
                Code = groupInfo.Code,
                Members = (from member in @group
                    join timeZone in timeZoneLookup on member.TimeZoneId equals timeZone.TimeZoneId
                    select new GroupResult.MemberResult
                    {
                        Id = member.Id,
                        Name = member.Name,
                        Progress = (int)(
                            (
                                from log in context.Logs
                                where log.UserId == member.Id &&
                                      log.ConsumedOn.AddHours(timeZone.TimeZoneOffsetHours).Date == timeZone.CurrentDate
                                select log.FluidOunces
                            )
                            .AsEnumerable()
                            .Sum() / (double)member.TargetFluidOunces * 100)
                    }).ToList()
            };

        return membersWithCalculatedDailyProgressByGroup.ToList();
    }

    private sealed record TimeZoneLookupInfo(string TimeZoneId, int TimeZoneOffsetHours, DateTime CurrentDate);

    private static List<TimeZoneLookupInfo> GetTimeZoneLookup(ApplicationDbContext context)
    {
        var timeZoneLookup =
            (from p in context.Preferences
                select new
                {
                    UserId = p.UserId,
                    TimeZoneId = p.TimeZoneId
                })
            .Distinct()
            .Select(k => new TimeZoneLookupInfo(k.TimeZoneId,
                TimeZoneInfo.FindSystemTimeZoneById(k.TimeZoneId).BaseUtcOffset.Hours,
                DateTime.UtcNow.AddHours(TimeZoneInfo.FindSystemTimeZoneById(k.TimeZoneId).BaseUtcOffset.Hours).Date))
            .ToList();
        return timeZoneLookup;
    }

    public IEnumerable<Membership> GetMembershipsForGroup(int groupId)
    {
        using var context = factory.CreateDbContext();

        return context.Memberships
            .Include(m => m.Account)
            .Include(m => m.Group)
            .Where(m => m.GroupId == groupId)
            .ToList();
    }
}