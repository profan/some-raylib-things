using System.Numerics;
using Raylib_cs;
using Utils;

Main();

void Main()
{
    Raylib.InitWindow(800, 480, "Lander");
    Game game = new Game();
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
    public Lander CurrentLander;
    public List<Particle> Particles = new List<Particle>();

    public void Start()
    {
        Reset();
    }

    public void Reset()
    {
        var screenWidth = Raylib.GetScreenWidth();
        var landerBounds = new Rectangle(Vector2.Zero, Vector2.One * 32.0f);
        var landerPosition = new Vector2((screenWidth / 2.0f) - landerBounds.Size.X, 0.0f);
        var landerOrientation = 0.0f;
        
        CurrentLander = new Lander(landerBounds, landerPosition, landerOrientation);
        Particles.Clear();
    }

    void UpdateParticles(float dt)
    {
        float w = Raylib.GetScreenWidth();
        float h = Raylib.GetScreenHeight();
        
        for (int i = 0; i < Particles.Count; i++)
        {
            Particle newParticle = Particles[i];

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

            Vector2 landerMin = CurrentLander.Position;
            Vector2 landerMax = CurrentLander.Position + CurrentLander.Bounds.Size;

            if ((newParticle.TotalLifeTime - newParticle.Lifetime) > 0.25f)
            {
                if (newParticle.Position.IsInsideBounds(landerMin, landerMax))
                {
                    Vector2 normalizedParticleVelocity = Vector2.Normalize(newParticle.Velocity);
                    Vector2 vectorToLanderForward = Vector2.Normalize(CurrentLander.Orientation.AsVector() - normalizedParticleVelocity);
                    Vector2 vectorToLanderLeft = Vector2.Normalize(CurrentLander.Orientation.AsVector().Perpendicular() - normalizedParticleVelocity);
                    
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
            
            Particles[i] = newParticle;
        }
        
        Particles.RemoveAll(p => p.Lifetime <= 0);
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
        float landerThrusterPower = CurrentLander.Thrust;
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

        Vector2 rotatedLanderForward = (CurrentLander.Orientation - (MathF.PI / 2.0f)).AsVector();
        
        CurrentLander.LinearVelocity += gravityVector;
        CurrentLander.LinearVelocity += rotatedLanderForward * thrustVector * landerThrusterPower;
        CurrentLander.AngularVelocity += (turnVector * landerRollSpeed);
        
        CurrentLander.LinearVelocity *= MathF.Pow(0.965f, dt / (1.0f/60.0f)); // drag??
        CurrentLander.AngularVelocity *= MathF.Pow(0.90f, dt / (1.0f/60.0f)); // drag??

        CurrentLander.Position += CurrentLander.LinearVelocity * dt;
        CurrentLander.Orientation += CurrentLander.AngularVelocity * dt;
        CurrentLander.Thrust = Math.Max(0.0f, CurrentLander.Thrust + thrustAdjustVector * dt);

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
        float particleVelocityMagnitude = CurrentLander.LinearVelocity.Length();
        
        Vector2 particlePosition = CurrentLander.Position + (Vector2.UnitX * particleOffsetMaxX).Rotated(CurrentLander.Orientation) * (Random.Shared.NextSingle() - 0.5f) * 2.0f +
                                   (CurrentLander.Bounds.Size with { X = CurrentLander.Bounds.Size.X * 0.5f }).RotatedAroundOrigin(CurrentLander.Orientation, CurrentLander.Bounds.Size / 2.0f);
        
        Vector2 particleVelocity =
            -accurateLanderForward.Rotated(((Random.Shared.NextSingle() - 0.5f) * 2.0f) * particleDeviation) *
            particleVelocityMagnitude * 2.0f;
        
        Particles.Add(new Particle(particlePosition, particleVelocity, particleLifetime, particleSize, Color.Orange));
    }

    public void Draw(float dt)
    {
        Rlgl.PushMatrix();
        Rlgl.Translatef(CurrentLander.Position.X + CurrentLander.Bounds.Size.X / 2.0f, CurrentLander.Position.Y + CurrentLander.Bounds.Size.Y / 2.0f, 0.0f);
        Rlgl.Rotatef(CurrentLander.Orientation.Rad2Deg(), 0.0f, 0.0f, 1.0f);
        Raylib.DrawRectangleLinesEx(CurrentLander.Bounds.Offset(-CurrentLander.Bounds.Size / 2.0f), 4.0f, Color.Gray);
        Rlgl.PopMatrix();
        
        Raylib.DrawText($" - Position: {CurrentLander.Position}", 12, 32, 20, Color.Black);
        Raylib.DrawText($" - Orientation: {CurrentLander.Orientation}", 12, 52, 20, Color.Black);
        Raylib.DrawText($" - Velocity: {CurrentLander.LinearVelocity.Length():0.0} u/s", 12, 72, 20, Color.Black);
        Raylib.DrawText($" - Thrust: {CurrentLander.Thrust:0.0} u/s", 12, 92, 20, Color.Black);

        Vector2 landerPosition = CurrentLander.Position + CurrentLander.Bounds.Size / 2.0f;
        Vector2 landerForward = landerPosition + CurrentLander.Orientation.AsVector().Rotated(-MathF.PI / 2.0f) *
            CurrentLander.Bounds.Radius() * 2.0f;
        
        // Raylib.DrawLineEx(landerPosition, landerForward, 1.0f, Color.Red);

        foreach (var particle in Particles)
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
    public float Size = size;
    public Color Color = color;
    
    public readonly Color InitialColor = color;
    public readonly float TotalLifeTime = lifetime;
}