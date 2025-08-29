using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace SharedKernel.Enums;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum TimeFilter
{
    LAST_30_DAYS,
    LAST_60_DAYS,
    LAST_90_DAYS,
    OVER_90_DAYS
}
