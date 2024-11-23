using System.Collections.Generic;
using FPSSystem.SoundSystem;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace FPSSystem.AgentSystem.SensorSystem
{
    public class SoundSensorComponent : SensorComponent, ISoundListener, ISensor
    {
        private List<Sound> _sounds;

        public override ISensor[] CreateSensors()
        {
            _sounds = new List<Sound>();
            return new ISensor[] { this };
        }

        public void HearSound(Sound sound)
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

        void ISensor.Update()
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
            return gameObject.name;
        }

        private void OnDrawGizmosSelected()
        {
            if (_sounds == null) return;
            foreach (var sound in _sounds)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(sound.Origin, sound.Radius);
            }
        }
    }
}
