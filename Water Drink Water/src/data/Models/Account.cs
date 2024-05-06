namespace TbdFriends.WaterDrinkWater.Data.Models;

public class Account
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public virtual Preference Preferences { get; set; } = null!;
}