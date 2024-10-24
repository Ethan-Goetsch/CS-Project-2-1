namespace FPSSystem.AgentSystem
{
    public interface IAgentEvent
    {

    }

    public record OnKilled(FPSAgent Agent) : IAgentEvent;

    public record OnHealed(FPSAgent Agent, float Previous, float New) : IAgentEvent
    {
        public float Amount => New - Previous;
    }

    public record OnDamaged(FPSAgent Agent, float Previous, float New) : IAgentEvent
    {
        public float Amount => New - Previous;
    }
}
