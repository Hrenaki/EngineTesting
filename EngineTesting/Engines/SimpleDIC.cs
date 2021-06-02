using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace EngineTesting.Engines
{
    public class SimpleDIC : Engine
    {
        public double InertiaMoment { get; private set; }
        public Func<double, double> Torque { get; private set; }
        public double CurrentRotationSpeed { get; private set; }
        public double Hm { get; private set; }
        public double Hv { get; private set; }
        public double C { get; private set; }

        public SimpleDIC(double inertiaMoment, Func<double, double> torque, double overheatTemperature, double hm, double hv, double c)
        {
            InertiaMoment = inertiaMoment;
            Torque = torque;
            OverheatTemperature = overheatTemperature;
            Hm = hm;
            Hv = hv;
            C = c;
        }

        public static SimpleDIC ReadConfigurationFrom(string file)
        {
            double inertiaMoment = 0;
            double overheatTemperature = 0;
            double hm = 0, hv = 0, c = 0;
            double[] torqueValues = null;
            double[] rotationSpeedValues = null;

            if (!File.Exists(file))
                return null;

            using (StreamReader sr = new StreamReader(file))
            {
                string[] data;
                while (!sr.EndOfStream)
                {
                    data = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (data[0])
                    {
                        case "I":
                            inertiaMoment = double.Parse(data[1]);
                            break;
                        case "M":
                            torqueValues = data.Skip(1).Select(t => double.Parse(t)).ToArray();
                            break;
                        case "V":
                            rotationSpeedValues = data.Skip(1).Select(t => double.Parse(t)).ToArray();
                            break;
                        case "T":
                            overheatTemperature = double.Parse(data[1]);
                            break;
                        case "Hm":
                            hm = double.Parse(data[1]);
                            break;
                        case "Hv":
                            hv = double.Parse(data[1]);
                            break;
                        case "C":
                            c = double.Parse(data[1]);
                            break;
                    }
                }
            }

            if (inertiaMoment <= 0 || torqueValues == null || rotationSpeedValues == null || overheatTemperature == 0 || hm == 0 || hv == 0 || c == 0 || rotationSpeedValues.Length != torqueValues.Length)
                return null;

            Func<double, double> torque = v =>
            {
                for(int i = 0; i < rotationSpeedValues.Length - 1; i++)
                    if(v >= rotationSpeedValues[i] && v <= rotationSpeedValues[i + 1])
                    {
                        double dv = rotationSpeedValues[i + 1] - rotationSpeedValues[i];
                        return torqueValues[i] * (rotationSpeedValues[i + 1] - v) / dv + torqueValues[i + 1] * (v - rotationSpeedValues[i]) / dv;
                    }
                return double.NegativeInfinity;
            };

            return new SimpleDIC(inertiaMoment, torque, overheatTemperature, hm, hv, c);
        }

        public override void Start(double enviromentTemperature)
        {
            Working = true;
            CurrentTemperature = enviromentTemperature;
        }
        public override void Update(double enviromentTemperature)
        {
            if(Working)
            {
                CurrentRotationSpeed += Torque(CurrentRotationSpeed) / InertiaMoment * Time.DeltaTime;
                double currentTorque = Torque(CurrentRotationSpeed);
                  CurrentTemperature += (currentTorque * Hm + CurrentRotationSpeed * CurrentRotationSpeed * Hv - C * (enviromentTemperature - CurrentTemperature)) * Time.DeltaTime;
            }
        }
        public override void Reset()
        {
            CurrentRotationSpeed = 0;
            CurrentTemperature = 0;
        }
    }
}