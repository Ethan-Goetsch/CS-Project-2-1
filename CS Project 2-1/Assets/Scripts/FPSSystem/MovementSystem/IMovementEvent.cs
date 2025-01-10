using FPSSystem.AgentSystem;
using UnityEngine;

namespace FPSSystem.MovementSystem
{
    public interface IMovementEvent : IAgentEvent
    {
        public record AgentMoveEvent(FPSAgent Agent, Vector3 Movement) : IMovementEvent;
        public record AgentRotateEvent(FPSAgent Agent, Vector3 Rotation) : IMovementEvent;
    }
}
