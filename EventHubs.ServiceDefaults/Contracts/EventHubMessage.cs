using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventHubs.ServiceDefaults.Contracts;

public record EventHubMessage
{
    public string Text { get; init; }
}
