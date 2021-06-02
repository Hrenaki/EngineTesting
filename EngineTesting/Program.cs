using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineTesting.Engines;
using EngineTesting.TestModules;

namespace EngineTesting
{
    class Program
    {
        static void Main(string[] args)
        {
            Time.DeltaTime = 5;
            
            TestModule testModule = new TestModule();
            testModule.TestUnits.Add(new TestUnit(SimpleDIC.ReadConfigurationFrom("se.txt"), 10));
            testModule.RunTests();

            Console.ReadLine();
        }
    }
}
