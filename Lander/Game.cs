using System.Numerics;
using Raylib_cs;
using Utils;

Main();

void Main()
{
    Raylib.InitWindow(800, 480, "Lander");
    var game = new Game();
    game.Start();

    while (!Raylib.WindowShouldClose())
    {
        var dt = Raylib.GetFrameTime();
        
        Raylib.BeginDrawing();
        Raylib.ClearBackground(Color.White);
        
        Raylib.DrawText("Lander", 12, 12, 20, Color.Black);
        
        game.Tick(dt);
        game.Draw(dt);
        
        Raylib.EndDrawing();
    }
}

public class Game
{
    Lander _currentLander;
    readonly List<Particle> _particles = [];

    public void Start()
    {
        Reset();
    }

    void Reset()
    {
        var screenWidth = Raylib.GetScreenWidth();
        var landerBounds = new Rectangle(Vector2.Zero, Vector2.One * 32.0f);
        var landerPosition = new Vector2((screenWidth / 2.0f) - landerBounds.Size.X, 0.0f);
        var landerOrientation = 0.0f;
        
        _currentLander = new Lander(landerBounds, landerPosition, landerOrientation);
        _particles.Clear();
    }

    void UpdateParticles(float dt)
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();
        
        for (int i = 0; i < _particles.Count; i++)
        {
            Particle newParticle = _particles[i];

            float t = newParticle.Lifetime / newParticle.TotalLifeTime;
            newParticle.Color = newParticle.InitialColor.Darken((1.0f - t) * 0.75f).WithAlpha(t);
            newParticle.Position += newParticle.Velocity * dt;
            newParticle.Lifetime -= dt;
            
            newParticle.Velocity *= MathF.Pow(0.985f, dt / (1.0f/60.0f)); // drag??

            if (newParticle.Position.IsInsideBounds(Vector2.Zero, new Vector2(w, h)) == false)
            {
                if (newParticle.Position.X > w || newParticle.Position.X < 0)
                {
                    newParticle.Velocity.X *= -1.0f;
                }
                else if (newParticle.Position.Y > h || newParticle.Position.Y < 0)
                {
                    newParticle.Velocity.Y *= -1.0f;
                }
            }

            // Vector2 landerMin = CurrentLander.Position + CurrentLander.Bounds.Size.RotatedAroundOrigin(CurrentLander.Orientation, CurrentLander.Bounds.Size / 2.0f)
            //                     - CurrentLander.Bounds.Rotated(CurrentLander.Orientation).Min();
            //
            // Vector2 landerMax = CurrentLander.Position + CurrentLander.Bounds.Size.RotatedAroundOrigin(CurrentLander.Orientation, CurrentLander.Bounds.Size / 2.0f)
            //                     - CurrentLander.Bounds.Rotated(CurrentLander.Orientation).Max();

            Vector2 landerMin = _currentLander.Position;
            Vector2 landerMax = _currentLander.Position + _currentLander.Bounds.Size;

            if ((newParticle.TotalLifeTime - newParticle.Lifetime) > 0.25f)
            {
                if (newParticle.Position.IsInsideBounds(landerMin, landerMax))
                {
                    Vector2 normalizedParticleVelocity = Vector2.Normalize(newParticle.Velocity);
                    Vector2 vectorToLanderForward = Vector2.Normalize(_currentLander.Orientation.AsVector() - normalizedParticleVelocity);
                    Vector2 vectorToLanderLeft = Vector2.Normalize(_currentLander.Orientation.AsVector().Perpendicular() - normalizedParticleVelocity);
                    
                    Vector2 vectorToLanderBackward = -vectorToLanderForward;
                    Vector2 vectorToLanderRight = -vectorToLanderLeft;
                    
                    if (newParticle.Position.X < landerMax.X && newParticle.Position.X > landerMin.X)
                    {
                        float directionX = Math.Min(
                            Vector2.Dot(vectorToLanderLeft, normalizedParticleVelocity),
                            Vector2.Dot(vectorToLanderRight, normalizedParticleVelocity)
                        );
                        newParticle.Velocity.X *= directionX;
                    }
                    if (newParticle.Position.Y < landerMax.Y && newParticle.Position.Y > landerMin.Y)
                    {
                        float directionY = Math.Min(
                            Vector2.Dot(vectorToLanderForward, normalizedParticleVelocity),
                            Vector2.Dot(vectorToLanderBackward, normalizedParticleVelocity)
                        );
                        newParticle.Velocity.Y *= directionY;
                    }
                }   
            }
            
            // Raylib.DrawRectangleLinesEx(new Rectangle(landerMin, landerMax - landerMin), 1.0f, Color.Red);
            // Raylib.DrawCircle((int)landerMin.X, (int)landerMin.Y, 4.0f, Color.Red);
            // Raylib.DrawCircle((int)landerMax.X, (int)landerMax.Y, 4.0f, Color.Red);
            
            _particles[i] = newParticle;
        }
        
