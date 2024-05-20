using blazor.wa.tbd.Components;
using blazor.wa.tbd.Services;
using Microsoft.AspNetCore.Components;

namespace blazor.wa.tbd.Pages;

public partial class Log
{
    [CascadingParameter] public ConsumptionStateProvider ConsumptionState { get; set; } = null!;
    
    private int FluidOuncesConsumed { get; set; } = 0;
    private DateTimeOffset LogDatePicker { get; set; } = DateTimeOffset.Now;
    private Dictionary<(string, string, string), List<(string, string)>> Logs { get; set; } = new()
    {
        [("2024", "5", "20")] = new ()
        {
            ("8:30 am", "1 L"),
            ("9:30 am", "0.5 L"),
            ("11:30 am", "2 L")
        },
        [("2024", "5", "19")] = new ()
        {
            ("6:30 am", "1 Glass")
        },
        [("2024", "5", "18")] = new ()
    };

    private async Task LogConsumption()
    {
        await ConsumptionState.LogConsumption(FluidOuncesConsumed);
    }
}