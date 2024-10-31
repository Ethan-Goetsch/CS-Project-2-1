using System.Collections.Generic;
using FPSSystem.SoundSystem;
using Unity.MLAgents.Sensors;

namespace FPSSystem.AgentSystem.SensorSystem
{
    public class SoundSensorComponent : SensorComponent, ISoundListener
    {
        private SoundSensor _sensor;

        public override ISensor[] CreateSensors()
        {
            _sensor = new SoundSensor();
            return new ISensor[] { _sensor };
        }

        public void HearSound(Sound sound)
        {
            _sensor.AddSound(sound);
        }
    }

    public class SoundSensor : ISensor
    {
        private List<Sound> _sounds;

        public SoundSensor()
        {
            _sounds = new List<Sound>();
        }

        public void AddSound(Sound sound)
        {
            _sounds.Add(sound);
        }

        public ObservationSpec GetObservationSpec()
        {
            return ObservationSpec.Vector(_sounds.Count);
        }

        public int Write(ObservationWriter writer)
        {
            for (var i = 0; i < _sounds.Count; i++)
            {
                writer.Add(_sounds[i].Origin);
            }
            return _sounds.Count;
        }

        public byte[] GetCompressedObservation()
        {
            return null;
        }

        public void Update()
        {
            _sounds.Clear();
        }

        public void Reset()
        {
            _sounds.Clear();
        }

        public CompressionSpec GetCompressionSpec()
        {
            return CompressionSpec.Default();
        }

        public string GetName()
        {
            return nameof(SoundSensor);
        }
    }
}
