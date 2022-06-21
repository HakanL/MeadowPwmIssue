using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Gateway.WiFi;
using Meadow.Hardware;

namespace MeadowPwmIssue
{
    // Change F7FeatherV2 to F7FeatherV1 for V1.x boards
    public class MeadowApp : App<F7FeatherV1, MeadowApp>
    {
        private IPwmPort pwmLightA;
        private IPwmPort pwmLightB;
        private PwmLed lightA;
        private PwmLed lightB;

        public MeadowApp()
        {
            Initialize();

            Thread.Sleep(Timeout.Infinite);
        }

        private void Initialize()
        {
            Console.WriteLine("Initialize hardware...");

            // Note that pin D03 and D04 are on the same PWM Timer Group, but according to the documentation that should be fine
            // as long as you have the same frequency, but the duty cycle can be different.
            pwmLightA = Device.CreatePwmPort(Device.Pins.D04, frequency: 1000);
            lightA = new PwmLed(pwmLightA, TypicalForwardVoltage.ResistorLimited);

            pwmLightB = Device.CreatePwmPort(Device.Pins.D03, frequency: 1000);
            lightB = new PwmLed(pwmLightB, TypicalForwardVoltage.ResistorLimited);

            // Expect the frequency to be 1000 Hz, but PwmLed will reset it to 100 Hz
            Console.WriteLine($"FrequencyA = {pwmLightA.Frequency} Hz    chn: {pwmLightA.Channel.Name}");
            Console.WriteLine($"FrequencyB = {pwmLightB.Frequency} Hz    chn: {pwmLightB.Channel.Name}");

            // Start PWM
            pwmLightA.Start();
            pwmLightB.Start();

            lightA.Brightness = 0.50F;

            // This line is what causes lightA's PWM to misbehave
            lightB.StartPulse(0.4F, 0.01F);
        }
    }
}
