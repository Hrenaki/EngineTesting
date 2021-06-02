using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EngineTesting.Engines;

namespace EngineTesting.TestModules
{
    public class TestModule
    {
        public List<TestUnit> TestUnits { get; }
        public TestModule()
        {
            TestUnits = new List<TestUnit>();
        }
        public void RunTests()
        {
            for(int i = 0; i < TestUnits.Count; i++)
            {
                Console.WriteLine("Case #" + i);
                TestUnits[i].Run();
                Console.WriteLine("----------\n");
            }
        }
    }

    public class TestUnit
    {
        Engine engine;
        double envTemperature;

        public TestUnit(Engine engine, double envTemperature)
        {
            this.engine = engine;
            this.envTemperature = envTemperature;
        }

        public virtual void Run()
        {
            if (engine == null)
            {
                Console.WriteLine("Cant find engine! Some problems with configuration or file!");
                return;
            }

            engine.Start(envTemperature);
            Console.WriteLine("Engine started!");
            Console.WriteLine(engine.CurrentTemperature);

            double ellapsedTime = 0;
            while(!engine.IsOverheated())
            {
                engine.Update(envTemperature);
                Console.WriteLine("Current temperature: " + engine.CurrentTemperature.ToString("F3"));
                ellapsedTime += Time.DeltaTime;
                Thread.Sleep(100);
            }

            Console.WriteLine("Engine overheated in " + ellapsedTime + "ms!");
        }
    }
}