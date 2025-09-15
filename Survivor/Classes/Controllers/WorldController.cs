namespace Survivor.Classes.Controllers
{
    public class WorldController
    {
        private bool _burnEnemies = false;
        private int _burnTimer = 0;
        private bool _pushFromPlayer = false;
        private int _pushTimer = 0;
        private float _gravity = 0.5f;
        private float _pushForce = 5f;
        public bool BurnEnemies => _burnEnemies;
        public bool PushFromPlayer => _pushFromPlayer;
        public float ApplyGravity => _gravity;
        public float ApplyPushForce => _pushForce;
        public void ActiveBurnEnemies()
        {
            _burnEnemies = true;
            _burnTimer = 500;
        }
        public void ActivePushFromPlayer()
        {
            _pushFromPlayer = true;
            _pushTimer = 500;
        }

        public void DeactivateBurnEnemies() => _burnEnemies = false;
        public void DeactivatePushFromPlayer() => _pushFromPlayer = false;

        public void UpdateWorldEffects()
        {
            if (_burnTimer > 0)
                _burnTimer--;
            else
                DeactivateBurnEnemies();

            if (_pushTimer > 0)
                _pushTimer--;
            else
                DeactivatePushFromPlayer();
        }

    }
}