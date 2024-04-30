namespace TbdFriends.WaterDrinkWater.Data.Results;

public class GroupResult
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Code { get; set; } = null!;
    
    public IEnumerable<MemberResult> Members { get; set; } = [];
    public int OwnerId { get; set; }

    public class MemberResult
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int Progress { get; set; }
    }
}