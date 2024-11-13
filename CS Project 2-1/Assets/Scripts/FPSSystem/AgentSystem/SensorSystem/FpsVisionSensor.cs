using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents.Sensors;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.AI;

namespace FPSSystem.AgentSystem.SensorSystem
{

    public class FpsVisionSensor : RayPerceptionSensorComponent3D
    {
        private RayPerceptionSensor m_sensorInternals;

        private float[] m_output;

        private int m_observedTagCount;

        private void Awake()
        {
            Debug.Log("FpsVisionSensor Awake called.");
        }

        // Required initialization to modify the MLAgents base behaviors 
        private void Start()
        {   
            name = "FPSVisionSensor";

            Debug.Log("FpsVisionSensor Start called.");

            // Set input parameters
            Helpers.SetInputParameters(this);

            m_observedTagCount = GetRayPerceptionInput().DetectableTags.Count;

            // Create actual sensor logic, to which the input will be passed internally in the library files
            base.CreateSensors();
            m_sensorInternals = RaySensor;

            if (m_sensorInternals == null)
            {
                Debug.Log("Vision sensor m_sensorInternals is null");
            }
        }


        public float[] CollectObservation()
        {

            List<float[]> raysOutput = new List<float[]>();


            // Not sure if we need to call Update ourselves, or if unity will do it, or if it is only called when the 
            // sensor is attached through the editor and not programatically
            m_sensorInternals.Update();

            float[] output = new float[GetRayPerceptionInput().OutputSize() * m_sensorInternals.RayPerceptionOutput.RayOutputs.Length];
            if (m_sensorInternals.RayPerceptionOutput.RayOutputs == null)
            {
                Debug.Log("Vision sensor RayPerceptionOutput.RayOutputs is null");
                return new float[0];
            }


            int idx = 0;
            foreach(var RayOutput in m_sensorInternals.RayPerceptionOutput.RayOutputs)
            {   
                float[] buffer = new float[(m_observedTagCount + 2) * m_sensorInternals.RayPerceptionOutput.RayOutputs.Length];
                // Process each ray (normalize, convert to float array). It is already normalized in the libraries internals
                // (observed distance is calculated as ratio of the hit distance and full length of ray for PercieveRay() in RayPerceptionSensor.cs)
                Helpers.Process(RayOutput, m_observedTagCount, idx, buffer); // fills "buffer" reference
                raysOutput.Add(buffer);
                idx++;
            }

            int idx1 = 0;
            foreach (var arr in raysOutput)
            {
                Array.Copy(arr, 0, output, idx1, arr.Length);
                idx1 += arr.Length;
            }

            return output;
        }


        // Add helper methods for processing data
        private static class Helpers
        {

            /// <summary>
            /// Sets preconfigured parameters for the raycast sensor used for input
            /// </summary>
            /// <remarks>
            /// This method initializes the properties of the vision sensor, such as the number of rays per direction,
            /// detectable tags, etc. 
            /// </remarks>
            public static void SetInputParameters(FpsVisionSensor sensor)
            {
                //name = "VisionSensor";
                sensor.RaysPerDirection = 16;
                sensor.MaxRayDegrees = 36f;

                sensor.DetectableTags = new List<string> { "Box" };

                sensor.RayLength = 16f;

                sensor.SphereCastRadius = 2f;

                sensor.RayLayerMask = LayerMask.GetMask("Default", "Water", "invisible"); // Set objects to ignore

                sensor.StartVerticalOffset = 0.6f; // ray source positioned to the beans eyes
                sensor.EndVerticalOffset = 1.36f;

                sensor.UseBatchedRaycasts = true; // Use batched raycast processing why not
            }

            public static void Process(RayPerceptionOutput.RayOutput outputToNormalize, int detectableTags, int idx, float[] buffer)
            {
                outputToNormalize.ToFloatArray(detectableTags, idx, buffer);
            }



        }

    }

}