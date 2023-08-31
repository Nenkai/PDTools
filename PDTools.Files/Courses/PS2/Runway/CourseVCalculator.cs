﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using static PDTools.Files.Courses.PS2.Runway.RunwayData;

namespace PDTools.Files.Courses.PS2.Runway
{
    public class RaceCourse
    {
        public RunwayData Runway { get; set; }

        public RaceCourse(RunwayData runway)
        {
            Runway = runway;
        }

        /*
        var pos = new Vector3(127.673156738281f, 1000.0f, -384.175567626953f);
        var calculator = new CourseVCalculator();
        calculator.Position = pos;
        getVPosition(calculator);
        */

        public float getVPosition(CourseVCalculator calc)
        {
            // Begin by first searching where the provided position is on the track, on the cluster and tri level
            if (calc.Result.Cluster <= 0)
            {
                Vector3 pos;
                pos.X = calc.Position.X;
                pos.Y = calc.Position.Y + 1000.0f;
                pos.Z = calc.Position.Z;

                if (!Runway.search(out RunwayResult result, pos))
                {
                    calc.Result = default;
                }

                calc.Result = new RunwayHint(result.TriIndex, result.Cluster);
            }

            return Runway.getVCoord(calc.Position, calc.Result);
        }
    }

    public class CourseVCalculator
    {
        public Vector3 Position { get; set; }
        public RunwayHint Result;
    }
}
