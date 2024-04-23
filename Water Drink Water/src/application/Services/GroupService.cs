﻿using Ardalis.Result;
using TbdFriends.WaterDrinkWater.Application.Contracts;
using TbdFriends.WaterDrinkWater.Data.Contracts;
using TbdFriends.WaterDrinkWater.Data.Models;
using viewmodels;

namespace TbdFriends.WaterDrinkWater.Application.Services;

public class GroupService(IGroupRepository repository, ICodeGenerator codeGenerator)
{
    public Result AddNewGroup(string name, int accountId)
    {
        var existingGroup = repository.GetByName(name, accountId);

        if (existingGroup is not null)
        {
            return Result.Conflict();
        }

        var group = new Group
        {
            Name = name,
            OwnerId = accountId,
            Code = codeGenerator.GenerateCode(),
            DateAdded = DateTime.UtcNow
        };

        repository.Add(group);

        repository.AddMembership(new Membership { GroupId = group.Id, AccountId = accountId });

        return Result.Success();
    }

    public IEnumerable<GroupViewModel> GetGroups(int accountId)
    {
        var groups = repository.GetGroups(accountId);

        return groups.Select(g => new GroupViewModel
        {
            Id = g.Id,
            Name = g.Name,
            Code = g.Code,
            OwnedByMe = g.OwnerId == accountId,
            Members = repository.GetMembershipsForGroup(g.Id)
                .Select(m => new MemberViewModel
                {
                    Id = m.AccountId,
                    Name = m.Account.Name,
                    IsOwner = g.OwnerId == m.AccountId
                })
        });
    }

    public bool JoinGroup(string code, int accountId)
    {
        var group = repository.GetByCode(code);

        if (group is null)
        {
            return false;
        }

        if (group.OwnerId == accountId)
        {
            return false;
        }

        var isAlreadyAMember = repository.GetMembershipsForGroup(group.Id)
            .Any(m => m.AccountId == accountId);

        if (isAlreadyAMember)
        {
            return false;
        }

        repository.AddMembership(new Membership { GroupId = group.Id, AccountId = accountId });

        return true;
    }
}