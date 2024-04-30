using TbdFriends.WaterDrinkWater.Data.Models;
using TbdFriends.WaterDrinkWater.Data.Results;

namespace TbdFriends.WaterDrinkWater.Data.Contracts;

public interface IGroupRepository
{
    void Add(Group group);
    Group? GetByName(string name, int accountId);
    IEnumerable<Group> GetGroups(int accountId);
    Group? GetByCode(string code);
    void AddMembership(Membership membership);

    IEnumerable<Membership> GetMembershipsForGroup(int groupId);
    IEnumerable<GroupResult> GetAllGroupMemberships(int accountId);
}