        _particles.RemoveAll(p => p.Lifetime <= 0);
    }

    public void Tick(float dt)
    {
        if (Raylib.IsKeyPressed(KeyboardKey.R))
        {
            Reset();
        }
        
        UpdateLander(dt);
        UpdateParticles(dt);
    }

    void UpdateLander(float dt)
    {
        float landerGravity = 0.98f / 5.0f;
        float landerThrusterPower = _currentLander.Thrust;
        float landerRollSpeed = MathF.PI / 128.0f; // radians per second
        Vector2 gravityVector = Vector2.UnitY * (landerGravity * landerGravity);

        var isThrustPressed = Raylib.IsKeyDown(KeyboardKey.Space);
        var isUpPressed = Raylib.IsKeyDown(KeyboardKey.Up);
        var isDownPressed = Raylib.IsKeyDown(KeyboardKey.Down);
        var isRightPressed = Raylib.IsKeyDown(KeyboardKey.Right);
        var isLeftPressed = Raylib.IsKeyDown(KeyboardKey.Left);

        float thrustAdjustVector = (isUpPressed - isDownPressed);
        float thrustVector = isThrustPressed ? 1.0f : 0.0f;
        float turnVector = (isRightPressed - isLeftPressed);

        Vector2 rotatedLanderForward = (_currentLander.Orientation - (MathF.PI / 2.0f)).AsVector();
        
        _currentLander.LinearVelocity += gravityVector;
        _currentLander.LinearVelocity += rotatedLanderForward * thrustVector * landerThrusterPower;
        _currentLander.AngularVelocity += (turnVector * landerRollSpeed);
        
        _currentLander.LinearVelocity *= MathF.Pow(0.965f, dt / (1.0f/60.0f)); // drag??
        _currentLander.AngularVelocity *= MathF.Pow(0.90f, dt / (1.0f/60.0f)); // drag??

        _currentLander.Position += _currentLander.LinearVelocity * dt;
        _currentLander.Orientation += _currentLander.AngularVelocity * dt;
        _currentLander.Thrust = Math.Max(0.0f, _currentLander.Thrust + thrustAdjustVector * dt);

        if (thrustVector != 0.0f && landerThrusterPower > 0.0f)
        {
            int amount = (int)(Math.Max(1.0f, thrustVector * landerThrusterPower * 4.0f));
            for (int i = 0; i < amount; i++)
            {
                SpawnThrusterParticle(rotatedLanderForward);
            }
        }
    }

    void SpawnThrusterParticle(Vector2 accurateLanderForward)
    {
        float particleSize = 4.0f;
        float particleLifetime = 2.0f;
        float particleOffsetMaxX = 10.0f;
        float particleDeviation = 22.5f.Deg2Rad();
        float particleVelocityMagnitude = _currentLander.LinearVelocity.Length();
        
        Vector2 particlePosition = _currentLander.Position + (Vector2.UnitX * particleOffsetMaxX).Rotated(_currentLander.Orientation) * (Random.Shared.NextSingle() - 0.5f) * 2.0f +
                                   (_currentLander.Bounds.Size with { X = _currentLander.Bounds.Size.X * 0.5f }).RotatedAroundOrigin(_currentLander.Orientation, _currentLander.Bounds.Size / 2.0f);
        
        Vector2 particleVelocity =
            -accurateLanderForward.Rotated(((Random.Shared.NextSingle() - 0.5f) * 2.0f) * particleDeviation) *
            particleVelocityMagnitude * 2.0f;
        
        _particles.Add(new Particle(particlePosition, particleVelocity, particleLifetime, particleSize, Color.Orange));
    }

    public void Draw(float dt)
    {
        Rlgl.PushMatrix();
        Rlgl.Translatef(_currentLander.Position.X + _currentLander.Bounds.Size.X / 2.0f, _currentLander.Position.Y + _currentLander.Bounds.Size.Y / 2.0f, 0.0f);
        Rlgl.Rotatef(_currentLander.Orientation.Rad2Deg(), 0.0f, 0.0f, 1.0f);
        Raylib.DrawRectangleLinesEx(_currentLander.Bounds.Offset(-_currentLander.Bounds.Size / 2.0f), 4.0f, Color.Gray);
        Rlgl.PopMatrix();
        
        Raylib.DrawText($" - Position: {_currentLander.Position}", 12, 32, 20, Color.Black);
        Raylib.DrawText($" - Orientation: {_currentLander.Orientation}", 12, 52, 20, Color.Black);
        Raylib.DrawText($" - Velocity: {_currentLander.LinearVelocity.Length():0.0} u/s", 12, 72, 20, Color.Black);
        Raylib.DrawText($" - Thrust: {_currentLander.Thrust:0.0} u/s", 12, 92, 20, Color.Black);

        Vector2 landerPosition = _currentLander.Position + _currentLander.Bounds.Size / 2.0f;
        Vector2 landerForward = landerPosition + _currentLander.Orientation.AsVector().Rotated(-MathF.PI / 2.0f) *
            _currentLander.Bounds.Radius() * 2.0f;
        
        // Raylib.DrawLineEx(landerPosition, landerForward, 1.0f, Color.Red);

        foreach (var particle in _particles)
        {
            Raylib.DrawRectangle((int)particle.Position.X, (int)particle.Position.Y, (int)particle.Size, (int)particle.Size, particle.Color);
        }
    }
}

public struct Lander(Rectangle bounds, Vector2 position, float orientation, Vector2 linearVelocity = default, float angularVelocity = 0.0f)
{
    public Rectangle Bounds = bounds;
    public Vector2 Position = position;
    public float Orientation = orientation;
    public Vector2 LinearVelocity = linearVelocity;
    public float AngularVelocity = angularVelocity;
    public float Thrust = 0.0f;
}

public struct Particle(Vector2 position, Vector2 velocity, float lifetime, float size, Color color)
{
    public Vector2 Position = position;
    public Vector2 Velocity = velocity;
    public float Lifetime = lifetime;
    public readonly float Size = size;
    public Color Color = color;
    
    public readonly Color InitialColor = color;
    public readonly float TotalLifeTime = lifetime;
}