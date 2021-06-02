using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineTesting.Engines
{
    public abstract class Engine
    {
        public bool Working { get; protected set; }
        public double CurrentTemperature { get; protected set; }
        public double OverheatTemperature { get; protected set; }
        public abstract void Start(double enviromentTemperature);
        public abstract void Update(double enviromentTemperature);
        public virtual bool IsOverheated() => CurrentTemperature >= OverheatTemperature;
        public abstract void Reset();
    }
}