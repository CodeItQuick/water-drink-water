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

    public IEnumerable<GroupResult> GetAllGroupMemberships(int accountId)
    {
        using var context = factory.CreateDbContext();

        var timeZoneLookup = (from p in context.Preferences
                select new
                {
                    UserId = p.UserId,
                    TimeZoneId = p.TimeZoneId
                })
            .Select(k => new
            {
                TimeZoneId = k.TimeZoneId,
                TimeZoneOffsetHours = TimeZoneInfo.FindSystemTimeZoneById(k.TimeZoneId).BaseUtcOffset.Hours,
                CurrentDate = DateTime.UtcNow
                    .AddHours(TimeZoneInfo.FindSystemTimeZoneById(k.TimeZoneId).BaseUtcOffset.Hours).Date
            }).ToList();

        var groupsWithMembers = (from g in context.Groups
            join m in context.Memberships on g.Id equals m.GroupId
            join a in context.Accounts on m.AccountId equals a.Id
            where g.OwnerId == accountId || m.AccountId == accountId
            group new { Account = a } by new { g.Id }
            into grp
            select new
            {
                grp.Key.Id,
                Members = (from m in grp
                    join p in context.Preferences on m.Account.Id equals p.UserId
                    select new
                    {
                        Id = m.Account.Id,
                        Name = m.Account.Name,
                        TimeZoneId = p.TimeZoneId,
                        TargetFluidOunces = p.TargetFluidOunces
                    }).AsEnumerable()
            }).ToList();

        var results = from g in groupsWithMembers
            join grp in context.Groups on g.Id equals grp.Id
            select new GroupResult
            {
                Id = g.Id,
                Name = grp.Name,
                Code = grp.Code,
                OwnerId = grp.OwnerId,
                Members = (from m in g.Members
                    join t in timeZoneLookup on m.TimeZoneId equals t.TimeZoneId
                    join l in context.Logs on m.Id equals l.UserId into logs
                    select new GroupResult.MemberResult()
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Progress = (int)((from log in logs.AsEnumerable()
                            where log.UserId == m.Id &&
                                  log.ConsumedOn.AddHours(t.TimeZoneOffsetHours).Date == t.CurrentDate
                            select log.FluidOunces).Sum() / (double)m.TargetFluidOunces * 100)
                    }).AsEnumerable()
            };

        return results.ToList();
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