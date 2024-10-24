using Sirenix.OdinInspector;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace GoalSystem
{
    public class MoveToGoalAgent : Agent
    {
        [TitleGroup("Components")]
        private CharacterController characterController;

        [TitleGroup("Goals")]
        public Material winMaterial, loseMaterial;
        public MeshRenderer floorMeshRenderer;
        public Transform Target;
        public float Speed = 5f;

        public override void Initialize()
        {
            characterController = GetComponent<CharacterController>();
        }

        public override void OnEpisodeBegin()
        {
            transform.localPosition = Vector3.zero;
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {

        }

        public override void CollectObservations(VectorSensor sensor)
        {
            sensor.AddObservation(transform.localPosition);
            sensor.AddObservation(transform.localRotation);

            sensor.AddObservation(Target.localPosition);
            sensor.AddObservation(Target.localRotation);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {

        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            var horizontal = actions.ContinuousActions[0];
            var vertical = actions.ContinuousActions[1];

            var movement = transform.right * horizontal;
            movement += transform.forward * vertical;
            movement.Normalize();

            characterController.Move(movement * (Speed * Time.deltaTime));
        }

        public override string ToString()
        {
            return gameObject.name;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.TryGetComponent<Goal>(out var goal))
            {
                floorMeshRenderer.material = winMaterial;
                SetReward(1f);
                EndEpisode();
            }
            else if (hit.gameObject.TryGetComponent<Wall>(out var wall))
            {
                floorMeshRenderer.material = loseMaterial;
                SetReward(-1f);
                EndEpisode();
            }
        }
    }
}
