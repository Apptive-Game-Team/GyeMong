using Util.ChangeListener;

namespace GyeMong.GameSystem.Creature.Player.Interface.Listener
{
    public interface IDashListener : IChangeListener<float>
    {
        void OnDashUsed(float cooldown);
    }
}