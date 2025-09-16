using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Survivor.Classes.Core;
using System.Collections.Generic;
using State = Survivor.Classes.Core.Enums.State;
namespace Survivor.Classes.Controllers
{
    public class EffectController
    {
        private List<SkillEffect> _skillList;

        public EffectController() => _skillList = new List<SkillEffect>();

        public void AddEffect(Animator.DrawData drawData, State type, Vector2 position, Vector2 size, Vector2 speed) =>
            _skillList.Add(new SkillEffect(drawData, type, position, size, speed));

        public void UpdateEffectList()
        {
            for (int i = _skillList.Count - 1; i >= 0; i--)
            {
                _skillList[i].Update(new(0,0));
                if (_skillList[i].AnimationFinished)
                    _skillList.RemoveAt(i);
                
            }
        }

        public void DrawEffects(SpriteBatch spriteBatch, GameTime gameTime)
        {
            foreach (SkillEffect effect in _skillList)
                effect.Draw(spriteBatch, gameTime);
        }

    }
}