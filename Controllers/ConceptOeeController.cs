using Microsoft.AspNetCore.Mvc;

namespace ConceptHias.Controllers
{
    [Route("api/concept/oee")]
    [ApiController]
    public class ConceptOeeController : ControllerBase
    {
        [HttpGet("summary")]
        public IActionResult GetSummary([FromQuery] string line = "ffs")
        {
            if (line == "batch")
            {
                return Ok(new
                {
                    oee = 68.3,
                    availability = 88.5,
                    performance = 79.2,
                    quality = 97.1,
                    packCount = 14280,
                    rejectCount = 24,
                    goodCount = 14256,
                    target = 18000,
                    lineSpeed = 58,
                    shiftSchedule = "Morning",
                    shiftHours = "06:00 – 14:00",
                    elapsed = "4h 23m",
                    remaining = "3h 37m",
                    lineName = "Batching Plant — Line 2"
                });
            }

            return Ok(new
            {
                oee = 75.1,
                availability = 91.2,
                performance = 83.7,
                quality = 98.4,
                packCount = 18640,
                rejectCount = 12,
                goodCount = 18628,
                target = 22000,
                lineSpeed = 62,
                shiftSchedule = "Morning",
                shiftHours = "06:00 – 14:00",
                elapsed = "4h 23m",
                remaining = "3h 37m",
                lineName = "FFS Packaging — Line 1"
            });
        }

        [HttpGet("alarms")]
        public IActionResult GetAlarms()
        {
            return Ok(new[]
            {
                new { id = "Seal.Temp.Horizontal_Hi",    timestamp = "2026-03-26 09:08:52 am", value = 198.4, text = "Horizontal Seal Temp Over Limit",    type = "High", group = "Sealing",  priority = 1, ackDate = "",                       clearedDate = "",                       clearedValue = 190.0, node = "packaging-line-01", unit = "°C",  color = "red"   },
                new { id = "Fill.Weight.Deviation_Hi",   timestamp = "2026-03-26 09:07:31 am", value = 3.8,   text = "Fill Weight Out of Tolerance",       type = "High", group = "Dosing",   priority = 1, ackDate = "",                       clearedDate = "",                       clearedValue = 2.0,   node = "packaging-line-01", unit = "g",   color = "red"   },
                new { id = "Film.Roll.Remaining_Lo",     timestamp = "2026-03-26 09:04:18 am", value = 95.0,  text = "Film Roll Running Low",              type = "Low",  group = "Film",     priority = 2, ackDate = "2026-03-26 09:05:00 am", clearedDate = "2026-03-26 09:06:30 am", clearedValue = 100.0, node = "packaging-line-01", unit = "m",   color = "white" },
                new { id = "Reject.Rate_Hi",             timestamp = "2026-03-26 09:02:45 am", value = 4.2,   text = "Pack Reject Rate High",              type = "High", group = "Quality",  priority = 2, ackDate = "2026-03-26 09:03:10 am", clearedDate = "2026-03-26 09:04:50 am", clearedValue = 2.0,   node = "packaging-line-01", unit = "%",   color = "blue"  },
                new { id = "Screw.Feed.Rate_Lo",         timestamp = "2026-03-26 09:00:12 am", value = 28.5,  text = "Screw Conveyor Feed Rate Low",       type = "Low",  group = "Conveyor", priority = 2, ackDate = "2026-03-26 09:01:00 am", clearedDate = "2026-03-26 09:02:20 am", clearedValue = 35.0,  node = "packaging-line-01", unit = "kg/h", color = "white" },
            });
        }

        [HttpGet("sensors")]
        public IActionResult GetSensors([FromQuery] string line = "ffs")
        {
            if (line == "batch")
            {
                return Ok(new[]
                {
                    new { name = "Aggregate Hopper", value = 68.5, unit = "% full", top = "72%", left = "22%" },
                    new { name = "Belt Conveyor",    value = 53.6, unit = "t/h",    top = "55%", left = "32%" },
                    new { name = "Mixer RPM",        value = 18.4, unit = "rpm",    top = "38%", left = "57%" },
                    new { name = "Cement Silo",      value = 83.0, unit = "% full", top = "22%", left = "74%" },
                });
            }
            return Ok(new[]
            {
                new { name = "Fill Weight",       value = 250.4, unit = "g",   top = "32%", left = "20%" },
                new { name = "Seal Temp (H-jaw)", value = 182.5, unit = "°C",  top = "68%", left = "36%" },
                new { name = "Screw Feed Rate",   value = 48.2,  unit = "kg/h",top = "38%", left = "62%" },
                new { name = "Motor Voltage",     value = 415.1, unit = "V",   top = "30%", left = "82%" },
            });
        }
    }
}
