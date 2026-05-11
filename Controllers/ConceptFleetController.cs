using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/fleet")]
    [ApiController]
    public class ConceptFleetController : ControllerBase
    {
        [HttpGet("dashboard")]
        public IActionResult GetDashboard([FromQuery] string line = "ffs")
        {
            if (line == "batch")
            {
                return Ok(new
                {
                    gaugeLabel = "batch/hr",
                    gaugeSub   = "Mixer Load",
                    gaugeVal   = 72,
                    tankLabel  = "Water Tank",
                    tankVal    = "47.0 m³",
                    outputTitle = "PLANT OUTPUT",
                    outputVal  = "47 batches/day",
                    feedLabel  = "FEED RATE",
                    feedVal    = "53.6",
                    feedUnit   = "t/h",
                    topbarLabel = "Mixer RPM",
                    topbarVal  = "18.4 rpm",
                    tags = new[]
                    {
                        new { tag = "Aggregate.Hopper.Level",   value = "68.5",  unit = "%" },
                        new { tag = "Cement.Silo.Level",        value = "83.0",  unit = "%" },
                        new { tag = "Belt.Feed.Rate",           value = "53.6",  unit = "t/h" },
                        new { tag = "Mixer.RPM",                value = "18.4",  unit = "rpm" },
                        new { tag = "Water.Flow.Rate",          value = "142.0", unit = "L/min" },
                        new { tag = "Batch.Weight",             value = "2450",  unit = "kg" },
                        new { tag = "Admixture.Flow",           value = "4.8",   unit = "L/min" },
                        new { tag = "Mixer.Motor.Current",      value = "38.2",  unit = "A" },
                        new { tag = "Mixer.Motor.Voltage",      value = "415.2", unit = "V" },
                        new { tag = "Conveyor.Motor.Current",   value = "12.7",  unit = "A" },
                        new { tag = "Batch.Count.Today",        value = "47",    unit = "batches" },
                        new { tag = "Cement.Consumed.Today",    value = "18420", unit = "kg" },
                        new { tag = "Aggregate.Consumed.Today", value = "52180", unit = "kg" },
                        new { tag = "Water.Consumed.Today",     value = "8640",  unit = "L" },
                        new { tag = "Energy.Meter",             value = "5832.1",unit = "kWh" },
                        new { tag = "Alarm.Count.Active",       value = "3",     unit = "—" },
                        new { tag = "Plant.Status",             value = "Running — Batch #47 in progress", unit = "—" },
                    },
                    alarms = new[]
                    {
                        new { id = "Aggregate.Hopper.Level_Lo", timestamp = "2026-03-26 08:09:42 am", value = "18.2", text = "Aggregate Hopper Low Level",       type = "Low",  group = "Hopper",  priority = 1, node = "batching-plant-02", unit = "%",    color = "red"   },
                        new { id = "Mixer.RPM_Hi",              timestamp = "2026-03-26 08:08:55 am", value = "28.6", text = "Mixer Over-Speed Alarm",            type = "High", group = "Mixer",   priority = 1, node = "batching-plant-02", unit = "rpm",  color = "red"   },
                        new { id = "Cement.Silo.Level_Lo",      timestamp = "2026-03-26 08:05:11 am", value = "22.4", text = "Cement Silo Low Level Warning",     type = "Low",  group = "Silo",    priority = 2, node = "batching-plant-02", unit = "%",    color = "white" },
                        new { id = "Belt.Feed.Rate_Lo",         timestamp = "2026-03-26 08:03:28 am", value = "31.0", text = "Belt Feed Rate Low",                type = "Low",  group = "Conveyor",priority = 2, node = "batching-plant-02", unit = "t/h",  color = "blue"  },
                        new { id = "Mixer.Motor.Current_Hi",    timestamp = "2026-03-26 08:01:04 am", value = "52.1", text = "Mixer Motor Overcurrent",           type = "High", group = "Mixer",   priority = 1, node = "batching-plant-02", unit = "A",    color = "blue"  },
                        new { id = "Water.Flow.Rate_Lo",        timestamp = "2026-03-26 07:58:33 am", value = "98.5", text = "Water Flow Rate Below Setpoint",    type = "Low",  group = "Mixer",   priority = 2, node = "batching-plant-02", unit = "L/min",color = "white" },
                    }
                });
            }

            return Ok(new
            {
                gaugeLabel  = "kg/h",
                gaugeSub    = "Screw Feed Rate",
                gaugeVal    = 96,
                tankLabel   = "Product Hopper",
                tankVal     = "57.0 kg",
                outputTitle = "LINE OUTPUT",
                outputVal   = "62 packs/min",
                feedLabel   = "FEED RATE",
                feedVal     = "48.2",
                feedUnit    = "kg/h",
                topbarLabel = "Line Speed",
                topbarVal   = "62 packs/min",
                tags = new[]
                {
                    new { tag = "Fill.Weight.Actual",       value = "250.4", unit = "g" },
                    new { tag = "Fill.Weight.Target",       value = "250.0", unit = "g" },
                    new { tag = "Seal.Temp.Horizontal",     value = "182.5", unit = "°C" },
                    new { tag = "Seal.Temp.Vertical",       value = "178.2", unit = "°C" },
                    new { tag = "Line.Speed",               value = "62",    unit = "packs/min" },
                    new { tag = "Screw.Feed.Rate",          value = "48.2",  unit = "kg/h" },
                    new { tag = "Film.Roll.Remaining",      value = "342.0", unit = "m" },
                    new { tag = "Dosing.Hopper.Weight",     value = "18.4",  unit = "kg" },
                    new { tag = "Motor.Current",            value = "14.3",  unit = "A" },
                    new { tag = "Motor.Voltage",            value = "415.1", unit = "V" },
                    new { tag = "Reject.Count.Today",       value = "12",    unit = "packs" },
                    new { tag = "Pack.Count.Today",         value = "18640", unit = "packs" },
                    new { tag = "Product.Consumed.Today",   value = "4660",  unit = "kg" },
                    new { tag = "Energy.Meter",             value = "3241.6",unit = "kWh" },
                    new { tag = "Alarm.Count.Active",       value = "2",     unit = "—" },
                    new { tag = "Line.Status",              value = "Running — Pack #18640 completed", unit = "—" },
                },
                alarms = new[]
                {
                    new { id = "Seal.Temp.Horizontal_Hi",  timestamp = "2026-03-26 09:08:52 am", value = "198.4", text = "Horizontal Seal Temp Over Limit",  type = "High", group = "Sealing",  priority = 1, node = "packaging-line-01", unit = "°C",  color = "red"   },
                    new { id = "Fill.Weight.Deviation_Hi", timestamp = "2026-03-26 09:07:31 am", value = "3.8",   text = "Fill Weight Out of Tolerance",     type = "High", group = "Dosing",   priority = 1, node = "packaging-line-01", unit = "g",   color = "red"   },
                    new { id = "Film.Roll.Remaining_Lo",   timestamp = "2026-03-26 09:04:18 am", value = "95.0",  text = "Film Roll Running Low",            type = "Low",  group = "Film",     priority = 2, node = "packaging-line-01", unit = "m",   color = "white" },
                    new { id = "Reject.Rate_Hi",           timestamp = "2026-03-26 09:02:45 am", value = "4.2",   text = "Pack Reject Rate High",            type = "High", group = "Quality",  priority = 2, node = "packaging-line-01", unit = "%",   color = "blue"  },
                    new { id = "Screw.Feed.Rate_Lo",       timestamp = "2026-03-26 09:00:12 am", value = "28.5",  text = "Screw Conveyor Feed Rate Low",     type = "Low",  group = "Conveyor", priority = 2, node = "packaging-line-01", unit = "kg/h",color = "blue"  },
                    new { id = "Motor.Current_Hi",         timestamp = "2026-03-26 08:57:08 am", value = "22.1",  text = "Line Motor Overcurrent",           type = "High", group = "Motor",    priority = 1, node = "packaging-line-01", unit = "A",   color = "white" },
                }
            });
        }
    }
}
