using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DejaVu.SelfHealthCheck.Utility
{
    public enum AppStatus
    {
        Up = 1, // all is up and running -green
        Down = 2, // all is down -red
        PerformanceDegraded=3,// if any replica or child component is degraded -yellow
        ReplicaDown =4, //any replica component is down -orange
        ChildDown = 5,// child component is down  -blue
        PerformanceDown=6 // both child nd rreplica is down -pruple

    }

    public enum HeartBeatStatus
    {
        Up = 1,
        Down,

    }
}
