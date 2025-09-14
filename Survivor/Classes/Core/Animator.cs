namespace Survivor.Classes.Core;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using State = Survivor.Classes.Core.Enums.State;
public class Animator
{
    public record DrawData(
        Texture2D spriteSheetIdle, int idleFrames,
        Texture2D spriteSheetRun, int runFrames,
        Texture2D spriteSheetAttack, int attackFrames,
        Texture2D spriteSheetDead, int deadFrames,
        Texture2D spriteSheetJump, int jumpFrames,
        Vector2 offSet,
        Vector2 spriteSize
    );

    private Texture2D _currentSprite;
    private State _state;
    private int _totalFrames;
    private readonly Texture2D _idleSheet;
    private readonly Texture2D _runSheet;
    private readonly Texture2D _attackSheet;
    private readonly Texture2D _deadSheet;
    private readonly Texture2D _jumpSheet;
    private readonly int _idleFrames;
    private readonly int _runFrames;
    private readonly int _attackFrames;
    private readonly int _deadFrames;
    private readonly int _jumpFrames;
    private readonly int _xSize;
    private readonly int _ySize;
    private readonly Vector2 _offset;
    private int _currentAnimationFrame = 0;
    private State prevState;

    private bool _animationFinished;
    private int _deathAnimationFrames;
    public bool AnimationFinished() => _animationFinished;

    public Animator(DrawData drawData, State state)
    {
        _idleSheet = drawData.spriteSheetIdle;
        _runSheet = drawData.spriteSheetRun;
        _attackSheet = drawData.spriteSheetAttack;
        _deadSheet = drawData.spriteSheetDead;
        _jumpSheet = drawData.spriteSheetJump;
        _currentSprite = drawData.spriteSheetIdle;
        _idleFrames = drawData.idleFrames;
        _runFrames = drawData.runFrames;
        _attackFrames = drawData.attackFrames;
        _deadFrames = drawData.deadFrames;
        _jumpFrames = drawData.jumpFrames;
        _state = state;
        _xSize = (int)drawData.spriteSize.X;
        _ySize = (int)drawData.spriteSize.Y;
        _offset = drawData.offSet;
    }

    public void SetState(State state) => _state = state;

    public void StartDeathAnimation()
    {
        _deathAnimationFrames = _deadFrames;
        _animationFinished = false;
    }

    public void SelectSprite()
    {
        switch (_state)
        {
            case State.Idle:
                _currentSprite = _idleSheet; _totalFrames = _idleFrames; _currentAnimationFrame = 0; break;
            case State.Running:
                _currentSprite = _runSheet; _totalFrames = _runFrames; _currentAnimationFrame = 0; break;
            case State.Attacking:
                _currentSprite = _attackSheet; _totalFrames = _attackFrames; _currentAnimationFrame = 0; break;
            case State.Dead:
                _currentSprite = _deadSheet; _totalFrames = _deadFrames; _currentAnimationFrame = 0; StartDeathAnimation(); break;
            case State.Jumping:
                _currentSprite = _jumpSheet; _totalFrames = _jumpFrames; _currentAnimationFrame = 0; break;
        }
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 position, string direction, GameTime gameTime)
    {
        if(prevState != _state || _totalFrames == 0)
            SelectSprite();
        SpriteEffects effects = direction == "right" ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        int frameWidth = _currentSprite.Width / _totalFrames;
        int frameHeight = _currentSprite.Height;
        int currentFrame = (int)Math.Floor((decimal)_currentAnimationFrame / 2);
        
        var sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight);

        if (_state == State.Dead && _currentAnimationFrame >= _deathAnimationFrames * 2 - 1)
            _animationFinished = true;
        else
            spriteBatch.Draw(
                _currentSprite,
                new Rectangle((int)position.X - (int)_offset.X, (int)position.Y - (int)_offset.Y, _xSize, _ySize),
                sourceRect,
                Color.White,
                0f,
                Vector2.Zero,
                effects,
                0f
            );
        
        if (_currentAnimationFrame == _totalFrames * 2 - 1)
            _currentAnimationFrame = 0;

        _currentAnimationFrame++;
        prevState = _state;
    }
}
