namespace BasicCombatGame;

public interface ICollectable
{
    bool IsCollected { get; }
    
    void OnCollect(Character collector);
}